using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KillAura : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float damageDelay = 1f;
    [SerializeField] private float distance = 5f;
    [SerializeField] private int maxEnemiesInRange = 16;
    [SerializeField] private Vector3 checkOffset = Vector3.zero;
    [SerializeField] private LayerMask attackingLayers;

    private void Start()
    {
        StartCoroutine(TryMakeDamage());
    }

    private IEnumerator TryMakeDamage()
    {
        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(damageDelay);

            if (CheckEnemies(out List<Health> enemsHealth))
            {
                foreach (Health health in enemsHealth)
                {
                    health.TakeOrAddHealth(-damage, AttackEffect.KillAura);
                }
            }
        }
    }

    private bool CheckEnemies(out List<Health> enemsHealth)
    {
        Collider[] enemsResult = new Collider[maxEnemiesInRange];
        enemsHealth = new List<Health>();

        int enemsInRange = Physics.OverlapSphereNonAlloc(transform.position + checkOffset, distance, enemsResult, attackingLayers);
        if (enemsInRange > 0)
        {
            for (int i = 0; i < enemsInRange; i++)
            {
                if ((enemsResult[i].transform.position - transform.position - checkOffset).magnitude <= distance
                    && enemsResult[i].gameObject.TryGetComponent(out Health enemyHealth))
                {
                    enemsHealth.Add(enemyHealth);
                }
            }
        }

        return enemsHealth.Count > 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + checkOffset, distance);
    }
}
