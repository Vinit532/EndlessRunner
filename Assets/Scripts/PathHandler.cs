using UnityEngine;
using System.Collections.Generic;

public class PathHandler : MonoBehaviour
{
    public List<GameObject> prefabs; // List of prefabs to instantiate
    public Transform parentTransform; // Parent transform for organization
    public GameObject player; // Reference to the player object

    private List<GameObject> instantiatedPrefabs = new List<GameObject>(); // List to keep track of instantiated prefabs
    private GameObject lastInstantiatedPrefab; // Reference to the last instantiated prefab

    void Start()
    {
        if (prefabs.Count == 0) return;

        // Instantiate the first prefab
        lastInstantiatedPrefab = Instantiate(prefabs[0], parentTransform);
        lastInstantiatedPrefab.transform.SetParent(parentTransform);
        instantiatedPrefabs.Add(lastInstantiatedPrefab);

        for (int i = 1; i < prefabs.Count; i++)
        {
            InstantiateAndAlignNextPrefab(prefabs[i]);
        }
    }

    void Update()
    {
        // Check if player has moved to the next prefab
        if (PlayerMovedToNextPrefab())
        {
            // Destroy the first prefab and re-instantiate it at the end
            GameObject firstPrefab = instantiatedPrefabs[0];
            instantiatedPrefabs.RemoveAt(0);
            Destroy(firstPrefab);

            GameObject newPrefabInstance = Instantiate(firstPrefab, parentTransform);
            newPrefabInstance.transform.SetParent(parentTransform);
            instantiatedPrefabs.Add(newPrefabInstance);

            // Align the new prefab
            Transform lastChild = GetLastChild(lastInstantiatedPrefab);
            Transform firstChild = GetFirstChild(newPrefabInstance);
            AlignPrefabs(lastChild, firstChild, newPrefabInstance);

            lastInstantiatedPrefab = newPrefabInstance;
        }
    }

    bool PlayerMovedToNextPrefab()
    {
        // Determine if the player has moved to the next prefab
        if (instantiatedPrefabs.Count < 2) return false;

        // Example: Check if player has moved past the midpoint of the current prefab
        float playerPositionZ = player.transform.position.z;
        float currentPrefabEndZ = instantiatedPrefabs[1].transform.position.z;

        return playerPositionZ > currentPrefabEndZ;
    }

    void InstantiateAndAlignNextPrefab(GameObject prefab)
    {
        // Get the last child of the last instantiated prefab
        Transform lastChild = GetLastChild(lastInstantiatedPrefab);

        // Get the first child of the new prefab
        GameObject newPrefabInstance = Instantiate(prefab);
        Transform firstChild = GetFirstChild(newPrefabInstance);

        // Align the new prefab
        AlignPrefabs(lastChild, firstChild, newPrefabInstance);

        // Set the parent for organization
        newPrefabInstance.transform.SetParent(parentTransform);

        // Update the last instantiated prefab
        lastInstantiatedPrefab = newPrefabInstance;

        // Add to the list of instantiated prefabs
        instantiatedPrefabs.Add(newPrefabInstance);
    }

    Transform GetLastChild(GameObject prefab)
    {
        return prefab.transform.GetChild(prefab.transform.childCount - 1);
    }

    Transform GetFirstChild(GameObject prefab)
    {
        return prefab.transform.GetChild(0);
    }

    void AlignPrefabs(Transform lastChild, Transform firstChild, GameObject newPrefabInstance)
    {
        // Get the mesh of the last and first child
        MeshFilter lastMeshFilter = lastChild.GetComponent<MeshFilter>();
        MeshFilter firstMeshFilter = firstChild.GetComponent<MeshFilter>();

        if (lastMeshFilter == null || firstMeshFilter == null)
        {
            Debug.LogError("Child objects must have MeshFilter components.");
            return;
        }

        Mesh lastMesh = lastMeshFilter.mesh;
        Mesh firstMesh = firstMeshFilter.mesh;

        // Get vertices in world space
        Vector3[] lastChildVertices = GetWorldSpaceVertices(lastChild, lastMesh);
        Vector3[] firstChildVertices = GetWorldSpaceVertices(firstChild, firstMesh);

        // Assuming we align the first vertex of last child to the first vertex of first child
        Vector3 lastVertex = lastChildVertices[0]; // You can choose which vertex to use
        Vector3 firstVertex = firstChildVertices[0]; // You can choose which vertex to use

        // Calculate offset
        Vector3 offset = lastVertex - firstVertex;

        // Apply the offset to the new prefab
        newPrefabInstance.transform.position += offset;
    }

    Vector3[] GetWorldSpaceVertices(Transform objTransform, Mesh mesh)
    {
        Vector3[] worldSpaceVertices = new Vector3[mesh.vertexCount];
        Vector3[] localVertices = mesh.vertices;

        for (int i = 0; i < localVertices.Length; i++)
        {
            worldSpaceVertices[i] = objTransform.TransformPoint(localVertices[i]);
        }

        return worldSpaceVertices;
    }
}
