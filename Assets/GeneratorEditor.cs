// using UnityEngine;
// using UnityEditor;
// using System.Collections;

// [CustomEditor(typeof(Generator))]
// public class GeneratorEditor : Editor {
//     public override void OnInspectorGUI() {
//         DrawDefaultInspector();

//         Generator Gen = (Generator)target;

//         if (GUILayout.Button("Generate")) {
//             Gen.Generate();
//         }
//         if (GUILayout.Button("Combine meshes")) {
//             Gen.Combine();
//         }
//     }
// }