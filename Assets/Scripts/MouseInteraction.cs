using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class MouseInteraction : NetworkBehaviour
{
    [SerializeField] private Canvas interactableCanvas;
    private Image interactableImage;
    private TextMeshProUGUI interactableText;
    private Camera cam;
    private GameObject obj;

    void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        if (!IsOwner)
        {
            return;
        }
        // Get UI elements
        interactableImage = interactableCanvas.transform.GetChild(0).GetComponent<Image>();
        interactableText = interactableImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        // Hide by default
        interactableImage.gameObject.SetActive(false);
        interactableText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mousePosition);

        // Cast a ray from the camera to the mouse position
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            GameObject hitObj = hit.collider.gameObject;
            //Interactable interactable = hitObj.GetComponent<Interactable>();
            var interactable = hitObj.GetComponent(typeof(Interactable)) as Interactable;

            if (interactable != null)
            {
                obj = hitObj;
                interactableImage.gameObject.SetActive(true);
                interactableText.gameObject.SetActive(true);

                // Interact on right-click
                if (Input.GetMouseButtonDown(1))
                {
                    Debug.Log("Right-clicked on interactable: " + obj.name);
                    interactable.Interact(gameObject);
                }
            }
            else
            {
                ClearUI();
            }
        }
        else
        {
            ClearUI();
        }
    }

    private void ClearUI()
    {
        obj = null;
        interactableImage.gameObject.SetActive(false);
        interactableText.gameObject.SetActive(false);
    }
}