using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HintPaperNotesManager : MonoBehaviour
{
    [Header("Player")]
    public GameObject player;

    [Header("정답 쪽지 이름 4개 (예: clue_note_S)")]
    public List<string> validNoteNames = new List<string>();

    [Header("전부 붙었을 때 이벤트")]
    public UnityEvent onAllNotesPlaced;

    HashSet<string> placedNames = new HashSet<string>();

    public bool TryPlaceNote(Transform slotTransform)
    {
        var grab = player.GetComponent<Player_Grab>();
        if (!grab.isGrab()) return false;

        foreach (string name in validNoteNames)
        {
            if (placedNames.Contains(name)) continue;

            if (grab.hasKey(name))
            {
                GameObject note = grab.PutOn(slotTransform.position);

                if (note != null)
                    StartCoroutine(SnapToSlot(note, slotTransform, grab.targetTime));

                placedNames.Add(name);
                Debug.Log($"[HintPaperNotesManager] {name} 부착 ({placedNames.Count}/{validNoteNames.Count})");

                if (placedNames.Count >= validNoteNames.Count)
                    onAllNotesPlaced?.Invoke();

                return true;
            }
        }

        return false;
    }

    public bool IsHoldingValidNote()
    {
        var grab = player.GetComponent<Player_Grab>();

        if (!grab.isGrab())
        {
            Debug.Log("[HintPaperNotesManager] 지금 손이 비어있음");
            return false;
        }

        foreach (string name in validNoteNames)
        {
            if (placedNames.Contains(name)) continue;
            if (grab.hasKey(name))
            {
                Debug.Log($"[HintPaperNotesManager] {name} 들고 있음 확인됨");
                return true;
            }
        }

        Debug.Log("[HintPaperNotesManager] 뭔가 들고는 있는데 유효한 쪽지 이름이랑 안 맞음");
        return false;
    }

    IEnumerator SnapToSlot(GameObject note, Transform slot, float delay)
    {
        yield return new WaitForSeconds(delay + 0.05f);

        if (note != null)
        {
            note.transform.position = slot.position;
            note.transform.rotation = slot.rotation;
        }
    }
}