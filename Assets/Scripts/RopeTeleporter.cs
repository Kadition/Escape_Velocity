using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

// Attached to each rope end GameObject
// Moves the player smoothly along the rope when interacted with
public class RopeTeleporter : MonoBehaviour, Interactable
{
    private GameObject ropeParent;
    private Transform nearEnd;
    private Transform farEnd;

    private static bool isTeleporting = false; // shared across both ends

    [SerializeField] private float travelTime = 1.5f; // seconds to move across rope
    [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private float offset = 1.5f; // vertical offset

    void Start()
    {
        if (transform.parent.gameObject != null)
        {
            ropeParent = transform.parent.gameObject;
        }
        
        if (transform.parent.gameObject != null && ropeParent.transform.childCount >= 2)
        {
            nearEnd = ropeParent.transform.GetChild(0);
            farEnd = ropeParent.transform.GetChild(1);
        }
        else
        {
            Debug.LogWarning($"{name}: RopeTeleporter parent does not have two ends!");
        }
    }

    public void Interact(GameObject player)
    {
        if (isTeleporting || nearEnd == null || farEnd == null)
            return;

        StartCoroutine(MoveAcrossRope(player));
    }

    private IEnumerator MoveAcrossRope(GameObject player)
    {
        isTeleporting = true;
        player.GetComponent<Rigidbody>().isKinematic = true;

        Transform start = (transform == nearEnd) ? nearEnd : farEnd;
        Transform end = (transform == nearEnd) ? farEnd : nearEnd;

        Vector3 startPos = start.position + start.up * offset;
        Vector3 endPos = end.position + end.up * offset;

        // Temporarily disable player movement if it uses a CharacterController or Rigidbody
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        float elapsed = 0f;
        while (elapsed < travelTime)
        {
            elapsed += Time.deltaTime;
            float t = moveCurve.Evaluate(elapsed / travelTime);
            player.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        player.transform.position = endPos;

        // Re-enable controller after movement
        if (cc) cc.enabled = true;

        yield return new WaitForSeconds(0.3f);
        isTeleporting = false;
        player.GetComponent<Rigidbody>().isKinematic = false;
    }

    public string GetPromptText()
    {
        return "Climb Rope";
    }
}
