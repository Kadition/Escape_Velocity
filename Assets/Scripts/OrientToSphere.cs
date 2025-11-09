
using UnityEngine;

//attached to the camera gameobject
// this script ensures that the camera follows the player as they walk across the surface of a sphere.
public class OrientToSphere : MonoBehaviour
{

    [SerializeField] Vector3 planetPos; // The position of the nearest planet
    [SerializeField] Vector3 playerToPlanetVector; // Vector from player to nearest planet
    [SerializeField] GameObject playerCenter; // the player gameobject
    [SerializeField] public bool scriptIsEnabled;
    void Start()
    {
        scriptIsEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (scriptIsEnabled)
        {
            planetPos = GetNearestPlanet().transform.position;
            playerToPlanetVector = planetPos - playerCenter.transform.position;
            transform.up = -playerToPlanetVector.normalized; // Set the camera's up direction to be away from the center of the current planet
        }
        
    
    }

    private GameObject GetNearestPlanet()
    {

        GameObject[] planets = GameObject.FindGameObjectsWithTag("Planet");
        float leastMag = Mathf.Infinity; //least magnitude between player and planets
        GameObject nearestPlanet = null;
        foreach (GameObject planet in planets)
        {
            if ((playerCenter.transform.position - planet.transform.position).magnitude < leastMag)
            {
                leastMag = (playerCenter.transform.position - planet.transform.position).magnitude;
                nearestPlanet = planet; 
            }
        }

        return nearestPlanet;
    }

}
