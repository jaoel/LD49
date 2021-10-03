using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    public class Wall : MonoBehaviour {
        private void Awake() {
            GetComponent<MeshRenderer>().enabled = false;
        }
        // Start is called before the first frame update
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        private void OnCollisionEnter(Collision collision) {

            //Player player = collision.collider.GetComponent<Player>();
            //
            //player.Too
            //collision.collider.attachedRigidbody.AddForce(-collision.collider.transform.forward * 20.0f, ForceMode.Impulse);
        }
    }
}

