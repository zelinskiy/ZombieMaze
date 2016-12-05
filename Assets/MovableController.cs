using UnityEngine;
using System.Collections;


/// <summary>
/// Usage: Add Move() to Update()
/// </summary>
public class MovableController : MonoBehaviour {

    public float RoomSize = 2.7f;
    private bool _isRunning;
    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        set
        {
            _isRunning = value;
            GetComponent<Animator>().SetBool("IsRunning", value);
        }
    }
    public Vector3 TargetPosition;
    public float Speed = 5f;
    
    

    public void MoveAt(Vector3 delta)
    {
        TargetPosition = transform.position + delta;
        IsRunning = true;
    }

    public void MoveTo(Vector3 position)
    {
        TargetPosition = position;
        IsRunning = true;
    }

    public void Move()
    {
        if (TargetPosition == transform.position)
        {
            IsRunning = false;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPosition, Speed * Time.deltaTime);
        }
    }
}
