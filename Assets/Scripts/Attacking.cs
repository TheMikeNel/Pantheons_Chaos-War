using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Attacking : MonoBehaviour
{
    [SerializeField] private float _baseAttackDamage = 10f;
    [SerializeField] private float _specialAttackDamage = 15f;

    public float damageMultiplier = 1f;

    [Header("Soul Stealer")]
    public GameObject SoulTrailPrefab;
    public Transform soulMouth;

    private Health _target;

    public void SetTarget(Health target)
    {
        this._target = target;
    }

    //Calling it in animation event
    public void BaseAttackTarget()
    {
        if (_target != null) _target.TakeOrAddHealth(-_baseAttackDamage * damageMultiplier);
    }

    //Calling it in animation event
    public void SpecialAttackTarget(AttackEffect attackEffect)
    {
        if (_target != null) _target.TakeOrAddHealth(-_specialAttackDamage * damageMultiplier, attackEffect);
    }

    //Calling it in animation event
    public void TakeTheSoul()
    {
        GameObject soul = Instantiate(SoulTrailPrefab, _target.transform);
        soul.GetComponent<SoulTrail>().soulMouth = this.soulMouth;
    }
}
