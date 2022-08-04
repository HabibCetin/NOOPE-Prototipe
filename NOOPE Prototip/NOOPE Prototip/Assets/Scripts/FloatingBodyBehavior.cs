//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

public class FloatingBodyBehavior : MonoBehaviour
{

    public GameObject mainCharacter;

    void Update()
    {
        transform.position = mainCharacter.transform.position + Vector3.up * 2f;
        transform.rotation = Quaternion.Lerp(transform.rotation, mainCharacter.transform.rotation, 1f * Time.deltaTime);

    }
}
