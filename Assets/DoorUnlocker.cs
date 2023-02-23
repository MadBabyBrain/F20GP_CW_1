using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlocker : MonoBehaviour
{
    void Update()
    {
        if (this.transform.parent.GetChild(1).childCount == 0) {
            this.gameObject.SetActive(false);
        }
    }
}
