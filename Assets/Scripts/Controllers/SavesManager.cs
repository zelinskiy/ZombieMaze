using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System;
using System.Xml.Serialization;
using System.IO;


/// <summary>
/// Used for storing information about game results
/// </summary>
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

    public const string SaveFileName = "saves.xml";

    /// <summary>
    /// Loads all saves from saves.xml
    /// </summary>
    /// <returns>List of all saved games</returns>
    public static List<SavedGame> LoadGames()
    {
        var filePath = Path.Combine(Application.dataPath, SaveFileName);
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

    /// <summary>
    /// Adds SavedGame to saves.xml
    /// </summary>
    /// <param name="savedGame">Game result to save</param>
    public static void SaveGame(SavedGame savedGame)
    {
        var SavedGames = LoadGames();
        SavedGames.Add(savedGame);

        var filePath = Path.Combine(Application.dataPath, SaveFileName);
        XmlSerializer writer = new XmlSerializer(typeof(List<SavedGame>));
        using(StreamWriter file = new StreamWriter(filePath))
        {
            writer.Serialize(file, SavedGames);
        }
    }

}
