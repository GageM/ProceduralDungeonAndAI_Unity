using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpringArm : MonoBehaviour
{
    [SerializeField, Tooltip("The Player This is Attached to")] Transform player;

    Camera mainCam;

    [SerializeField, Tooltip("The Target Distance Between the Camera and the Player")]
    [Range(0, 10)] float targetDistance;

    [SerializeField, Tooltip("The camera offset from any blocking objects")]
    [Range(0, 1)] float hitOffset;

    private Vector3 cameraPosition;
    private Vector3 targetCameraPosition;

    LayerMask cameraLayermask;
    int layermask = ~(1 << 7);

    void OnEnable()
    {
        mainCam = GetComponentInChildren<Camera>();
        cameraLayermask = ~(1 << LayerMask.GetMask("Player"));
    }

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(position, 0.1f);
        Gizmos.DrawWireSphere(cameraPosition, 0.1f);

        Gizmos.color = Color.red;

        Gizmos.DrawLine(position, mainCam.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        targetCameraPosition = transform.position - transform.forward * targetDistance;

        Ray ray = new Ray(targetCameraPosition, transform.forward);
        bool blocked = Physics.SphereCast(ray, 0.1f, out var hit, targetDistance, layermask);


        cameraPosition = blocked ? hit.point + transform.forward * hitOffset : transform.position - transform.forward * targetDistance;

        mainCam.transform.position = cameraPosition;

        mainCam.transform.LookAt (transform.position);
    }
}
