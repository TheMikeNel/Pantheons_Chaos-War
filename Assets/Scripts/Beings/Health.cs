using StarterAssets;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Health : MonoBehaviour
{
    [Header("Main")]
    [SerializeField] private bool _isPlayer = false;
    [SerializeField] private float _maxHealth = 100f;
    public float currentHealth = 100f;

    [Space]
    [Header("Hit Settings")]
    [SerializeField] private bool _enableEffectOnHit = false;
    [SerializeField, Tooltip("Must be on the enemy")] private ParticleSystem _hitParticles;
    [SerializeField] private bool _enableInvulAfterHit = false;
    [SerializeField] private float _InvulTime = 1f;

    [Space]
    [Header("Die Settings")]
    [SerializeField] private bool _destroyOnDeath = true;
    [SerializeField] private float _destroyDelay = 2f;
    [SerializeField] private bool _eventOnDeath = false;
    [SerializeField, Tooltip("Particles etc.")] private GameObject _eventPrefab;

    private Animator _anim;

    //States
    private bool _isDead = false;
    private bool _isInvul = false;

    //Properties
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => currentHealth;

    private void Start()
    {
        _anim = GetComponent<Animator>();
        currentHealth = _maxHealth;

        if (_isPlayer) EventBus.Instance.PlayerHealthHasChanges?.Invoke(this);
    }



    public void TakeOrAddHealth(float value, AttackEffect effect = AttackEffect.Default)
    {
        if (!_isDead)
        {
            // Add Health
            if (value > 0)
            {
                currentHealth += value;

                if (currentHealth > _maxHealth) currentHealth = _maxHealth;
            }

            // Else try Take Hit
            else if (value < 0 && !_isInvul && (!GetInBlock() || effect != AttackEffect.Default))
            {
                currentHealth += value;

                if (currentHealth <= 0) // Dead if Health < 0
                {
                    currentHealth = 0;
                    Death();
                }
                else
                {
                    if (effect == AttackEffect.NotBlocked) SetInBlock(false);
                    if (effect != AttackEffect.KillAura) TakeHit();
                }
            }

            if (_isPlayer) EventBus.Instance.PlayerHealthHasChanges?.Invoke(this);
        }
    }

    private void TakeHit()
    {
        _anim.SetTrigger("TakeHit");

        if (_enableEffectOnHit)
        {
            _hitParticles.Play();
        }

        if (_enableInvulAfterHit)
        {
            _isInvul = true;
            Invoke(nameof(ResetInvul), _InvulTime);
        }

    }

    private void ResetInvul() => _isInvul = false;

    private void Death()
    {
        if (_isPlayer) EventBus.Instance.PlayerIsDead?.Invoke();
        else EventBus.Instance.BeingDeadLayerInd?.Invoke(gameObject.tag, gameObject.layer);

        _anim.SetTrigger("Death");
        _isDead = true;

        //Disable States
        if (gameObject.TryGetComponent(out PlayerControl tpc) && TryGetComponent(out PlayerBattleSystem battleSys))
        {
            tpc.enabled = false; battleSys.enabled = false;
        }
        else if (TryGetComponent(out EnemyController enemy))
        {
            if (gameObject.TryGetComponent(out Attacking atk)) atk.enabled = false;

            enemy.enabled = false;
        }

        if (_eventOnDeath) Instantiate(_eventPrefab, transform.position, Quaternion.identity);

        if (_destroyOnDeath) StartCoroutine(DestroyAnimation());
    }

    private IEnumerator DestroyAnimation()
    {
        yield return new WaitForSeconds(_destroyDelay);

        for (int i = 0; i < 30; i++)
        {
            yield return new WaitForFixedUpdate();
            transform.localScale *= 0.8f;
        }

        Destroy(gameObject);
    }


    private bool GetInBlock()
    {
        AnimatorControllerParameter[] animParams = _anim.parameters;

        foreach (AnimatorControllerParameter param in animParams)
        {
            if (param.name == "InBlock" && param.type == AnimatorControllerParameterType.Bool)
                return _anim.GetBool("InBlock");
        }
        return false;
    }

    private void SetInBlock(bool value)
    {
        AnimatorControllerParameter[] animParams = _anim.parameters;

        foreach (AnimatorControllerParameter param in animParams)
        {
            if (param.name == "InBlock" && param.type == AnimatorControllerParameterType.Bool)
                _anim.SetBool("InBlock", value);
        }
    }

    public void MultiplyMaxHealth(float value, bool setCurrentHealthAsMax)
    {
        _maxHealth *= value;

        if (setCurrentHealthAsMax) currentHealth = _maxHealth;
    }
}

public enum AttackEffect
{
    Default,
    NotBlocked,
    KillAura
}
