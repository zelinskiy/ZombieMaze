using UnityEngine;
using System.Collections;
using System.Linq;

public class ZombieController : MovableController
{
    private RoomController CurrentRoomController;

    
    void Start () {
        TargetPosition = transform.position + new Vector3(RoomSize, 0);
        IsRunning = true;
        Speed = 0.5f;
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

        var x = CurrentRoomController.NeighbourRooms
            .OrderBy(nr => Random.value)
            .FirstOrDefault();
        if(x == CurrentRoomController.RightRoom)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (x == CurrentRoomController.LeftRoom)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }

        MoveTo(new Vector3(x.transform.position.x, x.transform.position.y, transform.position.z));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Room")
        {
            CurrentRoomController = other.gameObject.GetComponent<RoomController>();
        }
    }
}
