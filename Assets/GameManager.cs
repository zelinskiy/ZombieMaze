using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

    public PlayerController Player;
    public MazeController Maze;

    public List<MovableController> Actors;
    public List<ZombieController> Zombies;

	// Use this for initialization
	void Start ()
    {
        Actors = new List<MovableController>();
        Actors.Add(Player);
        Zombies = GameObject.FindGameObjectsWithTag("Zombie")
            .Select(z => z.GetComponent<ZombieController>())
            .ToList();
        Actors.AddRange(Zombies.Select(z => z as MovableController));
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
