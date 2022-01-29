using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    [CreateAssetMenu(fileName = "Level Holder", menuName = "LD49/Create Level Holder")]
    public class LevelHolder : ScriptableObject {
        public List<SceneReference> levels = new List<SceneReference>();

        public bool TryGetLevel(int levelIndex, out GameScene levelScene) {
            if (levelIndex >= 0 && levelIndex < levels.Count) {
                levelScene = new GameScene() {
                    fullPathWithExtension = levels[levelIndex].ScenePath,
                };
                return true;
            }

            levelScene = null;
            return false;
        }

        public bool TryGetLevel(string levelScenePath, out GameScene levelScene, out int index) {
            for (int i = 0; i < levels.Count; i++) {
                SceneReference level = levels[i];
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
                SceneReference level = levels[i];
                var levelScene = new GameScene() { fullPathWithExtension = level.ScenePath };
                if (levelScene.IsLoaded()) {
                    index = i;
                    return levelScene;
                }
            }

            index = -1;
            return null;
        }

        private void OnValidate() {
            HashSet<string> nameEntries = new HashSet<string>();
            foreach (var level in levels) {
                if (nameEntries.Contains(level.ScenePath)) {
                    Debug.LogError($"Duplicate level found in {name}: '{level.ScenePath}'");
                }
                nameEntries.Add(level.ScenePath);
            }
        }
    }
}
