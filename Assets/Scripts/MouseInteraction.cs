using UnityEngine.UI;
using UnityEngine;
using TMPro;


//Instructions
/* Items tagged as carryable can be moved by mousing over them and pressing E
*/

/*GLITCHES
Whenever E is pressed after setting down a carryable object, the object comes back to player. Related to GetKey()?
Only happening with Tower, has large colider?
Possibly fixed
*/
public class MouseInteraction : MonoBehaviour
{
    //UI panel which appears when the user can interact
    public Image InteractPanel;
    //Text tellign the playe rthey can carry  an item
    public TextMeshProUGUI carryableText;
    //may have an issue with cam, if so, just revert back to m_camera version.
    public Camera cam;
    public GameObject obj;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //setting default values
        InteractPanel.gameObject.SetActive(false);
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
        if (Physics.Raycast(ray, out RaycastHit hit, 2f) && hit.collider.gameObject.CompareTag("Interactable"))
        {
            //sets obj to the object hit by the ray at mouse pos, a panel appears when the player can interact with the item
            obj = hit.collider.gameObject;
            InteractPanel.gameObject.SetActive(true);

            //checks if the objects being hit  has carryable tag
            if (obj.CompareTag("Interactable"))
            {
                //Makes carry prompt text appear
                carryableText.gameObject.SetActive(true);
            }
            else
            {
                //Makes carry prompt text disappear
                carryableText.gameObject.SetActive(false);
            }
        }

        //Sets obj to an empty object if prior condition fails and deactivated UI
        else
        {
            obj = GameObject.Find("Empty").gameObject;
            InteractPanel.gameObject.SetActive(false);
        }


        //If the user is carrying an object and presses E, they drop the object.
        if (Input.GetKeyDown(KeyCode.E) && obj.CompareTag("Interactable"))
        {
            obj.transform.SetParent(null);
            obj = GameObject.Find("Empty").gameObject;
        }

        //When the user presses E while mousing over an obj, they begin to carry it
        if (Input.GetKeyDown(KeyCode.E) && obj.CompareTag("Interactable"))
        {

        }
    }
}