using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerController : MonoBehaviour
{
    public float handHoverDst = 0.3f;
    public float moveSpeed = 1f;
    public float dragSpeedBase = 0.5f;
    public float dragSpeedIsland = 0.2f;

    [HideInInspector] public float influenceRadius;

    [SceneObjectsOnly] public Transform hand;
    [SceneObjectsOnly] public ParticleSystem handParticleSystem;

    private Camera cam;
    private RaycastHit handRaycastHit;

    private Island island;
    private Vector3 handCoords;
    private Vector3 mouseMove;

    private bool isStarted;
    private bool isClicking;
    private bool isDragging;
    private Vector3 dragStartCoords;
    private float currentDragSpeed;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        influenceRadius = 10;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        isClicking = Input.GetMouseButton((int) MouseButton.LeftMouse);
        if (!isStarted && isClicking)
        {
            isStarted = true;
        }

        MoveHand();
        Move();
        UpdateEnergyLoss();
    }

    private void Move()
    {
        if (isDragging && Input.GetButtonUp("Fire1"))
        {
            isDragging = false;
            handCoords.x = hand.position.x;
            handCoords.z = hand.position.z;
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            dragStartCoords = handCoords;
        }

        if (isClicking && !isDragging)
        {
            var dragVec = dragStartCoords - handCoords;
            if (dragVec.magnitude > 0.5f)
            {
                isDragging = true;
                if (island && !island.connected)
                {
                    currentDragSpeed = dragSpeedIsland;
                }
                else
                {
                    currentDragSpeed = dragSpeedBase;
                }
            }
        }

        if (isDragging && (!island || island.connected) && mouseMove.sqrMagnitude > 0)
        {
            // move player/camera
            var tr = transform;
            var position = tr.position;
            position += -mouseMove * currentDragSpeed;
            tr.position = position;

            currentDragSpeed *= 0.99f;
        }

        if (isDragging && island && !island.connected && mouseMove.sqrMagnitude > 0)
        {
            // move island
            var tr = island.transform;
            var position = tr.position;
            position += mouseMove * currentDragSpeed;
            tr.position = position;

            // also move hand
            hand.position += mouseMove * currentDragSpeed;

            currentDragSpeed *= 0.99f;
        }
    }

    private void MoveHand()
    {
        if (!isStarted)
        {
            return;
        }

        mouseMove.x = Input.GetAxis("Mouse X");
        mouseMove.z = Input.GetAxis("Mouse Y");
        if (mouseMove.sqrMagnitude == 0)
        {
            return;
        }

        handCoords += mouseMove * moveSpeed;

        if (!isClicking && !isDragging)
        {
            var ray = new Ray(new Vector3(handCoords.x, 50, handCoords.z), Vector3.down);
            var hasHit = Physics.Raycast(ray, out handRaycastHit, 100, Masks.ISLAND);
            if (hasHit)
            {
                handCoords = handRaycastHit.point;
                handCoords.y += handHoverDst;

                island = handRaycastHit.collider.gameObject.GetComponentInParent<Island>();
            }
            else
            {
                island = null;

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

    private void UpdateEnergyLoss()
    {
        var vecToOrigin = Vector3.zero - hand.position;
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
}