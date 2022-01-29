using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LD49 {
    public class GameScene {
        /// <summary>
        /// This MUST be the full path to a scene, including its file extension
        /// For example: "Assets/Scenes/MainMenu.unity"
        /// </summary>
        public string fullPathWithExtension;

        private bool TryGetScene(out Scene scene) {
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene.path == fullPathWithExtension) {
                    return true;
                }
            }

            scene = new Scene();
            return false;
        }

        public bool IsLoaded() {
            return TryGetScene(out _);
        }

        public bool IsActiveScene() {
            Scene scene = SceneManager.GetActiveScene();
            return scene.path == fullPathWithExtension;
        }

        public void MakeActiveScene() {
            if (TryGetScene(out Scene scene)) {
                SceneManager.SetActiveScene(scene);
            }
        }

        public void Load(LoadSceneMode loadMode = LoadSceneMode.Single) {
            SceneManager.LoadScene(fullPathWithExtension, loadMode);
        }
        
        public AsyncOperation LoadAsync(LoadSceneMode loadMode = LoadSceneMode.Single) {
            return SceneManager.LoadSceneAsync(fullPathWithExtension, loadMode);
        }

        public void Unload() {
            if (IsLoaded()) {
                // Fire and forget?
                var _ = SceneManager.UnloadSceneAsync(fullPathWithExtension);
            }
        }
    }

    public static class Scenes {
        public static readonly GameScene MainMenu = new GameScene() {
            fullPathWithExtension = "Assets/Scenes/MainMenu.unity",
        };

        public static readonly GameScene End = new GameScene() {
            fullPathWithExtension = "Assets/Scenes/EndScene.unity",
        };

        public static readonly GameScene Core = new GameScene() {
            fullPathWithExtension = "Assets/Scenes/_GameCore.unity",
        };
    }
}
