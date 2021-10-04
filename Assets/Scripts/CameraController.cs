using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    public class CameraController : MonoBehaviour {
        public Vector3 offset;
        public Transform target;

        private void Update() {
            if (target == null) {
                GameObject player = GameObject.Find("Player");
                if (player != null) {
                    target = player.transform.Find("Visual/Player/Armature/root/spine1").transform;
                }
            }
        }

        private void LateUpdate() {
            if (target != null) {
                transform.position = Vector3.Lerp(transform.position, target.position + offset, 2f * Time.deltaTime);
            }
        }
    }
}
