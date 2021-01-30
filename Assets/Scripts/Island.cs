using Sirenix.OdinInspector;
using UnityEngine;

public class Island : MonoBehaviour
{
    public bool connected;

    [SceneObjectsOnly] public Light light;

    void Start()
    {
        light.enabled = connected;
    }

    // Update is called once per frame
    void Update()
    {
    }
}