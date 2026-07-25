using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public event Action OnInventoryChanged;

    private const int MaxSlotCount = 8;

    private readonly List<string> items = new List<string>();

    public IReadOnlyList<string> Items => items;

    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "inventory.json");

    private void Awake()
    {
        // InventoryManager가 여러 개 생기는 것을 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // 씬이 바뀌어도 인벤토리 유지
        DontDestroyOnLoad(gameObject);

        LoadInventory();
    }

    public bool HasItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        return items.Contains(itemId);
    }

    public bool AddItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogWarning("추가하려는 아이템 ID가 비어 있습니다.");
            return false;
        }

        if (items.Count >= MaxSlotCount)
        {
            Debug.Log("인벤토리가 가득 찼습니다.");
            return false;
        }

        // 열쇠는 중복 획득하지 않는다고 가정
        if (items.Contains(itemId))
        {
            Debug.Log($"{itemId}은(는) 이미 가지고 있습니다.");
            return false;
        }

        items.Add(itemId);

        SaveInventory();
        OnInventoryChanged?.Invoke();

        Debug.Log($"{itemId}을(를) 인벤토리에 추가했습니다.");

        return true;
    }

    public bool RemoveItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        bool removed = items.Remove(itemId);

        if (!removed)
        {
            return false;
        }

        SaveInventory();
        OnInventoryChanged?.Invoke();

        Debug.Log($"{itemId}을(를) 인벤토리에서 제거했습니다.");

        return true;
    }

    public bool UseItem(string itemId)
    {
        if (!HasItem(itemId))
        {
            Debug.Log($"{itemId}을(를) 가지고 있지 않습니다.");
            return false;
        }

        items.Remove(itemId);

        SaveInventory();
        OnInventoryChanged?.Invoke();

        Debug.Log($"{itemId}을(를) 사용했습니다.");

        return true;
    }

    public bool UseItemAt(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= items.Count)
        {
            Debug.Log("해당 슬롯에는 사용할 아이템이 없습니다.");
            return false;
        }

        string usedItem = items[slotIndex];
        items.RemoveAt(slotIndex);

        SaveInventory();
        OnInventoryChanged?.Invoke();

        Debug.Log($"{usedItem}을(를) 사용했습니다.");

        return true;
    }

    public void SaveInventory()
    {
        try
        {
            InventorySaveData saveData = new InventorySaveData
            {
                items = new List<string>(items)
            };

            string json = JsonUtility.ToJson(saveData, true);

            File.WriteAllText(SavePath, json);

            Debug.Log($"인벤토리 저장 완료: {SavePath}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"인벤토리 저장 실패: {exception.Message}");
        }
    }

    public void LoadInventory()
    {
        items.Clear();

        if (!File.Exists(SavePath))
        {
            Debug.Log("저장된 인벤토리 파일이 없습니다.");
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);

            InventorySaveData saveData =
                JsonUtility.FromJson<InventorySaveData>(json);

            if (saveData?.items != null)
            {
                int loadCount = Mathf.Min(
                    saveData.items.Count,
                    MaxSlotCount
                );

                for (int i = 0; i < loadCount; i++)
                {
                    string itemId = saveData.items[i];

                    if (!string.IsNullOrWhiteSpace(itemId))
                    {
                        items.Add(itemId);
                    }
                }
            }

            OnInventoryChanged?.Invoke();

            Debug.Log($"인벤토리 불러오기 완료: {items.Count}개");
        }
        catch (Exception exception)
        {
            Debug.LogError($"인벤토리 불러오기 실패: {exception.Message}");
        }
    }

    public void ClearInventory()
    {
        items.Clear();

        SaveInventory();
        OnInventoryChanged?.Invoke();

        Debug.Log("인벤토리를 모두 비웠습니다.");
    }

    public void DeleteSaveData()
    {
        items.Clear();

        try
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
            }

            OnInventoryChanged?.Invoke();

            Debug.Log("인벤토리 저장 파일을 삭제했습니다.");
        }
        catch (Exception exception)
        {
            Debug.LogError($"저장 파일 삭제 실패: {exception.Message}");
        }
    }
}

[Serializable]
public class InventorySaveData
{
    public List<string> items = new List<string>();
}