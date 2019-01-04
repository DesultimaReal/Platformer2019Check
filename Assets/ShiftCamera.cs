using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftCamera : MonoBehaviour {
    Transform CameraTransform;
    public int CameraWidth;
    float height;
    float width;

    private void Start()
    {
        Camera cam = Camera.main;
        CameraTransform = cam.transform;
         height = 2f * cam.orthographicSize;
         width = height * cam.aspect;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.tag == "Player")
        {
            //transform.position = new Vector3(transform.position.x + width, transform.position.y, transform.position.z);
            CameraPan();
        }
    }

    public void CameraPan()
    {
        for (int i = 0; i < width; i++)
        {
            CameraTransform.position = new Vector3(CameraTransform.position.x + 1, CameraTransform.position.y, CameraTransform.position.z);
        }
    }
}
