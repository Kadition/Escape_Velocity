using UnityEngine;

//attached to the player game object
//this script places the endpoints of the rope at the player's position on R press, also detects if another endpoint already exists.
public class PlaceRopeEndPoints : MonoBehaviour
{

    [SerializeField] bool anotherEndExists;
    [SerializeField] GameObject ropeEndPrefab;
    [SerializeField] GameObject ropeParentPrefab;
    [SerializeField] GameObject firstRopeEnd;
    [SerializeField] GameObject secondRopeEnd;
    Vector3 zOffset = new Vector3(0f, 0f, 1f);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anotherEndExists = false;
    }

    // When the R key is pressed, place rope endpoints
    /*If another endpoint already exists, 
    place the second endpoint, 
    create the rope parent object, 
    set both endpoints as children, 
    and connect them using ConnectPoints script*/
    void Update()
    {
        if(anotherEndExists == false)
        {
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameObject ropeEnd = Instantiate(ropeEndPrefab, transform.position + zOffset, Quaternion.identity);
                firstRopeEnd = ropeEnd;
                anotherEndExists = true;
            }
        } else if(anotherEndExists == true)
        {   
            if (Input.GetKeyDown(KeyCode.R))
            {
                //place second rope end
                GameObject ropeParent = Instantiate(ropeParentPrefab);
                GameObject ropeEnd = Instantiate(ropeEndPrefab, transform.position + zOffset, Quaternion.identity);
                secondRopeEnd = ropeEnd;
                secondRopeEnd.transform.parent = ropeParent.transform;
                anotherEndExists = false;
                //create rope parent and set both rope ends as children
                
                
                firstRopeEnd.transform.parent = ropeParent.transform;
                //get ConnectPoints script and set near and far endpoints
                var connectPoints = ropeParent.GetComponent<ConnectPoints>();
                connectPoints.nearEnd = firstRopeEnd;
                connectPoints.farEnd = secondRopeEnd;
                //Connect the rope ends
                connectPoints.StartCoroutine(connectPoints.ConnectRopeEnds());
            }
        }
    }
}
