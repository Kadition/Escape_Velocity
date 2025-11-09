using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
    public bool grounded;
    public Vector3 NormGravity = new Vector3(0f, -9.81f, 0f);
    public Vector3 gravityVector;
    public bool gravityUpdated;
    [SerializeField] private Rigidbody rig;
    public float moveForce = 20f;
    public float jumpForce = 6f;

    public float flipSpeed = 10f;

    public bool overrideMovement = false;

    GameObject[] planetList;

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

        planetList = GameObject.FindGameObjectsWithTag("Planet");
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

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        
        if(overrideMovement)
        {
            transform.position = placeToTransform.position;
            return;
        }

        inputX = Input.GetAxisRaw("Horizontal");
        inputZ = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            jumpPressed = true;
    }

    void FixedUpdate()
    {
        if (!IsOwner | overrideMovement)
        {
            return;
        }

        foreach (GameObject planet in planetList)
        {
            PlanetScript planetScript = planet.GetComponent<PlanetScript>();

            float distance = Vector3.Distance(transform.position, planet.transform.position);

            if(distance <= planetScript.visualRadius * planetScript.affectRadiusFactor)
            {
                // Make a new vector that is a rotated version of gravityVector

                Vector3 g = (planet.transform.position - transform.position).normalized * Mathf.Lerp(planetScript.maxAcc, planetScript.maxAcc*planetScript.dropOff, (distance - planetScript.visualRadius) / (planetScript.visualRadius * planetScript.affectRadiusFactor));

                updateGravity(g);
            }
        }

        if (!gravityUpdated)
        {
            gravityVector = NormGravity;
        }

        Vector3 gravityDirection = gravityVector.normalized;

        Vector3 up = -gravityDirection.normalized;

        // Recompute a forward direction that is perpendicular to up
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, up).normalized;

        Quaternion targetRot = Quaternion.LookRotation(forward, up);

        // Smooth rotate toward that absolute orientation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 180f * Time.deltaTime * flipSpeed);

        float x = inputX;
        float z = inputZ;

        // Movement direction relative to facing direction
        Vector3 moveDir = (transform.forward * z + transform.right * x).normalized;

        // Apply movement force continuously
        rig.AddForce(moveDir * moveForce, ForceMode.Force);

        // Apply gravity continuously
        rig.AddForce(gravityVector, ForceMode.Force);

        // Jump against gravity
        if (jumpPressed && grounded)
        {
            rig.AddForce(-gravityDirection * jumpForce, ForceMode.Impulse);
            Debug.Log("Jumped!");
        }
        else if(jumpPressed){
            Debug.Log("Failure of a jump");
        }

        jumpPressed = false;
        gravityUpdated = false;
    }
}