using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Stats

    [Header("Stats")]

    public TextMeshProUGUI playerStats;

    [Space]

    public float MaxHealth;
    protected float currentHealth;
    public float MaxEnergy;
    protected float currentEnergy;
    public float MaxShield;
    protected float currentShield;

    [Space]

    public float moveSpeed;
    public float runMultiplier;

    [Space]

    public float playerDashCooldown;
    public float playerDashDuration;
    public float playerDashSpeed;
    public float playerDashCost;

    [Space]

    public float playerAttackCooldown;
    public float playerHealCooldown;
    [HideInInspector]public int bowChargeState = 0;

    #endregion

    #region Components

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }

    #endregion

    #region States

    public PlayerStateMachine stateMachine;

    public PlayerIdleState idleState;
    public PlayerMoveState moveState;
    public PlayerAttackState attackState;
    public PlayerMagicState magicState;
    public PlayerRunState runState;
    public PlayerDashState dashState;

    #endregion

    #region Timers

    [HideInInspector] public float dashTimer;
    [HideInInspector] public float attackTimer;
    [HideInInspector] public float healTimer;

    #endregion

    #region Equipment

    [Header("Weapon Information")]
    public PlayerWeapon currentWeapon;
    [HideInInspector] public int currentWeaponIndex;

    [HideInInspector] public PlayerBasicSword basicSword;
    [HideInInspector] public PlayerBasicMagic basicMagic;
    [HideInInspector] public PlayerBasicBow basicBow;

    private PlayerWeapon[] weapons = new PlayerWeapon[3];
    #endregion

    #region Hitboxes

    [Header("Hitboxes")]
    public HitBox swordUpHitBox;
    public HitBox swordRightHitBox;
    public HitBox swordDownHitBox;
    public HitBox swordLeftHitBox;

    #endregion

    private void Awake()
    {
        StateMachineInit();
        WeaponsInit();
        StatsInit();
    }

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine.Initialize(idleState);
    }

    void Update()
    {
        stateMachine.currentState.Update();

        UpdateCooldowns();
        CheckWeaponSwap();
        UpdateUI();

        anim.SetFloat("BowChargeState", bowChargeState);
    }

    public void SetVelocity(float xVelocity, float yVelocity, float multiplier)
    {
        if(xVelocity != 0 || yVelocity != 0)
            rb.velocity = new Vector2(xVelocity, yVelocity).normalized * moveSpeed * multiplier;
        else
            rb.velocity = Vector2.zero;
    }

    public bool CanDash() => (dashTimer <= 0 && currentEnergy >= playerDashCost);

    private void UpdateCooldowns()
    {
        dashTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;
        healTimer -= Time.deltaTime;
    }

    private void CheckWeaponSwap()
    {
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            currentWeaponIndex++;
        }

        else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            currentWeaponIndex--;
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (currentWeaponIndex < 0)
                currentWeaponIndex = weapons.Length - 1;

            else if (currentWeaponIndex > weapons.Length - 1)
                currentWeaponIndex = 0;

            currentWeapon = weapons[currentWeaponIndex];
            anim.SetInteger("Weapon", currentWeaponIndex);
        }
    }

    public void IncrementBowState()
    {
        bowChargeState++;
    }

    public void ResetBowState()
    {
        bowChargeState = 0;
    }

    private void UpdateUI()
    {
        playerStats.SetText(
            "Health: " + currentHealth +
            "\nShield: " + currentShield +
            "\nEnergy: " + currentEnergy +
            "\nWeapon: " + currentWeapon.name 
        );
    }

    private void StateMachineInit()
    {
        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(stateMachine, this, "Idle");
        moveState = new PlayerMoveState(stateMachine, this, "Move");
        attackState = new PlayerAttackState(stateMachine, this, "Attack");
        magicState = new PlayerMagicState(stateMachine, this, "Attack");
        runState = new PlayerRunState(stateMachine, this, "Run");
        dashState = new PlayerDashState(stateMachine, this, "Dash");
    }

    private void WeaponsInit()
    {
        basicSword = GetComponentInChildren<PlayerBasicSword>();
        basicMagic = GetComponentInChildren<PlayerBasicMagic>();
        basicBow = GetComponentInChildren<PlayerBasicBow>();

        weapons[0] = basicSword;
        weapons[1] = basicBow;
        weapons[2] = basicMagic;
        
        currentWeaponIndex = 0;
        currentWeapon = weapons[currentWeaponIndex];
    }

    private void StatsInit()
    {
        currentHealth = MaxHealth;
        currentEnergy = MaxEnergy;
        currentShield = MaxShield;
    }

    public float GetEnergy() => currentEnergy;
    public void UseEnergy(float energy)
    {
        currentEnergy -= energy;

        if(currentEnergy < 0)
        {
            currentEnergy = 0;
        }
    }

    public void RegenEnergy()
    {
        if(currentEnergy < MaxEnergy) 
            currentEnergy += Time.deltaTime * 2;

        if(currentEnergy > MaxEnergy)
            currentEnergy = MaxEnergy;
    }
}
