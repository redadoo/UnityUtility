using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Security.Cryptography;

namespace UnityUtility.DataPersistence
{
    public class FileDataHandler
    {
        private readonly string dataDirPath;
        private readonly string dataFileName;
        private readonly bool useEncryption;

        private const string SALT = "your-game-salt-change-this";
        private const int KEY_SIZE = 32;
        private const int HMAC_SIZE = 32;

        public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption)
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
            this.useEncryption = useEncryption;
        }

        public T Load<T>() where T : class
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);

            if (!File.Exists(fullPath))
                return null;

            try
            {
                byte[] fileBytes = File.ReadAllBytes(fullPath);

                string json;

                if (useEncryption)
                {
                    if (fileBytes.Length < HMAC_SIZE)
                        throw new Exception("Invalid file format");

                    byte[] storedHmac = new byte[HMAC_SIZE];
                    byte[] encryptedData = new byte[fileBytes.Length - HMAC_SIZE];

                    Array.Copy(fileBytes, 0, storedHmac, 0, HMAC_SIZE);
                    Array.Copy(fileBytes, HMAC_SIZE, encryptedData, 0, encryptedData.Length);

                    byte[] key = GetKey();

                    byte[] computedHmac = ComputeHMAC(encryptedData, key);
                    if (!storedHmac.SequenceEqual(computedHmac))
                    {
                        Debug.LogError("Save file has been tampered with or corrupted.");
                        return null;
                    }

                    json = Decrypt(encryptedData, key);
                }
                else
                {
                    json = Encoding.UTF8.GetString(fileBytes);
                }

                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading file: {fullPath}\n{ex}");
                return null;
            }
        }

        public void Save<T>(T data)
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string json = JsonUtility.ToJson(data, true);
                byte[] fileBytes;

                if (useEncryption)
                {
                    byte[] key = GetKey();
                    byte[] encryptedData = Encrypt(json, key);
                    byte[] hmac = ComputeHMAC(encryptedData, key);

                    fileBytes = new byte[hmac.Length + encryptedData.Length];

                    Array.Copy(hmac, 0, fileBytes, 0, hmac.Length);
                    Array.Copy(encryptedData, 0, fileBytes, hmac.Length, encryptedData.Length);
                }
                else
                {
                    fileBytes = Encoding.UTF8.GetBytes(json);
                }

                File.WriteAllBytes(fullPath, fileBytes);

#if UNITY_EDITOR
                Debug.Log($"Saved data to: {fullPath}");
#endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error saving file: {fullPath}\n{ex}");
            }
        }

        public bool SaveExists()
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            return File.Exists(fullPath);
        }

        private byte[] GetKey()
        {
            string deviceId = SystemInfo.deviceUniqueIdentifier;

            using var derive = new Rfc2898DeriveBytes(
                deviceId,
                Encoding.UTF8.GetBytes(SALT),
                10000,
                HashAlgorithmName.SHA256
            );

            return derive.GetBytes(KEY_SIZE);
        }
        
        private byte[] Encrypt(string plainText, byte[] key)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();

            using MemoryStream ms = new();

            ms.Write(aes.IV, 0, aes.IV.Length);

            using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            using (StreamWriter sw = new(cs))
            {
                sw.Write(plainText);
            }

            return ms.ToArray();
        }

        private string Decrypt(byte[] encryptedData, byte[] key)
        {
            using Aes aes = Aes.Create();
            aes.Key = key;

            byte[] iv = new byte[aes.BlockSize / 8];
            Array.Copy(encryptedData, iv, iv.Length);

            aes.IV = iv;

            using MemoryStream ms = new(encryptedData, iv.Length, encryptedData.Length - iv.Length);
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader sr = new(cs);

            return sr.ReadToEnd();
        }

        private byte[] ComputeHMAC(byte[] data, byte[] key)
        {
            using var hmac = new HMACSHA256(key);
            return hmac.ComputeHash(data);
        }
    }
}