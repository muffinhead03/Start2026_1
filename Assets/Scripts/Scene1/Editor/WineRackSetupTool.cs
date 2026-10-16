using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 와인렉 퍼즐 세팅 도구 (에디터 전용, 빌드에 안 들어감).
/// 메뉴: Tools > Wine Rack > Setup Tool
///
/// 1) 칸 세팅: 이름이 slot_1 ~ slot_26 인 칸 (대소문자, 앞자리 0 상관없음)에 공용 배치 퍼즐 컴포넌트를 붙이고 연결
///    (Play_Audio, Event_On_Ray, Object_PutOn, Object_Puzzle_Piece / Id = 번호 - 1)
/// 2) 병 세팅: 와인병 4개를 "집을 수 있는 물건"으로 변환 (Object_Grabbable, wine_1 ~ wine_4)
/// 3) 정답 세팅: Object_Puzzle.pwd를 26글자 정답 문자열로 설정
/// 4) 검사: 위 세 가지가 제대로 됐는지 Console에 보고
///
/// 여러 번 눌러도 안전하게(기존 이벤트 연결은 지우고 다시 연결) 만들어져 있음. Ctrl+Z로 되돌리기 가능.
/// </summary>
public class WineRackSetupTool : EditorWindow
{
    const int SlotCount = 26;
    const int BottleCount = 4;
    static readonly Regex SlotNameRegex = new Regex(@"^slot_(\d+)$", RegexOptions.IgnoreCase);

    static readonly string[] BottleLabels = { "RubyRed (B)", "TawnyBrown (T)", "Burgundy (H)", "Maroon (Y)" };

    [Header("공통")]
    [SerializeField] GameObject player;
    [SerializeField] Scene_UI_Manager sceneUI;

    [Header("칸")]
    [SerializeField] Transform slotsRoot;
    [SerializeField] Material transparentMat;
    [SerializeField] AudioClip putOnSound;
    [SerializeField] string keyName = "wine";

    [Header("퍼즐")]
    [SerializeField] Object_Puzzle puzzle;

    [Header("병 (순서대로 wine_1 ~ wine_4)")]
    [SerializeField] GameObject[] bottles = new GameObject[BottleCount];
    [Tooltip("각 병의 정답 칸 번호 (1~26)")]
    [SerializeField] int[] answerSlots = { 11, 16, 4, 7 };

    Vector2 scroll;
    SerializedObject self;

    [MenuItem("Tools/Wine Rack/Setup Tool")]
    static void Open() => GetWindow<WineRackSetupTool>("Wine Rack");

    void OnEnable() => self = new SerializedObject(this);

    // ───────── UI ─────────

