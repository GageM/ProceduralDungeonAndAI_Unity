using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnQueue : MonoBehaviour
{
    // Queue of enemies to spawn
    Queue<Enemy> enemySpawnQueue;

    // Queue of spawn points 
    Queue<SpawnPoint> spawnPointQueue;

    [SerializeField, Tooltip("An array of all enemies")]
    Enemy[] enemyTypes;

    [SerializeField, Tooltip("An array of all spawn points")]
    SpawnPoint[] spawnPoints;

    // Keeps track of whether there is something spawning and how much time has passed during spawning
    bool isSpawning;
    float spawnTimer;

    // Enemy spawn selection
    int enemyIndex;

    // Spawn point selection
    int spawnPointIndex;

    // UI
    [SerializeField]
    TextMeshProUGUI counterText;
    [SerializeField]
    TextMeshProUGUI timerText;
    [SerializeField]
    TextMeshProUGUI warningText;

    // The remaining time during spawning
    float spawnTimeRemaining;

    // The maximum number of concurrent spawns
    [SerializeField, Tooltip("The maximum number of concurrent spawns")]
    int spawnCap;

    // Start is called before the first frame update
    void Start()
    {
        // Create the queues for the enemies and their corrosponding spawn points
        enemySpawnQueue = new Queue<Enemy>();
        spawnPointQueue = new Queue<SpawnPoint>();
    }

    // Update is called once per frame
    void Update()
    {
        // Spawn an enemy is there is one to spawn & there is not one currently spawning
        if(enemySpawnQueue != null && !isSpawning && enemySpawnQueue.Count > 0)
        {
            StartCoroutine(SpawnEnemy());
        }

        // Update the timer if there is an enemy spawning
        if(isSpawning)
        {
            spawnTimer += Time.deltaTime;
            spawnTimeRemaining = Mathf.Round((enemySpawnQueue.Peek().spawnTime - spawnTimer));
        }

        if(enemySpawnQueue.Count > 0)
        {
            counterText.text = enemySpawnQueue.Count.ToString() + " Enemies Spawning";
            timerText.text = "Next Enemy in " + spawnTimeRemaining.ToString() + "s";
        }
        else
        {
            counterText.text = string.Empty;
            timerText.text = string.Empty;
        }
    }

    public void OnEnemyTypeChanged(int _enemyIndex)
    {
        enemyIndex = _enemyIndex;
    }

    public void OnSpawnPointChanged(int _spawnPointIndex)
    {
        spawnPointIndex = _spawnPointIndex;
    }

    public void AddToSpawnQueue()
    {
        if (enemySpawnQueue.Count < spawnCap)
        {
            if (enemyTypes[enemyIndex] && spawnPoints[spawnPointIndex])
            {
                enemySpawnQueue.Enqueue(enemyTypes[enemyIndex]);
                spawnPointQueue.Enqueue(spawnPoints[spawnPointIndex]);
            }
        }
        else
        {
            StartCoroutine(SpawnCapMessage());
        }
    }

    IEnumerator SpawnEnemy()
    {
        // Signal that there is an enemy spawn in progress
        isSpawning = true;

        // get the enemy to spawn
        Enemy tempEnemy = enemySpawnQueue.Peek();
        SpawnPoint tempSpawnPoint = spawnPointQueue.Peek();

        // Wait for the time required to spawn the object
        yield return new WaitForSeconds(tempEnemy.spawnTime);

        // Signal that the enemy is finished spawning
        isSpawning = false;

        // Set the position for the object to spawn
        Vector3 spawnRange = new Vector3(Random.Range(-tempSpawnPoint.spawnRadius, tempSpawnPoint.spawnRadius), 0f, Random.Range(-tempSpawnPoint.spawnRadius, tempSpawnPoint.spawnRadius));
        Vector3 spawnPosition = tempSpawnPoint.transform.position + spawnRange;

        // Instantiate the enemy at the spawn point
        if (spawnPoints[spawnPointIndex])
        {
            Instantiate(tempEnemy, spawnPosition, tempSpawnPoint.transform.rotation);
        }

        // Remove the enemy from the queue
        enemySpawnQueue.Dequeue();

        // Remove the spawn point from the queue
        spawnPointQueue.Dequeue();

        // Reset the spawn timer
        spawnTimer = 0f;
    }

    IEnumerator SpawnCapMessage()
    {
        warningText.text = "Spawn Cap Reached, Cannot Spawn";

        yield return new WaitForSeconds(5f);

        warningText.text = string.Empty;
    }
}
