using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace UnityUtility.DataPersistence
{
    public class DataPersistenceManager<TGameData>
        where TGameData : class, new()
    {
        [Header("File Storage Config")]
        [SerializeField] private bool save = true;
        [SerializeField] private string fileName = "save.dat";
        [SerializeField] private bool useEncryption = true;

        private FileDataHandler dataHandler;
        private List<IDataPersistence<TGameData>> dataPersistenceObjects;

        public Action OnLoad;
        public Action OnSave;
        public Action OnNewGame;

        public TGameData gameData;

        private bool isInitialized = false;
        private bool autoUpdateDataPersistenceObjects = false;

        public void Init()
        {
            if (!save) return;

            dataHandler = new FileDataHandler(
                Application.persistentDataPath,
                fileName,
                useEncryption
            );

            dataPersistenceObjects = FindAllDataPersistenceObjects();
            isInitialized = true;
        }

        public void StartGame()
        {
            if (!save || !isInitialized) return;

            LoadGame();
        }

        public void NewGame()
        {
            if (!save) return;

            gameData = new TGameData();
            OnNewGame?.Invoke();
        }

        public void LoadGame()
        {
            if (!save || !isInitialized) return;

            gameData = dataHandler.Load<TGameData>();

            if (gameData == null)
            {
                Debug.LogWarning("No valid save found. Creating new game.");
                NewGame();
            }


            foreach (var obj in dataPersistenceObjects)
            {
                try
                {
                    obj.LoadData(gameData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error loading data into {obj}: {ex}");
                }
            }

            OnLoad?.Invoke();
        }

        public void SaveGame()
        {
            if (!save || !isInitialized) return;

            if (autoUpdateDataPersistenceObjects)
            {
                dataPersistenceObjects = FindAllDataPersistenceObjects();
            }

            foreach (var obj in dataPersistenceObjects)
            {
                try
                {
                    obj.SaveData(ref gameData);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error saving data from {obj}: {ex}");
                }
            }

            dataHandler.Save(gameData);

            OnSave?.Invoke();
        }

        public bool HaveSaves()
        {
            return save && dataHandler != null && dataHandler.SaveExists();
        }

        public void HandleApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
                SaveGame();
        }

        public void HandleApplicationQuit()
        {
            SaveGame();
        }

        public void UpdateReference()
        {
            dataPersistenceObjects = FindAllDataPersistenceObjects();
        }

        private List<IDataPersistence<TGameData>> FindAllDataPersistenceObjects()
        {
            return UnityEngine.Object
                .FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IDataPersistence<TGameData>>()
                .ToList();
        }
    }
}