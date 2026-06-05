using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;

public class OllamaClient : MonoBehaviour
{
    const string URL = "http://localhost:11434/api/generate";

    public IEnumerator RequestHint(string systemPrompt, string userPrompt, Action<string> onComplete, string rawHint)
    {
        // JSON 수동 조립 (JsonUtility 파싱 오류 방지)
        string jsonBody = "{" +
            "\"model\":\"gemma3:4b\"," +
            "\"system\":" + EscapeJson(systemPrompt) + "," +
            "\"prompt\":" + EscapeJson(userPrompt) + "," +
            "\"stream\":false," +
            "\"options\":{" +
                "\"temperature\":0.7," +
                "\"num_predict\":80" +
            "}" +
        "}";

        byte[] bytes = Encoding.UTF8.GetBytes(jsonBody);

        using UnityWebRequest req = new UnityWebRequest(URL, "POST");
        req.uploadHandler   = new UploadHandlerRaw(bytes);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        bool requestFinished = false;
        bool fallbackSent = false;

        // 10초 대기 후 아직 응답이 없으면 출력
        StartCoroutine(WaitForResponse(rawHint, () => requestFinished, (msg) => {
            fallbackSent = true;
            onComplete?.Invoke(msg);
        }));
        yield return req.SendWebRequest();

        requestFinished = true;
        
        if (fallbackSent) yield break;

        if (req.result == UnityWebRequest.Result.Success)
        {
            string raw = req.downloadHandler.text;
            Debug.Log("[Ollama 원문 응답] " + raw); // 확인용 로그

            string parsed = ParseResponse(raw);
            onComplete?.Invoke(parsed);
        }
        else
        {
            Debug.LogError("[Ollama 오류] " + req.error);
            Debug.LogError("[응답 본문] " + req.downloadHandler.text);
            onComplete?.Invoke("...무언가가 방해하고 있어.");
        }
    }

    private IEnumerator WaitForResponse(string rawHint, Func<bool> isFinished, Action<string> onComplete)
    {
        yield return new WaitForSeconds(10f);

        if (!isFinished())
        {
            onComplete?.Invoke("..." + rawHint + "...");
        }
    }

    // "response" 필드 값만 문자열로 추출
    string ParseResponse(string raw)
    {
        const string key = "\"response\":\"";
        int start = raw.IndexOf(key, StringComparison.Ordinal);
        if (start < 0)
        {
            Debug.LogError("[파싱 실패] response 필드를 찾을 수 없음. 원문: " + raw);
            return "...";
        }

        start += key.Length;
        int end = raw.IndexOf("\"", start);
        if (end < 0)
        {
            Debug.LogError("[파싱 실패] response 닫는 따옴표를 찾을 수 없음");
            return "...";
        }

        // 이스케이프 문자 복원 (\n → 줄바꿈 등)
        string result = raw.Substring(start, end - start);
        result = result.Replace("\\n", "\n").Replace("\\\"", "\"").Replace("\\\\", "\\");

        // 영어 괄호 부분 제거 (모델이 한국어 뒤에 영어 번역을 붙이는 경우)
        int bracketStart = result.IndexOf('(');
        if (bracketStart > 0)
            result = result.Substring(0, bracketStart).Trim();

        // 따옴표 제거
        result = result.Trim('"', '"', '"');

        return result.Trim();
    }

    // 문자열을 JSON 안에 안전하게 넣기 위해 이스케이프 처리
    string EscapeJson(string s)
    {
        s = s.Replace("\\", "\\\\")
             .Replace("\"", "\\\"")
             .Replace("\n", "\\n")
             .Replace("\r", "\\r")
             .Replace("\t", "\\t");
        return "\"" + s + "\"";
    }
}
