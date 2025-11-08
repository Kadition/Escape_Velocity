using UnityEngine;

//attached to the player gameobject
// this script will handle teleporting the player when they interact with a rope object
public class RopeTeleporter : MonoBehaviour, Interactable
{
    [SerializeField] GameObject ropeParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void interact()
    {
        //teleport player to far end of rope
        Transform farEnd = ropeParent.transform.GetChild(0);
        transform.position = farEnd.position + new Vector3(0, 1, 0);
    }
}
