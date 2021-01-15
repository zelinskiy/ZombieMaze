using UnityEngine;

using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class MazeController : MonoBehaviour {

    public const float RoomSize = 2.7f;

    /// <summary>
    /// Maze Width W >>
    /// </summary>
    private int Width;
    /// <summary>
    /// Maze Height ^ H ^
    /// </summary>
    private int Height;
    
    public GameObject RoomPrefab;
    
    public GameObject[,] Rooms;
    public List<RoomController> RoomsControllers;

    /// <summary>
    /// Injected automatically from GameManager
    /// </summary>
    public GameManagerController GameManager;

    /// <summary>
    /// Builds grid of RoomPrefabs
    /// </summary>
    /// <param name="width">Maze Width</param>
    /// <param name="height">Maze Height</param>
    public void BuildMaze(int width, int height)
    {

        if(width < 2 || height < 2)
        {
            throw new System.Exception("Maze size is too small, minimum size allowed is 2x2");
        }
        Width = width;
        Height = height;       

        Rooms = new GameObject[Height, Width];
        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                var rot = transform.rotation;
                var pos = transform.position;
                
                //translate to correct position in rooms grid
                pos.x += 2.7f * j;
                pos.y += 2.7f * i;

                var newRoom = (GameObject)Instantiate(RoomPrefab, pos, rot);
                newRoom.transform.parent = transform;
                Rooms[i, j] = newRoom;
                var rcontroller = newRoom.GetComponent<RoomController>();
                RoomsControllers.Add(rcontroller);
                rcontroller.Id = i * Width + j;
                
                
            }
        }
    }

    public void SetMazeEntranceOpened(bool value)
    {
        Rooms[0, 0].GetComponent<RoomController>().LeftWall.SetActive(!value);
    }

    /// <summary>
    /// Setting references to each room neighbours
    /// </summary>
    public void InterconnectMaze()
    {
        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                var roomController = Rooms[i, j].GetComponent<RoomController>();
                if( i + 1 < Height)
                {
                    roomController.TopRoom = Rooms[i + 1, j];
                }
                if(i - 1 >= 0)
                {
                    roomController.BottomRoom = Rooms[i - 1, j];
                }
                if( j + 1 < Width)
                {
                    roomController.RightRoom = Rooms[i, j + 1];                    
                }
                if(j - 1 >= 0)
                {
                    roomController.LeftRoom = Rooms[i, j - 1];
                }
                roomController.SetNeighbourRooms();                
                
                
            }
        }

    }
    
    /// <summary>
    /// Makes passes between maze rooms
    /// </summary>
    public void SetupMazeDFS()
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

    /// <summary>
    /// Makes from 0 to extraPasses randomly
    /// </summary>
    /// <param name = "extraPasses" > maximum number of new passes</param>
    public void SimplifyMazeConfiguration(int extraPasses)
    {
        for(int i = 0; i < extraPasses; i++)
        {
            var randomRoom = GetRandomRoom();
            var randomNeighbour = GetRandomRoom(randomRoom.NeighbourRooms.Select(nr => nr.GetComponent<RoomController>()));
            randomRoom.MakePassToNeighbour(randomNeighbour);            
        }
    }

    public RoomController GetRandomRoom()
    {
        return GetRandomRoom(RoomsControllers);
    }

    public RoomController GetRandomRoom(IEnumerable<RoomController> rooms)
    {
        return rooms
            .OrderBy(nr => Random.value)
            .FirstOrDefault();
    }

    public GameObject GetRandomRoom(IEnumerable<GameObject> rooms)
    {
        return rooms
            .OrderBy(nr => Random.value)
            .FirstOrDefault();
    }

    /// <summary>
    /// Dijkstra based pathfinding algorithm
    /// </summary>
    /// <param name="Room1">Initial room</param>
    /// <param name="Room2">Destination room</param>
    /// <returns>Correctly ordered sequence of rooms</returns>
    public IEnumerable<RoomController> FindShortestPath(RoomController Room1, RoomController Room2)
    {
        if(Room1 == null)
        {
            throw new System.NullReferenceException("Source room is Null");
        }
        if (Room2 == null)
        {
            throw new System.NullReferenceException("Destination room is Null");
        }
        if (Room1.Id == Room2.Id)
        {
            return new List<RoomController>()
            {
                Room1
            };
        }
        
        var Q = new List<RoomController>();
        var INF = 100000000;
        foreach (var room in RoomsControllers)
        {
            room.DijkstraDistance = INF;
            room.DijkstraPrev = null;
            Q.Add(room);
        }
        Room1.DijkstraDistance = 0;
        while(Q.Count != 0)
        {
            Q = Q.OrderBy(r => r.DijkstraDistance).ToList();
            var room = Q.First();
            Q.RemoveAt(0);

            foreach (var neighbour in room.ReachableNeighbourRooms
                .Select(nr => nr.GetComponent<RoomController>()))
            {
                var alt = room.DijkstraDistance + 1;
                if(alt < neighbour.DijkstraDistance)
                {
                    neighbour.DijkstraDistance = alt;
                    neighbour.DijkstraPrev = room;
                }
            }
        }   

        var res = new List<RoomController>();
        var prev = Room2;
        while(prev != null)
        {
            res.Add(prev);          
            prev = prev.DijkstraPrev;
        }
        res.Reverse();
        return res;
    }


}
