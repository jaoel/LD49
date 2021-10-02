using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    public class CameraController : MonoBehaviour {
        public Vector3 offset;
        public Transform target;

        private void LateUpdate() {
            if (target != null) {
                transform.position = Vector3.Lerp(transform.position, target.position + offset, 2f * Time.deltaTime);
            }
        }
    }
}