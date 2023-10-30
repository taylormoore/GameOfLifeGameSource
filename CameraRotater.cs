using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This component is used for rotation of camera in main menu.
public class CameraRotater : MonoBehaviour
{
    public float rotationAmount;

    void FixedUpdate()
    {
        transform.Rotate(0f, rotationAmount, 0f, Space.Self);
    }
}
