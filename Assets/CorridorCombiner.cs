using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorCombiner : MonoBehaviour
{
    private List<MeshFilter[]> mFilters;
    public MeshFilter[] meshFilters;

    void Start()
    {
        // Combine();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.K)) Combine();
    }

    public void Combine() {
        mFilters = new List<MeshFilter[]>();
        int count = 0;
        foreach (Transform c in this.transform) {
            mFilters.Add(c.GetComponent<CombineMeshes>().meshFilters);
            count += c.GetComponent<CombineMeshes>().meshFilters.Length;
        }

        this.meshFilters = new MeshFilter[count];

        count = 0;
        foreach (MeshFilter[] m in this.mFilters) {
            for (int i = 0; i < m.Length; i++) {
                this.meshFilters[count] = m[i];
                count++;
            }
        }

    }
}
