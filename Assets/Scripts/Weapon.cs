using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float _baseDamage = 10f;
    [SerializeField, Range(0.1f, 5.0f)] private float _attackSpeedMultiplier = 1f;
    [SerializeField] private float _attackRadius = 2f;
    [SerializeField] private LayerMask _attackLayers;

    public float GetBaseDamage() => _baseDamage;

    public float GetAttackSpeed() => _attackSpeedMultiplier;

    public float GetAttackRadius() => _attackRadius;

    public LayerMask GetAttackLayers() => _attackLayers;
}
