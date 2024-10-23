using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs; // Array of obstacle prefabs (trees, rocks, etc.)
    public Transform player; // Reference to the player
    public float minSpawnDistance = 10f; // Minimum distance in front of the player to spawn obstacles
    public float spawnDistance = 54f; // Distance in front of the player to spawn obstacles
    public float spawnWidth = 60f; // Width of the spawn area
    public float spawnInterval = 1.84f; // Time between spawns
    public int obstaclesPerSpawn = 12; // Number of obstacles to spawn each time
    public float obstacleLifetime = 10f; // Time before removing obstacles

    public LayerMask terrainMask; // Layer mask for terrain collision checking
    public LayerMask obstacleMask; // Layer mask for obstacle collision checking
    public float minSpawnDistanceBetweenObstacles = 2.76f; // Minimum distance between obstacles to prevent overlap
    private BoxCollider terrainCollider; // Reference to the terrain collider
    private List<GameObject> spawnedObstacles = new List<GameObject>();

    void Start()
    {
        terrainCollider = GameObject.FindWithTag("Terrain").GetComponent<BoxCollider>();
        if (terrainCollider == null)
        {
            Debug.LogError("Terrain collider not found");
            return;
        }
        StartCoroutine(SpawnObstacles());
    }

    IEnumerator SpawnObstacles()
    {
        while (true)
        {
            for (int i = 0; i < obstaclesPerSpawn; i++) 
            {
                SpawnObstacle();
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnObstacle()
    {
        Vector3 spawnPosition = CalculateSpawnPosition();

        // Check for nearby obstacles to prevent overlap
        if (!IsPositionOccupied(spawnPosition))
        {
            // Randomly choose between different obstacle types (trees, rocks, etc.)
            GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];
            if (obstaclePrefabs != null) {
                // Adjust the rotation to match the terrain's angle
                Quaternion spawnRotation = CalculateSpawnRotation(spawnPosition);
                GameObject spawnedObstacle = Instantiate(obstaclePrefab, spawnPosition, spawnRotation);

                // Add to the list of obstacles and schedule removal
                spawnedObstacles.Add(spawnedObstacle);
                StartCoroutine(RemoveObstacleAfterTime(spawnedObstacle, obstacleLifetime));
            }
        }
    }

    Vector3 CalculateSpawnPosition()
    {
        // Calculate the center of the spawn area in front of the player
        float randomDistance = Random.Range(minSpawnDistance, spawnDistance);
        Vector3 spawnCenter = player.position + player.forward * randomDistance;

        // Random X offset for the horizontal spawn location
        float randomX = Random.Range(-spawnWidth / 2f, spawnWidth / 2f);

        // Calculate the spawn position within the bounds of the terrain's box collider
        Vector3 spawnPosition = spawnCenter + player.right * randomX;

        // Use raycasting to find the correct Y position on the terrain
        Ray ray = new Ray(spawnPosition + Vector3.up * 100f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, terrainMask))
        {
            spawnPosition.y = hit.point.y;
        }
        return spawnPosition;
    }

    Quaternion CalculateSpawnRotation(Vector3 spawnPosition)
    {
        // Use raycasting to find the normal of the terrain at the spawn position
        Ray ray = new Ray(spawnPosition + Vector3.up * 100f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f, terrainMask))
        {
            // Calculate the rotation based on the terrain normal
            Vector3 terrainNormal = hit.normal;
            Quaternion terrainRotation = Quaternion.FromToRotation(Vector3.up, terrainNormal);
            return terrainRotation;
        }
        else
        {
            // Default rotation if raycast fails
            return Quaternion.Euler(30f, Random.Range(0f, 360f), 0f);
        }
    }

    bool IsPositionOccupied(Vector3 spawnPosition)
    {

        // Perform a sphere check to ensure no other obstacles are too close
        Collider[] colliders = Physics.OverlapSphere(spawnPosition, minSpawnDistanceBetweenObstacles, obstacleMask);
        return colliders.Length > 0;
    }

    IEnumerator RemoveObstacleAfterTime(GameObject obstacle, float delay)
    {
        yield return new WaitForSeconds(delay);
        spawnedObstacles.Remove(obstacle);
        Destroy(obstacle);
    }

    // Visualizing with Gizmos
    void OnDrawGizmos()
    {
        if (player == null) return;

        // Draw spawn area rectangle in front of the player
        Gizmos.color = Color.green;
        Vector3 spawnCenter = player.position + player.forward * spawnDistance;
        
        // Define the corners of the spawn area
        Vector3 leftBound = spawnCenter - player.right * (spawnWidth / 2f);
        Vector3 rightBound = spawnCenter + player.right * (spawnWidth / 2f);
        
        // Draw the rectangle as lines between the corners
        Gizmos.DrawLine(leftBound, rightBound); // Front side of the rectangle
        Gizmos.DrawLine(leftBound, leftBound - player.forward * spawnDistance); // Left side going backward
        Gizmos.DrawLine(rightBound, rightBound - player.forward * spawnDistance); // Right side going backward
        Gizmos.DrawLine(leftBound - player.forward * spawnDistance, rightBound - player.forward * spawnDistance); // Back side
    
    }
}
