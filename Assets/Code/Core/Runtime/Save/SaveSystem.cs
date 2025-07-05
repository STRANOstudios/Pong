namespace AndreaFrigerio.Core.Runtime.Save
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// Serializes any object to JSON, stores it in
    /// <see cref="Application.persistentDataPath"/>, and can encrypt the
    /// payload with AES-128.
    /// </summary>
    public static class SaveSystem
    {
        #region Constants

        private const string EncryptionKey = "AndreaFrigerio01"; // 16 bytes
        private const bool UseEncryption = false;

        #endregion

        #region Internal helpers

        private static string PathFor(string file) =>
            Path.Combine(Application.persistentDataPath, file + ".json");

        #endregion

        #region Public API

        /// <summary>
        /// Saves an object of type <typeparamref Name="T"/> to a file, optionally encrypted.
        /// </summary>
        /// <typeparam Name="T">The type of the data to save.</typeparam>
        /// <param Name="data">The data object to save.</param>
        /// <param Name="fileName">The Name of the file (without extension).</param>
        public static void Save<T>(T data, string fileName)
        {
            string json = JsonUtility.ToJson(data);

            string encryptedJson = UseEncryption ? Encrypt(json, EncryptionKey) : json;

            File.WriteAllText(PathFor(fileName), encryptedJson);

            Debug.Log("<color=green>Saved</color> " + fileName);
        }

        /// <summary>
        /// Loads an object of type <typeparamref Name="T"/> from a file, optionally decrypting it.
        /// </summary>
        /// <typeparam Name="T">The type of the data to load.</typeparam>
        /// <param Name="fileName">The Name of the file (without extension).</param>
        /// <returns>The deserialized data object, or a new instance if not found or empty.</returns>
        public static T Load<T>(string fileName) where T : new()
        {
            string path = PathFor(fileName);
            if (File.Exists(path))
            {
                string encryptedJson = File.ReadAllText(path);

                if (string.IsNullOrEmpty(encryptedJson)) return new T();

                string json = UseEncryption ? Decrypt(encryptedJson, EncryptionKey) : encryptedJson;

                Debug.Log("<color=green>Loaded</color> " + fileName);

                return JsonUtility.FromJson<T>(json);
            }

            Debug.Log("<color=red>File not found</color> " + fileName);

            return new T();
        }

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param Name="fileName">The Name of the file (without extension).</param>
        /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
        public static bool Exists(string fileName)
        {
            return File.Exists(PathFor(fileName));
        }

        /// <summary>
        /// Deletes a saved file.
        /// </summary>
        /// <param Name="fileName">The Name of the file (without extension).</param>
        public static void Delete(string fileName)
        {
            string path = PathFor(fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        #endregion

        #region Encryption (AES-128)

        /// <summary>
        /// Encrypts a plain text using AES-128 algorithm.
        /// </summary>
        /// <param Name="plainText">The plain text to encrypt.</param>
        /// <param Name="key">The encryption key. It must be exactly 16 bytes/characters long.</param>
        /// <returns>The encrypted text in Base64 format.</returns>
        /// <exception cref="ArgumentException">Thrown if the key is not exactly 16 bytes/characters long.</exception>
        private static string Encrypt(string plainText, string key)
        {
            // Convert the key to bytes using UTF8 encoding
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            // Check if the key is exactly 16 bytes/characters long
            if (keyBytes.Length != 16)
            {
                throw new ArgumentException("The key must be exactly 16 bytes/characters long.");
            }

            // Create a new instance of the AES algorithm
            using Aes aes = Aes.Create();

            // Set the encryption key
            aes.Key = keyBytes;

            // Generate a new IV (Initialization Vector)
            aes.GenerateIV();

            // Set the padding mode to PKCS7
            aes.Padding = PaddingMode.PKCS7;

            // Create a new MemoryStream
            using MemoryStream ms = new();

            // Write the IV to the MemoryStream
            ms.Write(aes.IV, 0, aes.IV.Length);

            // Create a new CryptoStream using the MemoryStream and the AES encryptor
            using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                // Create a new StreamWriter using the CryptoStream
                using StreamWriter writer = new(cs);

                // Write the plain text to the StreamWriter
                writer.Write(plainText);
            }

            // Return the encrypted text in base64 format
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Decrypts a cipher text using AES-128 algorithm.
        /// </summary>
        /// <param Name="cipherText">The cipher text to decrypt (in Base64 format).</param>
        /// <param Name="key">The decryption key. It must be exactly 16 bytes/characters long.</param>
        /// <returns>The decrypted plain text.</returns>
        /// <exception cref="ArgumentException">Thrown if the key is not exactly 16 bytes/characters long.</exception>
        private static string Decrypt(string cipherText, string key)
        {
            // Convert the cipher text to bytes using Base64 decoding
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            // Extract the initialization vector (IV) from the cipher bytes
            byte[] iv = new byte[16];
            Array.Copy(cipherBytes, iv, iv.Length);

            // Extract the encrypted data from the cipher bytes
            byte[] encryptedData = new byte[cipherBytes.Length - iv.Length];
            Array.Copy(cipherBytes, iv.Length, encryptedData, 0, encryptedData.Length);

            // Convert the key to bytes using UTF8 encoding
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            if (keyBytes.Length != 16)
            {
                // Throw an exception if the key is not exactly 16 bytes/characters long
                throw new ArgumentException("The key must be exactly 16 bytes/characters long.");
            }

            // Create a new instance of the AES algorithm
            using Aes aes = Aes.Create();
            aes.Key = keyBytes; // Set the decryption key
            aes.IV = iv; // Set the initialization vector
            aes.Padding = PaddingMode.PKCS7; // Set the padding mode to PKCS7

            // Create a new MemoryStream to hold the encrypted data
            using MemoryStream ms = new(encryptedData);
            // Create a new CryptoStream to decrypt the data
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            // Create a new StreamReader to read the decrypted data
            using StreamReader reader = new(cs);
            // Return the decrypted text
            return reader.ReadToEnd();
        }

        #endregion
    }
}
