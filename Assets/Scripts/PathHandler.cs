using UnityEngine;
using System.Collections.Generic;

public class PathHandler : MonoBehaviour
{
    public List<GameObject> prefabs; // List of prefabs to instantiate
    public Transform parentTransform; // Parent transform for organization

    private GameObject lastInstantiatedPrefab; // Reference to the last instantiated prefab

    void Start()
    {
        if (prefabs.Count == 0) return;

        // Instantiate the first prefab
        lastInstantiatedPrefab = Instantiate(prefabs[0], parentTransform);
        lastInstantiatedPrefab.transform.SetParent(parentTransform);

        for (int i = 1; i < prefabs.Count; i++)
        {
            InstantiateAndAlignNextPrefab(prefabs[i]);
        }
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
