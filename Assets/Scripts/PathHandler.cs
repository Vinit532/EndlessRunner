using UnityEngine;
using System.Collections.Generic;

public class PathHandler : MonoBehaviour
{
    public GameObject player;                     // Reference to the player object
    public List<GameObject> pathPrefabs;          // List of different path segment prefabs
    public int initialSegments = 5;               // Number of segments to start with
    public float distanceToSpawnNext = 15f;       // Distance from the last segment to spawn the next one

    private Queue<GameObject> activePaths;        // Queue to keep track of active path segments
    private GameObject lastSegment;               // Reference to the last path segment
    private float playerLastZPosition;            // The Z position of the player at the last check

    void Start()
    {
        activePaths = new Queue<GameObject>();

        // Instantiate initial path segments
        for (int i = 0; i < initialSegments; i++)
        {
            SpawnNextPathSegment();
        }

        playerLastZPosition = player.transform.position.z;
    }

    void Update()
    {
        // Check if the player has moved enough to warrant spawning a new segment
        if (player.transform.position.z - playerLastZPosition >= distanceToSpawnNext)
        {
            playerLastZPosition = player.transform.position.z;
            SpawnNextPathSegment();
            RemoveOldPathSegment();
        }
    }

    // Function to spawn the next path segment
    void SpawnNextPathSegment()
    {
        int randomIndex = Random.Range(0, pathPrefabs.Count);
        GameObject newSegment;

        if (lastSegment != null)
        {
            Transform lastChildEndPoint = GetLastChildEndPoint(lastSegment.transform);
            newSegment = Instantiate(pathPrefabs[randomIndex], lastChildEndPoint.position, lastChildEndPoint.rotation);
        }
        else
        {
            // For the first segment, use the default position
            newSegment = Instantiate(pathPrefabs[randomIndex], Vector3.zero, Quaternion.identity);
        }

        activePaths.Enqueue(newSegment);
        lastSegment = newSegment;
    }

    // Function to remove the oldest path segment
    void RemoveOldPathSegment()
    {
        if (activePaths.Count > 0)
        {
            GameObject oldSegment = activePaths.Dequeue();
            Destroy(oldSegment);
        }
    }

    // Function to find the end point of the last child of the given segment
    Transform GetLastChildEndPoint(Transform segment)
    {
        Transform lastChild = segment.GetChild(segment.childCount - 1);
        Transform endPoint = lastChild.Find("EndPoint");

        if (endPoint == null)
        {
            Debug.LogError("EndPoint not found on last child of the path segment!");
            // Default to the position and rotation of the last child if no EndPoint is found
            endPoint = lastChild;
        }

        return endPoint;
    }
}
