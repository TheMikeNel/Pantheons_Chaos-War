using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerBattleSystem : MonoBehaviour
{
    [Header("Attacking")]
    public float damageMultiplier = 1f;
    [SerializeField] private Transform _weaponAnchor;
    [SerializeField] private GameObject _startWeaponPrefab;
    [SerializeField] private Transform _attackCenter;
    [SerializeField] private int _maxAttackCombo = 3;
    [SerializeField] private float _resetComboByTime = 1.0f;

    [Header("Blocking")]
    [SerializeField, Tooltip("Possible block holding time")] public float maxBlockStamina = 1f;
    [SerializeField] private float _blockStaminaIntakeSpeed = 0.5f;
    [SerializeField] private float _blockStaminaResetSpeed = 0.5f;

    [Header("Moving")]
    [SerializeField] private float _moveSpeedInAttack = 1f;
    [SerializeField] private float _moveSpeedInAirAttack = 2f;
    [SerializeField] private float _moveSpeedInBlock = 1f;

    //Components
    private Animator _anim;
    private ThirdPersonController _TPC;
    private Weapon _weapon;

    //States
    private readonly Collider[] _enemiesResult = new Collider[16];
    private int _atkCombo = 0;
    private float _atkSpeed = 1f;
    private float _currentBlockStamina;
    private bool _inBlock = false;
    private bool _canBlocking = true;
    
    //Properties
    private int AttackCombo
    {
        get => _atkCombo;
        set 
        { 
            _atkCombo = value;
            _anim.SetInteger("AttackCombo", _atkCombo);
        }
    }
    public float AttackSpeed
    {
        get => _atkSpeed;
        set
        {
            _atkSpeed = value;
            if (_anim) _anim.SetFloat("AttackSpeed", _atkSpeed);
        }
    }
    private bool InBlock
    {
        get =>_inBlock;
        set
        {
            _inBlock = value;
            _anim.SetBool("InBlock", _inBlock);

            if (value) SetPlayerSpeedForTime(_moveSpeedInBlock, 0);
            else ResetPlayerSpeed();
        }
    }
    public float BlockStamina
    {
        get => _currentBlockStamina;
        set
        {
            _currentBlockStamina = value;
            EventBus.Instance.PlayerBattleSystemHasChanges?.Invoke(this);
        }
    }

    //Temporary vars
    private float _tempSpeed;
    private float _tempSprint;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _TPC = GetComponent<ThirdPersonController>();
        _tempSpeed = _TPC.MoveSpeed;
        _tempSprint = _TPC.SprintSpeed;

        if (_startWeaponPrefab != null)
        {
            Instantiate(_startWeaponPrefab, _weaponAnchor);

            if (_startWeaponPrefab.TryGetComponent(out _weapon)) 
                AttackSpeed = _weapon.GetAttackSpeed();
        }

        EventBus.Instance.PlayerBattleSystemHasChanges?.Invoke(this);
    }

    void Update()
    {
        BlockingSystem();

        if (!InBlock && Input.GetMouseButtonUp(0))
        {
            if (AttackCombo < _maxAttackCombo) AttackCombo++;
            StopAllCoroutines();
            StartCoroutine(ResetComboTimer());
        }
    }

    private void BlockingSystem()
    {
        if (!InBlock && _canBlocking && Input.GetMouseButton(1))
        {
            if (AttackCombo > 0)
            {
                AttackCombo = 0;
                StopAllCoroutines();
            }

            InBlock = true;
        }

        if (InBlock) BlockingStaminaIntake();

        if (InBlock && Input.GetMouseButtonUp(1))
        {
            InBlock = false;
            _canBlocking = false;
            ResetPlayerSpeed();
        }

        if (!InBlock) BlockingStaminaRecovery();
    }

    private IEnumerator ResetComboTimer()
    {
        PlayerInAttack(true);

        yield return new WaitForSeconds(_resetComboByTime);
        AttackCombo = 0;
        PlayerInAttack(false);
    }

    private void PlayerInAttack(bool isAttack)
    {
        if (isAttack)
        {
            if (_anim.GetBool("FreeFall")) SetPlayerSpeedForTime(_moveSpeedInAirAttack, 0);
            else SetPlayerSpeedForTime(_moveSpeedInAttack, 0);
        }
        else
        {
            ResetPlayerSpeed();
        }
    }

    private bool TryGetEnemies(out List<Health> result)
    {
        result = new List<Health>();

        if (_weapon != null)
        {
            int enemies = Physics.OverlapSphereNonAlloc(_attackCenter.position, _weapon.GetAttackRadius(), _enemiesResult, _weapon.GetAttackLayers());

            if (enemies > 0)
            {
                for (int i = 0; i < enemies; i++)
                {
                    if ((_enemiesResult[i].transform.position - transform.position).magnitude <= _weapon.GetAttackRadius()
                        && _enemiesResult[i].TryGetComponent(out Health checkH))
                    {
                        result.Add(checkH);
                    }
                }
            }
        }

        return result.Count > 0;
    }

    private void BlockingStaminaIntake()
    {
        if (InBlock)
        {
            BlockStamina -= _blockStaminaIntakeSpeed * Time.deltaTime;

            if (BlockStamina <= 0)
            {
                BlockStamina = 0;
                _canBlocking = false;
                InBlock = false;
            }
        }
    }

    private void BlockingStaminaRecovery()
    {
        if (!InBlock && BlockStamina < maxBlockStamina)
        {
            BlockStamina += _blockStaminaResetSpeed * Time.deltaTime;

            if (BlockStamina >= maxBlockStamina)
            {
                BlockStamina = maxBlockStamina;
                _canBlocking = true;
            }
        }
    }

    /// <summary>
    /// Set Player Speed For Time. Can be used in animations.
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="time">If set to 0, it is permanent</param>
    public void SetPlayerSpeedForTime(float speed, float time)
    {
        _TPC.MoveSpeed = speed;
        _TPC.SprintSpeed = speed;

        if (time > 0) Invoke(nameof(ResetPlayerSpeed), time);
    }

    public void ResetPlayerSpeed()
    {
        _TPC.MoveSpeed = _tempSpeed;
        _TPC.SprintSpeed = _tempSprint;
    }

    // Using in animation
    public void TryAttacking()
    {
        if (TryGetEnemies(out List<Health> enemies))
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].TakeOrAddHealth(_weapon.GetBaseDamage() * damageMultiplier * -1);
            }
        }
    }

    public void SetPlayerWeapon(GameObject weapon)
    {
        Destroy(_weapon.gameObject);
        _startWeaponPrefab = weapon;
        _startWeaponPrefab.TryGetComponent(out _weapon);
    }
}
