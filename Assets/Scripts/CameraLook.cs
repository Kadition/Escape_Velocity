using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public float mouseSensitivity = 300f;
    float xRotation = 0f;

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
