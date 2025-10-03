using System;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public enum SaveType
    {
        None,
        Player,
        World,
        Collectables,
        All,
    }
    
    public class SaveManager : MonoBehaviour
    {
        
        public delegate void SaveDataEventHandler(SaveType saveType, Dictionary<string, object> data);
        public static event SaveDataEventHandler SaveStarted;
        public static event Action SaveFinished;

        public static event Action LoadStarted;
        public static event SaveDataEventHandler LoadFinished;
        
        private static string PlayerSaveFileName = "PlayerSave";
        private static string WorldSaveFileName = "WorldSave";
        private static string CollectablesSaveFileName = "CollectablesSave";

        private Dictionary<string, object> saveData;

        private void Start()
        {
            saveData = new Dictionary<string, object>();
            LoadStarted?.Invoke();
            LoadAllData(ref saveData);
            LoadFinished?.Invoke(SaveType.All, saveData);
        }

        private void LoadAllData(ref Dictionary<string, object> data)
        {
            LoadPlayer(ref data);
            LoadWorld(ref data);
            LoadCollectables(ref data);
        }

        private void LoadPlayer(ref Dictionary<string, object> data)
        {
            
        }
        
        private void LoadWorld(ref Dictionary<string, object> data)
        {
            
        }
        
        private void LoadCollectables(ref Dictionary<string, object> data)
        {
            
        }
    }
}
