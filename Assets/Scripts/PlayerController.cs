using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speedFactor;

    void Start()
    {
    }

    void Update()
    {
        var dx = Input.GetAxis("Horizontal");
        var dz = Input.GetAxis("Vertical");

        if (dx != 0 || dz != 0)
        {
            transform.position += new Vector3(dx, 0, dz) * speedFactor;
        }
    }
}