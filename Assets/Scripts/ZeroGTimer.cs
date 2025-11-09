using UnityEngine;
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
}
