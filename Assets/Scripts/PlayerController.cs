using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speedFactor;
    public float handHoverDst;

    [HideInInspector] public float influenceRadius;

    [SceneObjectsOnly] public Transform hand;
    [SceneObjectsOnly] public ParticleSystem handParticleSystem;

    private Camera cam;
    private RaycastHit handRaycastHit = new RaycastHit();
    private Vector3 handCoords = new Vector3();

    private void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        influenceRadius = 10;
    }

    void Update()
    {
        Move();
        MoveHand();
        UpdateEnergyLoss();
    }

    private void UpdateEnergyLoss()
    {
        var vecToOrigin = Vector3.zero - handCoords;
        var loosingPower = vecToOrigin.magnitude > influenceRadius;

        if (loosingPower && !handParticleSystem.emission.enabled)
        {
            var emissionModule = handParticleSystem.emission;
            emissionModule.enabled = true;
        }

        if (!loosingPower && handParticleSystem.emission.enabled)
        {
            var emissionModule = handParticleSystem.emission;
            emissionModule.enabled = false;
        }

        if (loosingPower)
        {
            handParticleSystem.transform.rotation = Quaternion.LookRotation(vecToOrigin);
        }
    }

    private void Move()
    {
        var dx = Input.GetAxis("Horizontal");
        var dz = Input.GetAxis("Vertical");
        if (dx != 0 || dz != 0)
        {
            transform.position += new Vector3(dx, 0, dz) * speedFactor;
        }
    }

    private void MoveHand()
    {
        // find if we hit an island
        var ray = cam.ScreenPointToRay(Input.mousePosition);
        var hasHit = Physics.Raycast(ray, out handRaycastHit, 100, Masks.ISLAND);
        if (hasHit)
        {
            handCoords = handRaycastHit.point;
            handCoords.y += handHoverDst;
        }
        else
        {
            // find intersection point with the base plane
            Plane plane = new Plane(Vector3.up, 0);
            if (plane.Raycast(ray, out var distance))
            {
                handCoords = ray.GetPoint(distance);
            }
        }

        hand.position = handCoords;
    }
}