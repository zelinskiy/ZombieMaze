﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class RoomController : MonoBehaviour {

    /// <summary>
    /// Used for debugging Dijkstra pathfinding 
    /// </summary>
    public GameObject Indicator;

    /// <summary>
    /// If there is a coin in this room
    /// </summary>
    public bool HasCoin;

    /// <summary>
    /// Unique within Maze object
    /// </summary>
    public int Id;


    public GameObject TopWall;
    public GameObject BottomWall;
    public GameObject LeftWall;
    public GameObject RightWall;

    public GameObject TopRoom;
    public GameObject BottomRoom;
    public GameObject LeftRoom;
    public GameObject RightRoom;

    public GameObject[] Walls;

    /// <summary>
    /// All rooms near this one
    /// </summary>
    public GameObject[] NeighbourRooms;

    /// <summary>
    /// Rooms which player can reach through passes
    /// </summary>
    public List<GameObject> ReachableNeighbourRooms;

    public int DijkstraDistance;
    public RoomController DijkstraPrev;  
    public bool VisitedDFS;
    
    

    private bool _indicated;

    /// <summary>
    /// Used for debugging Dijkstra pathfinding 
    /// </summary>
    public bool Indicated
    {
        get
        {
            return _indicated;
        }
        set
        {
            Indicator.SetActive(value);
            _indicated = value;

        }
    }

    void Awake()
    {
        Walls = new GameObject[]
        {
            TopWall,
            RightWall,
            BottomWall,
            LeftWall
        };        
    }

    void Start ()
    {
        SetNeighbourRooms();
    }

    public void SetNeighbourRooms()
    {
        NeighbourRooms = new GameObject[]
        {
            TopRoom,
            RightRoom,
            BottomRoom,
            LeftRoom
        };
        NeighbourRooms = NeighbourRooms.Where(nr => nr != null).ToArray();

        SetReachableRooms();
    }

    void SetReachableRooms()
    {
        ReachableNeighbourRooms = new List<GameObject>();
        if (TopRoom != null && !TopWall.activeSelf)
        {
            ReachableNeighbourRooms.Add(TopRoom);
        }
        if (BottomRoom != null && !BottomWall.activeSelf)
        {
            ReachableNeighbourRooms.Add(BottomRoom);
        }
        if (LeftRoom != null && !LeftWall.activeSelf)
        {
            ReachableNeighbourRooms.Add(LeftRoom);
        }
        if (RightRoom != null && !RightWall.activeSelf)
        {
            ReachableNeighbourRooms.Add(RightRoom);
        }
    }

    public void MakePassToNeighbour(RoomController neighbour)
    {
        if (ReachableNeighbourRooms.Contains(neighbour.gameObject))
        {
            return;
        }
        if(TopRoom != null && neighbour.Id == TopRoom.GetComponent<RoomController>().Id)
        {
            TopRoom.GetComponent<RoomController>().BottomWall.SetActive(false);
            TopWall.SetActive(false);
        }
        else if (BottomRoom != null && neighbour.Id == BottomRoom.GetComponent<RoomController>().Id)
        {
            BottomRoom.GetComponent<RoomController>().TopWall.SetActive(false);
            BottomWall.SetActive(false);
        }
        else if (LeftRoom != null && neighbour.Id == LeftRoom.GetComponent<RoomController>().Id)
        {
            LeftRoom.GetComponent<RoomController>().RightWall.SetActive(false);
            LeftWall.SetActive(false);
        }
        else if (RightRoom != null && neighbour.Id == RightRoom.GetComponent<RoomController>().Id)
        {
            RightRoom.GetComponent<RoomController>().LeftWall.SetActive(false);
            RightWall.SetActive(false);
        }
        else
        {
            throw new System.ArgumentException("Incorrect neighbour passed");
        }
    }

    
}
