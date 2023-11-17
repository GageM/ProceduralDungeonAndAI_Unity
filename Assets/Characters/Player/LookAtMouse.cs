using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour
{
    Camera mainCam;

    public Vector3 mousePos;
    public Vector3 mouseWorldPos;
    public Vector3 mouseTargetPos;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse cursor position
        mousePos = Mouse.current.position.ReadValue();
        mousePos.z = mainCam.nearClipPlane;
        mouseWorldPos = mainCam.transform.position + mainCam.ScreenToViewportPoint(mousePos);
        RaycastHit hit;
        Physics.Raycast(new Ray(mouseWorldPos, mainCam.transform.forward), out hit);
        mouseTargetPos = hit.point; 
        mouseTargetPos.y = transform.position.y;

        // Look at mouse cursor
        transform.LookAt(mouseTargetPos);
    }
}
