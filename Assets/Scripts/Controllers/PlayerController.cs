using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MovableController {
    
    
    public RoomController CurrentRoomController;

    public GameManagerController GameManager;
    

    // Use this for initialization
    void Start ()
    {
        Speed = 4f;
        MoveAt(new Vector3(RoomSize, 0));        
    }
	
	// Update is called once per frame
	void Update ()
    {
        Move();
        HandleInput();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var tag = other.gameObject.tag;

        switch (tag)
        {
            case "Room":
                CurrentRoomController = other.gameObject.GetComponent<RoomController>();                
                break;
            case "Zombie":
                print("GAME OVER!");
                break;
            case "Coin":
                GameManager.AddCoin();
                CurrentRoomController.HasCoin = false;
                Destroy(other.gameObject);
                break;
        }
        
    }

    void HandleInput()
    {      
        if(IsRunning == true)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.W) && !CurrentRoomController.TopWall.activeSelf)
        {
            MoveAt(new Vector3(0, RoomSize));            
        }
        else if (Input.GetKey(KeyCode.S) && !CurrentRoomController.BottomWall.activeSelf)
        {
            MoveAt(new Vector3(0, -RoomSize));
        }
        else if (Input.GetKey(KeyCode.A) && !CurrentRoomController.LeftWall.activeSelf)
        {
            MoveAt(new Vector3(-RoomSize, 0));
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (Input.GetKeyDown(KeyCode.D) && !CurrentRoomController.RightWall.activeSelf)
        {
            MoveAt(new Vector3(RoomSize, 0));
            GetComponent<SpriteRenderer>().flipX = false;
        }
        
        
    }
}
