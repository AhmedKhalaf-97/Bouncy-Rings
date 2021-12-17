using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingObject : MonoBehaviour
{
    public float thrust;
    public Material transparentMaterial;

    Transform myTransform;
    Rigidbody rb;
    MeshRenderer meshRenderer;
    BoxCollider myCollider;
    Player player;

    public bool isPulledByMagnet;

    [Header("Rings Overlapping Checking Variables")]
    public float radius = 0.1f;
    public int collidersLimit = 2;
    public float checkRate = 0.5f;
    float nextCheck;

    Collider[] colliders;
    bool isCoroutineStarted;
    bool stopCheckingForOverlapping;

    void OnEnable()
    {
        myCollider = GetComponent<BoxCollider>();
        Invoke("SetColliderTrigger", 1.5f);
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player").GetComponent<Player>();
        meshRenderer = GetComponent<MeshRenderer>();
        myTransform = transform;

        thrust = player.thrustValue;
    }

    void Update()
    {
        if (!stopCheckingForOverlapping)
        {
            if (Time.time > nextCheck)
            {
                nextCheck = Time.time + checkRate;

                CheckIfRingsOverlapped();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isPulledByMagnet)
        {
            if (Application.isMobilePlatform)
            {
                rb.AddForce(new Vector3(Input.acceleration.x, (Input.acceleration.y - Player.yAxisOffset), ((Input.acceleration.y - Player.yAxisOffset) * 0.05f)) * thrust);
            }
            else
            {
                rb.AddForce(new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse Y")) * thrust);
            }
        }
        else //To Reverse Magnet Effect
        {
            foreach(GameObject magnetGO in player.magnetGameobjects)
            {
                if(transform.localPosition.x != magnetGO.transform.position.x)
                {
                    transform.gameObject.layer = LayerMask.NameToLayer("Floating");
                    rb.isKinematic = false;
                    isPulledByMagnet = false;
                }
            }
        }
    }

    void CheckIfRingsOverlapped()
    {
        colliders = Physics.OverlapSphere(myTransform.position, radius, 9);

        if (colliders.Length > collidersLimit)
        {
            if (!isCoroutineStarted)
            {
                StartCoroutine(SettingTheRingTriggerFalseAndTrue());
            }
        }
    }

    IEnumerator SettingTheRingTriggerFalseAndTrue()
    {
        isCoroutineStarted = true;

        myCollider.isTrigger = false;
        yield return new WaitForSeconds(0.2f);
        myCollider.isTrigger = true;

        isCoroutineStarted = false;
    }

    void SetColliderTrigger() //Called in OnEnable Function.
    {
        myCollider.isTrigger = true;
    }

    public void RemoveThisObject()
    {
        StartCoroutine(RemoveThisObjectCoroutine());
    }

    IEnumerator RemoveThisObjectCoroutine()
    {
        stopCheckingForOverlapping = true;

        gameObject.layer = LayerMask.GetMask("Default");

        meshRenderer.material = transparentMaterial;

        var myMaterial = meshRenderer.material;

        Color color = myMaterial.color;
        color.a = 0f;
        myMaterial.color = color;

        yield return new WaitForSeconds(2f);

        rb.isKinematic = true;
        Destroy(gameObject);
    }
}
