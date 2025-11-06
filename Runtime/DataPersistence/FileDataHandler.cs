using System;
using System.IO;
using UnityEngine;

namespace UnityUtility.DataPersistence
{
    public class FileDataHandler
    {
        private readonly string dataDirPath;
        private readonly string dataFileName;
        private readonly bool useEncryption;
        private const string encryptionCodeWord = "word";

        public FileDataHandler(string dataDirPath, string dataFileName, bool useEncrypt)
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
            this.useEncryption = useEncrypt;
        }

        public T Load<T>() where T : class
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            T loadedData = null;

            if (File.Exists(fullPath))
            {
                try
                {
                    string dataToLoad;
                    using (FileStream stream = new(fullPath, FileMode.Open))
                    using (StreamReader reader = new(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }

                    if (useEncryption)
                        dataToLoad = EncryptDecrypt(dataToLoad);

                    loadedData = JsonUtility.FromJson<T>(dataToLoad);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error occurred when trying to load data from file: {fullPath}\n{ex}");
                }
            }

            return loadedData;
        }

        public void Save<T>(T data)
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string dataToStore = JsonUtility.ToJson(data, true);

                if (useEncryption)
                    dataToStore = EncryptDecrypt(dataToStore);

                using (FileStream stream = new(fullPath, FileMode.Create))
                using (StreamWriter writer = new(stream))
                {
                    writer.Write(dataToStore);
                }

#if UNITY_EDITOR
                Debug.Log($"Saved data to: {fullPath}");
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error occurred when trying to save data to file: {fullPath}\n{ex}");
            }
        }

        private string EncryptDecrypt(string data)
        {
            char[] modifiedChars = new char[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                modifiedChars[i] = (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
            }
            return new string(modifiedChars);
        }
    }
}
