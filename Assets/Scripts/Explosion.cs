using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float expDamage = 10f;
    public float expRadius = 4f;
    public float expDelay = 0.4f;
    public Vector3 expCenterOffset = Vector3.zero;
    public LayerMask damageableLayers;

    public bool destroyAfterDelay = false;
    public float destroyDelay = 3f;

    private Collider[] _result = new Collider[32];
    private int _resultCount;

    void Start()
    {
        Invoke(nameof(DoExplosion), expDelay);
    }

    private void DoExplosion()
    {
        _resultCount = Physics.OverlapSphereNonAlloc(transform.position + expCenterOffset, expRadius, _result, damageableLayers);

        if (_resultCount > 0 )
        {
            for (int i = 0; i < _resultCount; i++)
            {
                if ((_result[i].transform.position - (transform.position + expCenterOffset)).magnitude <= expRadius / 2 && _result[i].TryGetComponent(out Health health))
                    health.TakeOrAddHealth(-expDamage);
            }
        }

        if (destroyAfterDelay) Destroy(gameObject, destroyDelay);
    }
}
