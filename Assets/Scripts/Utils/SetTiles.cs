using UnityEngine;
using System.Collections;

public class SetTiles : MonoBehaviour {
    

    void Start()
    {
        var scale = GetComponent<Transform>().localScale;
        GetComponent<MeshRenderer>().material.mainTextureScale = new Vector2(scale.x, scale.y);
    }

}
