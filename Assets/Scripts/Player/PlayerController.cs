using System;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public bool grounded;
    private Vector3 NormGravity = new Vector3(0f, 0f, 0f);
    private Vector3 gravityVector;
    private bool gravityUpdated;
    [SerializeField] private Rigidbody rig;

    [SerializeField] private Animator animator;

    public float moveForce = 20f;
    public float jumpForce = 6f;

    public float flipSpeed = 10f;
    private float spaceMovementFactor = 0.2f;//0.2f;

    public float maxGroundedVelocity = 10;

    public Vector3 lastGravityDir;

    public bool overrideMovement = false;

    private bool lastOverridden = false;

    private float quickBuffer = 0.1f;
    private float quickTimer = 0f;
    GameObject[] planetList;

    private bool savingSlam = false;

    public Transform placeToTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!IsOwner)
        {
            return;
        }

        grounded = false;
        gravityVector = NormGravity;
        gravityUpdated = false;
        rig.useGravity = false;
        lastGravityDir = new Vector3(0f, 1f, 0f);

        planetList = GameObject.FindGameObjectsWithTag("Planet");
    }

    public Vector3 getNormGravity(){
        return NormGravity;
    }

    public Vector3 getGravityVector(){
        return gravityVector;
    }

    public void updateGravity(Vector3 g){
        
        if (!IsOwner || overrideMovement)
        {
            return;
        }
        
        gravityVector = g;
        gravityUpdated = true;
    }


    void OnCollisionStay(Collision collision)
    {
        if (!IsOwner || overrideMovement)
        {
            return;
        }
        
        grounded = false;

        foreach (var contact in collision.contacts)
        {
            // If the contact normal points roughly opposite gravity direction
            if (Vector3.Dot(contact.normal, -gravityVector.normalized) > 0.6f) 
            {
                grounded = true;
                break;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (!IsOwner || overrideMovement)
        {
            return;
        }
        
        grounded = false;
    }

    float inputX = 0f;
    float inputZ = 0f;

    bool jumpPressed = false;

    bool superJumpPressed = false;

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (overrideMovement)
        {
            transform.position = placeToTransform.position;

            return;
        }
        
        animator.SetBool("isGrab", false);

        inputX = Input.GetAxisRaw("Horizontal");
        inputZ = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jumpTrigger");
            jumpPressed = true;
        }


        if (Input.GetKeyDown(KeyCode.G))
            superJumpPressed = true;
    }

    void FixedUpdate()
    {
        if (!IsOwner | overrideMovement)
        {
            return;
        }
        float? coefFriction = null;
        foreach (GameObject planet in planetList)
        {
            PlanetScript planetScript = planet.GetComponent<PlanetScript>();

            float distance = Vector3.Distance(transform.position, planet.transform.position);

            if(distance <= planetScript.visualRadius * planetScript.affectRadiusFactor)
            {
                // Make a new vector that is a rotated version of gravityVector

                Vector3 g = (planet.transform.position - transform.position).normalized * Mathf.Lerp(planetScript.maxAcc, planetScript.maxAcc*planetScript.dropOff, (distance - planetScript.visualRadius) / (planetScript.visualRadius * planetScript.affectRadiusFactor));

                updateGravity(g);

                coefFriction = planetScript.coefFriction;
            }
        }
        Vector3 gravityDirection;
        if (!gravityUpdated)
        {
            gravityVector = NormGravity;
            gravityDirection = lastGravityDir;
        }
        else
        {
            gravityDirection = gravityVector.normalized;

            Vector3 up = -gravityDirection.normalized;

            // Recompute a forward direction that is perpendicular to up
            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, up).normalized;

            Quaternion targetRot = Quaternion.LookRotation(forward, up);

            // Smooth rotate toward that absolute orientation
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 180f * Time.deltaTime * flipSpeed);
        }

        float x = inputX;
        float z = inputZ;

        if(grounded){
            animator.SetBool("isGrounded", true);

            // Movement direction relative to facing direction
            Vector3 moveDir = (transform.forward * z + transform.right * x).normalized;

            if (moveDir == Vector3.zero)
            {
                animator.SetBool("isWalk", false);
            }
            else
            {
                animator.SetBool("isWalk", true);
            }

            // Apply movement force continuously
            rig.AddForce(moveDir * moveForce, ForceMode.Force);

            // Apply gravity continuously
            rig.AddForce(gravityVector, ForceMode.Force);

            // Friction uwu
            if (coefFriction != null)
            {
                rig.AddForce(-rig.linearVelocity * ((float) coefFriction), ForceMode.Force);
            }

            // Jump against gravity
            if (jumpPressed)// && grounded)
            {
                rig.AddForce(-gravityDirection * jumpForce, ForceMode.Impulse);
            }

            if(rig.linearVelocity.magnitude > maxGroundedVelocity){
                if(savingSlam){
                    rig.linearVelocity = Vector3.zero; //rig.linearVelocity * (maxGroundedVelocity / rig.linearVelocity.magnitude);
                }
            }
            savingSlam = false;


        }
        else{
            if(!gravityUpdated){
                savingSlam = true;
            }
            animator.SetBool("isGrounded", false);

            Vector3 moveDir = (transform.forward * z + transform.right * x).normalized;

            if (moveDir == Vector3.zero)
            {
                animator.SetBool("isWalk", false);
            }
            else
            {
                animator.SetBool("isWalk", true);
            }

            // Apply movement force continuously
            rig.AddForce(moveDir * moveForce * spaceMovementFactor, ForceMode.Force);

            // Apply gravity continuously
            rig.AddForce(gravityVector, ForceMode.Force);

            // Jump against gravity
            if (Input.GetKey(KeyCode.C) && !Input.GetKey(KeyCode.Space))// && grounded)
            {
                // MAKE THIS NOT AGAINST GRAVITY
                rig.AddForce(-gravityDirection * jumpForce * spaceMovementFactor, ForceMode.Force);
            }
            else if(Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.C))
            {
                //ALSO MAKE THIS NOT AGAINST GRAVITY
                rig.AddForce(gravityDirection * jumpForce * spaceMovementFactor, ForceMode.Force); 
            }
        }
        if (superJumpPressed){
            rig.AddForce(-gravityDirection * jumpForce * 100, ForceMode.Impulse);
        }

        jumpPressed = false;
        gravityUpdated = false;
        superJumpPressed = false;
        lastGravityDir = gravityDirection;
        lastOverridden = overrideMovement;
    }
}