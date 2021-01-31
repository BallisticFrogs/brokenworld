using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TutoManager : MonoBehaviour
{
    public static TutoManager INSTANCE;

    [SceneObjectsOnly] public GameObject msgStart;
    [SceneObjectsOnly] public GameObject msgFood;
    [SceneObjectsOnly] public GameObject msgMove;
    [SceneObjectsOnly] public GameObject msgMoveIslands;
    [SceneObjectsOnly] public GameObject msgConnectIslands;
    [SceneObjectsOnly] public GameObject msgFindIslands;

    [HideInInspector] public int harvests;
    [HideInInspector] public float dstMoved;
    [HideInInspector] public float islandDstMoved;
    [HideInInspector] public int islandsConnected;

    private readonly Dictionary<GameObject, Func<bool>> conditionsByStep = new Dictionary<GameObject, Func<bool>>();

    private List<GameObject> steps = new List<GameObject>();

    private int currentStep = -1;

    private void Awake()
    {
        INSTANCE = this;

        conditionsByStep.Add(msgStart, () => GameManager.INSTANCE.isStarted);
        conditionsByStep.Add(msgFood, () => harvests > 0);
        conditionsByStep.Add(msgMove, () => dstMoved > 10);
        conditionsByStep.Add(msgMoveIslands, () => islandDstMoved > 5);
        conditionsByStep.Add(msgConnectIslands, () => islandsConnected > 0);
        conditionsByStep.Add(msgFindIslands, () => dstMoved > 20);

        steps.Add(msgStart);
        steps.Add(msgFood);
        steps.Add(msgMove);
        steps.Add(msgMoveIslands);
        steps.Add(msgConnectIslands);
        steps.Add(msgFindIslands);
    }

    private void Start()
    {
        var finalTip = msgFindIslands.GetComponentInChildren<TMP_Text>();
        var count = GameManager.INSTANCE.islands.Count(i => !i.connected);
        finalTip.text = $"Find the <color=#457CD3>{count}</color> remaining islands to win.";

        StartNextStep();
    }

    private void Update()
    {
        var step = steps[currentStep];
        var condition = conditionsByStep[step];
        if (condition.Invoke())
        {
            step.SetActive(false);
            StartNextStep();
        }
    }

    private void StartNextStep()
    {
        currentStep++;
        ResetStats();

        if (currentStep < steps.Count)
        {
            var nextStep = steps[currentStep];
            Debug.Log($"starting tuto step {currentStep} : {nextStep.name}");
            nextStep.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ResetStats()
    {
        harvests = 0;
        dstMoved = 0;
        islandDstMoved = 0;
        islandsConnected = 0;
    }
}