using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Xml.Serialization;
using System.IO;

public struct SavedGame
{
    public string UserName;
    public int Coins;
    public int TimeInMazeSeconds;
    public string GameOverReason;
    public DateTime GameBegin;
}

public static class SavesManager
{
    
    public static List<SavedGame> LoadGames()
    {
        var filePath = Path.Combine(Application.dataPath, "data.xml");
        List<SavedGame> SavedGames = new List<SavedGame>();
        if (!File.Exists(filePath))
        {
            return SavedGames;
        }        
        
        XmlSerializer writer = new XmlSerializer(typeof(List<SavedGame>));
        using (StreamReader file = new StreamReader(filePath))
        {
            SavedGames = (List<SavedGame>)writer.Deserialize(file);
        }
        return SavedGames;
    }

    public static void SaveGame(SavedGame savedGame)
    {
        var SavedGames = LoadGames();
        SavedGames.Add(savedGame);

        var filePath = Path.Combine(Application.dataPath, "data.xml");
        XmlSerializer writer = new XmlSerializer(typeof(List<SavedGame>));
        using(StreamWriter file = new StreamWriter(filePath))
        {
            writer.Serialize(file, SavedGames);
        }
    }

}
