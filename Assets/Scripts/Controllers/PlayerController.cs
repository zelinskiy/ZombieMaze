using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MovableController {

    public GameManagerController GameManager;

    public RoomController CurrentRoomController { get; set; }

    public bool UserInputActive;

    // Use this for initialization
    void Start ()
    {
        Speed = 4f;
        StartCoroutine(WalkInsideMaze());
    }

    IEnumerator WalkInsideMaze()
    {
        UserInputActive = false;
        MoveAt(new Vector3(RoomSize, 0));
        yield return new WaitForSeconds(1);
        CurrentRoomController.LeftWall.SetActive(true);
        UserInputActive = true;
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
                GameOver("Zombie", other.GetComponent<ZombieController>());
                break;
            case "Mummy":
                GameOver("Mummy", other.GetComponent<ZombieController>());                
                break;
            case "Coin":
                Destroy(other.gameObject);
                GameManager.AddCoin();
                CurrentRoomController.HasCoin = false;                
                break;
        }
        
    }

    void GameOver(string reason, ZombieController enemy)
    {        
        transform.position = new Vector3(
                enemy.transform.position.x,
                enemy.transform.position.y,
                enemy.transform.position.z + 0.001f);
        enemy.StartAttacking();
        GameOver(reason);
    }

    void GameOver(string reason)
    {
        GetComponent<Animator>().SetBool("IsRunning", false);
        UserInputActive = false;
        Speed = 0.0f;
        StartCoroutine(GameManager.GameOver(reason));
    }

    void HandleInput()
    {
        if (!UserInputActive)
        {
            return;
        }
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
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameOver("Escape");

        }
        
        
    }
}
