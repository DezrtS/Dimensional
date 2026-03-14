using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scriptables.Save;
using Systems.Quests;
using UnityEngine;
using Utilities;

namespace Managers
{
    public enum DataType
    {
        Player,
        Action,
        Collectable,
        Quest,
        World,
        Scene
    }
    
    [Serializable]
    public class PlayerData
    {
        public int health;
    }

    [Serializable]
    public class ActionData
    {
        public List<string> actionShapes = new();
    }

    [Serializable]
    public class CollectableData
    {
        public int collectables;
        public List<string> collectedCollectables = new();
        public List<string> collectedKeys = new();
        public List<string> collectedTickets = new();
        public List<string> collectedShapes = new();
    }

    [Serializable]
    public class QuestData
    {
        public List<QuestInstanceData> quests = new();
    }

    [Serializable]
    public class WorldData
    {
        public int regionId;
        public int levelId;
        public string checkpointId;
        public List<string> completedTutorials = new();
        public List<string> completedCutscenes = new();
    }

    [Serializable]
    public class SceneData
    {
        public string scene;
    }

    [Serializable]
    public class SaveData
    {
        public PlayerData playerData = new();
        public ActionData actionData = new();
        public CollectableData collectableData = new();
        public QuestData questData = new();
        public WorldData worldData = new();
        public SceneData sceneData = new();
    }

    [Serializable]
    public class SaveEntry
    {
        public string id;
        public string value;
    }

    [Serializable]
    public class SaveData2
    {
        public List<SaveEntry> entries = new();
    }
    
    public class SaveManager : SingletonPersistent<SaveManager>
    {
        public delegate void SaveDataEventHandler(SaveData saveData, List<DataType> dataTypes);
        public static event SaveDataEventHandler Saving;
        public static event SaveDataEventHandler Loaded;
        
        [SerializeField] private List<SaveVariable> dirtyVariables = new List<SaveVariable>();
        [SerializeField] private bool resetAllOnAwake;
        
        [SerializeField] private bool saveOnQuit;
        [SerializeField] private List<DataType> saveOnQuitTypes;

        private const string SaveFolderName = "SpheroSave";
        private const string SaveFileName = "Save";
        private const string PlayerSaveFileName = "PlayerSave";
        private const string ActionSaveFileName = "ActionSave";
        private const string CollectableSaveFileName = "CollectableSave";
        private const string QuestSaveFileName = "QuestSave";
        private const string WorldSaveFileName = "WorldSave";
        private const string SceneSaveFileName = "SceneSave";
        
        private static readonly List<SaveVariable> SaveVariables = new();
        private SaveData _saveData;

        public static int SaveId { get; set; }
        
        private void Awake()
        {
            _saveData = new SaveData();
            if (resetAllOnAwake)
            {
                ResetAll();
            }
            else
            {
                Load();   
            }
        }

        public void SetDirty(SaveVariable saveVariable)
        {
            if (!dirtyVariables.Contains(saveVariable)) dirtyVariables.Add(saveVariable);
        }

        public static void Register(SaveVariable saveVariable)
        {
            if (SaveVariables.Contains(saveVariable)) return;
            SaveVariables.Add(saveVariable);
        }

        public static void ResetAll()
        {
            foreach (var saveVariable in SaveVariables)
            {
                saveVariable.Reset();
            }
            Save(string.Empty, SaveFileName);
        }

        private static void Load()
        {
            var saveData = Load<SaveData2>(SaveFileName);
            if (saveData == null)
            {
                ResetAll();
                return;
            }
            
            var dictionary = saveData.entries.ToDictionary(x => x.id, x => x);
            foreach (var saveVariable in SaveVariables)
            {
                if (!saveVariable.Load || !dictionary.ContainsKey(saveVariable.Id))
                {
                    saveVariable.Reset();
                    continue;
                }
                saveVariable.Restore(dictionary[saveVariable.Id].value);
            }
        }

