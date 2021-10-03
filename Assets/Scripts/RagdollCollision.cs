using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollCollision : MonoBehaviour {

    [SerializeField]
    private Rigidbody ragdollPushBody = null;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    private void OnTriggerEnter(Collider other) {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Props") {
            ragdollPushBody.AddForce(Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), transform.forward) * Quaternion.AngleAxis(Random.Range(-65.0f, 65.0f), Vector3.up)
                * (-transform.forward + Vector3.up) * Random.Range(25, 35), ForceMode.Impulse);

            other.attachedRigidbody.AddForce((-transform.forward + Vector3.up) * Random.Range(75, 150), ForceMode.Impulse);
        }
    }
}
