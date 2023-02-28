using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotator : MonoBehaviour
{
    void Update()
    {
        this.transform.RotateAround(this.transform.position, Vector3.up, Time.deltaTime * 5f);
    }
}
