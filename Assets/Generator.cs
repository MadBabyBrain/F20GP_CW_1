// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Generator : MonoBehaviour
// {
//     public int x, y;
//     public GameObject prefab, empty;

//     private List<MeshFilter[]> mFilters;
//     private MeshFilter[] meshFilters;
//     private CombineInstance[] combine;
//     public Material mat;

//     private void Start() {
//         // Combine();
//     }

//     private void Update() {
//         if (Input.GetKeyDown(KeyCode.J)) Combine();
//     }

//     public void Generate() {
//         for (int i = 0; i < x; i++) {
//             GameObject e = GameObject.Instantiate(empty, new Vector3(0, 0, i * 10), Quaternion.identity);
//             e.name = "Row " + "(" + i + ")";
//             e.transform.parent = this.transform;
//             for (int j = 0; j < y; j++) {
//                 GameObject obj = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab);
//                 // GameObject obj = GameObject.Instantiate(prefab, new Vector3(j * 10, 0, i * 10), Quaternion.identity);
//                 obj.name = "Corridor " + "(" + j + ")";
//                 obj.transform.SetPositionAndRotation(new Vector3(j * 10, 0, i * 10), Quaternion.identity);
//                 obj.transform.parent = e.transform;
//             }
//         }
//     }

//     public void Combine() {
//         mFilters = new List<MeshFilter[]>();
//         int count = 0;

//         foreach (Transform c in this.transform) {
//             if (c.GetComponent<CorridorCombiner>() != null) {
//                 mFilters.Add(c.GetComponent<CorridorCombiner>().meshFilters);
//                 count += c.GetComponent<CorridorCombiner>().meshFilters.Length;
//             }
//         }

//         meshFilters = new MeshFilter[count];
//         combine = new CombineInstance[count];

//         count = 0;
//         foreach (MeshFilter[] mf in mFilters) {
//             for (int i = 0; i < mf.Length; i++) {
//                 meshFilters[count] = mf[i];
//                 count++;
//             }
//         }

//         for (int i = 0; i < meshFilters.Length; i++) {
//             combine[i].mesh = meshFilters[i].sharedMesh;
//             combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
//             meshFilters[i].gameObject.SetActive(false);
//         }

//         // foreach(Transform c in this.transform) {
//         //     if (c.gameObject.activeSelf) {
//         //         count++;
//         //     }
//         // }

//         // meshFilters = new MeshFilter[count];
//         // combine = new CombineInstance[count];
//         // count = 0;

//         // foreach(Transform c in this.transform) {
//         //     if (c.gameObject.activeSelf && c.GetComponent<MeshFilter>() != null) {
//         //         meshFilters[count] = c.GetComponent<MeshFilter>();
//         //         combine[count].mesh = meshFilters[count].sharedMesh;
//         //         combine[count].transform = meshFilters[count].transform.localToWorldMatrix;
//         //         meshFilters[count].gameObject.SetActive(false);
//         //     }
//         // }

//         Mesh m = new Mesh();
//         this.GetComponent<MeshFilter>().sharedMesh = m;
//         this.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);
//         this.transform.gameObject.SetActive(true);

//         this.GetComponent<MeshRenderer>().material = this.mat;

//         this.GetComponent<MeshCollider>().sharedMesh = this.GetComponent<MeshFilter>().mesh;

//     }
// }
