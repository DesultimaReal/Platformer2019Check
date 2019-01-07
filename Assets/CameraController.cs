using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public static CameraController instance;

    public Vector3 CameraStartPos;
    public Vector3 PlayerStartPos;  

    // Use this for initialization
    void Start () {
		if (instance == null)
        {
             instance= this;
        }
        else
        {
            Destroy(this);
        }

        SaveSpawn();
    }

    public void SaveSpawn()
    {
        CameraStartPos = transform.position;
        PlayerStartPos = Player.instance.transform.position;
    }
	public void Respawn()
    {
        Player.instance.transform.position = PlayerStartPos;
        transform.position = CameraStartPos;

    }
}
