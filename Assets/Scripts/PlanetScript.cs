// PlanetScript.cs
using System;
using System.Diagnostics.Contracts;
using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    public float maxAcc = 100f;
    public float dropOff = 0.5f;
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
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if(distance <= visualRadius * affectRadiusFactor){
            // Make a new vector that is a rotated version of gravityVector

            Vector3 g = (transform.position - player.transform.position).normalized * Mathf.Lerp(maxAcc, maxAcc*dropOff, (distance - visualRadius) / (visualRadius * affectRadiusFactor));

            pc.updateGravity(g);
        }
    }

}
