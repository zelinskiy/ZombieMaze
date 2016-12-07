using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerController : MonoBehaviour {

    public int SimplicityLevel;

    public PlayerController Player;
    public MazeController Maze;
    public Text CoinsDisplayText;
    public Text UserNameText;

    public int MazeWidth;
    public int MazeHeight;

    public int MaximumCoins = 10;
    public int CoinsSpawnDelaySeconds = 5;
    public int PlayerCoins = 0;
    private int CoinsInMazeCount
    {
        get
        {
            return Maze.RoomsControllers.Count(rc => rc.HasCoin);
        }
    }   
    
    public GameObject CoinPrefab;
    public GameObject ZombiePrefab;
    public GameObject MummyPrefab;

    public GameObject GameOverUILayer;
    public Text GameOverStatsText;

    public List<ZombieController> Enemies
    {
        get
        {
            var res = new List<ZombieController>();
            res.AddRange(Zombies);
            if (Mummy != null)
            {
                res.Add(Mummy);
            }
            return res;
        }
    }
    public List<MovableController> Actors
    {
        get
        {
            var res = new List<MovableController>();
            res.Add(Player);
            res.AddRange(Enemies.Select(e => e as MovableController));
            return res;
        }
    }

    public List<ZombieController> Zombies
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("Zombie")
                .Select(z => z.GetComponent<ZombieController>())
                .ToList();
        }
    }
    public MummyController Mummy
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("Mummy")
                .Select(m => m.GetComponent<MummyController>())
                .FirstOrDefault();
        }
    }

    private DateTime GameStartedAt;

    void Awake()
    {
        Player.GameManager = this;
        Maze.GameManager = this;

        MazeWidth = PlayerPrefs.GetInt("MazeWidth");
        MazeHeight = PlayerPrefs.GetInt("MazeHeight");

        Maze.BuildMaze(MazeWidth, MazeHeight);
        Maze.InterconnectMaze();
        Maze.SetupMazeDFS();
        Maze.SimplifyMazeConfiguration(SimplicityLevel);
        
        UserNameText.text = PlayerPrefs.GetString("UserName");
        GameStartedAt = DateTime.Now;


    }
    
	void Start ()
    {        
        StartCoroutine(SpawnCoins());
        SpawnZombie();

    }
    

    public IEnumerable<RoomController> FindShortestPathToPlayer(RoomController sourceRoom)
    {
        return Maze.FindShortestPath(sourceRoom, Player.CurrentRoomController);
    }

    /// <summary>
    /// Coroutine spawning coins infinitely
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCoins()
    {
        SpawnCoin();
        yield return new WaitForSeconds(CoinsSpawnDelaySeconds);
        yield return SpawnCoins();
    }

    void SpawnZombie()
    {
        var randomRoom = Maze.GetRandomRoom();
        var newZombieObject = (GameObject)Instantiate(
            ZombiePrefab, 
            randomRoom.transform.position, 
            randomRoom.transform.rotation);
        var newZombieController = newZombieObject.GetComponent<ZombieController>();
        newZombieController.GameManager = this;

        
    }

    void SpawnMummy()
    {
        var randomRoom = Maze.GetRandomRoom();
        var newMummyObject = (GameObject)Instantiate(
            MummyPrefab,
            randomRoom.transform.position,
            randomRoom.transform.rotation);
        var newMummyController = newMummyObject.GetComponent<MummyController>();
        newMummyController.GameManager = this;

    }


    /// <summary>
    /// Spawns new coin in an empty room
    /// </summary>
    void SpawnCoin()
    {
        if (Maze.RoomsControllers.All(rc => rc.HasCoin))
        {
            throw new Exception("Too much coins in the Maze!");
        }
        if (CoinsInMazeCount == MaximumCoins)
        {
            return;
        }
        var randomRoom = Maze.GetRandomRoom();
        if (randomRoom.HasCoin)
        {
            SpawnCoin();
        }
        randomRoom.HasCoin = true;
        var pos = randomRoom.transform.position;
        var newCoin = (GameObject)Instantiate(
            CoinPrefab, 
            randomRoom.transform.position, 
            randomRoom.transform.rotation);       
    }
    

    public void AddCoin()
    {
        PlayerCoins++;
        CoinsDisplayText.text = "Coins: " + PlayerCoins;
        
        if (PlayerCoins == 5)
        {
            SpawnZombie();           
        }
        if (PlayerCoins == 10)
        {
            SpawnMummy();
        }
        if (PlayerCoins == 20)
        {
            ZombieController.IsMovingRandomly = false;
        }
        foreach (var e in Enemies)
        {
            e.Speed = ZombieController.ZombieBaseSpeed + 
                ZombieController.ZombieBaseSpeed * (0.25f * PlayerCoins);
        }
        if(Mummy != null)
        {
            Mummy.Speed *= 2.0f;
        }  
    }


    public IEnumerator GameOver(string reason)
    {
        if(reason == "Mummy")
        {
            PlayerCoins = 0;
        }
        else if (reason == "Zombie") { }
        else if (reason == "Escape") { }
        else
        {
            throw new ArgumentException("Incorrect reason for game over");
        }

        var newSavedGame = new SavedGame()
        {
            UserName = PlayerPrefs.GetString("UserName"),
            Coins = PlayerCoins,
            TimeInMazeSeconds = (DateTime.Now - GameStartedAt).Seconds,
            GameOverReason = reason,
            GameBegin = GameStartedAt
        };

        GameOverUILayer.SetActive(true);

        GameOverStatsText.text = "UserName: " + newSavedGame.UserName +
            "\nCoins Collected: " + newSavedGame.Coins +
            "\nGameOver reason: " + reason +
            "\nTime in Maze: " + newSavedGame.TimeInMazeSeconds + " seconds";

        SavesManager.SaveGame(newSavedGame);

        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("MainMenu");
    }



}
