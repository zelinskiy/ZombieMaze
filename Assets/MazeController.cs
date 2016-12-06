using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class MazeController : MonoBehaviour {

    public int Width = 10;
    public int Height = 5;

    public GameObject[,] Rooms;


    public List<RoomController> RoomsControllers;

    public GameManagerController GameManager;
    public GameObject RoomPrefab;
    

	void Awake ()
    {
        BuildMaze();
        InterconnectMaze();
        SetupMazeDFS();
    }

    void BuildMaze()
    {
        Rooms = new GameObject[Height, Width];
        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                var rot = transform.rotation;
                var pos = transform.position;
                pos.x += 2.7f * j;
                pos.y += 2.7f * i;
                var newRoom = (GameObject)Instantiate(RoomPrefab, pos, rot);
                newRoom.transform.parent = transform;
                Rooms[i, j] = newRoom;
                var rcontroller = newRoom.GetComponent<RoomController>();
                RoomsControllers.Add(rcontroller);
                rcontroller.Id = i * Width + j;
                foreach (var w in rcontroller.Walls)
                {
                    w.SetActive(true);
                }

                if (i == Height - 1)
                {
                    rcontroller.TopWall.SetActive(true);
                }
                else if (i == 0)
                {
                    rcontroller.BottomWall.SetActive(true);
                }

                if (j == Width - 1)
                {
                    rcontroller.RightWall.SetActive(true);
                }
                else if (j == 0)
                {
                    rcontroller.LeftWall.SetActive(true);
                }
            }
        }


        Rooms[0, 0].GetComponent<RoomController>().LeftWall.SetActive(false);
    }

    void InterconnectMaze()
    {
        for (var i = 1; i < Height - 1; i++)
        {
            for (var j = 1; j < Width - 1; j++)
            {
                var roomController = Rooms[i, j].GetComponent<RoomController>();
                roomController.TopRoom = Rooms[i + 1, j];
                roomController.BottomRoom = Rooms[i - 1, j ];
                roomController.LeftRoom = Rooms[i, j - 1];
                roomController.RightRoom = Rooms[i, j + 1];
            }
        }

        //Leftest rooms
        for (var i = 1; i < Height - 1; i++)
        {            
            var roomController = Rooms[i, 0].GetComponent<RoomController>();
            roomController.TopRoom = Rooms[i + 1, 0];
            roomController.BottomRoom = Rooms[i - 1, 0];
            roomController.RightRoom = Rooms[i,1];            
        }

        //Rightest rooms
        for (var i = 1; i < Height - 1; i++)
        {
            var roomController = Rooms[i, Width - 1].GetComponent<RoomController>();
            roomController.TopRoom = Rooms[i + 1, Width - 1];
            roomController.BottomRoom = Rooms[i - 1, Width - 1];
            roomController.LeftRoom = Rooms[i, Width - 2];
        }

        //Top rooms
        for (var j = 1; j < Width - 1; j++)
        {
            var roomController = Rooms[Height - 1, j].GetComponent<RoomController>();
            roomController.BottomRoom = Rooms[Height - 2, j];
            roomController.LeftRoom = Rooms[Height - 1, j - 1];
            roomController.RightRoom = Rooms[Height - 1, j + 1];
        }

        //Bottom rooms
        for (var j = 1; j < Width - 1; j++)
        {
            var roomController = Rooms[0, j].GetComponent<RoomController>();
            roomController.TopRoom = Rooms[1, j];
            roomController.LeftRoom = Rooms[0, j - 1];
            roomController.RightRoom = Rooms[0, j + 1];
        }

        var topRightRoom = Rooms[Height - 1, Width - 1].GetComponent<RoomController>();
        var topLeftRoom = Rooms[Height - 1, 0].GetComponent<RoomController>();
        var bottomRightRoom = Rooms[0, Width - 1].GetComponent<RoomController>();
        var bottomLeftRoom = Rooms[0, 0].GetComponent<RoomController>();

        topLeftRoom.RightRoom = Rooms[Height - 1, 1];
        topLeftRoom.BottomRoom = Rooms[Height - 2, 0];

        topRightRoom.LeftRoom = Rooms[Height - 1, Width - 2];
        topRightRoom.BottomRoom = Rooms[Height - 2, Width - 1];

        bottomRightRoom.LeftRoom = Rooms[0, Width - 2];
        bottomRightRoom.TopRoom = Rooms[1, Width - 1];

        bottomLeftRoom.RightRoom = Rooms[0, 1];
        bottomLeftRoom.TopRoom = Rooms[1, 0];

        foreach(var r in Rooms)
        {
            r.GetComponent<RoomController>().SetNeighbourRooms();
        }

    }
       
    void SetupMazeDFS()
    {
        var backtrack = new Stack();
        
        var currentRoom = Rooms[0, 0].GetComponent<RoomController>();
        var beginRoom = currentRoom;
        var nextRoom = currentRoom.NeighbourRooms
                .Select(r => r.GetComponent<RoomController>())
                .OrderBy(nr => Random.value)
                .Where(rc => !rc.VisitedDFS)
                .FirstOrDefault();
        currentRoom.MakePassToNeighbour(nextRoom);
        currentRoom.VisitedDFS = true;
        backtrack.Push(currentRoom);
        currentRoom = nextRoom;

        while (currentRoom.Id != beginRoom.Id)
        {

            nextRoom = currentRoom.NeighbourRooms
                .Select(r => r.GetComponent<RoomController>())
                .OrderBy(nr => Random.value)
                .Where(rc => !rc.VisitedDFS)
                .FirstOrDefault();
            
            if(nextRoom == null)
            {
                currentRoom = backtrack.Pop() as RoomController;
                continue;
            }
            else
            {
                currentRoom.MakePassToNeighbour(nextRoom);                
                backtrack.Push(currentRoom);
                currentRoom = nextRoom;
            }
            currentRoom.VisitedDFS = true;
            
        }

    }
    
    public IEnumerable<RoomController> FindShortestPath(RoomController Room1, RoomController Room2)
    {
        var Q = new List<RoomController>();
        var INF = 100000000;
        foreach (var r in RoomsControllers)
        {
            r.DijkstraDistance = INF;
            r.DijkstraPrev = null;
            Q.Add(r);
        }
        Room1.DijkstraDistance = 0;
        while(Q.Count != 0)
        {
            Q = Q.OrderBy(r => r.DijkstraDistance).ToList();
            var u = Q.First();
            Q.RemoveAt(0);

            foreach (var v in u.ReachableNeighbourRooms
                .Select(nr => nr.GetComponent<RoomController>()))
            {

                var alt = u.DijkstraDistance + 1;
                if(alt < v.DijkstraDistance)
                {
                    v.DijkstraDistance = alt;
                    v.DijkstraPrev = u;
                }
            }
        }

        foreach(var r in RoomsControllers)
        {
            r.Indicated = false;
        }

        var res = new List<RoomController>();
        var prev = Room2.DijkstraPrev;
        while(prev != null)
        {
            res.Add(prev);
            prev.Indicated = true;
            prev = prev.DijkstraPrev;
        }

        return res;

    }

	void Update ()
    {
	
	}


}
