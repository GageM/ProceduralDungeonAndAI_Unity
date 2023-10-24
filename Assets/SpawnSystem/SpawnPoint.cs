using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SpawnPoint : MonoBehaviour
{

    [SerializeField, Range(0, 10), Tooltip("The maximum spawn radius from this point")]
    public float spawnRadius;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnDrawGizmos()
    {
        // Draw a sphere showing the spawn radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    // Update is called once per frame
    void Update()
    {   
    }
}
