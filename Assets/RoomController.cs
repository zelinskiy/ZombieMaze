using UnityEngine;
using System.Collections;
using System.Linq;

public class RoomController : MonoBehaviour {

    public GameObject TopWall;
    public GameObject BottomWall;
    public GameObject LeftWall;
    public GameObject RightWall;

    public GameObject TopRoom;
    public GameObject BottomRoom;
    public GameObject LeftRoom;
    public GameObject RightRoom;

    public GameObject[] Walls;
    public GameObject[] NeighbourRooms;

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

    void SetNeighbourRooms()
    {
        NeighbourRooms = new GameObject[]
       {
            TopRoom,
            RightRoom,
            BottomRoom,
            LeftRoom
       };
       NeighbourRooms = NeighbourRooms.Where(nr => nr != null).ToArray();
    }


    public int CountNeighbours()
    {
        return NeighbourRooms.Count(nr => nr != null);
    }
}
