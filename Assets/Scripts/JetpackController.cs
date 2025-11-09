using System;
using UnityEngine;

public class JetpackController : MonoBehaviour
{
    private bool jetpackIsAvailable = true;
    [SerializeField] float lateralVelocity = 1f;
    [SerializeField] float groundCheckDistance = 1.1f;
    [SerializeField] private bool isGrounded;
    private Rigidbody playerRb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        // Raycast to check if player is grounded
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance))
        {
            isGrounded = true;
            jetpackIsAvailable = true; // Reset jetpack when grounded
        }
        else
        {
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        //On E key press, move player to the right with impulse force
        if (Input.GetKey(KeyCode.E) && jetpackIsAvailable)
        {
            playerRb.AddForce(Vector3.right * 1 * lateralVelocity, ForceMode.Impulse);
            jetpackIsAvailable = false;
        }
        //On Q key press, move player to the left with impulse force
        else if (Input.GetKey(KeyCode.Q) && jetpackIsAvailable)
        {
            playerRb.AddForce(Vector3.left * lateralVelocity, ForceMode.Impulse);
            jetpackIsAvailable = false;
        }
    }
}
