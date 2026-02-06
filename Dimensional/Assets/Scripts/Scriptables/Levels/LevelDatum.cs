using Managers;
using UnityEngine;

namespace Scriptables.Levels
{
    public enum LevelType
    {
        None,
        Objective,
        TimeTrial,
        Special,
    }
    
    [CreateAssetMenu(fileName = "LevelDatum", menuName = "Scriptable Objects/Levels/LevelDatum")]
    public class LevelDatum : ScriptableObject
    {
        [SerializeField] private string levelId;
        [Space] 
        [SerializeField] private string levelName;
        [SerializeField] private RegionType regionType;
        [SerializeField] private LevelType levelType;
        [SerializeField] private int collectablesCount;
        
        public string LevelId => levelId;
        
        public string LevelName => levelName;
        public RegionType RegionType => regionType;
        public LevelType LevelType => levelType;
        public int CollectablesCount => collectablesCount;
    }
}
