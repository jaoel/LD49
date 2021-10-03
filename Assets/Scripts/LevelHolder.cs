using System.Collections.Generic;
using UnityEngine;

namespace LD49 {
    [CreateAssetMenu(fileName = "Level Holder", menuName = "LD49/Create Level Holder")]
    public class LevelHolder : ScriptableObject {
        public List<Level> levelPrefabs = new List<Level>();
    }
}
