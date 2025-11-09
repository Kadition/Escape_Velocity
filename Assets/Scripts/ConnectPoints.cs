using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attached to the empty rope parent gameobject, parent of the two rope endpoints
// this script will handle connecting two rope endpoints
public class ConnectPoints : MonoBehaviour
{
    [SerializeField] public GameObject nearEnd; //near endpoint of the rope
    [SerializeField] public GameObject farEnd;
    [SerializeField] GameObject ropeSegment;
    [SerializeField] float segmentLength = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public IEnumerator ConnectRopeEnds()
    {
        /*nearEnd = transform.GetChild(1).gameObject;
        farEnd = transform.GetChild(0).gameObject;
        
        Vector3 direction = farEnd.transform.position - nearEnd.transform.position;
        float distance = direction.magnitude;
        int numSegsments = Mathf.CeilToInt(distance / segmentLength);


        int count = 0;

        //instantiate first segment at near end with halfsize to fill gap
        Vector3 firstPos = nearEnd.transform.position + (direction.normalized * (segmentLength / 2));
        Quaternion firstRot = Quaternion.FromToRotation(Vector3.up, direction);
        GameObject firstInst = Instantiate(ropeSegment, firstPos, firstRot, transform);
        firstInst.GetComponent<HingeJoint>().connectedBody = transform.GetChild(count).GetComponent<Rigidbody>();
        firstInst.transform.localScale = new Vector3(0.1f, segmentLength / 2, 0.1f);

        while (count < numSegsments - 1)
        {
            Vector3 pos = nearEnd.transform.position + direction.normalized * (segmentLength * (count + 1));
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, direction);
            GameObject inst = Instantiate(ropeSegment, pos, rot, transform);
            inst.transform.localScale = new Vector3(0.1f, segmentLength / 2, 0.1f);
            inst.GetComponent<HingeJoint>().connectedBody = transform.GetChild(count).GetComponent<Rigidbody>();

            count++;
        }
        float coveredLength = segmentLength * (count + 1);
        
        Debug.Log("Creating last segment to fill gap of length: " + (distance - coveredLength));
        Vector3 finalPos = nearEnd.transform.position + direction.normalized * coveredLength;
        Quaternion finalRot = Quaternion.FromToRotation(Vector3.up, direction);
        GameObject lastInst = Instantiate(ropeSegment, finalPos, finalRot, transform);
        lastInst.transform.localScale = new Vector3(0.1f, 0.25f / 4f, 0.1f);
        lastInst.GetComponent<HingeJoint>().connectedBody = farEnd.GetComponent<Rigidbody>();*/

        nearEnd = transform.GetChild(1).gameObject;
        farEnd = transform.GetChild(0).gameObject;

        Vector3 direction = farEnd.transform.position - nearEnd.transform.position;
        float distance = direction.magnitude;
        int numSegments = Mathf.Max(1, Mathf.CeilToInt(distance / segmentLength));
        Vector3 dirNorm = direction.normalized;

        // Start chaining from the near end Rigidbody
        Rigidbody prevRb = nearEnd.GetComponent<Rigidbody>();
        int count = 0;
        // place segments along the line, starting half a segment from nearEnd
        float currentOffset = segmentLength / 2f;
        for (int i = 0; i < numSegments; i++)
        {
            Vector3 pos = nearEnd.transform.position + dirNorm * currentOffset;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, direction);
            GameObject inst = Instantiate(ropeSegment, pos, rot, transform);
            count++;

            inst.transform.localScale = new Vector3(0.1f, segmentLength / 2f, 0.1f);

            HingeJoint hj = inst.GetComponent<HingeJoint>();
            // connect to the previous link (or nearEnd for first)
            hj.connectedBody = prevRb;


            // advance chain
            prevRb = inst.GetComponent<Rigidbody>();
            currentOffset += segmentLength;
            yield return new WaitForEndOfFrame();

        }

        farEnd.GetComponent<HingeJoint>().connectedBody = prevRb;
    }
}
