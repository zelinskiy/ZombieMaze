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

    public bool VisitedDFS;

    public int Id;

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
    }

    public void MakePassToNeighbour(RoomController neighbour)
    {
        var neighbours = NeighbourRooms.Select(nr => nr.GetComponent<RoomController>());
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
            throw new System.Exception("Incorrect neighbour passed");
        }
    }


    public int CountNeighbours()
    {
        return NeighbourRooms.Count(nr => nr != null);
    }
}
