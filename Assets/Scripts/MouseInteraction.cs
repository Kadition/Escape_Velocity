using UnityEngine.UI;
using UnityEngine;
using TMPro;


// Items with interactable component can be moved by mousing over them and pressing the right mouse button
// attached to the player object

public class MouseInteraction : MonoBehaviour
{
    //UI panel which appears when the user can interact
    public Image interactableImage;
    //Text tellign the playe rthey can carry  an item
    public TextMeshProUGUI interactableText;
    //may have an issue with cam, if so, just revert back to m_camera version.
    public Camera cam;
    public GameObject obj;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //setting default values
        interactableImage.gameObject.SetActive(false);
    }

    void Awake()
    {
        //sets a reference for the main camera.
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //Constantly shoots a ray at mousePos
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mousePosition);

        //checks if the ray hit an interactable object
        if (Physics.Raycast(ray, out RaycastHit hit, 2f) && obj.GetComponent<Interactable>() != null)
        {
            //sets obj to the object hit by the ray at mouse pos, a panel appears when the player can interact with the item
            obj = hit.collider.gameObject;
            interactableImage.gameObject.SetActive(true);

            //checks if the objects being hit has iteractable component
            if (obj.GetComponent<Interactable>() != null)
            {
                //Makes carry prompt text appear
                interactableText.gameObject.SetActive(true);
            }
            else
            {
                //Makes carry prompt text disappear
                interactableText.gameObject.SetActive(false);
            }
        }

        //Sets obj to an empty object if prior condition fails and deactivated UI
        else
        {
            obj = null;
            interactableImage.gameObject.SetActive(false);
        }

        //Interact with object when right mouse button is pressed
        if (Input.GetMouseButtonDown(1) && obj.GetComponent<Interactable>() != null)
        {
            obj.GetComponent<Interactable>().interact();
        }
    }
}