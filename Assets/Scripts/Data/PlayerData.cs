using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class PlayerData {

    public const string VERSION = "0.01";

    public string filename;
    public string version;
    public DateTime last_save;

    //-------------------

    public string current_scene = "";
    public int current_entry_index;
    public string current_checkpoint = "";

    public float master_volume = 1f;
    public float music_volume = 1f;
    public float sfx_volume = 1f;

    public Dictionary<string, int> unique_ids = new Dictionary<string, int>();


    //-------------------

    public static PlayerData player_data = null;
    
    public PlayerData(string name)
    {
        filename = name;
        version = VERSION;
        
        music_volume = 1f;
        sfx_volume = 1f;

        NewGame();
    }

    public void FixData()
    {
        //Fix data to make sure old save files compatible with new game version
        if (unique_ids == null)
            unique_ids = new Dictionary<string, int>();

    }
    
    public void NewGame()
    {

    }

    // ---- Unique Ids ----
    public void SetUniqueID(string unique_id, int val)
    {
        if (!string.IsNullOrEmpty(unique_id))
        {
            if (!unique_ids.ContainsKey(unique_id))
                unique_ids[unique_id] = val;
        }
    }

    public void RemoveUniqueID(string unique_id)
    {
        if (unique_ids.ContainsKey(unique_id))
            unique_ids.Remove(unique_id);
    }

    public int GetUniqueID(string unique_id)
    {
        if (unique_ids.ContainsKey(unique_id))
            return unique_ids[unique_id];
        return 0;
    }

    public bool HasUniqueID(string unique_id)
    {
        return unique_ids.ContainsKey(unique_id);
    }

    public void Save()
    {
        PlayerData.Get().last_save = System.DateTime.Now;
        version = VERSION;
        SaveSystem.Save(filename, player_data);
    }

    public void Restart()
    {
        player_data = new PlayerData(filename);
        player_data.FixData();
    }

    public static void Unload()
    {
        player_data = null;
        SaveSystem.Unload();
    }

    public void Delete()
    {
        SaveSystem.Delete(filename);
        player_data = new PlayerData(filename);
    }

    public static void Load(string name)
    {
        if (player_data == null)
            player_data = SaveSystem.Load(name);
        if (player_data == null)
            player_data = new PlayerData(name);
        player_data.FixData();
    }
    
    public static PlayerData Get()
    {
        return player_data;
    }
}
