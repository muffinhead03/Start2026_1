using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;

public class Player_Grab : MonoBehaviour
{
    [Header("Grab")]
    public Transform Hand;
    public float targetTime = 1f;

    [Header("Inventory 연동")]
    [SerializeField]
    private PerceiveObjectHandPivot handPerception;

    [SerializeField]
    private InventoryData inventoryData;

    [Header("Release")]
    public float throwForce;

    private bool isGrabbing;
    private GameObject GrabbingObject;

    private Vector3 originalPos;
    private Quaternion originalRot;
    private Vector3 releasePos;

    private Coroutine moveCoroutine;

    private void Start()
    {
        isGrabbing = false;
        GrabbingObject = null;
    }

    public bool isGrab()
    {
        return isGrabbing;
    }

    public bool hasKey(string key)
    {
        if (GrabbingObject == null)
            return false;

        Object_Grabbable grabbable =
            GrabbingObject.GetComponent<Object_Grabbable>();

        if (grabbable == null)
            return false;

        string s = grabbable.objectName;

        if (string.IsNullOrEmpty(s) ||
            string.IsNullOrEmpty(key))
        {
            return false;
        }

        return Regex.IsMatch(s, key);
    }

    public void UseKey()
    {
        if (GrabbingObject == null)
            return;
        
        StopMoveCoroutine();

        GameObject usedObject = GrabbingObject;
        
        Object_Grabbable grabbable =
            GrabbingObject.GetComponent<Object_Grabbable>();

        if (grabbable != null)
        {
            string s = grabbable.objectName;

            // 기존 문자열 인벤토리에서 제거
            Player_Inventory.RemoveItem(s);

            // 새 InventoryData에서도 제거
            inventoryData?.RemoveBySource(grabbable);
        }

        GrabbingObject.transform.SetParent(null,true);
        GrabbingObject.SetActive(false);

        isGrabbing = false;
        GrabbingObject = null;

        // HandPivot이 비었음을 즉시 알림
        handPerception?.ForceScan();
    }

