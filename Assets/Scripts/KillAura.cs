using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAura : MonoBehaviour
{
    [SerializeField] private string enemiesTag = "Enemy";
    [SerializeField] private float damage = 1f;
    [SerializeField] private float damageDelay = 1f;
    [SerializeField] private float distance = 5f;
    [SerializeField] private Vector3 checkOffset = Vector3.zero;
    [SerializeField] private LayerMask attackingLayers;

    private List<Health> _enemies;
    private bool _damagesNow = false;

    //private void FixedUpdate()
    //{
    //    Collider[] _enemsResult = new Collider[16];
    //    int enemies = Physics.OverlapSphereNonAlloc(transform.position + checkOffset, distance, _enemsResult, attackingLayers);

    //    if (enemies > 0)
    //    {
    //        Debug.Log("enemies in range: " + enemies);

    //        for (int i = 0; i < enemies; i++)
    //        {
    //            if ((_enemsResult[i].transform.position - transform.position + checkOffset).magnitude <= distance)
    //            {
    //                //if (_enemsResult[i].gameObject.TryGetComponent(out Health checkH))
    //                _enemies.Add(_enemsResult[i].gameObject.GetComponent<Health>());

    //                //if (!_damagesNow) StartCoroutine(MakeDamage());
    //            }
    //        }
    //    }

    //}

    private IEnumerator MakeDamage()
    {
        _damagesNow = true;

        Debug.Log("Coroutine Started");

        while (_enemies.Count > 0)
        {
            yield return new WaitForSeconds(damageDelay);

            foreach (Health health in _enemies)
            {
                if ((health.transform.position - transform.position).magnitude <= distance)
                {
                    _enemies.Remove(health);
                    continue;
                }

                health.TakeOrAddHealth(-damage, AttackEffect.KillAura);
            }
            Debug.Log("Enemies In Range: " + _enemies.Count);
        }

        _damagesNow = false;

        Debug.Log("Coroutine Stopped");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + checkOffset, distance);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter: " + other.gameObject.layer);

        if (attackingLayers == (attackingLayers | (1 << other.gameObject.layer)) && (other.transform.position - transform.position).magnitude <= distance)
        {
            _enemies.Add(other.gameObject.GetComponent<Health>());-
            if (!_damagesNow && _enemies.Count > 0) StartCoroutine(MakeDamage());
        }
    }
}