    void OnGUI()
    {
        if (self == null) self = new SerializedObject(this);
        self.Update();
        scroll = EditorGUILayout.BeginScrollView(scroll);

        EditorGUILayout.LabelField("공통", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(self.FindProperty("player"));
        EditorGUILayout.PropertyField(self.FindProperty("sceneUI"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("칸 (slot_1 ~ slot_26 의 부모)", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(self.FindProperty("slotsRoot"));
        EditorGUILayout.PropertyField(self.FindProperty("transparentMat"), new GUIContent("투명 머티리얼"));
        EditorGUILayout.PropertyField(self.FindProperty("putOnSound"), new GUIContent("놓는 소리 (선택)"));
        EditorGUILayout.PropertyField(self.FindProperty("keyName"), new GUIContent("열쇠 이름"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("퍼즐", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(self.FindProperty("puzzle"), new GUIContent("Object_Puzzle"));

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("병 (wine_1 ~ wine_4) / 정답 칸", EditorStyles.boldLabel);
        var bottlesProp = self.FindProperty("bottles");
        var slotsProp = self.FindProperty("answerSlots");
        bottlesProp.arraySize = BottleCount;
        slotsProp.arraySize = BottleCount;
        for (int i = 0; i < BottleCount; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(bottlesProp.GetArrayElementAtIndex(i),
                new GUIContent($"wine_{i + 1}  {BottleLabels[i]}"));
            EditorGUILayout.PropertyField(slotsProp.GetArrayElementAtIndex(i), GUIContent.none, GUILayout.Width(40));
            EditorGUILayout.EndHorizontal();
        }
        self.ApplyModifiedProperties();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox($"정답 문자열: {BuildAnswer()}", MessageType.None);

        EditorGUILayout.Space();
        if (GUILayout.Button("1. 칸 26개 세팅", GUILayout.Height(28))) SetupSlots();
        if (GUILayout.Button("2. 병 4개 세팅", GUILayout.Height(28))) SetupBottles();
        if (GUILayout.Button("3. 퍼즐 정답 세팅", GUILayout.Height(28))) SetupPuzzle();
        EditorGUILayout.Space();
        if (GUILayout.Button("4. 칸 기준점을 바닥으로 (병 뜨는 문제)", GUILayout.Height(28))) MovePivotsToFloor();
        EditorGUILayout.Space();
        if (GUILayout.Button("검사 (Console에 결과 출력)", GUILayout.Height(28))) Validate();

        EditorGUILayout.EndScrollView();
    }

    // ───────── 1. 칸 ─────────

    void SetupSlots()
    {
        if (!Require(slotsRoot, "Slots Root") || !Require(player, "Player") || !Require(sceneUI, "Scene UI") ||
            !Require(puzzle, "Object_Puzzle") || !Require(transparentMat, "투명 머티리얼")) return;

        var slots = CollectSlots(out var problems);
        if (problems.Length > 0)
        {
            Debug.LogError("[WineRackSetup] 칸 이름 문제 — 먼저 고쳐주세요:\n" + problems);
            return;
        }

        Undo.SetCurrentGroupName("Wine Rack: 칸 세팅");
        int group = Undo.GetCurrentGroup();

        foreach (var pair in slots)
        {
            int number = pair.Key;
            GameObject go = pair.Value;

            var mr = GetPreviewRenderer(go);
            if (mr == null)
            {
                Debug.LogWarning($"[WineRackSetup] {go.name}: Mesh Renderer가 없음 — 미리보기 표시가 안 됨", go);
            }
            else
            {
                Undo.RecordObject(mr, "disable preview");
                mr.enabled = false; // 병 들고 다가갈 때만 Object_PutOn이 켜줌
            }

            if (go.GetComponent<Collider>() == null)
            {
                Undo.AddComponent<BoxCollider>(go);
                Debug.LogWarning($"[WineRackSetup] {go.name}: Collider가 없어서 Box Collider를 추가함 — 크기 확인 필요", go);
            }

            var playAudio = GetOrAdd<Play_Audio>(go); // AudioSource는 RequireComponent로 같이 붙음
            var source = go.GetComponent<AudioSource>();
            if (source != null) { Undo.RecordObject(source, "audio"); source.playOnAwake = false; }

            var ray = GetOrAdd<Event_On_Ray>(go);
            var putOn = GetOrAdd<Object_PutOn>(go);
            var piece = GetOrAdd<Object_Puzzle_Piece>(go);

            // Object_PutOn (private SerializeField라 SerializedObject로 설정)
            var so = new SerializedObject(putOn);
            so.FindProperty("player").objectReferenceValue = player;
            so.FindProperty("mat_trans").objectReferenceValue = transparentMat;
            so.FindProperty("mesh").objectReferenceValue = mr;
            so.FindProperty("keyName").stringValue = keyName;
            if (putOnSound != null) so.FindProperty("audio_puton").objectReferenceValue = putOnSound;
            so.ApplyModifiedProperties();

            // Object_Puzzle_Piece
            Undo.RecordObject(piece, "piece");
            piece.puzzle = puzzle;
            piece.putOn = putOn;
            piece.id = number - 1;

            // Event_On_Ray
            Undo.RecordObject(ray, "ray events");
            ray.OnEnter = ClearCalls(ray.OnEnter);
            ray.OnExit = ClearCalls(ray.OnExit);
            ray.OnClick = ClearCalls(ray.OnClick);
            UnityEventTools.AddPersistentListener(ray.OnClick, putOn.OnInteract);
            UnityEventTools.AddPersistentListener(ray.OnEnter, putOn.OnEnter);
            UnityEventTools.AddBoolPersistentListener(ray.OnEnter, sceneUI.SwitchCursor, true);
            UnityEventTools.AddPersistentListener(ray.OnExit, putOn.OnExit);
            UnityEventTools.AddBoolPersistentListener(ray.OnExit, sceneUI.SwitchCursor, false);

            // Object_PutOn 이벤트 → Piece
            Undo.RecordObject(putOn, "putOn events");
            putOn.putDown = ClearCalls(putOn.putDown);
            putOn.pickUp = ClearCalls(putOn.pickUp);
            UnityEventTools.AddPersistentListener(putOn.putDown, piece.PutDown);
            UnityEventTools.AddPersistentListener(putOn.pickUp, piece.PickUp);

            EditorUtility.SetDirty(ray);
            EditorUtility.SetDirty(putOn);
            EditorUtility.SetDirty(piece);
        }

        Undo.CollapseUndoOperations(group);
        EditorSceneManager.MarkSceneDirty(slotsRoot.gameObject.scene);
        Debug.Log($"[WineRackSetup] 칸 {slots.Count}개 세팅 완료");
    }

    // ───────── 2. 병 ─────────

    void SetupBottles()
    {
        if (!Require(player, "Player") || !Require(sceneUI, "Scene UI")) return;

        int grabbableLayer = LayerMask.NameToLayer("Grabbable");
        if (grabbableLayer < 0) Debug.LogWarning("[WineRackSetup] 'Grabbable' 레이어가 없음 — 레이어는 직접 설정해주세요");

        Undo.SetCurrentGroupName("Wine Rack: 병 세팅");
        int group = Undo.GetCurrentGroup();

        for (int i = 0; i < BottleCount; i++)
        {
            GameObject go = bottles[i];
            if (go == null)
            {
                Debug.LogWarning($"[WineRackSetup] wine_{i + 1} ({BottleLabels[i]}) 칸이 비어 있어서 건너뜀");
                continue;
            }

            // 예전 라벨 확대 기능 끄기 (예전 힌트 스텝 4·5를 완료 처리하는 것 방지)
            DisableByTypeName(go, "WineLabelInspect");
            DisableByTypeName(go, "Wineracklabel");

            if (go.GetComponent<Collider>() == null)
                Debug.LogWarning($"[WineRackSetup] {go.name}: Collider가 없음 — 레이에 안 맞음", go);

            var grab = GetOrAdd<Object_Grabbable>(go);
            Undo.RecordObject(grab, "grab");
            grab.objectName = $"wine_{i + 1}";
            grab.player = player;
            EditorUtility.SetDirty(grab);

            var ray = GetOrAdd<Event_On_Ray>(go);
            Undo.RecordObject(ray, "ray events");
            ray.OnEnter = ClearCalls(ray.OnEnter);
            ray.OnExit = ClearCalls(ray.OnExit);
            ray.OnClick = ClearCalls(ray.OnClick);
            UnityEventTools.AddPersistentListener(ray.OnClick, grab.OnGrab);
            UnityEventTools.AddBoolPersistentListener(ray.OnEnter, sceneUI.SwitchCursor, true);
            UnityEventTools.AddBoolPersistentListener(ray.OnExit, sceneUI.SwitchCursor, false);
            EditorUtility.SetDirty(ray);

            if (grabbableLayer >= 0) SetLayerRecursive(go, grabbableLayer);

            if (string.IsNullOrEmpty(grab.InventoryObjectName))
                Debug.LogWarning($"[WineRackSetup] {go.name}: Inventory Object Name(Localization Key)이 비어 있음 — 인벤토리 표시 이름이 안 나올 수 있음", go);
        }

        Undo.CollapseUndoOperations(group);
        var anyBottle = System.Array.Find(bottles, b => b != null);
        if (anyBottle != null) EditorSceneManager.MarkSceneDirty(anyBottle.scene);
        Debug.Log("[WineRackSetup] 병 세팅 완료");
    }

    // ───────── 3. 정답 ─────────

    void SetupPuzzle()
    {
        if (!Require(puzzle, "Object_Puzzle")) return;
        string answer = BuildAnswer();
        if (answer == null) return;

        Undo.RecordObject(puzzle, "Wine Rack: 정답 세팅");
        puzzle.pwd = answer;
        EditorUtility.SetDirty(puzzle);
        EditorSceneManager.MarkSceneDirty(puzzle.gameObject.scene);
        Debug.Log($"[WineRackSetup] 정답 세팅 완료: {answer}");
    }

    string BuildAnswer()
    {
        var chars = new char[SlotCount];
        for (int i = 0; i < SlotCount; i++) chars[i] = '-';
        var used = new HashSet<int>();
        for (int i = 0; i < BottleCount; i++)
        {
            int slot = answerSlots != null && i < answerSlots.Length ? answerSlots[i] : 0;
            if (slot < 1 || slot > SlotCount || !used.Add(slot)) return null; // 범위 밖 or 중복
            chars[slot - 1] = (char)('1' + i);
        }
        return new string(chars);
    }

    // ───────── 4. 검사 ─────────

    void Validate()
    {
        var sb = new StringBuilder("[WineRackSetup] 검사 결과\n");
        int errors = 0;
        void Err(string msg) { sb.AppendLine("  ❌ " + msg); errors++; }

        // 칸
        if (slotsRoot == null) Err("Slots Root가 비어 있음");
        else
        {
            var slots = CollectSlots(out var problems);
            if (problems.Length > 0) { sb.Append(problems); errors++; }

            foreach (var pair in slots)
            {
                int n = pair.Key; var go = pair.Value;
                var putOn = go.GetComponent<Object_PutOn>();
                var piece = go.GetComponent<Object_Puzzle_Piece>();
                var ray = go.GetComponent<Event_On_Ray>();
                var mr = GetPreviewRenderer(go);

                if (putOn == null || piece == null || ray == null) { Err($"{go.name}: 컴포넌트 누락 (PutOn/Piece/Event_On_Ray)"); continue; }
                if (go.GetComponent<Play_Audio>() == null) Err($"{go.name}: Play_Audio 없음");
                if (go.GetComponent<Collider>() == null) Err($"{go.name}: Collider 없음");
                if (mr != null && mr.enabled) Err($"{go.name}: Mesh Renderer가 켜져 있음 (시작부터 미리보기가 보임)");

                if (piece.id != n - 1) Err($"{go.name}: Id가 {piece.id} (기대값 {n - 1})");
                if (piece.putOn != putOn) Err($"{go.name}: Piece.Put On이 자기 자신이 아님");
                if (puzzle != null && piece.puzzle != puzzle) Err($"{go.name}: Piece.Puzzle이 다른 오브젝트를 가리킴");

                var so = new SerializedObject(putOn);
                if (so.FindProperty("mesh").objectReferenceValue != mr) Err($"{go.name}: PutOn 메쉬 렌더러가 자기 것이 아님");
                if (so.FindProperty("player").objectReferenceValue == null) Err($"{go.name}: PutOn Player 비어 있음");
                if (so.FindProperty("mat_trans").objectReferenceValue == null) Err($"{go.name}: PutOn 투명 머티리얼 비어 있음");
                if (so.FindProperty("keyName").stringValue != keyName) Err($"{go.name}: PutOn 열쇠 이름이 '{keyName}'이 아님");

                if (!HasCall(ray.OnClick, putOn, "OnInteract")) Err($"{go.name}: OnClick → 자기 PutOn.OnInteract 연결 없음");
                if (!HasCall(ray.OnEnter, putOn, "OnEnter")) Err($"{go.name}: OnEnter → 자기 PutOn.OnEnter 연결 없음");
                if (!HasCall(ray.OnExit, putOn, "OnExit")) Err($"{go.name}: OnExit → 자기 PutOn.OnExit 연결 없음");
                if (!HasCall(putOn.putDown, piece, "PutDown")) Err($"{go.name}: putDown → 자기 Piece.PutDown 연결 없음");
                if (!HasCall(putOn.pickUp, piece, "PickUp")) Err($"{go.name}: pickUp → 자기 Piece.PickUp 연결 없음");
            }
            sb.AppendLine($"  칸 {slots.Count}개 확인");
        }

        // 병
        for (int i = 0; i < BottleCount; i++)
        {
            var go = bottles[i];
            if (go == null) { Err($"wine_{i + 1} ({BottleLabels[i]}) 칸이 비어 있음"); continue; }
            var grab = go.GetComponent<Object_Grabbable>();
            var ray = go.GetComponent<Event_On_Ray>();
            if (grab == null || ray == null) { Err($"{go.name}: Object_Grabbable / Event_On_Ray 없음"); continue; }
            if (grab.objectName != $"wine_{i + 1}") Err($"{go.name}: Object Name이 '{grab.objectName}' (기대값 wine_{i + 1})");
            if (!HasCall(ray.OnClick, grab, "OnGrab")) Err($"{go.name}: OnClick → OnGrab 연결 없음");
            if (IsEnabledByTypeName(go, "WineLabelInspect")) Err($"{go.name}: WineLabelInspect가 아직 켜져 있음");
            if (LayerMask.LayerToName(go.layer) != "Grabbable") Err($"{go.name}: 레이어가 Grabbable이 아님");
        }

        // 정답
        string answer = BuildAnswer();
        if (answer == null) Err("정답 칸 번호가 범위 밖이거나 중복됨");
        else if (puzzle == null) Err("Object_Puzzle이 비어 있음");
        else if (puzzle.pwd != answer) Err($"Object_Puzzle.pwd가 '{puzzle.pwd}' (기대값 '{answer}')");

        sb.Insert(0, errors == 0 ? "✅ " : $"❌ 문제 {errors}개 — ");
        if (errors == 0) Debug.Log(sb.ToString()); else Debug.LogError(sb.ToString());
    }

    // ───────── 4. 칸 기준점을 바닥으로 ─────────
    //
    // Object_PutOn은 병을 칸의 transform.position(= Cube 중심)에 놓는데,
    // 와인병 모델은 기준점이 병 바닥이라 Cube 높이의 절반만큼 떠 보임.
    // 그래서 칸 오브젝트의 기준점만 Cube 바닥으로 내리고,
    // 콜라이더와 미리보기 Cube는 원래 자리에 그대로 보이게 바꿈:
    //
    //   slot_N (기준점 = 바닥, Box Collider center를 위로 올림, Event_On_Ray / PutOn / Piece)
    //   └─ Preview (원래 Cube 메시, 미리보기 표시용)
    //
    // 이미 Preview 자식이 있는 칸은 건너뜀 (여러 번 눌러도 안전).

    const string PreviewChildName = "Preview";

    void MovePivotsToFloor()
    {
        if (!Require(slotsRoot, "Slots Root")) return;

        var slots = CollectSlots(out var problems);
        if (problems.Length > 0)
        {
            Debug.LogError("[WineRackSetup] 칸 이름 문제 — 먼저 고쳐주세요:\n" + problems);
            return;
        }

        Undo.SetCurrentGroupName("Wine Rack: 칸 기준점을 바닥으로");
        int group = Undo.GetCurrentGroup();
        int changed = 0, skipped = 0;

        foreach (var pair in slots)
        {
            GameObject go = pair.Value;
            Transform t = go.transform;

            if (t.Find(PreviewChildName) != null) { skipped++; continue; }

            var mf = go.GetComponent<MeshFilter>();
            var mr = go.GetComponent<MeshRenderer>();
            var box = go.GetComponent<BoxCollider>();
            if (mf == null || mr == null || box == null)
            {
                Debug.LogWarning($"[WineRackSetup] {go.name}: Mesh Filter / Mesh Renderer / Box Collider 중 없는 게 있어서 건너뜀", go);
                skipped++;
                continue;
            }

            // 메시 바닥까지의 로컬 거리 (기본 Cube면 0.5)
            float bottomLocal = -(mf.sharedMesh != null ? mf.sharedMesh.bounds.min.y : -0.5f);

            // 1) 칸 기준점을 메시 바닥으로 내림 (월드 기준)
            Undo.RecordObject(t, "move pivot");
            Vector3 down = t.TransformVector(new Vector3(0f, bottomLocal, 0f));
            t.position -= down;

            // 2) 콜라이더는 원래 자리에 남도록 center를 올림
            Undo.RecordObject(box, "collider center");
            box.center += new Vector3(0f, bottomLocal, 0f);

            // 3) 미리보기 Cube를 자식으로 만들어 원래 자리에 둠
            var preview = new GameObject(PreviewChildName);
            Undo.RegisterCreatedObjectUndo(preview, "create preview");
            preview.layer = go.layer;
            preview.transform.SetParent(t, false);
            preview.transform.localPosition = new Vector3(0f, bottomLocal, 0f);
            preview.transform.localRotation = Quaternion.identity;
            preview.transform.localScale = Vector3.one;

            var pmf = preview.AddComponent<MeshFilter>();
            pmf.sharedMesh = mf.sharedMesh;
            var pmr = preview.AddComponent<MeshRenderer>();
            pmr.sharedMaterials = mr.sharedMaterials;
            pmr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            pmr.enabled = false;

            // 4) 칸 자체의 메시는 제거 (미리보기는 자식이 담당)
            Undo.DestroyObjectImmediate(mr);
            Undo.DestroyObjectImmediate(mf);

            // 5) Object_PutOn이 자식 미리보기를 켜고 끄도록 연결
            var putOn = go.GetComponent<Object_PutOn>();
            if (putOn != null)
            {
                var so = new SerializedObject(putOn);
                so.FindProperty("mesh").objectReferenceValue = pmr;
                so.ApplyModifiedProperties();
            }

            changed++;
        }

        Undo.CollapseUndoOperations(group);
        EditorSceneManager.MarkSceneDirty(slotsRoot.gameObject.scene);
        Debug.Log($"[WineRackSetup] 칸 기준점 이동 완료: {changed}개 변경, {skipped}개 건너뜀");
    }

    static MeshRenderer GetPreviewRenderer(GameObject go)
    {
        var child = go.transform.Find(PreviewChildName);
        if (child != null && child.TryGetComponent(out MeshRenderer childMr)) return childMr;
        return go.GetComponent<MeshRenderer>();
    }

    // ───────── 공통 ─────────

    SortedDictionary<int, GameObject> CollectSlots(out string problems)
    {
        var result = new SortedDictionary<int, GameObject>();
        var sb = new StringBuilder();

        foreach (Transform child in slotsRoot)
        {
            if (!child.gameObject.activeSelf) continue; // 꺼둔 칸(뺄 2칸 등)은 무시
            var m = SlotNameRegex.Match(child.name);
            if (!m.Success)
            {
                sb.AppendLine($"  ❌ 이름 형식이 slot_번호가 아님: {child.name} (빼는 칸이면 꺼두세요)");
                continue;
            }
            int n = int.Parse(m.Groups[1].Value);
            if (n < 1 || n > SlotCount) { sb.AppendLine($"  ❌ 번호 범위 밖: {child.name}"); continue; }
            if (result.ContainsKey(n)) { sb.AppendLine($"  ❌ 번호 중복: {child.name}"); continue; }
            result[n] = child.gameObject;
        }
        for (int n = 1; n <= SlotCount; n++)
            if (!result.ContainsKey(n)) sb.AppendLine($"  ❌ slot_{n} 없음");

        problems = sb.ToString();
        return result;
    }

    static T GetOrAdd<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        return c != null ? c : Undo.AddComponent<T>(go);
    }

    // 기존 persistent 연결을 모두 지우고 같은 이벤트 객체를 돌려줌 (null이면 새로 생성)
    static UnityEvent ClearCalls(UnityEvent e)
    {
        if (e == null) return new UnityEvent();
        for (int i = e.GetPersistentEventCount() - 1; i >= 0; i--)
            UnityEventTools.RemovePersistentListener(e, i);
        return e;
    }

    static bool HasCall(UnityEventBase e, Object target, string method)
    {
        if (e == null) return false;
        for (int i = 0; i < e.GetPersistentEventCount(); i++)
            if (e.GetPersistentTarget(i) == target && e.GetPersistentMethodName(i) == method) return true;
        return false;
    }

    static void DisableByTypeName(GameObject go, string typeName)
    {
        if (go.GetComponent(typeName) is Behaviour b && b.enabled)
        {
            Undo.RecordObject(b, "disable " + typeName);
            b.enabled = false;
            EditorUtility.SetDirty(b);
        }
    }

    static bool IsEnabledByTypeName(GameObject go, string typeName)
        => go.GetComponent(typeName) is Behaviour b && b.enabled;

    static void SetLayerRecursive(GameObject go, int layer)
    {
        Undo.RecordObject(go, "layer");
        go.layer = layer;
        foreach (Transform child in go.transform) SetLayerRecursive(child.gameObject, layer);
    }

    static bool Require(Object o, string label)
    {
        if (o != null) return true;
        Debug.LogError($"[WineRackSetup] '{label}' 칸을 먼저 채워주세요");
        return false;
    }
}