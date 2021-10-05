using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts {
    public class RemoveInWebgl : MonoBehaviour {
        private void Start() {
#if UNITY_WEBGL
            Destroy(gameObject);
#endif
        }
    }
}
