using UnityEngine;
using UnityEditor;

namespace LD49.Editor {
    public class CreatePropsFromSelected {
        [MenuItem("Tools/Make Props (Replace Existing)")]
        static void MakePropsReplace() {
            foreach (GameObject selected in Selection.gameObjects) {
                if (Selection.activeObject is GameObject gameObject) {
                    CreateOrReplacePrefab(selected, true);
                }
            }
        }

        [MenuItem("Tools/Make Props")]
        static void MakeProps() {
            foreach (GameObject selected in Selection.gameObjects) {
                if (Selection.activeObject is GameObject gameObject) {
                    CreateOrReplacePrefab(selected, false);
                }
            } 
        }

        private static void CreateOrReplacePrefab(GameObject toCopy, bool replace) {
            Material defaultMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/UnlitPalette.mat");
            GameObject copy = GameObject.Instantiate(toCopy);
            copy.name = toCopy.name;
            copy.transform.position = Vector3.zero;
            copy.transform.rotation = Quaternion.identity;
            copy.layer = LayerMask.NameToLayer("Props");

            Rigidbody rigidbody = copy.AddComponent<Rigidbody>();
            MeshCollider meshCollider = copy.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            Prop prop = copy.AddComponent<Prop>();

            foreach (var renderer in copy.GetComponentsInChildren<Renderer>()) {
                renderer.sharedMaterial = defaultMaterial;
            }

            if (!replace && AssetDatabase.LoadAssetAtPath<GameObject>(GetPath(copy)) != null) {
                Debug.Log($"Skipping {copy.name} as it already exists.");
            } else {
                PrefabUtility.SaveAsPrefabAssetAndConnect(copy, GetPath(copy), InteractionMode.UserAction, out bool success);
                if (!success) {
                    Debug.LogError($"Could not create prefab for game object {copy.name}");
                }
            }

            GameObject.DestroyImmediate(copy);
        }

        private static string GetPath(GameObject copy) {
            return $"Assets/Prefabs/Props/{copy.name}.prefab";
        }
    }
}
