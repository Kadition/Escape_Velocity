using System.Runtime.Serialization;
using UnityEngine;

public class PartBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject playerObject;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //detect if a player is close enough to collect the part
        float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);
        if (distanceToPlayer < 2f)
        {
            PartCounter.partsCollected += 1;
            Destroy(gameObject);
        }
    }
}
