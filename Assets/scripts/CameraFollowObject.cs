using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundObject : MonoBehaviour
{
    public Transform target;
    public float speed = 5.0f;
    public float distance = 2.0f;
    public float scrollSpeed = 1.0f;
    public Camera ThisCamera;
    public float vibrationIntensity = 0.1f; // Adjust this value to control the vibration intensity
    private Vector3 offset;
    private Vector3 initialOffset;

    void Start()
    {
        initialOffset = new Vector3(0, 0, -distance);
        offset = initialOffset;
        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }

    bool IsMouseInCameraViewport(Camera camera, Vector3 mousePosition)
    {
        Rect viewportRect = camera.pixelRect;
        return viewportRect.Contains(mousePosition);
    }

    void Update()
    {
        if (IsMouseInCameraViewport(ThisCamera, Input.mousePosition))
        {
            if (Input.GetMouseButton(1))
            {
                offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * speed, Vector3.up) * offset;
                offset = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * speed, transform.right) * offset;
            }
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (scrollInput != 0f)
            {
                distance -= scrollInput * scrollSpeed;
                distance = Mathf.Clamp(distance, 1.0f, 50.0f); 
                offset = offset.normalized * distance;
            }
        }

        // Apply position and rotation
        transform.position = target.position + offset;
        transform.LookAt(target.position);

    }
}
