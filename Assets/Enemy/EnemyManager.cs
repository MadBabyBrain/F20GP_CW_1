using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    public GameObject target;
    void Start()
    {
        this.target = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        foreach (Transform c in this.transform)
        {
            Vector3 pos = Vector3.zero;

            foreach (Transform c2 in c)
            {
                c2.gameObject.SetActive(Mathf.Abs(Vector3.Distance(this.target.transform.position, c2.position)) < 40f);
            }
        }
    }
}