        private void Save()
        {
            if (dirtyVariables.Count <= 0) return;

            var saveData = Load<SaveData2>(SaveFileName) ?? new SaveData2();
            var dictionary = saveData.entries.ToDictionary(x => x.id, x => x);
            foreach (var dirtyVariable in dirtyVariables)
            {
                if (!dirtyVariable.Save) continue;
                var saveEntry = new SaveEntry { id = dirtyVariable.Id, value = dirtyVariable.Capture() };
                dictionary[dirtyVariable.Id] = saveEntry;
            }
            saveData.entries = dictionary.Values.ToList();
            Save(saveData, SaveFileName);
        }

        public void RequestSave(List<DataType> dataTypes)
        {
            //RequestLoad(dataTypes, false);
            _saveData = new SaveData() { collectableData = _saveData.collectableData };
            Saving?.Invoke(_saveData, dataTypes);
            foreach (var dataType in dataTypes)
            {
                switch (dataType)
                {
                    case DataType.Player:
                        Save(_saveData.playerData, PlayerSaveFileName);
                        break;
                    case DataType.Action:
                        Save(_saveData.actionData, ActionSaveFileName);
                        break;
                    case DataType.Collectable:
                        Save(_saveData.collectableData, CollectableSaveFileName);
                        break;
                    case DataType.Quest:
                        Save(_saveData.questData, QuestSaveFileName);
                        break;
                    case DataType.World:
                        Save(_saveData.worldData, WorldSaveFileName);
                        break;
                    case DataType.Scene:
                        Save(_saveData.sceneData, SceneSaveFileName);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
                }   
            }
        }
        
        public void RequestLoad(List<DataType> dataTypes, bool invokeLoaded = true)
        {
            var loadedDataTypes = new List<DataType>();
            foreach (var dataType in dataTypes)
            {
                switch (dataType)
                {
                    case DataType.Player:
                        var playerSaveData = Load<PlayerData>(PlayerSaveFileName);
                        if (playerSaveData == null)
                        {
                            _saveData.playerData = new PlayerData();
                            continue;
                        }
                        _saveData.playerData = playerSaveData;
                        break;
                    case DataType.Action:
                        var actionSaveData = Load<ActionData>(ActionSaveFileName);
                        if (actionSaveData == null)
                        {
                            _saveData.actionData = new ActionData();
                            continue;
                        }
                        _saveData.actionData = actionSaveData;
                        break;
                    case DataType.Collectable:
                        var collectableSaveData = Load<CollectableData>(CollectableSaveFileName);
                        if (collectableSaveData == null)
                        {
                            _saveData.collectableData = new CollectableData();
                            continue;
                        }
                        _saveData.collectableData = collectableSaveData;
                        break;
                    case DataType.Quest:
                        var questSaveData = Load<QuestData>(QuestSaveFileName);
                        if (questSaveData == null)
                        {
                            _saveData.questData = new QuestData();
                            continue;
                        }
                        _saveData.questData = questSaveData;
                        break;
                    case DataType.World:
                        var worldSaveData = Load<WorldData>(WorldSaveFileName);
                        if (worldSaveData == null)
                        {
                            _saveData.worldData = new WorldData();
                            continue;
                        }
                        _saveData.worldData = worldSaveData;
                        break;
                    case DataType.Scene:
                        var sceneSaveData = Load<SceneData>(SceneSaveFileName);
                        if (sceneSaveData == null)
                        {
                            _saveData.sceneData = new SceneData();
                            continue;
                        }
                        _saveData.sceneData = sceneSaveData;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dataType), dataType, null);
                }
                loadedDataTypes.Add(dataType);
            }
            if (invokeLoaded) Loaded?.Invoke(_saveData, loadedDataTypes);
        }

        private static T Load<T>(string fileName) where T : class
        {
            var folderPath = Path.Combine(Application.persistentDataPath, SaveFolderName + SaveId);
            var filePath = Path.Combine(folderPath, fileName);
            if (!Directory.Exists(folderPath) || !File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            return string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<T>(json);
        }

        private static void Save<T>(T data, string fileName)
        {
            var folderPath = Path.Combine(Application.persistentDataPath, SaveFolderName + SaveId);
            var filePath = Path.Combine(folderPath, fileName);
            if (data == null) return;
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            var json = JsonUtility.ToJson(data);
            File.WriteAllText(filePath, json);
        }

        private void OnApplicationQuit()
        {
            if (!saveOnQuit) return;
            Save();
            RequestSave(saveOnQuitTypes);
        }
    }
}