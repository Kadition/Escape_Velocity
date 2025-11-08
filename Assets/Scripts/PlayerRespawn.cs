using System.Collections;
using UnityEngine;
using UnityEngine.UI;

//Attached to the Player GameObject
// This script handles player respawning at a designated point when the Backspace key is pressed
public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] Canvas blackCanvas;
    [SerializeField] Image blackImage;
    [SerializeField] private float fadeSpeed = 0.1f;
    [SerializeField] private Vector3 respawnPoint = new Vector3(0, 1, 0);
    [SerializeField] private float respawnDelay = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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

    private void RespawnPlayer()
    {
        StartCoroutine(FadeToBlack(fadeSpeed));
    }

        
    
    
    IEnumerator FadeToBlack(float fadeSpeed)
    {
        blackCanvas.enabled = true;
        Color color = blackImage.color;
        color.a = 0;
        // Fade to black
        while (color.a < 1)
        {
            color.a += fadeSpeed;
            blackImage.color = color;
            yield return null;
        }
        // Hold black for a moment
        yield return new WaitForSeconds(2f);
        transform.position = respawnPoint;
        // Fade back to transparent
        while (color.a > 0)
        {
            color.a -= fadeSpeed * 2;
            blackImage.color = color;
            yield return null;
        }
        blackCanvas.enabled = false;

    }
}
