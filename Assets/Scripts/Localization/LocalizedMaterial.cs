using System;
using System.Collections.Generic;
using UnityEngine;

/*
 * LocalizedMaterial
 *
 * Renderer에 붙여서 현재 언어(LanguageCurrentStatus.CurrentLanguage)에 맞는
 * Material로 교체해 주는 컴포넌트.
 *
 * - Korean   : 기본 Material (컴포넌트를 붙이면 현재 Material이 자동으로 들어감)
 * - English  : Mat_..._English 머티리얼
 * - Japanese : 준비 중 (비워두면 Korean Material 사용)
 *
 * 언어가 바뀌면 OnLanguageChanged 이벤트를 받아 즉시 다시 적용한다.
 * Material이 여러 개인 Renderer는 slots에 materialIndex별로 추가하면 된다.
 */
[DisallowMultipleComponent]
[RequireComponent(typeof(Renderer))]
public class LocalizedMaterial : MonoBehaviour
{
    [Serializable]
    public class Slot
    {
        [Tooltip("Renderer의 Materials 배열 중 교체할 인덱스")]
        public int materialIndex = 0;

        [Tooltip("기본(한국어) Material")]
        public Material korean;

        [Tooltip("영어 Material (비워두면 한국어 사용)")]
        public Material english;

        [Tooltip("일본어 Material - 준비 중 (비워두면 한국어 사용)")]
        public Material japanese;

        public Material Get(LanguageType language)
        {
            Material result = language switch
            {
                LanguageType.English => english,
                LanguageType.Japanese => japanese,
                _ => korean
            };

            // 해당 언어 Material이 없으면 한국어로 대체
            return result != null ? result : korean;
        }
    }

    [SerializeField] private List<Slot> slots = new() { new Slot() };

    private Renderer targetRenderer;

    private void Awake()
    {
        targetRenderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        LanguageCurrentStatus.OnLanguageChanged += Apply;

        // 비활성 상태에서 언어가 바뀌었을 수 있으므로 켜질 때마다 적용
        Apply(LanguageCurrentStatus.CurrentLanguage);
    }

    private void OnDisable()
    {
        LanguageCurrentStatus.OnLanguageChanged -= Apply;
    }

    public void Apply(LanguageType language)
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        // sharedMaterials를 사용해서 Material 인스턴스가 복제되지 않도록 함
        Material[] materials = targetRenderer.sharedMaterials;
        bool changed = false;

        foreach (Slot slot in slots)
        {
            if (slot == null)
            {
                continue;
            }

            if (slot.materialIndex < 0 || slot.materialIndex >= materials.Length)
            {
                Debug.LogWarning(
                    $"[LocalizedMaterial] {name}: materialIndex {slot.materialIndex}가 범위를 벗어남",
                    this
                );
                continue;
            }

            Material target = slot.Get(language);

            if (target != null && materials[slot.materialIndex] != target)
            {
                materials[slot.materialIndex] = target;
                changed = true;
            }
        }

        if (changed)
        {
            targetRenderer.sharedMaterials = materials;
        }
    }

#if UNITY_EDITOR
    // 컴포넌트를 처음 붙이거나 Reset 했을 때
    // 현재 Renderer의 Material들을 한국어(기본) 칸에 자동으로 채움
    private void Reset()
    {
        Renderer r = GetComponent<Renderer>();
        slots = new List<Slot>();

        if (r == null)
        {
            slots.Add(new Slot());
            return;
        }

        Material[] materials = r.sharedMaterials;

        for (int i = 0; i < materials.Length; i++)
        {
            slots.Add(new Slot
            {
                materialIndex = i,
                korean = materials[i]
            });
        }
    }
#endif
}
