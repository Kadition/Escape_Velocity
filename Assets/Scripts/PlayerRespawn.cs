using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

//Attached to the Player GameObject
// This script handles player respawning at a designated point when the Backspace key is pressed
public class PlayerRespawn : NetworkBehaviour
{
    [SerializeField] private AudioClip[] wilhelmScream;
    [SerializeField] Canvas blackCanvasPrefab;
    Canvas blackCanvas;
    [SerializeField] Image blackImagePrefab;
    Image blackImage;
    [SerializeField] private float fadeSpeed;

    Coroutine coroutine;

    bool hasReset = false;
    private Vector3 respawnPoint = new Vector3(0, 102, 0);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(!IsOwner)
        {
            return;
        }
        blackCanvas = Instantiate(blackCanvasPrefab, Vector3.zero, Quaternion.identity);
        blackImage = Instantiate(blackImagePrefab, blackCanvas.transform);
        blackCanvas.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if(!IsOwner)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            transform.position = respawnPoint;
            gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            if(coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        if (!hasReset && transform.position.magnitude > 50000)
        {
            RespawnPlayer();
            hasReset = true;
        }
    }

    public void RespawnPlayer()
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = StartCoroutine(FadeToBlack(fadeSpeed));
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
        gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

        hasReset = false;

        if (wilhelmScream != null)
        {
            SoundFXManager.instance.PlaySoundFXCLip(wilhelmScream[Random.Range(0, wilhelmScream.Length)], transform, 0.5f);
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
