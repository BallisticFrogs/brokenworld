using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Island : MonoBehaviour
{
    public bool connected;
    public int followers;

    [SceneObjectsOnly] public Light localLight;

    private List<WheatField> wheatFields = new List<WheatField>();

    void Awake()
    {
        var fields = GetComponentsInChildren<WheatField>();
        foreach (var field in fields)
        {
            wheatFields.Add(field);
        }
    }

    void Start()
    {
        localLight.enabled = connected;
    }

    void Update()
    {
    }
}