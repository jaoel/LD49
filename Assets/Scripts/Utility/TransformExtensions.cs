using UnityEngine;

public static class TransformExtensions {
    public static Transform FindRecursive(this Transform transform, string name) {
        Transform found = transform.Find(name);
        if (found != null) {
            return found;
        }

        foreach (Transform child in transform) {
            found = FindRecursive(child, name);
            if (found != null) {
                return found;
            }
        }

        return null;
    }
}