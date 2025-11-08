using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    public bool grounded;
    public Vector3 NormGravity = new Vector3(0f, -9.81f, 0f);
    public Vector3 gravityVector;
    public bool gravityUpdated;
    private Rigidbody rig;
    public float moveForce = 20f;
    public float jumpForce = 6f;

    public float flipSpeed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!IsOwner)
        {
            return;
        }

        grounded = false;
        gravityVector = NormGravity;
        gravityUpdated = false;
        rig = GetComponent<Rigidbody>();
        rig.useGravity = false;
    }

    public void updateGravity(Vector3 g){
        
        if (!IsOwner)
        {
            return;
        }
        
        gravityVector = g;
        gravityUpdated = true;
    }


    void OnCollisionStay(Collision collision)
    {
        if (!IsOwner)
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
        if (!IsOwner)
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
        if(!IsOwner)
        {
            return;
        }

        inputX = Input.GetAxisRaw("Horizontal");
        inputZ = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Space))
            jumpPressed = true;
    }

    void FixedUpdate()
    {
        if(!IsOwner)
        {
            return;
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
        rig.AddForce(moveDir * moveForce, ForceMode.Acceleration);

        // Apply gravity continuously
        rig.AddForce(gravityVector, ForceMode.Acceleration);

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