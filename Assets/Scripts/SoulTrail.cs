using UnityEngine;

public class SoulTrail : MonoBehaviour
{
    public Transform soulMouth;

    void FixedUpdate()
    {
        transform.LookAt( soulMouth );
    }
}
