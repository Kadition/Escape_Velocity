using Unity.Netcode;
using UnityEngine;

public class PartBehavior : NetworkBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // ! if anyone ever reads this, please never have a find game object with tag in the update loop, this is literally so bad
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            //detect if a player is close enough to collect the part
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < 2f)
            {
                jonasSillyRpc();
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    public void jonasSillyRpc()
    {
        PartCounter.partsCollected += 1;
        Destroy(gameObject);
    }
}
