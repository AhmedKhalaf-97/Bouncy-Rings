using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius = 5F;
    public float power = 10F;
    public LayerMask explosionLayer;

    public RainCameraController rcc;

    ParticleSystem[] bubbelsEffect = new ParticleSystem[2];

    Vector3 explosionPos;

    [Header("Audio Stuff")]
    AudioSource[] audioSources = new AudioSource[2];

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            bubbelsEffect[i] = transform.GetChild(i).GetComponentInChildren<ParticleSystem>();
            audioSources[i] = transform.GetChild(i).GetComponent<AudioSource>();
        }
    }

    public void Explode(int childIndex)
    {
        bubbelsEffect[childIndex].Play();

        rcc.StopImmidiate();
        rcc.Play();

        explosionPos = transform.GetChild(childIndex).position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius, explosionLayer);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 3F);
        }

        audioSources[childIndex].Play();
    }

    //void OnDrawGizmosSelected()
    //{
    //    foreach(Transform child in transform)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(child.position, radius);
    //    }
    //}
}
