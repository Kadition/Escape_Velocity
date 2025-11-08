using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public float mouseSensitivity = 300f;
    float pitch = 0f;

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        // Rotate pitch around camera's local X-axis
        transform.localRotation = Quaternion.AngleAxis(pitch, Vector3.right);

        // if you need the camera to not be on the player, do this
        Camera.main.transform.position = transform.position;
    }
}
