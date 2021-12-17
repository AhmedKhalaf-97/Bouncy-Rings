using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingObjectSpawner : MonoBehaviour
{
    public GameObject floatingObject;
    public int numberOfObjects;

    public Player player;
    public PlayModes playModes;

    Vector3 randomPosition;

    void OnEnable()
    {
        InstantiateObjects();
    }

    public void InstantiateObjects()
    {
        playModes.instantiatedRingsCount = 0;
        Invoke("GetNumerOfInsatiatedRingsInvoke", 0.23f); //0.2 from coroutine + 0.03 to give time

        foreach (Transform child in transform)
        {
            StartCoroutine(InstantiateObjectsCoroutine(child));
        }
    }

    IEnumerator InstantiateObjectsCoroutine(Transform _transform)
    {
        for (int i = 0; i < (numberOfObjects / transform.childCount); i++)
        {
            yield return new WaitForSeconds(0.2f);
            GameObject ring = Instantiate(floatingObject, _transform.position, Quaternion.identity);
            playModes.GiveRingColors(ring, numberOfObjects);
        }

        player.GetAllFloatingObjectsInstanceIds();
    }

    void GetNumerOfInsatiatedRingsInvoke()
    {
        player.mainMenu.GetNumerOfInsatiatedRings();
    }
}