using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DialogueQuests
{
    /// <summary>
    /// Script to write a class to the disk, or to read a file containing class from the disk
    /// </summary>

    [System.Serializable]
    public class SaveSystem
    {
        private const string last_save_id = "last_save_dq";
        private const string extension = ".dq.data";

        //Load any file to a class, make sure the class is marked with [System.Serializable]
        public static T LoadFile<T>(string filename) where T : class
        {
            T data = null;
            if (File.Exists(Application.persistentDataPath + "/" + filename + extension))
            {
                FileStream file = null;
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    file = File.Open(Application.persistentDataPath + "/" + filename + extension, FileMode.Open);
                    data = (T)bf.Deserialize(file);
                    file.Close();
                }
                catch (System.Exception e) { Debug.Log("Error Loading Data " + e); if (file != null) file.Close(); }
            }
            return data;
        }

        //Save any class to a file, make sure the class is marked with [System.Serializable]
        public static void SaveFile<T>(string filename, T data) where T : class
        {
            FileStream file = null;
            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                file = File.Create(Application.persistentDataPath + "/" + filename + extension);
                bf.Serialize(file, data);
                file.Close();
            }
            catch (System.Exception e) { Debug.Log("Error Saving Data " + e); if (file != null) file.Close(); }
        }

        public static void DeleteFile(string filename)
        {
            string path = (Application.persistentDataPath + "/" + filename + extension);
            if (File.Exists(path))
                File.Delete(path);
        }

        public static void SetLastSave(string filename)
        {
            PlayerPrefs.SetString(last_save_id, filename);
        }

        public static string GetLastSave()
        {
            return PlayerPrefs.GetString(last_save_id, "");
        }
    }

}