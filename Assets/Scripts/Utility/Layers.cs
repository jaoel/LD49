using UnityEngine;

namespace LD49 {
    public static class Layers {
        public static int Default = LayerMask.NameToLayer("Default");
        public static int TransparentFX = LayerMask.NameToLayer("TransparentFX");
        public static int IgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        public static int Trigger = LayerMask.NameToLayer("Trigger");
        public static int Water = LayerMask.NameToLayer("Water");
        public static int UI = LayerMask.NameToLayer("UI");
        public static int Props = LayerMask.NameToLayer("Props");
        public static int RagdollTrigger = LayerMask.NameToLayer("RagdollTrigger");
        public static int Wall = LayerMask.NameToLayer("Wall");

        public static int GetMask(int layer) {
            return 1 << layer;
        }

        public static int GetMask(params int[] layers) {
            int mask = 0;
            for (int i = 0; i < layers.Length; i++) {
                mask |= 1 << layers[i];
            }
            return mask;
        }
    }
}