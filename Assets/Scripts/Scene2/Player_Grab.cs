using System.Collections;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Grab : MonoBehaviour
{
    [Header("Grab")]
    public Transform Hand;
    public float targetTime = 1f;

    [Header("Release")]
    public float throwForce;

    bool isGrabbing;
    GameObject GrabbingObject;

    Vector3 originalPos;
    Quaternion originalRot;
    Vector3 releasePos;

    void Start()
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
        if (GrabbingObject == null || GrabbingObject.GetComponent<Object_Grabbable>() == null) return false;

        string s = GrabbingObject.GetComponent<Object_Grabbable>().objectName;
        return Regex.IsMatch(s, key);
    }

    public void UseKey()
    {
        if (GrabbingObject == null) return;

        string s = GrabbingObject.GetComponent<Object_Grabbable>().objectName;
        Player_Inventory.RemoveItem(s);

        GrabbingObject.transform.parent = null;
        GrabbingObject.SetActive(false);

        isGrabbing = false;
        GrabbingObject = null;
    }

    public void Grab(Object_Grabbable grab)
    {
        if (isGrabbing) return;

        isGrabbing = true;
        GrabbingObject = grab.gameObject;

        GrabbingObject.transform.parent = Hand;

        StartCoroutine(MoveToTargetPosition(Hand.position, GrabbingObject));

        //if (GrabbingObject.GetComponent<Object_Key>() != null) GrabbingObject.GetComponent<Object_Key>().Collect();
        Player_Inventory.AddItem(grab.objectName);
    }

    public void Release()
    {
        if (!isGrabbing) return;

        string s = GrabbingObject.GetComponent<Object_Grabbable>().objectName;
        Player_Inventory.RemoveItem(s);

        releasePos = transform.position + new Vector3(0, 1.5f, 0) + 0.4f * transform.forward;

        GrabbingObject.transform.position = releasePos;
        GrabbingObject.transform.parent = null;
        GrabbingObject.transform.GetComponent<Collider>().enabled = true;
        GrabbingObject.transform.GetComponent<Rigidbody>().isKinematic = false;

        StartCoroutine(CollisionMode(GrabbingObject.gameObject));

        GrabbingObject.transform.GetComponent<Rigidbody>().AddForce(Hand.forward * throwForce, ForceMode.Impulse);

        isGrabbing = false;
        GrabbingObject = null;
    }

    public GameObject PutOn(Vector3 targetPos)
    {
        if (!isGrabbing) return null;

        //if (GrabbingObject.GetComponent<Object_Key>() != null)
        //{
        //    string s = GrabbingObject.GetComponent<Object_Key>().keyName;
        //    Player_Inventory.RemoveItem(s);
        //}
        string s = GrabbingObject.GetComponent<Object_Grabbable>().objectName;
        Player_Inventory.RemoveItem(s);

        GrabbingObject.transform.parent = null;
        isGrabbing = false;
        GameObject targetObj = GrabbingObject;
        GrabbingObject = null;

        StartCoroutine(MoveToTargetPosition(targetPos, targetObj));

        return targetObj;
    }

    IEnumerator CollisionMode(GameObject grab)
    {
        grab.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        yield return new WaitForSeconds(3.0f);

        grab.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    IEnumerator MoveToTargetPosition(Vector3 targetPos, GameObject targetObj)
    {
        // 플레이어 이동 잠금
        GetComponent<Player_Move>().SetMoveLock(true);

        targetObj.transform.GetComponent<Collider>().enabled = false;
        targetObj.transform.GetComponent<Rigidbody>().isKinematic = true;

        originalPos = targetObj.transform.position;
        originalRot = targetObj.transform.rotation;

        float t = 0f;

        while (t < targetTime)
        {
            t += Time.deltaTime;
            float tp = t / targetTime;

            targetObj.transform.position = Vector3.Lerp(originalPos, targetPos, tp);

            targetObj.transform.rotation = Quaternion.Lerp(originalRot, Quaternion.identity, tp);

            yield return null;
        }

        GetComponent<Player_Move>().SetMoveLock(false);
    }
}
