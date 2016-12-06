using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ZombieController : MovableController
{
    public RoomController CurrentRoomController;
    public static readonly float ZombieBaseSpeed = 2f;
    public List<GameObject> RoomsPath;

    public bool IsMovingRandomly;
    
    void Awake ()
    {
        Speed = ZombieBaseSpeed;
        IsMovingRandomly = true;
    }
    

    void Update()
    {
        Move();
        TryMoveFurther();
    }

    void TryMoveFurther()
    {
        if (IsRunning || CurrentRoomController == null)
        {
            return;
        }

        print(CurrentRoomController.ReachableNeighbourRooms.Count);
        var nextRoom = CurrentRoomController.ReachableNeighbourRooms
                .OrderBy(nr => Random.value)
                .FirstOrDefault();
        if (!IsMovingRandomly && RoomsPath.Count > 0)
        {
            nextRoom = RoomsPath[0];
            RoomsPath.RemoveAt(0);
        }
        
        if(nextRoom == CurrentRoomController.RightRoom)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (nextRoom == CurrentRoomController.LeftRoom)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }

        MoveTo(new Vector3(
            nextRoom.transform.position.x,
            nextRoom.transform.position.y, 
            transform.position.z));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Room")
        {
            CurrentRoomController = other.gameObject.GetComponent<RoomController>();
        }
    }
}
