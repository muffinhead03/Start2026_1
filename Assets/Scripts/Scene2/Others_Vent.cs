using UnityEngine;
using System.Collections;

public class Others_Vent : MonoBehaviour
{
    [Header("Position")]
    [SerializeField] Transform targetPos;

    [SerializeField] private Play_Audio audio_player;

    private void TeleportTarget()
    {
        transform.position = targetPos.position;
        transform.rotation = targetPos.rotation;
    }

    public void Drop()
    {
        TeleportTarget();

        Rigidbody rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = false;

        StartCoroutine(CollisionMode());

        audio_player.PlayAudio(0);
    }

    private IEnumerator CollisionMode()
    {
        Rigidbody rigid = GetComponent<Rigidbody>();

        rigid.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        yield return new WaitForSeconds(3f);

        rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }
}
