using UnityEngine;
using UnityEditor;

namespace LD49.Editor {
    public class CreatePropsFromSelected {
        [MenuItem("Tools/Make Props")]
        static void DoSomething() {
            foreach (GameObject selected in Selection.gameObjects) {
                if (Selection.activeObject is GameObject gameObject) {
                    CreateOrReplacePrefab(selected);
                }
            } 
        }

        private static void CreateOrReplacePrefab(GameObject toCopy) {
            Material defaultMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/UnlitPalette.mat");
            GameObject copy = GameObject.Instantiate(toCopy);
            copy.name = toCopy.name;
            copy.transform.position = Vector3.zero;
            copy.transform.rotation = Quaternion.identity;

            Rigidbody rigidbody = copy.AddComponent<Rigidbody>();
            MeshCollider meshCollider = copy.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            Prop prop = copy.AddComponent<Prop>();

            foreach (var renderer in copy.GetComponentsInChildren<Renderer>()) {
                renderer.sharedMaterial = defaultMaterial;
            }

            PrefabUtility.SaveAsPrefabAssetAndConnect(copy, GetPath(copy), InteractionMode.UserAction, out bool success);
            if (!success) {
                Debug.LogError($"Could not create prefab for game object {copy.name}");
                return;
            }
        }

        private static string GetPath(GameObject copy) {
            return $"Assets/Prefabs/Props/{copy.name}.prefab";
        }
    }
}
