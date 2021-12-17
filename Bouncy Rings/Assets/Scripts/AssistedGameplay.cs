using UnityEngine;

public class AssistedGameplay : MonoBehaviour
{
    public float radius;
    public LayerMask ringsLayer;

    Transform myTransform;

    Collider[] ringsCollider;
    Transform coneTriggerTransform;

    void OnEnable()
    {
        coneTriggerTransform = transform.parent.GetChild(0);

        myTransform = transform;
    }

    void Update()
    {
        ringsCollider = Physics.OverlapSphere(myTransform.position, radius, ringsLayer);

        if (ringsCollider.Length != 0)
        {
            Transform ringTransfom = ringsCollider[0].transform;

            ringTransfom.localEulerAngles = new Vector3(90f, 0, 0);
            ringTransfom.localPosition = new Vector3(coneTriggerTransform.position.x, ringTransfom.localPosition.y, 0f);
        }
    }

    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(myTransform.position, radius);
    //}
}