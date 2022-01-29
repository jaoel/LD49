using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using UnityEngine.SceneManagement;

namespace LD49 {
    public class MainSceneBootstrapper : MonoBehaviour {
        private void Awake() {
            if (!Scenes.Core.IsLoaded()) {
                Scenes.Core.Load(LoadSceneMode.Additive);
            }
        }
    }
}
