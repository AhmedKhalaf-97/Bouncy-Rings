using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public Transform magnetCenter;
    public float radius;
    public LayerMask ringsLayer;
    public float startTime = 1f;
    float timeLength;

    Collider[] ringsCollider;
    Transform coneTriggerTransform;

    void OnEnable()
    {
        coneTriggerTransform = transform.parent.GetChild(0);

        timeLength = Time.time + startTime;
    }

    void Update()
    {
        if(Time.time < timeLength)
        {
            return;
        }

        ringsCollider = Physics.OverlapSphere(magnetCenter.position, radius, ringsLayer);

        if (ringsCollider.Length != 0)
        {
            Transform ringTransfom = ringsCollider[0].transform;
            Rigidbody ringRB = ringTransfom.GetComponent<Rigidbody>();
            FloatingObject ringFO = ringTransfom.GetComponent<FloatingObject>();

            ringFO.isPulledByMagnet = true;
            ringTransfom.gameObject.layer = LayerMask.NameToLayer("MagnetedRing");
            ringRB.isKinematic = true;

            ringTransfom.localEulerAngles = new Vector3(90f, 0, 0);
            ringTransfom.localPosition = new Vector3(coneTriggerTransform.position.x, ringTransfom.localPosition.y, 0f);
            ringRB.isKinematic = false;

            ringRB.MovePosition(ringTransfom.position + Vector3.down * Time.deltaTime);

            ringRB.AddForce(new Vector3(0f, -5f, 0f) * (ringFO.thrust * 2));
        }
    }

    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(magnetCenter.position, radius);
    //}
}
