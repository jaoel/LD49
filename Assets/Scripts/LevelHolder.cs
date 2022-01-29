using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    [CreateAssetMenu(fileName = "Level Holder", menuName = "LD49/Create Level Holder")]
    public class LevelHolder : ScriptableObject {
        [System.Serializable]
        public class LevelEntry {
            public SceneReference scene;
            public FMODUnity.EventReference musicEvent;
        }

        public List<LevelEntry> levels = new List<LevelEntry>();

        public bool TryGetLevel(int levelIndex, out GameScene levelScene) {
            if (levelIndex >= 0 && levelIndex < levels.Count) {
                levelScene = new GameScene() {
                    fullPathWithExtension = levels[levelIndex].scene.ScenePath,
                };
                return true;
            }

            levelScene = null;
            return false;
        }

        public bool TryGetLevel(string levelScenePath, out GameScene levelScene, out int index) {
            for (int i = 0; i < levels.Count; i++) {
                SceneReference level = levels[i].scene;
                if (level.ScenePath == levelScenePath) {
                    levelScene = new GameScene() {
                        fullPathWithExtension = level.ScenePath,
                    };

                    index = i;
                    return true;
                }
            }

            Debug.LogError($"Could not find level with scene path '{levelScenePath}'");
            levelScene = null;
            index = -1;
            return false;
        }

        public GameScene GetLoadedLevelScene(out int index) {
            for (int i = 0; i < levels.Count; i++) {
                SceneReference level = levels[i].scene;
                var levelScene = new GameScene() { fullPathWithExtension = level.ScenePath };
                if (levelScene.IsLoaded()) {
                    index = i;
                    return levelScene;
                }
            }

            index = -1;
            return null;
        }

        public FMODUnity.EventReference GetMusicEvent(int levelIndex) {
            if (levelIndex >= 0 && levelIndex < levels.Count) {
                return levels[levelIndex].musicEvent;
            }
            return new FMODUnity.EventReference();
        }
    }
}
