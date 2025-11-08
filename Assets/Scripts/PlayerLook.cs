using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 300f;
    private PlayerController player;

    void Start(){
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
        player = GetComponent<PlayerController>();
    }
    void Update()
    {
        float mouseX = -Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // Use player-relative UP, not world up
        Vector3 playerUp = -player.transform.up;
        transform.Rotate(playerUp, mouseX, Space.World);
    }
}
