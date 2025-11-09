/*using UnityEngine;
using System.Collections;
//attached to the player game object
// this script will respawn the player if they are in zero gravity for too long
public class ZeroGTimer : MonoBehaviour
{
    PlayerController pC;
    [SerializeField] float timerLength = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pC = gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pC.gravityVector == pC.NormGravity)
        {
            StartCoroutine(DeathTimer());
        }
        else
        {
            StopCoroutine(DeathTimer());
        }
    }

    IEnumerator DeathTimer()
    {
        yield return new WaitForSeconds(timerLength);
        GetComponent<PlayerRespawn>().RespawnPlayer();
    }
}*/

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.Netcode;

public class ZeroGTimer : NetworkBehaviour
{
    PlayerController pC;
    [SerializeField] float timerLength = 10f;
    Coroutine deathCoroutine;
    [SerializeField] Canvas redCanvas;
    [SerializeField] Image redImage;

    void Start()
    {
        if(!IsOwner)
        {
            return;
        }
        pC = gameObject.GetComponent<PlayerController>();
        redCanvas.enabled = false;
    }

    void Update()
    {
        if(!IsOwner)
        {
            return;
        }
        if (pC.getGravityVector() == pC.getNormGravity())
        {
            if (deathCoroutine == null)
            {
                deathCoroutine = StartCoroutine(DeathTimer());

            }
        }
        else
        {
            if (deathCoroutine != null)
            {
                StopCoroutine(deathCoroutine);
                deathCoroutine = null;
                ResetRedFade();
            }
        }
    }

    IEnumerator DeathTimer()
    {
        StartCoroutine(FadeToRed());
        yield return new WaitForSeconds(timerLength);
        GetComponent<PlayerRespawn>().RespawnPlayer();
        ResetRedFade();
        deathCoroutine = null; // reset after completion
    }

    IEnumerator FadeToRed()
    {
        redCanvas.enabled = true;
        Color color = redImage.color;
        color.a = 0;
        // Fade to black
        while (color.a < 0.5)
        {
            color.a += 0.001f;
            redImage.color = color;
            yield return null;
        }
    }
    
    public void ResetRedFade()
    {
        // jump back to transparent
        StopCoroutine(FadeToRed());
        Color color = redImage.color;
        color.a = 0;
        redImage.color = color;
        redCanvas.enabled = false;
    }
}

