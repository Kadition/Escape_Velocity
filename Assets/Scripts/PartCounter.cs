using UnityEngine;

// Attached to the GameManager object
// Keeps track of the number of ship parts collected by the player
public class PartCounter : MonoBehaviour
{
    public static int partsCollected = 0;
    private bool allPartsCollected = false;

    void Update()
    {
        if (!allPartsCollected && partsCollected >= 3)
        {
            allPartsCollected = true;
            Debug.Log("All parts collected!");
            // Trigger any event here, like enabling a ship or finishing the level
        }
    }
}
