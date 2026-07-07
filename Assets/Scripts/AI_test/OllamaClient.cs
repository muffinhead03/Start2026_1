using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OllamaClient : MonoBehaviour
{
    const string URL = "http://localhost:11434/api/generate";

    [Header("테스트할 모델 (Inspector에서 변경)")]
    [Tooltip("gemma4:e2b / gemma4:e4b / gemma4-e2b-q4 / gemma4-e4b-q4 중 하나 입력")]
    public string modelName = "gemma4:e2b";

    [Header("think 옵션 (gemma4 계열 thinking 스트림 억제)")]
    public bool useThinkFalse = true;

    public IEnumerator RequestHint(string systemPrompt, string userPrompt, Action<string> onComplete)
    {
        string optionsBlock =
            "\"options\":{" +
                "\"temperature\":0.7," +
                "\"num_predict\":80" +
            "}";

        string thinkField = useThinkFalse ? "\"think\":false," : "";

        // JSON 수동 조립 (JsonUtility 파싱 오류 방지)
        string jsonBody = "{" +
            "\"model\":\"" + modelName + "\"," +
            thinkField +
            "\"system\":" + EscapeJson(systemPrompt) + "," +
            "\"prompt\":" + EscapeJson(userPrompt) + "," +
            "\"stream\":false," +
            optionsBlock +
        "}";

        Debug.Log($"[Ollama 요청] model={modelName}, think:false={useThinkFalse}");

        byte[] bytes = Encoding.UTF8.GetBytes(jsonBody);

        using UnityWebRequest req = new UnityWebRequest(URL, "POST");
        req.uploadHandler   = new UploadHandlerRaw(bytes);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        float startTime = Time.realtimeSinceStartup;
        yield return req.SendWebRequest();
        float elapsed = Time.realtimeSinceStartup - startTime;

        Debug.Log($"[Ollama 응답 시간] model={modelName} think:false={useThinkFalse} → {elapsed:F2}초");

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