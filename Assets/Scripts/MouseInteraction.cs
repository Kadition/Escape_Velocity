using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MouseInteraction : MonoBehaviour
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
        // Get UI elements
        interactableImage = interactableCanvas.transform.GetChild(0).GetComponent<Image>();
        interactableText = interactableImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        // Hide by default
        interactableImage.gameObject.SetActive(false);
        interactableText.gameObject.SetActive(false);
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        Ray ray = cam.ScreenPointToRay(mousePosition);

        // Cast a ray from the camera to the mouse position
        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            GameObject hitObj = hit.collider.gameObject;
            Interactable interactable = hitObj.GetComponent<Interactable>();

            if (interactable != null)
            {
                obj = hitObj;
                interactableImage.gameObject.SetActive(true);
                interactableText.gameObject.SetActive(true);
                Debug.Log("Hovering over interactable: " + obj.name);
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

        // Interact on right-click
        if (Input.GetMouseButtonDown(1) && obj != null)
        {
            Interactable interactable = obj.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.interact();
            }
        }
    }

    private void ClearUI()
    {
        obj = null;
        interactableImage.gameObject.SetActive(false);
        interactableText.gameObject.SetActive(false);
    }
}