using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class ZombieController : MovableController
{
    public GameManagerController GameManager;

    public RoomController CurrentRoomController { get; set; }
    public static readonly float ZombieBaseSpeed = 0.5f;
    
    private static bool _isMovingRandomly;
    public static bool IsMovingRandomly {
        get
        {
            return _isMovingRandomly;
        }
        set
        {
            _isMovingRandomly = value;
        }
    }

    private RoomController PreviousRoom;

    void Awake()
    {
        Speed = ZombieBaseSpeed;
        IsMovingRandomly = true;
    }

    void Start()
    {
        if (GameManager == null)
        {
            throw new System.ArgumentNullException("Game Manager is null (maybe it is not injected properly?)");
        }
    }


    void Update()
    {
        Move();
        TryMoveFurther();
    }


    /// <summary>
    /// Selects next target room and moves to it
    /// </summary>
    void TryMoveFurther()
    {
        if (IsRunning || CurrentRoomController == null)
        {
            return;
        }        
        
        var nextRoom = CurrentRoomController.ReachableNeighbourRooms
                .OrderBy(nr => Random.value)
                .FirstOrDefault();
        if (nextRoom == null)
        {
            throw new System.Exception("Zombie stunned!");
        }
        if (IsMovingRandomly)
        {
            //If we are in a straightforward corridor, do not change the direction
            if (CurrentRoomController.ReachableNeighbourRooms.Count == 2)
            {
                var room1 = CurrentRoomController.ReachableNeighbourRooms[0].GetComponent<RoomController>();
                var room2 = CurrentRoomController.ReachableNeighbourRooms[1].GetComponent<RoomController>();
                if (room1 == PreviousRoom)
                {
                    nextRoom = room2.gameObject;
                }
                else
                {
                    nextRoom = room1.gameObject;
                }
            }
            //Do not return back in fork location
            else if (CurrentRoomController.ReachableNeighbourRooms.Count > 2 && nextRoom == PreviousRoom)
            {
                nextRoom = CurrentRoomController.ReachableNeighbourRooms
                    .Where(r => r != PreviousRoom)
                    .OrderBy(nr => Random.value)
                    .FirstOrDefault();
            }
        }   
        else if (!IsMovingRandomly)
        {
            nextRoom = GameManager
                    .FindShortestPathToPlayer(CurrentRoomController)
                    .Select(r => r.gameObject)
                    .Skip(1)
                    .FirstOrDefault();
        }
        
       

        
        

        if (nextRoom == CurrentRoomController.RightRoom)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (nextRoom == CurrentRoomController.LeftRoom)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }

        PreviousRoom = CurrentRoomController;
        MoveTo(new Vector3(
            nextRoom.transform.position.x,
            nextRoom.transform.position.y, 
            transform.position.z));
    }

    public void StartAttacking()
    {
        Speed = 0.0f;
        GetComponent<Animator>().SetBool("IsAttacking", true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Room")
        {
            CurrentRoomController = other.gameObject.GetComponent<RoomController>();
        }
    }
}
