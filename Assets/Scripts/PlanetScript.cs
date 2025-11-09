using UnityEngine;
using System.Collections.Generic;

public class PlanetScript : MonoBehaviour
{
    public float maxAcc = 100f;
    public float dropOff = 0.5f;
    public float affectRadiusFactor = 1.5f;
    public float visualRadius;
    public float coefFriction = 1.5f;

    private const float KADENBAD = 12f;

    public int numObjectsGenerated = 0;
    public GameObject[] objectsToGenerate;
    public float minDegree = 10f;
    public float minScale = 1.0f;
    public float maxScale = 1.5f;
    private List<Vector3> placedDirections = new List<Vector3>();
    private float objectCullSize = 15f; // Distance consistency scaler

    void Start()
    {
        visualRadius = transform.localScale.x * 0.5f * KADENBAD; // Bang
        
        for (int i = 0; i < numObjectsGenerated;i++){
            SpawnObject();
        }

    }

    void SpawnObject()
    {
        Vector3 direction = Vector3.zero;
        bool valid = false;

        // Try up to 30 attempts to find a spaced direction
        for (int attempts = 0; attempts < 30; attempts++)
        {
            direction = Random.onUnitSphere;

            // Check against previously placed objects
            valid = true;
            foreach (Vector3 placed in placedDirections)
            {
                float angle = Vector3.Angle(direction, placed);
                if (angle < minDegree)
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
                break;
        }

        if (!valid)
            return; // No valid spot found, skip

        placedDirections.Add(direction);

        Vector3 position = transform.position + direction * visualRadius - (direction * 0.2f);

        GameObject prefab = objectsToGenerate[Random.Range(0, objectsToGenerate.Length)];
        GameObject rock = Instantiate(prefab, position, Quaternion.identity);

        rock.transform.up = direction;

        float scale = Random.Range(minScale, maxScale);
        rock.transform.localScale *= scale;
        LODGroup lod = rock.GetComponent<LODGroup>();
        if (lod == null)
            lod = rock.AddComponent<LODGroup>();

        Renderer[] renderers = rock.GetComponentsInChildren<Renderer>();

        // LOD0: visible
        // LOD1: invisible (distance-cull)
        LOD[] lods = new LOD[2];
        lods[0] = new LOD(0.5f, renderers);      // visible when close
        lods[1] = new LOD(0.01f, new Renderer[0]); // disappears when far
        lod.SetLODs(lods);

        lod.RecalculateBounds();

        // FORCE CONSISTENT DISAPPEAR DISTANCE:
        lod.size = objectCullSize / scale; // <--- SHRINK or GROW this to tune render distance

        lod.fadeMode = LODFadeMode.CrossFade; // Optional: smooth fade-out
    }
}
