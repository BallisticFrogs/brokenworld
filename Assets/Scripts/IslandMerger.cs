using UnityEngine;

public class IslandMerger : MonoBehaviour
{
    private Island island;
    private Rigidbody body;

    void Start()
    {
        island = GetComponentInParent<Island>();
        body = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            var other = contact.otherCollider.gameObject;
            var otherMerger = other.GetComponent<IslandMerger>();
            if (island.connected && !otherMerger.island.connected)
            {
                otherMerger.island.connected = true;
                otherMerger.island.localLight.enabled = true;
                GameManager.INSTANCE.PlayIslandMergeVFX(contact.point);
                PlayerController.INSTANCE.AddFollowers(otherMerger.island.followers);

                // TODO SFX
            }
        }
    }
}