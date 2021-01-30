using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class PlayerController : MonoBehaviour
{
    public float handDefaultHeight = 1f;
    public float handHoveringHeight = 0.5f;
    public float moveSpeed = 1f;
    public float dragSpeedBase = 0.5f;
    public float dragSpeedIsland = 0.2f;

    [HideInInspector] public float influenceRadius;

    [SceneObjectsOnly] public Transform hand;
    [SceneObjectsOnly] public Transform handSphere;
    [SceneObjectsOnly] public Light handPointLight;
    [SceneObjectsOnly] public ParticleSystem handLightParticleSystem;
    [SceneObjectsOnly] public ParticleSystem handLifeParticleSystem;

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
    private float currentLightFactor;
    private float sphereBaseScale;
    // private float bonusHeight;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Start()
    {
        sphereBaseScale = handSphere.localScale.x;
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
        UpdateHandLighting();
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
            var dragDist = (dragStartCoords - handCoords).magnitude;
            var distFactor = Mathf.Clamp(dragDist * 0.02f, 0, 1);
            position += -mouseMove * EasingFunction.EaseInOutCubic(currentDragSpeed, 0, distFactor);
            tr.position = position;
        }

        if (isDragging && island && !island.connected && mouseMove.sqrMagnitude > 0)
        {
            var dragDist = (dragStartCoords - handCoords).magnitude;
            var distFactor = Mathf.Clamp(dragDist * 0.025f, 0, 1);
            var move = mouseMove * EasingFunction.EaseInOutCirc(currentDragSpeed, 0, distFactor);

            // move island
            var tr = island.transform;
            var position = tr.position;
            position += move;
            tr.position = position;

            // also move hand
            hand.position += move;
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
                handCoords.y += handHoveringHeight;

                island = handRaycastHit.collider.gameObject.GetComponentInParent<Island>();
            }
            else
            {
                island = null;

                // find intersection point with the base plane
                Plane plane = new Plane(Vector3.up, -(handDefaultHeight));
                if (plane.Raycast(ray, out var distance))
                {
                    handCoords = ray.GetPoint(distance);
                }
            }

            hand.position = handCoords;
        }
    }

    private void UpdateHandLighting()
    {
        var makingLight = isClicking; // && !isDragging;

        // increase light level
        if (makingLight && currentLightFactor < 1)
        {
            currentLightFactor += Time.deltaTime * 0.5f;
            if (currentLightFactor > 1)
            {
                currentLightFactor = 1;
            }
        }

        // decrease light level
        if (!makingLight && currentLightFactor > 0)
        {
            currentLightFactor -= Time.deltaTime * 0.5f;
            if (currentLightFactor < 0)
            {
                currentLightFactor = 0;
            }
        }

        // update actual light
        handPointLight.range = EasingFunction.EaseOutCubic(6, 15, currentLightFactor);
        handPointLight.intensity = EasingFunction.EaseOutCubic(1, 2, currentLightFactor);

        // update sphere height/size
        handSphere.localScale =
            Vector3.one * (sphereBaseScale * EasingFunction.EaseInOutCubic(1f, 1.5f, currentLightFactor));
        // bonusHeight = EasingFunction.EaseInOutCubic(0f, 1f, currentLightFactor);

        // update particle emission rate
        var emissionModule = handLightParticleSystem.emission;
        emissionModule.rateOverTime = EasingFunction.EaseInOutCubic(0f, 3, currentLightFactor);
    }

    private void UpdateEnergyLoss()
    {
        var vecToOrigin = Vector3.zero - hand.position;
        var loosingPower = vecToOrigin.magnitude > influenceRadius;

        if (loosingPower && !handLifeParticleSystem.emission.enabled)
        {
            var emissionModule = handLifeParticleSystem.emission;
            emissionModule.enabled = true;
        }

        if (!loosingPower && handLifeParticleSystem.emission.enabled)
        {
            var emissionModule = handLifeParticleSystem.emission;
            emissionModule.enabled = false;
        }

        if (loosingPower)
        {
            handLifeParticleSystem.transform.rotation = Quaternion.LookRotation(vecToOrigin);
        }
    }
}