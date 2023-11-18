using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LookAtMouse : MonoBehaviour
{
    Camera mainCam;

    public Vector3 mousePos;
    public Vector3 mouseWorld;

    public Vector3 lookTarget;

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
        mouseWorld = mainCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mousePos.z));

        RaycastHit hit;
        Physics.Raycast(new Ray(mouseWorld, mainCam.transform.forward), out hit);

        lookTarget = hit.point;

        //mousePos += mainCam.transform.position;
        Vector3 diff = lookTarget - transform.position;


        float rotationY = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, rotationY, 0.0f);


        // Look at mouse cursor

    }
}
