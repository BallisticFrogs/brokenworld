using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;

    [SceneObjectsOnly] public ParticleSystem mergeParticleSystem;

    private void Awake()
    {
        INSTANCE = this;
        Debug.Log("registering game manager=" + this);
        Debug.Log("mergeParticleSystem=" + mergeParticleSystem);
    }

    public void PlayIslandMergeVFX(Vector3 contactPoint)
    {
        mergeParticleSystem.transform.position = contactPoint;
        mergeParticleSystem.Play();
    }
}