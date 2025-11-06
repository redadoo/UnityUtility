using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace UnityUtility.DataPersistence
{
    public class DataPersistenceManager<TGameData> : GenericSingleton<DataPersistenceManager<TGameData>>
        where TGameData : class, new()
    {
        [Header("File Storage Config")]
        [SerializeField] private string fileName;
        [SerializeField] private bool useEncryption;
        [SerializeField] private bool save;

        private TGameData gameData;

        private List<IDataPersistence<TGameData>> dataPersistenceObjects;
        private FileDataHandler dataHandler;

        protected override void Awake()
        {
            base.Awake();

            if (save)
            {
                dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
                dataPersistenceObjects = FindAllDataPersistenceObjects();
                LoadGame();
            }
        }

        private void Start()
        {
            if (save)
            {
                dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
                dataPersistenceObjects = FindAllDataPersistenceObjects();
                LoadGame();
            }
        }

        public void NewGame()
        {
            if (!save) return;
            gameData = new TGameData();
        }

        public void LoadGame()
        {
            if (!save) return;

            gameData = dataHandler.Load<TGameData>();

            if (gameData == null)
            {
                Debug.Log("No data was found. Initializing data to defaults.");
                NewGame();
            }

            foreach (var obj in dataPersistenceObjects)
                obj.LoadData(gameData);
        }

        public void SaveGame()
        {
            if (!save) return;

            foreach (var obj in dataPersistenceObjects)
                obj.SaveData(ref gameData);

            dataHandler.Save(gameData);
        }

        private void OnApplicationQuit()
        {
            if (save)
                SaveGame();
        }

        private List<IDataPersistence<TGameData>> FindAllDataPersistenceObjects()
        {
            var objs = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
                .OfType<IDataPersistence<TGameData>>();

            return new List<IDataPersistence<TGameData>>(objs);
        }
    }
}
