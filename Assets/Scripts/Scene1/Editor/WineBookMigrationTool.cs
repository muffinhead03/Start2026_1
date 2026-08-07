using UnityEngine;
using UnityEditor;
using UnityEditor.Events;
using System.Collections.Generic;
using System.Linq;

public static class WineBookMigrationTool
{
    // (localPosition.x, localPosition.y) -> 정답 알파벳 매칭용 원래 글자
    static readonly Dictionary<(float, float), string> letterByPosition = new Dictionary<(float, float), string>
    {
        { (30.3f,      -3.08f),  "A" },
        { (30.404f,    -3.08f),  "B" },
        { (30.529f,    -3.08f),  "C" },
        { (30.658998f, -3.08f),  "D" },
        { (30.803f,    -3.08f),  "E" },
        { (30.917f,    -3.08f),  "F" },
        { (31.018f,    -3.075f), "G" },
        { (30.291f,    -3.49f),  "H" },
        { (30.415f,    -3.484f), "I" },
        { (30.539f,    -3.49f),  "J" },
        { (30.669f,    -3.49f),  "K" },
        { (30.807f,    -3.484f), "L" },
        { (30.923f,    -3.49f),  "M" },
        { (31.041f,    -3.477f), "N" },
        { (30.285f,    -3.875f), "O" },
        { (30.402f,    -3.862f), "P" },
        { (30.526f,    -3.856f), "Q" },
        { (30.643f,    -3.862f), "R" },
        { (30.787f,    -3.856f), "S" },
        { (30.916f,    -3.861f), "T" },
        { (31.034f,    -3.859f), "U" },
        { (30.28f,     -4.246f), "V" },
        { (30.384f,    -4.227f), "W" },
        { (30.468f,    -4.264f), "X" },
        { (30.328f,    -4.653f), "Y" },
        { (30.508f,    -4.653f), "Z" },
    };

    [MenuItem("Tools/WineScene/1. Convert Books To Grabbable")]
    static void ConvertBooks()
    {
        GameObject booksWine = GameObject.Find("books_wine");
        if (booksWine == null)
        {
            Debug.LogError("[WineBookMigration] 'books_wine' 오브젝트를 못 찾았어요. Hierarchy 이름 확인해주세요.");
            return;
        }

        // 이미 씬에 있는 정상 작동하는 Object_Grabbable에서 player 참조를 복사해옴
        // (이름을 몰라도 안전하게 같은 값을 재사용하기 위함)
        Object_Grabbable playerSource = Object.FindObjectsByType<Object_Grabbable>(FindObjectsSortMode.None)
            .FirstOrDefault(o => o.player != null);

        if (playerSource == null)
            Debug.LogWarning("[WineBookMigration] 참고할 기존 Object_Grabbable(player 필드 채워진 것)을 못 찾았어요. 변환 후 각 책의 Player 필드를 수동으로 확인해주세요.");

        int converted = 0, skipped = 0;

        foreach (Transform child in booksWine.transform)
        {
            Vector3 pos = child.localPosition;
            string letter = FindClosestLetter(pos.x, pos.y);

            if (letter == null)
            {
                Debug.LogWarning($"[매칭 실패] {child.name} @ ({pos.x:F3},{pos.y:F3}) — 수동 확인 필요", child.gameObject);
                skipped++;
                continue;
            }

            GameObject go = child.gameObject;

            // 1. Missing Script(Book) 제거
            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

            // 2. Object_Grabbable 추가
            Object_Grabbable grabbable = go.GetComponent<Object_Grabbable>();
            if (grabbable == null)
                grabbable = go.AddComponent<Object_Grabbable>();

            grabbable.objectName = "book_" + letter;
            if (playerSource != null) grabbable.player = playerSource.player;

            // 3. Event_On_Ray.OnClick 재배선 (끊긴 옛날 리스너 제거 후 OnGrab 연결)
            Event_On_Ray ray = go.GetComponent<Event_On_Ray>();
            if (ray != null)
            {
                for (int i = ray.OnClick.GetPersistentEventCount() - 1; i >= 0; i--)
                    UnityEventTools.RemovePersistentListener(ray.OnClick, i);

                UnityEventTools.AddVoidPersistentListener(ray.OnClick, grabbable.OnGrab);

                // ★ 추가된 줄: 프리팹 인스턴스 오버라이드로 확정 기록
                PrefabUtility.RecordPrefabInstancePropertyModifications(ray);
            }

            // 4. 레이어를 Grabbable로
            int grabbableLayer = LayerMask.NameToLayer("Grabbable");
            if (grabbableLayer >= 0) go.layer = grabbableLayer;
            else Debug.LogWarning($"'Grabbable' 레이어가 프로젝트에 없어요 — {go.name} 레이어 수동 확인 필요");

            EditorUtility.SetDirty(go);
            converted++;
        }

        Debug.Log($"[WineBookMigration] 변환 완료: {converted}권 성공, {skipped}권 매칭 실패");
    }

    static string FindClosestLetter(float x, float y)
    {
        foreach (var kv in letterByPosition)
            if (Mathf.Abs(kv.Key.Item1 - x) < 0.01f && Mathf.Abs(kv.Key.Item2 - y) < 0.01f)
                return kv.Value;
        return null;
    }
}