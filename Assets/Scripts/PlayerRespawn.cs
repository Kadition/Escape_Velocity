using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Attached to the Player GameObject
// This script handles player respawning at a designated point when the Backspace key is pressed
public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip wilhelmScream;
    [SerializeField] Canvas blackCanvasPrefab;
    Canvas blackCanvas;
    [SerializeField] Image blackImagePrefab;
    Image blackImage;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private Vector3 respawnPoint = new Vector3(0, 1, 0);
    [SerializeField] private float respawnDelay = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blackCanvas = Instantiate(blackCanvasPrefab, Vector3.zero, Quaternion.identity);
        blackImage = Instantiate(blackImagePrefab, blackCanvas.transform);
        blackCanvas.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RespawnPlayer();
        }
    }

    public void RespawnPlayer()
    {
        StartCoroutine(FadeToBlack(fadeSpeed));
    }




    IEnumerator FadeToBlack(float fadeSpeed)
    {
        blackCanvas.enabled = true;

        // Ensure the image starts fully transparent
        Color color = blackImage.color;
        color.a = 0f;
        blackImage.color = color;

        // Fade to black over time
        while (blackImage.color.a < 1f)
        {
            color = blackImage.color;
            color.a += fadeSpeed * Time.deltaTime;
            color.a = Mathf.Clamp01(color.a); // prevent overshooting
            blackImage.color = color;
            yield return null;

            Debug.Log("Fading to black, alpha: " + color.a);
        }

        // Respawn player at spawn point
        transform.position = respawnPoint;

        if (wilhelmScream != null)
        {
            SoundFXManager.instance.PlaySoundFXCLip(wilhelmScream, transform, 0.5f);
        }

        // Hold black screen
        yield return new WaitForSeconds(2f);

        // Fade back to transparent
        while (blackImage.color.a > 0f)
        {
            color = blackImage.color;
            color.a -= fadeSpeed * 2 * Time.deltaTime;
            color.a = Mathf.Clamp01(color.a);
            blackImage.color = color;
            yield return null;
        }

        blackCanvas.enabled = false;
    }


}
