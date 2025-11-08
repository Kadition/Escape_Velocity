// PlanetScript.cs
using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    public Vector3 gravityVector = new Vector3(0f, -9.81f, 0f);

    public float affectRadiusFactor = 1.5f;


    private GameObject player;
    private PlayerController pc;

    private float visualRadius;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        pc = player.GetComponent<PlayerController>();

        visualRadius = transform.localScale.x * 0.5f;
    }

    void FixedUpdate()
    {
        
        if(Vector3.Distance(player.transform.position, transform.position) <= visualRadius * affectRadiusFactor){
            // Make a new vector that is a rotated version of gravityVector
            Vector3 g = (transform.position - player.transform.position).normalized * gravityVector.magnitude;

            pc.updateGravity(g);
        }
    }

}
