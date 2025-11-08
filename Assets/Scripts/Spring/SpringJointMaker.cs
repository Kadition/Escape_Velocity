using UnityEngine;

public class SpringJointMaker : MonoBehaviour
{
    SpringJoint springJoint;

    const float springAmount = 80f;
    const float damperAmount = 10f;

    public GameObject rope;

    public Transform connectedPlayer;

    public bool attached = false;

    void Update()
    {
        if (!attached)
        {
            return;
        }

        AlignCylinder(transform.position, connectedPlayer.position);
    }

    public void MakeJoint(Rigidbody rb)
    {
        if (springJoint != null)
        {
            Destroy(springJoint);
        }

        springJoint = gameObject.AddComponent<SpringJoint>();

        springJoint.spring = springAmount;

        springJoint.damper = damperAmount;

        springJoint.connectedBody = rb;

        springJoint.anchor = new Vector3(0, 0, 0);

        springJoint.connectedAnchor = new Vector3(0, 0, 0);

        // then do any other good settings here
        // havent tested this, but i would assume it would work
        // also the line render kinda goes hard, maybe just keep it unless you can find a better solution?
        // also, should the rope just be a larger max spring size? like make the max spring size like 4 and then with rope like 8 or 12?
        // so this isnt a thing, best idea would be to do a check between the distance between the two objects, and then manually clamp it
        // ex - cubeTransform.position = Vector3.ClampMagnitude(cubeTransform.position - hands.position, 4) + hands.position;
        // nah it jitters, just apply a force opposite of velocity

        // ! alright i got it
        // ignore all the previous stuff, the distance you can go is regulated by the spring number, since when teh spring is stronger
        // it is harder to get away (cause ya know, spring stronger)
        // so, make the spring strong, but induce heavy drag on the side of the player, as you can start to get some serious speed
        // so you will want to limit that with some drag based on velocity (cause dont do this when in air and not on spring, or will be boring)
        // (or maybe do idk that is "more realistic" i guess, for there to be no drag, but it could also get sickening)

        // upon more testing you can set the linear drag on the rigid body to be high to keep it good, but i would also set it back to 0 when releasing
    }

    public void AlignCylinder(Vector3 start, Vector3 end)
    {
        // Set position to midpoint
        rope.transform.position = (start + end) / 2f;

        // Set scale (assuming the cylinder's height is along Y)
        float length = Vector3.Distance(start, end);
        Vector3 scale = transform.localScale;
        scale.y = length / 2f; // Unity's default cylinder is 2 units tall
        transform.localScale = scale;

        // Set rotation
        rope.transform.up = (end - start).normalized;
    }
}