   public void Grab(Object_Grabbable grab)
{
    if (grab == null)
    {
        Debug.LogWarning(
            "[Player_Grab] 잡을 물체가 없습니다.",
            this
        );

        return;
    }

    if (Hand == null)
    {
        Debug.LogError(
            "[Player_Grab] HandPivot이 연결되지 않았습니다.",
            this
        );

        return;
    }

    if (inventoryData == null)
    {
        Debug.LogError(
            "[Player_Grab] InventoryData가 연결되지 않았습니다.",
            this
        );

        return;
    }

    GameObject newObject =
        grab.gameObject;

    GameObject previousObject =
        GrabbingObject;

    /*
     * 현재 들고 있는 동일한 실제 오브젝트를
     * 다시 클릭한 경우에는 처리하지 않습니다.
     */
    if (previousObject == newObject)
    {
        return;
    }

    /*
     * 이름이 아니라 실제 Object_Grabbable 참조를 기준으로
     * 이미 인벤토리에 등록됐는지 검사합니다.
     */
    bool alreadyRegistered =
        inventoryData.FindIndexBySource(
            grab
        ) >= 0;

    /*
     * 신규 물체인데 8개 슬롯이 모두 사용 중이라면
     * 기존 A를 보관하거나 B를 잡기 전에 중단합니다.
     *
     * 따라서 현재 손에 든 A는 그대로 유지됩니다.
     */
    if (!alreadyRegistered &&
        !HasEmptyInventorySlot())
    {
        Debug.LogWarning(
            $"[Player_Grab] 인벤토리가 가득 차서 " +
            $"'{grab.objectName}'을 획득할 수 없습니다. " +
            $"최대 수량={inventoryData.SlotCount}",
            grab
        );

        return;
    }

    /*
     * A를 들고 있었다면 InventoryData 오브젝트 아래에 보관합니다.
     */
    if (isGrabbing &&
        previousObject != null)
    {
        StoreCurrentObject();
    }

    /*
     * 현재 손에 든 물체를 B로 교체합니다.
     */
    isGrabbing = true;
    GrabbingObject = newObject;

    newObject.SetActive(true);

    /*
     * B를 HandPivot의 직접 자식으로 이동합니다.
     */
    newObject.transform.SetParent(
        Hand,
        true
    );

    newObject.transform.SetAsLastSibling();


    StopMoveCoroutine();
    
    //윤민주 기존 코루틴
    moveCoroutine =StartCoroutine(MoveToTargetPosition(Hand.position,newObject));

    /*
     * 이미 등록된 동일 오브젝트라면
     * 기존 문자열 인벤토리에 다시 추가하지 않습니다.
     *
     * 같은 objectsName의 다른 오브젝트는
     * SourceObject가 다르므로 정상적으로 추가됩니다.
     */
    if (!alreadyRegistered)
    {
        Player_Inventory.AddItem(
            grab.objectName
        );
    }

    /*
     * A 보관과 B 배치가 모두 끝난 후 한 번만 스캔합니다.
     */
    handPerception?.ForceScan();

    Debug.Log(
        $"[Player_Grab] 물체 교체 완료: " +
        $"Previous=" +
        $"{(previousObject != null ? previousObject.name : "null")}, " +
        $"Current={newObject.name}, " +
        $"ObjectName={grab.objectName}, " +
        $"AlreadyRegistered={alreadyRegistered}, " +
        $"InventoryCount={GetInventoryObjectCount()}/" +
        $"{inventoryData.SlotCount}",
        grab
    );
}

private void StoreCurrentObject()
    {
        if (GrabbingObject == null)
        {
            isGrabbing = false;
            return;
        }

        StopMoveCoroutine();

        GameObject storedObject =
            GrabbingObject;

        Transform storageParent =
            inventoryData != null
                ? inventoryData.transform
                : null;

        /*
         * 기존 A를 HandPivot에서 빼고
         * Hierarchy상 InventoryData 아래로 이동합니다.
         */
        storedObject.transform.SetParent(
            storageParent,
            true
        );

        Rigidbody[] rigidbodies =
            storedObject.GetComponentsInChildren<Rigidbody>(
                true
            );

        for (int i = 0;
             i < rigidbodies.Length;
             i++)
        {
            Rigidbody body =
                rigidbodies[i];

            if (body == null)
            {
                continue;
            }

            body.linearVelocity =
                Vector3.zero;

            body.angularVelocity =
                Vector3.zero;

            body.isKinematic = true;
            body.detectCollisions = false;
        }

        Collider[] colliders =
            storedObject.GetComponentsInChildren<Collider>(
                true
            );

        for (int i = 0;
             i < colliders.Length;
             i++)
        {
            Collider targetCollider =
                colliders[i];

            if (targetCollider != null)
            {
                targetCollider.enabled = false;
            }
        }

        /*
         * Hierarchy에는 InventoryData 아래에 남겨두고
         * 게임 화면에서는 숨깁니다.
         */
        storedObject.SetActive(false);

        Debug.Log(
            $"[Player_Grab] 기존 물체 보관 완료: " +
            $"Object={storedObject.name}, " +
            $"Parent=" +
            $"{(storageParent != null ? storageParent.name : "null")}",
            storedObject
        );

        isGrabbing = false;
        GrabbingObject = null;

        /*
         * 교체 보관에서는 삭제나 중간 스캔을 하지 않습니다.
         * B가 HandPivot에 들어간 뒤 Grab() 마지막에서 스캔합니다.
         */
    }
    public void Release()
    {
        if (!isGrabbing ||
            GrabbingObject == null)
        {
            return;
        }

        /*
         * GrabbingObject를 null로 만들기 전에
         * 현재 물체 참조를 저장합니다.
         */
        GameObject releasedObject =
            GrabbingObject;

        Object_Grabbable releasedGrabbable =
            releasedObject.GetComponent<Object_Grabbable>();

        if (releasedGrabbable != null)
        {
            string s =
                releasedGrabbable.objectName;

            // 기존 문자열 인벤토리에서 제거
            Player_Inventory.RemoveItem(s);

            /*
             * E키로 내려놓으면
             * 새 InventoryData에서도 삭제합니다.
             */
            inventoryData?.RemoveBySource(
                releasedGrabbable
            );
        }

        releasePos =
            transform.position +
            new Vector3(0f, 1.5f, 0f) +
            0.4f * transform.forward;

        releasedObject.transform.position =
            releasePos;

        releasedObject.transform.SetParent(
            null,
            true
        );

        Collider targetCollider =
            releasedObject.GetComponent<Collider>();

        if (targetCollider != null)
        {
            targetCollider.enabled = true;
        }

        Rigidbody targetRigidbody =
            releasedObject.GetComponent<Rigidbody>();

        if (targetRigidbody != null)
        {
            targetRigidbody.isKinematic = false;

            StartCoroutine(
                CollisionMode(releasedObject)
            );

            targetRigidbody.AddForce(
                Hand.forward * throwForce,
                ForceMode.Impulse
            );
        }
        else
        {
            Debug.LogWarning(
                $"[Player_Grab] '{releasedObject.name}'에 " +
                "Rigidbody가 없습니다.",
                releasedObject
            );
        }

        isGrabbing = false;
        GrabbingObject = null;

        /*
         * HandPivot에서 물체가 빠진 상태를 즉시 검사합니다.
         * InventoryUIManager는 EquippedIndex를 -1로 변경합니다.
         */
        handPerception?.ForceScan();

        Debug.Log(
            $"[Player_Grab] 물체 내려놓기: " +
            $"{releasedObject.name}",
            releasedObject
        );
    }

