using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using grouflon;

public class MaskGenerator : MonoBehaviour
{
    public Material room1Material;
    public Material room2Material;

    public Vector3 size = new Vector3(2.0f, 3.0f, 2.0f);

    public void generateMesh()
    {
        UnityTools.DestroyAllChildren(transform);

        int layer = LayerMask.NameToLayer("Mask");

        m_room1Mask = _generateHalfMask("Room1", Quaternion.Euler(0.0f, 180.0f, 0.0f), room1Material);
        m_room1Mask.tag = "Room1";
        m_room1Mask.transform.SetParent(transform, false);
        m_room1Mask.layer = layer;

        m_room2Mask = _generateHalfMask("Room2", Quaternion.identity, room2Material);
        m_room2Mask.tag = "Room2";
        m_room2Mask.transform.SetParent(transform, false);
        m_room2Mask.layer = layer;
    }

    private GameObject _generateHalfMask(string _name, Quaternion _baseRotation, Material _material)
    {
        GameObject go = new GameObject(_name);
        MeshFilter meshFilter = go.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = go.AddComponent<MeshCollider>();
        meshRenderer.material = _material;
        Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        Vector3 extent = size * 0.5f;
        Vector3[] vertices = new Vector3[8];
        vertices[0] = _baseRotation * new Vector3(extent.x, 0.0f, -extent.z);
        vertices[1] = _baseRotation * new Vector3(extent.x, 0.0f, extent.z);
        vertices[2] = _baseRotation * new Vector3(-extent.x, 0.0f, extent.z);
        vertices[3] = _baseRotation * new Vector3(-extent.x, 0.0f, -extent.z);
        vertices[4] = _baseRotation * new Vector3(extent.x, size.y, -extent.z);
        vertices[5] = _baseRotation * new Vector3(extent.x, size.y, extent.z);
        vertices[6] = _baseRotation * new Vector3(-extent.x, size.y, extent.z);
        vertices[7] = _baseRotation * new Vector3(-extent.x, size.y, -extent.z);

        int[] indices = new int[8*3];
        indices[(0*3) + 0] = 3; indices[(0*3) + 1] = 2; indices[(0*3) + 2] = 6;
        indices[(1*3) + 0] = 3; indices[(1*3) + 1] = 6; indices[(1*3) + 2] = 7;
        indices[(2*3) + 0] = 2; indices[(2*3) + 1] = 1; indices[(2*3) + 2] = 5;
        indices[(3*3) + 0] = 2; indices[(3*3) + 1] = 5; indices[(3*3) + 2] = 6;

        indices[(4*3) + 0] = 3; indices[(4*3) + 1] = 0; indices[(4*3) + 2] = 4;
        indices[(5*3) + 0] = 3; indices[(5*3) + 1] = 4; indices[(5*3) + 2] = 7;
        indices[(6*3) + 0] = 0; indices[(6*3) + 1] = 1; indices[(6*3) + 2] = 5;
        indices[(7*3) + 0] = 0; indices[(7*3) + 1] = 5; indices[(7*3) + 2] = 4;

        mesh.SetVertices(vertices);
        mesh.SetIndices(indices, MeshTopology.Triangles, 0);

        return go;
    }

    private GameObject m_room1Mask;
    private GameObject m_room2Mask;
}

 [CustomEditor(typeof(MaskGenerator))]
 class MaskGeneratorEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Mesh"))
        {
            ((MaskGenerator)target).generateMesh();
        }
    }
 }