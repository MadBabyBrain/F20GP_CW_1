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
        foreach (Transform c in this.transform) {
            Vector3 pos = Vector3.zero;

            foreach (Transform c2 in c) {
                // pos += c2.position;
                c2.gameObject.SetActive(Mathf.Abs(Vector3.Distance(this.target.transform.position, c2.position)) < 40f);
            }

            // pos /= c.childCount;

            // Vector3 oldpos = c.position;

            // c.position = (c.childCount == 0) ? Vector3.zero : pos /= c.childCount;

            // c.gameObject.SetActive(Mathf.Abs(Vector3.Distance(this.target.transform.position, c.position)) < 40f);
            
            // foreach (Transform c2 in c) {
            //     c2.position -= c.position - oldpos;
            // }

            // foreach (Transform c2 in c.transform) {
            //     bool e = (Mathf.Abs(Vector3.Distance(this.target.transform.position, c2.position)) < 40f);
            //     c2.GetComponent<Finder>().enabled = e;
            //     c2.GetComponent<NavMeshAgent>().enabled = e;
            // }
        }        
    }
}
