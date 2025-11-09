using Unity.Netcode;
using UnityEngine;

public class PlayerLook : NetworkBehaviour
{
    public float mouseSensitivity = 300f;
    [SerializeField] private PlayerController player;

    void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }
    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        float mouseX = -Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // Use player-relative UP, not world up
        Vector3 playerUp = -player.transform.up;
        transform.Rotate(playerUp, mouseX, Space.World);
    }
}
