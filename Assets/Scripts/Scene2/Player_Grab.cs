using System.Collections;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Grab : MonoBehaviour
{
    [Header("Grab")]
    public Transform Hand;
    public float targetTime = 1f;

    bool isGrabbing;
    public GameObject GrabbingObject;

    Vector3 originalPos;
    Quaternion originalRot;
    Vector3 targetPos;
    Vector3 releasePos;

    void Start()
    {
        isGrabbing = false;
        GrabbingObject = null;
    }

    public bool hasKey(string key)
    {
        if (GrabbingObject == null || GrabbingObject.GetComponent<Object_Key>() == null) return false;

        string s = GrabbingObject.GetComponent<Object_Key>().keyName;
        return Regex.IsMatch(s, key);
    }

    public void UseKey()
    {
        if (GrabbingObject == null || GrabbingObject.GetComponent<Object_Key>() == null) return;

        string s = GrabbingObject.GetComponent<Object_Key>().keyName;
        Player_Inventory.RemoveItem(s);

        GrabbingObject.transform.parent = null;
        GrabbingObject.GetComponent<Object_Key>().UseKey();

        isGrabbing = false;
        GrabbingObject = null;
    }

    public void Grab(GameObject grab)
    {
        if (isGrabbing) return;

        isGrabbing = true;
        GrabbingObject = grab;

        StartCoroutine(MoveToGrabPosition());
    }

    public void Release()
    {
        if (!isGrabbing) return;

        releasePos = transform.position + new Vector3(0, 1.8f, 0) + 0.4f * transform.forward;

        GrabbingObject.transform.position = releasePos;
        GrabbingObject.transform.parent = null;
        GrabbingObject.transform.GetComponent<Collider>().isTrigger = false;
        GrabbingObject.transform.GetComponent<Rigidbody>().isKinematic = false;

        StartCoroutine(CollisionMode(GrabbingObject.gameObject));

        isGrabbing = false;
        GrabbingObject = null;
    }

    IEnumerator CollisionMode(GameObject grab)
    {
        grab.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        yield return new WaitForSeconds(3.0f);

        grab.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    IEnumerator MoveToGrabPosition()
    {
        // 플레이어 이동 잠금
        GetComponent<Player_Move>().SetMoveLock(true);

        GrabbingObject.transform.GetComponent<Collider>().isTrigger = true;
        GrabbingObject.transform.GetComponent<Rigidbody>().isKinematic = true;
        GrabbingObject.transform.parent = Hand;

        originalPos = GrabbingObject.transform.position;
        originalRot = GrabbingObject.transform.rotation;

        targetPos = Hand.position;

        float t = 0f;

        while (t < targetTime)
        {
            t += Time.deltaTime;
            float tp = t / targetTime;

            GrabbingObject.transform.position = Vector3.Lerp(originalPos, targetPos, tp);

            GrabbingObject.transform.rotation = Quaternion.Lerp(originalRot, Quaternion.identity, tp);

            yield return null;
        }

        GetComponent<Player_Move>().SetMoveLock(false);
    }
}
