using UnityEngine;
using System.Collections;


/// <summary>
/// Usage: Add Move() to Update()
/// </summary>
public class MovableController : MonoBehaviour {

    private Vector3 _targetPosition;
    public Vector3 TargetPosition
    {
        get
        {
            return _targetPosition;
        }
    }
    public float Speed = 5f;
    
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
    
    
    
    /// <summary>
    /// Translates TargetPosition on specified delta
    /// </summary>
    /// <param name="delta">DIrection and value of translation</param>
    public void MoveAt(Vector3 delta)
    {
        _targetPosition = transform.position + delta;
        IsRunning = true;
    }

    /// <summary>
    /// Sets TargetPosition
    /// </summary>
    /// <param name="position"></param>
    public void MoveTo(Vector3 position)
    {
        _targetPosition = position;
        IsRunning = true;
    }

    /// <summary>
    /// Must be executed each frame
    /// </summary>
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
