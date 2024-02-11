using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    [SerializeField] PlayableDirector timeline;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && timeline != null)
        {
            timeline.Play();
            Destroy(gameObject);
        }
    }
}
