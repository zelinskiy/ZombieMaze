using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class GameManagerController : MonoBehaviour {

    public PlayerController Player;
    public MazeController Maze;
    public Text CoinsDisplayText;

    public int MaximumCoins = 10;
    public int CoinsSpawnDelay = 5;
    private int CoinsCount
    {
        get
        {
            return Maze.RoomsControllers.Count(rc => rc.HasCoin);
        }
    }

    public int PlayerCoins = 0;

    public GameObject CoinPrefab;
    public GameObject ZombiePrefab;

    public List<MovableController> Actors;
    public List<ZombieController> Zombies
    {
        get
        {
            return GameObject.FindGameObjectsWithTag("Zombie")
                .Select(z => z.GetComponent<ZombieController>())
                .ToList();
        }
    }

	// Use this for initialization
	void Start ()
    {
        Actors = new List<MovableController>();
        Actors.Add(Player);
        Actors.AddRange(Zombies.Select(z => z as MovableController));
        StartCoroutine(SpawnCoins());
        SpawnZombie();

    }

    void Update()
    {
        var zombie = Zombies.First();
        
        zombie.IsMovingRandomly = false;
        zombie.RoomsPath = Maze
            .FindShortestPath(
                Player.CurrentRoomController, 
                zombie.CurrentRoomController)
            .Select(r => r.gameObject)
            .ToList();
    }
	

    IEnumerator SpawnCoins()
    {
        SpawnCoin();
        yield return new WaitForSeconds(CoinsSpawnDelay);
        yield return SpawnCoins();
    }

    ZombieController SpawnZombie()
    {        
        var randomRoom = Maze.Rooms[Random.Range(0, Maze.Height), Random.Range(0, Maze.Width)];
        var randomRoomController = randomRoom.GetComponent<RoomController>();
        var pos = randomRoom.transform.position;
        var newZombie = (GameObject)Instantiate(
            ZombiePrefab, 
            randomRoom.transform.position, 
            randomRoom.transform.rotation);
        return newZombie.GetComponent<ZombieController>();
        
    }

    void SpawnCoin()
    {
        if (Maze.RoomsControllers.All(rc => rc.HasCoin))
        {
            throw new System.Exception("Too much coins in the Maze!");
        }
        if (CoinsCount == MaximumCoins)
        {
            return;
        }
        var randomRoom = Maze.Rooms[Random.Range(0, Maze.Height), Random.Range(0, Maze.Width)];
        var randomRoomController = randomRoom.GetComponent<RoomController>();
        if (randomRoomController.HasCoin)
        {
            SpawnCoin();
        }
        randomRoomController.HasCoin = true;
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
        foreach (var z in Zombies)
        {
            z.Speed = ZombieController.ZombieBaseSpeed + 
                ZombieController.ZombieBaseSpeed * (0.25f * PlayerCoins);
        }
        
        


    }

}