    public GameObject PutOn(Vector3 targetPos)
    {
        if (!isGrabbing ||
            GrabbingObject == null)
        {
            return null;
        }

        GameObject targetObj =
            GrabbingObject;

        Object_Grabbable grabbable =
            targetObj.GetComponent<Object_Grabbable>();

        if (grabbable != null)
        {
            string s = grabbable.objectName;

            // 기존 문자열 인벤토리에서 제거
            Player_Inventory.RemoveItem(s);

            /*
             * PutOn으로 HandPivot에서 물체를 꺼내므로
             * 새 InventoryData에서도 제거합니다.
             */
            inventoryData?.RemoveBySource(
                grabbable
            );
        }

        targetObj.transform.SetParent(
            null,
            true
        );

        isGrabbing = false;
        GrabbingObject = null;

        StartCoroutine(
            MoveToTargetPosition(
                targetPos,
                targetObj
            )
        );

        // HandPivot이 비었음을 즉시 알림
        handPerception?.ForceScan();

        return targetObj;
    }

    private IEnumerator CollisionMode(
        GameObject grab)
    {
        if (grab == null)
            yield break;

        Rigidbody targetRigidbody =
            grab.GetComponent<Rigidbody>();

        if (targetRigidbody == null)
            yield break;

        targetRigidbody.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;

        yield return new WaitForSeconds(3f);

        if (targetRigidbody != null)
        {
            targetRigidbody.collisionDetectionMode =
                CollisionDetectionMode.Discrete;
        }
    }

    private IEnumerator MoveToTargetPosition(
        Vector3 targetPos,
        GameObject targetObj)
    {
        Player_Move playerMove =
            GetComponent<Player_Move>();

        // 플레이어 이동 잠금
        if (playerMove != null)
        {
            playerMove.SetMoveLock(true);
        }

        if (targetObj == null)
        {
            if (playerMove != null)
            {
                playerMove.SetMoveLock(false);
            }

            yield break;
        }

        Collider targetCollider =
            targetObj.GetComponent<Collider>();

        if (targetCollider != null)
        {
            targetCollider.enabled = false;
        }

        Rigidbody targetRigidbody =
            targetObj.GetComponent<Rigidbody>();

        if (targetRigidbody != null)
        {
            targetRigidbody.isKinematic = true;
        }

        originalPos =
            targetObj.transform.position;

        originalRot =
            targetObj.transform.rotation;

        float duration =
            Mathf.Max(0.0001f, targetTime);

        float t = 0f;

        while (t < duration)
        {
            if (targetObj == null)
            {
                if (playerMove != null)
                {
                    playerMove.SetMoveLock(false);
                }

                yield break;
            }

            t += Time.deltaTime;

            float tp =
                Mathf.Clamp01(t / duration);

            targetObj.transform.position =
                Vector3.Lerp(
                    originalPos,
                    targetPos,
                    tp
                );

            targetObj.transform.rotation =
                Quaternion.Lerp(
                    originalRot,
                    Quaternion.identity,
                    tp
                );

            yield return null;
        }

        if (targetObj != null)
        {
            targetObj.transform.position =
                targetPos;

            targetObj.transform.rotation =
                Quaternion.identity;

            /*
             * 아직 Hand 아래에 있는 물체라면
             * HandPivot 기준 위치를 정확히 맞춥니다.
             */
            if (targetObj.transform.parent == Hand)
            {
                targetObj.transform.localPosition =
                    Vector3.zero;

                targetObj.transform.localRotation =
                    Quaternion.identity;

                targetObj.transform.SetAsLastSibling();

                handPerception?.ForceScan();
            }
        }

        if (playerMove != null)
        {
            playerMove.SetMoveLock(false);
        }
    }

        private void StopMoveCoroutine()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(
                moveCoroutine
            );

            moveCoroutine = null;
        }

        /*
         * 이동 코루틴을 중간에 멈췄을 때
         * 플레이어 이동 잠금이 남지 않도록 해제합니다.
         */
        Player_Move playerMove =
            GetComponent<Player_Move>();

        if (playerMove != null)
        {
            playerMove.SetMoveLock(false);
        }
    }

    private int GetInventoryObjectCount()
    {
        if (inventoryData == null)
        {
            return 0;
        }

        int count = 0;

        for (int i = 0;
             i < inventoryData.SlotCount;
             i++)
        {
            if (inventoryData.GetObjectAt(i) != null)
            {
                count++;
            }
        }

        return count;
    }

        private bool HasEmptyInventorySlot()
    {
        if (inventoryData == null)
        {
            return false;
        }

        for (int i = 0;
             i < inventoryData.SlotCount;
             i++)
        {
            if (inventoryData.GetObjectAt(i) == null)
            {
                return true;
            }
        }

        return false;
    }

}