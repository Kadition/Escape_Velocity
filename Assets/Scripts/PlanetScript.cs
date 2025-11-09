using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    public float maxAcc = 100f;
    public float dropOff = 0.5f;
    public float affectRadiusFactor = 1.5f;
    public float visualRadius;
    public float coefFriction = 1.5f;

    private const float KADENBAD = 12f;

    void Start()
    {
        visualRadius = transform.localScale.x * 0.5f * KADENBAD; // Bang
        // MeshFilter mf = GetComponent<MeshFilter>();
        // Mesh mesh = mf.sharedMesh;

        // // Mesh bounds are in local space, so multiply by scale
        // Vector3 scaledExtents = Vector3.Scale(mesh.bounds.extents, transform.lossyScale);
        // visualRadius = scaledExtents.x; // x = radius because it's a sphere 
    }
}
