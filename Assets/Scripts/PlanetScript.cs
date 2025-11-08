using UnityEngine;

public class PlanetScript : MonoBehaviour
{
    public float maxAcc = 100f;
    public float dropOff = 0.5f;
    public float affectRadiusFactor = 1.5f;
    public float visualRadius;

    void Start()
    {
        visualRadius = transform.localScale.x * 0.5f;
    }
}
