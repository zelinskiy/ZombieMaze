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

    void Awake()
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


        var nextRoom = CurrentRoomController.ReachableNeighbourRooms
                .OrderBy(nr => Random.value)
                .FirstOrDefault();
        if (!IsMovingRandomly)
        {
            nextRoom = GameManager
                    .FindShortestPathToPlayer(CurrentRoomController)
                    .Select(r => r.gameObject)
                    .Skip(1)
                    .FirstOrDefault();
        }
        if (nextRoom == null)
        {
            return;
        }

        if (nextRoom == CurrentRoomController.RightRoom)
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
