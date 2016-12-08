using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerController : MonoBehaviour {

    

    
    [Header("Game Objects")]
    public PlayerController Player;
    public MazeController Maze;

    [Header("Coins Spawn Settings")]
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


    [Header("Prefabs")]
    public GameObject CoinPrefab;
    public GameObject ZombiePrefab;
    public GameObject MummyPrefab;

    [Header("GUI")]
    public Text CoinsDisplayText;
    public Text UserNameText;
    public GameObject GameOverUILayer;
    public Text GameOverStatsText;


    //Actors
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

    private int MazeWidth;
    private int MazeHeight;
    private int SimplicityLevel;

    private DateTime GameStartedAt;

    void Awake()
    {
        //Injecting GameManager
        Player.GameManager = this;
        Maze.GameManager = this;

        LoadMazeUserPrefs();

        Maze.BuildMaze(MazeWidth, MazeHeight);
        Maze.InterconnectMaze();
        Maze.SetupMazeDFS();
        Maze.SimplifyMazeConfiguration(SimplicityLevel);  
    }

    void LoadMazeUserPrefs()
    {
        MazeWidth = 5;
        if (PlayerPrefs.HasKey("MazeWidth"))
        {
            MazeWidth = PlayerPrefs.GetInt("MazeWidth");
        }

        MazeHeight = 5;
        if (PlayerPrefs.HasKey("MazeHeight"))
        {
            MazeHeight = PlayerPrefs.GetInt("MazeHeight");
        }

        SimplicityLevel = 20;
        if (PlayerPrefs.HasKey("SimplicityLevel"))
        {
            SimplicityLevel = PlayerPrefs.GetInt("SimplicityLevel");
        }

        UserNameText.text = "UserName";
        if (PlayerPrefs.HasKey("UserName"))
        {
            UserNameText.text = PlayerPrefs.GetString("UserName");
        }
    }
    
	void Start ()
    {
        GameStartedAt = DateTime.Now;
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

    /// <summary>
    /// Spawns GameObject in a random room, and returns specified component
    /// </summary>
    /// <typeparam name="TComponent">Type of Component to be extracted</typeparam>
    /// <param name="prefab">GameObject to be instantiated</param>
    /// <returns>specified Component</returns>
    TComponent SpawnInRandomRoom<TComponent>(GameObject prefab)
    {
        var randomRoom = Maze.GetRandomRoom();
        var newObject = (GameObject)Instantiate(
            prefab,
            randomRoom.transform.position,
            randomRoom.transform.rotation);
        return newObject.GetComponent<TComponent>();
    }

    void SpawnZombie()
    {
        var newZombieController = SpawnInRandomRoom<ZombieController>(ZombiePrefab);
        newZombieController.GameManager = this;        
    }

    void SpawnMummy()
    {
        var newMummyController = SpawnInRandomRoom<MummyController>(MummyPrefab);
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
        Instantiate(
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
                ZombieController.ZombieBaseSpeed * (0.05f * PlayerCoins);
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
