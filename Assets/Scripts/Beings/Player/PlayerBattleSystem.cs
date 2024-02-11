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

    [Header("Skills")]
    [Header("Ultimate")]
    [SerializeField] private GameObject ultimateObject;
    [SerializeField, Range(0.1f, 360f)] public float ultimateRechargingTime = 15f;
    [SerializeField] private float ultimateDuration = 10f;

    //Components
    private Animator _anim;
    private ThirdPersonController _TPC;
    private GameObject _weaponObject;
    private Weapon _weapon;

    //States
    private readonly Collider[] _enemiesResult = new Collider[16];
    private int _atkCombo = 0;
    private float _atkSpeed = 1f;
    private bool _canUltimate = false;
    private float _currentUltimateCharge = 0f;
    private bool _canBlocking = true;
    private float _currentBlockStamina;
    private bool _inBlock = false;
    
    //Properties
    public GameObject Weapon
    {
        get => _weaponObject;
        set
        {
            if (_weaponObject) Destroy(_weaponObject);

            _weaponObject = value;

            if (value) _weaponObject = Instantiate(value, _weaponAnchor);

            if (_weaponObject.TryGetComponent(out _weapon))
            {
                AttackSpeed = _weapon.GetAttackSpeed();
            }
            else AttackSpeed = 1f;
        }
    }
    public int AttackCombo
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
    public float UltimateCharge
    {
        get => _currentUltimateCharge;
        set
        {
            _currentUltimateCharge = value;
            EventBus.Instance.PlayerBattleSystemHasChanges?.Invoke(this);
        }
    }
    public bool InBlock
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
    private GameObject _tempUltimateObj;
    private float _tempSpeed;
    private float _tempSprint;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _TPC = GetComponent<ThirdPersonController>();
        _tempSpeed = _TPC.MoveSpeed;
        _tempSprint = _TPC.SprintSpeed;

        if (_startWeaponPrefab != null) Weapon = _startWeaponPrefab;

        EventBus.Instance.PlayerBattleSystemHasChanges?.Invoke(this);
    }

    void Update()
    {
        SkillsSystem();

        BlockingSystem();

        if (!InBlock && Input.GetMouseButtonUp(0))
        {
            if (AttackCombo < _maxAttackCombo) AttackCombo++;
            StopAllCoroutines();
            StartCoroutine(ResetComboTimer());
        }
    }

    private void SkillsSystem()
    {
        if (_canUltimate && ultimateObject && Input.GetKeyDown(KeyCode.R))
        {
            _canUltimate = false;
            _tempUltimateObj = Instantiate(ultimateObject, transform.position, Quaternion.identity);
            StartCoroutine(UltimateWorking());
        }
        UltimateCharging();
    }

    private IEnumerator UltimateWorking()
    {
        _canUltimate = false;

        yield return new WaitForSeconds(ultimateDuration);

        Destroy(_tempUltimateObj);
        UltimateCharge = 0f;
    }

    private void UltimateCharging()
    {
        if (UltimateCharge < ultimateRechargingTime)
        {
            UltimateCharge += Time.deltaTime;
        }

        if (UltimateCharge >= ultimateRechargingTime)
        {
            UltimateCharge = ultimateRechargingTime;
            _canUltimate = true;
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
}
