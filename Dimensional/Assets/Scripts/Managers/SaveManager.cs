using System;
using System.Collections.Generic;
using System.IO;
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
        public List<string> actionShapes;
    }

    [Serializable]
    public class CollectableData
    {
        public int collectables;
        public List<string> keys;
        public List<string> tickets;
        public List<string> shapes;
    }

    [Serializable]
    public class QuestData
    {
        public List<int> questIds;
        public List<int> completedQuestIds;
    }

    [Serializable]
    public class WorldData
    {
        public int regionId;
        public int levelId;
        public int checkpointId;
    }

    [Serializable]
    public class SceneData
    {
        public string scene;
    }

    [Serializable]
    public class SaveData
    {
        public PlayerData playerData;
        public ActionData actionData;
        public CollectableData collectableData;
        public QuestData questData;
        public WorldData worldData;
        public SceneData sceneData;
    }
    
    public class SaveManager : SingletonPersistent<SaveManager>
    {
        public delegate void SaveDataEventHandler(SaveData saveData, List<DataType> dataTypes);
        public static event SaveDataEventHandler Saving;
        public static event SaveDataEventHandler Loaded;

        [SerializeField] private bool saveOnQuit;

        private const string SaveFolderName = "SpheroSave";
        private const string PlayerSaveFileName = "PlayerSave";
        private const string ActionSaveFileName = "ActionSave";
        private const string CollectableSaveFileName = "CollectableSave";
        private const string QuestSaveFileName = "QuestSave";
        private const string WorldSaveFileName = "WorldSave";
        private const string SceneSaveFileName = "SceneSave";
        
        private SaveData _saveData;

        public static int SaveId { get; set; }
        
        private void Awake()
        {
            _saveData = new SaveData();
        }

        public void RequestSave(List<DataType> dataTypes)
        {
            RequestLoad(dataTypes, false);
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
            return JsonUtility.FromJson<T>(json);
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
            RequestSave(new List<DataType>() { DataType.Player, DataType.Action, DataType.Collectable, DataType.Quest, DataType.World });
        }
    }
}