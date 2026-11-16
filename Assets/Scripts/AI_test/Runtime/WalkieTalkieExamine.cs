using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class WalkieTalkieExamine : MonoBehaviour
{
    [Header("무전기 3D 오브젝트")]
    [SerializeField] Transform walkieTalkieObject;   // 씬 안의 무전기 모델
    [SerializeField] Transform examinePoint;         // 카메라 자식, 화면에 뜰 위치/각도

    [Header("연출 설정")]
    [SerializeField] float moveDuration = 0.4f;      // 집어드는 이동 시간
    [SerializeField] float rotateSpeed  = 20f;       // 초당 회전 각도 (자동 회전)

    [Header("조사 중 정자세 각도 (모델 pivot이 이상하면 여기서 보정)")]
    [SerializeField] Vector3 targetLocalEuler = Vector3.zero;

    [Header("자동 회전축 (기본값 Y축 = 원래 동작, 씬마다 다르게 설정 가능)")]
    [SerializeField] Vector3 rotateAxis = Vector3.up;

    [Header("무전 소리 (패널 열 때 / 퍼즐 힌트 버튼 누를 때 한 번씩)")]
    [Tooltip("30초씩 자른 무전 소리들. 매번 이 중 하나를 랜덤 재생 (직전과 같은 건 피함). 비워두면 소리 없이 기존 동작 그대로")]
    [SerializeField] AudioClip[] walkieClips;
    [Tooltip("설정 화면 볼륨이 먹도록 AudioMixer의 Master 그룹 연결")]
    [SerializeField] AudioMixerGroup outputGroup;
    [SerializeField, Range(0f, 1f)] float walkieVolume = 0.3f;
    [Tooltip("소리 나는 중에 패널을 닫으면 이 시간 동안 줄이며 끔")]
    [SerializeField] float stopFadeTime = 0.2f;

    [Header("재생 길이 (Inspector에서 조절)")]
    [Tooltip("이 시간(초)만큼 재생 후 자동으로 줄어들며 꺼짐. 0이면 클립 끝까지 재생")]
    [SerializeField, Min(0f)] float playDuration = 3f;
    [Tooltip("자동으로 꺼질 때 줄어드는 시간(초)")]
    [SerializeField, Min(0f)] float autoFadeTime = 1f;
    [Tooltip("체크하면 클립 중간 아무 지점에서 시작 → 30초짜리 클립에서 매번 다른 구간이 나옴")]
    [SerializeField] bool randomStartPoint = true;

    Vector3    originalPos;
    Quaternion originalRot;
    Transform  originalParent;
    Coroutine  moveCoroutine;
    Coroutine  stopCoroutine;
    AudioSource walkieSource;
    int        lastClipIndex = -1;
    bool       isRotating = false;

    void Awake()
    {
        if (walkieClips == null || walkieClips.Length == 0) return;

        // 이 오브젝트는 패널 여닫기와 상관없이 항상 켜져 있어서 끌 때 페이드 코루틴이 끝까지 돎
        walkieSource = gameObject.AddComponent<AudioSource>();
        walkieSource.loop         = false;
        walkieSource.playOnAwake  = false;
        walkieSource.spatialBlend = 0f;   // 2D (UI 소리)
        walkieSource.outputAudioMixerGroup = outputGroup;
    }

    // 패널 열 때(StartExamine) + HintManager의 퍼즐 힌트 버튼에서 호출.
    // 이미 재생 중이면 끊고 새 클립으로 다시 재생 (겹쳐서 두 배로 시끄러워지지 않게)
    public void PlayWalkieSound()
    {
        if (walkieSource == null) return;

        AudioClip clip = PickRandomClip();
        if (clip == null) return;

        if (stopCoroutine != null) { StopCoroutine(stopCoroutine); stopCoroutine = null; }
        walkieSource.Stop();
        walkieSource.clip   = clip;
        walkieSource.volume = walkieVolume;
        walkieSource.time   = PickStartTime(clip);
        walkieSource.Play();

        // 지정한 시간 뒤 자동으로 줄이며 끔 (0이면 끝까지)
        if (playDuration > 0f)
            stopCoroutine = StartCoroutine(FadeOutAndStop(playDuration, autoFadeTime));
    }

    // 시작 지점: 재생 길이만큼은 남도록 랜덤. 클립이 짧으면 처음부터
    float PickStartTime(AudioClip clip)
    {
        if (!randomStartPoint || playDuration <= 0f) return 0f;

        float maxStart = clip.length - (playDuration + autoFadeTime);
        return maxStart > 0f ? Random.Range(0f, maxStart) : 0f;
    }

    // 직전에 나온 클립은 빼고 랜덤 (클립이 1개뿐이면 그거)
    AudioClip PickRandomClip()
    {
        int count = walkieClips.Length;
        if (count == 1) { lastClipIndex = 0; return walkieClips[0]; }

        int index = Random.Range(0, count - 1);
        if (index >= lastClipIndex && lastClipIndex >= 0) index++;   // 직전 인덱스 건너뜀

        lastClipIndex = index;
        return walkieClips[index];   // Inspector 빈 칸이면 null → 그 회차만 소리 없음
    }

    void StopWalkieSound()
    {
        if (walkieSource == null || !walkieSource.isPlaying) return;

        if (stopCoroutine != null) StopCoroutine(stopCoroutine);
        stopCoroutine = StartCoroutine(FadeOutAndStop(0f, stopFadeTime));
    }

    // delay초 기다렸다가 fadeTime초 동안 줄이며 끔.
    // 자동 종료(재생 길이)와 패널 닫기 둘 다 이걸 씀 → 닫으면 기다리던 자동 종료는 취소되고 바로 줄어듦
    IEnumerator FadeOutAndStop(float delay, float fadeTime)
    {
        if (delay > 0f)
            yield return new WaitForSecondsRealtime(delay);   // timeScale=0 상황에서도 멈추지 않게

        float from = walkieSource.volume;
        float t = 0f;
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            walkieSource.volume = Mathf.Lerp(from, 0f, t / fadeTime);
            yield return null;
        }
        walkieSource.Stop();
        walkieSource.volume = walkieVolume;
        stopCoroutine = null;
    }

    void Update()
    {
        if (isRotating && walkieTalkieObject != null)
        {
            // Space.Self: 물체 자기 자신의 축 기준 회전 → targetLocalEuler로 삐딱하게 포즈 잡혀있어도
            // 흔들림 없이 깨끗하게 돎 (팽이 자기 축 회전과 같은 원리).
            // rotateAxis 값(예: (1,0,0), (0,1,0), (0,0,1))을 바꿔가며 원하는 방향 찾으면 됨.
            walkieTalkieObject.Rotate(rotateAxis, rotateSpeed * Time.deltaTime, Space.Self);
        }
    }

    public void StartExamine()
    {
        PlayWalkieSound();   // 패널 열 때마다 무조건 한 번 (모델 연결 안 된 씬이어도 소리는 남)

        if (walkieTalkieObject == null || examinePoint == null) return;

        walkieTalkieObject.gameObject.SetActive(true);   // ← 추가

        originalParent = walkieTalkieObject.parent;
        originalPos    = walkieTalkieObject.localPosition;
        originalRot    = walkieTalkieObject.localRotation;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToExamine());
    }

    public void EndExamine()
    {
        isRotating = false;
        StopWalkieSound();   // 재생 중에 닫으면 짧게 줄이며 끔
        if (walkieTalkieObject == null) return;

        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveBack());
    }

    IEnumerator MoveToExamine()
    {
        walkieTalkieObject.SetParent(examinePoint, true);

        float t = 0f;
        Vector3    startPos = walkieTalkieObject.localPosition;
        Quaternion startRot = walkieTalkieObject.localRotation;
        Quaternion endRot   = Quaternion.Euler(targetLocalEuler);   // ← 변경: identity 대신 지정한 각도

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = t / moveDuration;
            walkieTalkieObject.localPosition = Vector3.Lerp(startPos, Vector3.zero, p);
            walkieTalkieObject.localRotation = Quaternion.Slerp(startRot, endRot, p);   // ← 변경
            yield return null;
        }

        walkieTalkieObject.localPosition = Vector3.zero;
        walkieTalkieObject.localRotation = endRot;   // ← 변경
        isRotating = true;
    }

    IEnumerator MoveBack()
    {
        walkieTalkieObject.SetParent(originalParent, true);

        float t = 0f;
        Vector3    startPos = walkieTalkieObject.localPosition;
        Quaternion startRot = walkieTalkieObject.localRotation;

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float p = t / moveDuration;
            walkieTalkieObject.localPosition = Vector3.Lerp(startPos, originalPos, p);
            walkieTalkieObject.localRotation = Quaternion.Slerp(startRot, originalRot, p);
            yield return null;
        }

        walkieTalkieObject.localPosition = originalPos;
        walkieTalkieObject.localRotation = originalRot;

        walkieTalkieObject.gameObject.SetActive(false);
    }
}