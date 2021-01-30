using Sirenix.OdinInspector;
using UnityEngine;

public class Island : MonoBehaviour
{
    public bool connected;
    public int followers;
    
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