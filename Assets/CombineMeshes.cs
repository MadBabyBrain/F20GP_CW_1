using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineMeshes : MonoBehaviour
{
    public Mesh m;
    public MeshFilter[] meshFilters;
    public CombineInstance[] combine;
    public Material mat;
    private void Start() {
        Combine();
    }

    public void Combine() {
        int count = 0;

        foreach(Transform c in this.transform) {
            if (c.gameObject.activeSelf && c.GetComponent<MeshFilter>() != null && c.name != "Ground" && c.name != "Ceiling") {
                count++;
            }
        }

        this.meshFilters = new MeshFilter[count];
        this.combine = new CombineInstance[count];
        count = 0;

        foreach(Transform c in this.transform) {
            if (c.gameObject.activeSelf && c.GetComponent<MeshFilter>() != null && c.name != "Ground" && c.name != "Ceiling") {
                meshFilters[count] = c.GetComponent<MeshFilter>();
                // combine[count].mesh = meshFilters[count].sharedMesh;
                // combine[count].transform = meshFilters[count].transform.localToWorldMatrix;
                
                // // Vector3 pos;
                // // Quaternion rot;
                // // meshFilters[count].transform.GetPositionAndRotation(out pos, out rot);
                // // Vector3 sca = meshFilters[count].transform.lossyScale;

                // // combine[count].transform = new Matrix4x4();
                // // combine[count].transform.SetTRS(pos, rot, sca);

                // // Vector3 pos = combine[count].transform.GetColumn(3);
                // // combine[count].transform.SetColumn(3, new Vector4(pos.x / 1f, pos.y / 1f, pos.z / 1f, 1));
                
                // meshFilters[count].gameObject.SetActive(false);
                count++;
            }
        }

        // this.m = new Mesh();
        // this.GetComponent<MeshFilter>().sharedMesh = this.m;
        // this.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
        // this.GetComponent<MeshRenderer>().material = this.mat;
        // this.transform.gameObject.SetActive(true);
    }
}
