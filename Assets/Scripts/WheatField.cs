using Sirenix.OdinInspector;
using UnityEngine;

public class WheatField : MonoBehaviour
{
    [SceneObjectsOnly] public GameObject modelUngrown;
    [SceneObjectsOnly] public GameObject modelGrown;
    [SceneObjectsOnly] public ParticleSystem growthEffect;

    public float growthTime = 2f;
    public float harvestTime = 3f;

    [HideInInspector] public Island island;
    [HideInInspector] public float growth;

    private bool isGrowing;
    private float harvestDelay;

    private void Awake()
    {
        island = GetComponentInParent<Island>();
    }

    private void Start()
    {
        ResetField();

        growthEffect.Play();
        StopGrowing();
    }

    private void ResetField()
    {
        growth = 0;
        harvestDelay = harvestTime;
        modelUngrown.SetActive(true);
        modelGrown.SetActive(false);
    }

    private void Update()
    {
        if (isGrowing)
        {
            growth += Time.deltaTime / growthTime;

            if (growth >= 1)
            {
                modelUngrown.SetActive(false);
                modelGrown.SetActive(true);
            }
        }

        if (growth >= 1)
        {
            StopGrowing();
            harvestDelay -= Time.deltaTime;
        }

        if (harvestDelay <= 0)
        {
            Harvest();
        }
    }

    public void StartGrowing()
    {
        if (growth < 1 && !isGrowing)
        {
            isGrowing = true;
            var emissionModule = growthEffect.emission;
            emissionModule.enabled = true;
        }
    }

    public void StopGrowing()
    {
        isGrowing = false;
        var emissionModule = growthEffect.emission;
        emissionModule.enabled = false;
    }

    private void Harvest()
    {
        ResetField();

        // SFX
        SoundManager.INSTANCE.Play(SoundManager.INSTANCE.harvest, 1, 0, 0.7f);

        // add food
        var food = Random.Range(800, 1300);
        PlayerController.INSTANCE.AddFood(food);
    }
}