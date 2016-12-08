using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MovableController {

    public GameManagerController GameManager;

    public RoomController CurrentRoomController { get; set; }

    public bool UserInputActive;
    
    void Start ()
    {
        if (GameManager == null)
        {
            throw new System.ArgumentNullException("Game Manager is null (maybe it is not injected properly?)");
        }

        Speed = 4f;
        StartCoroutine(WalkInsideMaze());        
    }

    /// <summary>
    /// Executed when game starts
    /// </summary>
    /// <returns></returns>
    IEnumerator WalkInsideMaze()
    {
        UserInputActive = false;
        GameManager.Maze.SetMazeEntranceOpened(true);
        MoveAt(new Vector3(MazeController.RoomSize, 0));
        yield return new WaitForSeconds(1);
        GameManager.Maze.SetMazeEntranceOpened(false);
        UserInputActive = true;
    }
	

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
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (Camera.main.orthographicSize > 1)
            {
                Camera.main.orthographicSize -= 0.25f;
            }

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (Camera.main.orthographicSize < 100)
            {
                Camera.main.orthographicSize += 0.25f;
            }
        }

        if (!UserInputActive || IsRunning == true)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W) && !CurrentRoomController.TopWall.activeSelf)
        {
            MoveAt(new Vector3(0, MazeController.RoomSize));            
        }
        else if (Input.GetKey(KeyCode.S) && !CurrentRoomController.BottomWall.activeSelf)
        {
            MoveAt(new Vector3(0, -MazeController.RoomSize));
        }
        else if (Input.GetKey(KeyCode.A) && !CurrentRoomController.LeftWall.activeSelf)
        {
            MoveAt(new Vector3(-MazeController.RoomSize, 0));
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (Input.GetKey(KeyCode.D) && !CurrentRoomController.RightWall.activeSelf)
        {
            MoveAt(new Vector3(MazeController.RoomSize, 0));
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameOver("Escape");
        }
        


    }
}
