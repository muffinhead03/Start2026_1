using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class StartingSceneSettingController : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Button xButton;

    [Header("Background Volume")]
    [SerializeField] private RectTransform backgroundControlBar;
    [SerializeField] private RectTransform backgroundControlButton;

    [Header("Sound Effect Volume")]
    [SerializeField] private RectTransform soundEffectControlBar;
    [SerializeField] private RectTransform soundEffectControlButton;

    [Header("Language")]
    [SerializeField] private Button leftLanguageArrow;
    [SerializeField] private Button rightLanguageArrow;
    [SerializeField] private TMP_Text currentLanguageText;

    private void Awake()
    {
        if (uiPanel == null)
        {
            uiPanel = gameObject;
        }

        if (xButton != null)
        {
            xButton.onClick.RemoveListener(ClosePanel);
            xButton.onClick.AddListener(ClosePanel);
        }

        if (leftLanguageArrow != null)
        {
            leftLanguageArrow.onClick.RemoveListener(OnClickLeftLanguageButton);
            leftLanguageArrow.onClick.AddListener(OnClickLeftLanguageButton);
        }

        if (rightLanguageArrow != null)
        {
            rightLanguageArrow.onClick.RemoveListener(OnClickRightLanguageButton);
            rightLanguageArrow.onClick.AddListener(OnClickRightLanguageButton);
        }

        AddDragEvent(backgroundControlButton, OnDragBackgroundVolume);
        AddDragEvent(soundEffectControlButton, OnDragSoundEffectVolume);
    }

    private void OnEnable()
    {
        GameData.LoadAll();

        RefreshBackgroundButtonPosition();
        RefreshSoundEffectButtonPosition();
        RefreshLanguageText();
    }

    private void ClosePanel()
    {
        GameData.SaveAll();

        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }

    private void OnClickLeftLanguageButton()
    {
        GameData.MoveLanguageIndex(-1);
        RefreshLanguageText();
    }

    private void OnClickRightLanguageButton()
    {
        GameData.MoveLanguageIndex(1);
        RefreshLanguageText();
    }

    private void RefreshLanguageText()
    {
        if (currentLanguageText != null)
        {
            currentLanguageText.text = GameData.CurrentLanguage.ToUpper();
        }
    }

    private void OnDragBackgroundVolume(PointerEventData eventData)
    {
        int volume = MoveButtonOnBar(
            backgroundControlBar,
            backgroundControlButton,
            eventData
        );

        GameData.BackgroundVolume = volume;
        GameData.SaveVolumes();
    }

    private void OnDragSoundEffectVolume(PointerEventData eventData)
    {
        int volume = MoveButtonOnBar(
            soundEffectControlBar,
            soundEffectControlButton,
            eventData
        );

        GameData.ObjectSoundVolume = volume;
        GameData.SaveVolumes();
    }

    private int MoveButtonOnBar(RectTransform bar, RectTransform button, PointerEventData eventData)
    {
        if (bar == null || button == null)
        {
            return 100;
        }

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            bar,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        float minX = bar.rect.xMin;
        float maxX = bar.rect.xMax;

        float clampedX = Mathf.Clamp(localPoint.x, minX, maxX);

        Vector3 targetWorldPosition = bar.TransformPoint(new Vector3(clampedX, 0f, 0f));

        button.position = new Vector3(
            targetWorldPosition.x,
            button.position.y,
            button.position.z
        );

        float normalized = Mathf.InverseLerp(minX, maxX, clampedX);
        int volume = Mathf.RoundToInt(normalized * 100f);

        return Mathf.Clamp(volume, 0, 100);
    }

    private void RefreshBackgroundButtonPosition()
    {
        SetButtonPositionByVolume(
            backgroundControlBar,
            backgroundControlButton,
            GameData.BackgroundVolume
        );
    }

    private void RefreshSoundEffectButtonPosition()
    {
        SetButtonPositionByVolume(
            soundEffectControlBar,
            soundEffectControlButton,
            GameData.ObjectSoundVolume
        );
    }

    private void SetButtonPositionByVolume(RectTransform bar, RectTransform button, int volume)
    {
        if (bar == null || button == null)
        {
            return;
        }

        volume = Mathf.Clamp(volume, 0, 100);

        float normalized = volume / 100f;

        float minX = bar.rect.xMin;
        float maxX = bar.rect.xMax;

        float targetX = Mathf.Lerp(minX, maxX, normalized);

        Vector3 targetWorldPosition = bar.TransformPoint(new Vector3(targetX, 0f, 0f));

        button.position = new Vector3(
            targetWorldPosition.x,
            button.position.y,
            button.position.z
        );
    }

    private void AddDragEvent(RectTransform target, UnityEngine.Events.UnityAction<PointerEventData> dragAction)
    {
        if (target == null)
        {
            return;
        }

        EventTrigger eventTrigger = target.GetComponent<EventTrigger>();

        if (eventTrigger == null)
        {
            eventTrigger = target.gameObject.AddComponent<EventTrigger>();
        }

        eventTrigger.triggers.Clear();

        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;

        dragEntry.callback.AddListener((eventData) =>
        {
            dragAction.Invoke((PointerEventData)eventData);
        });

        eventTrigger.triggers.Add(dragEntry);
    }
}