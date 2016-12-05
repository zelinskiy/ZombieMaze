using UnityEngine;
using System.Collections;

public class MazeController : MonoBehaviour {

    public int Width = 10;
    public int Height = 5;

    public GameObject[,] Rooms;

    public GameObject RoomPrefab;
    
	void Start ()
    {
        BuildMaze();
        InterconnectMaze();

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
                foreach (var w in rcontroller.Walls)
                {
                    w.SetActive(false);
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
	
	void Update () {
	
	}


}
