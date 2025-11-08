// PlanetScript.cs
using UnityEngine;
using System.Collections.Generic;


public class PlanetScript : MonoBehaviour
{
    public float maxAcc = 100f;
    public float dropOff = 0.5f;
    public float affectRadiusFactor = 1.5f;


    private GameObject player;
    private PlayerController pc;

    private float visualRadius;
    public GameObject[] rocks;
    private List<Vector3> placedDirections = new List<Vector3>();

    public float minAngleBetweenItems = 10;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        pc = player.GetComponent<PlayerController>();

        visualRadius = transform.localScale.x * 0.5f;
        for (int i = 0; i < 30;i++){
            SpawnRock();
        }
    }

    void SpawnRock()
    {
        Vector3 direction = Vector3.zero;
        bool valid = false;

        // Try up to 30 attempts to find a spaced direction
        for (int attempts = 0; attempts < 30; attempts++)
        {
            direction = Random.onUnitSphere;

            // Check against previously placed rocks
            valid = true;
            foreach (Vector3 placed in placedDirections)
            {
                float angle = Vector3.Angle(direction, placed);
                if (angle < minAngleBetweenItems)
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

        GameObject prefab = rocks[Random.Range(0, rocks.Length)];
        GameObject rock = Instantiate(prefab, position, Quaternion.identity);

        rock.transform.up = direction;

        float scale = Random.Range(4f, 8f);
        rock.transform.localScale *= scale;
    }


    void FixedUpdate()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if(distance <= visualRadius * affectRadiusFactor){
            // Make a new vector that is a rotated version of gravityVector

            Vector3 g = (transform.position - player.transform.position).normalized * Mathf.Lerp(maxAcc, maxAcc*dropOff, (distance - visualRadius) / (visualRadius * affectRadiusFactor));

            pc.updateGravity(g);
        }
    }

}
