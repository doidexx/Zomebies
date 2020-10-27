using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    public Transform player = null;
    public Transform cameraPivot = null;
    public float mouseSentivityMultiplier = 1f;
    public bool inverted = false;

    float xRotation = 0f;

    private void Start()
    {
        LockMouse();
    }

    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (Cursor.visible == true) return;

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        player.Rotate(Vector3.up * mouseX * mouseSentivityMultiplier);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -70, 80);
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
