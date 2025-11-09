using UnityEngine;

//attached to the rope end gameobjects
//this script teleports the player to the opposite rope end when interacted with
public class RopeTeleporter : MonoBehaviour, Interactable
{
    [SerializeField] GameObject ropeParentPrefab;
    GameObject ropeParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ropeParent = Instantiate(transform.parent.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void interact()
    {
        if(transform.GetSiblingIndex() == 0) // if player is at near end
        {
            Transform farEnd = ropeParent.transform.GetChild(1); // get far end
            transform.position = farEnd.position + (farEnd.up * 1.5f); // teleport to far end plus offset
        } else if(transform.GetSiblingIndex() == 1) // if player is at far end
        {
            Transform nearEnd = ropeParent.transform.GetChild(0); // get near end
            transform.position = nearEnd.position + (nearEnd.up * 1.5f); // teleport to near end plus offset
        }
    }
}
