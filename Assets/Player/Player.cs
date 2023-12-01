using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : Entity
{
    #region Stats

    [Header("Stats")]

    public TextMeshProUGUI playerStats;

    [Space]

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


    private int swordLevel;
    private int bowLevel;
    private int magicLevel;
    private Dictionary<int, int> indexToWeaponLevel = new Dictionary<int, int>() {};


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

    [Space]

    public PlayerSword basicSword;
    public PlayerMagic basicMagic;
    public PlayerBow basicBow;
    [Space]
    public PlayerBow intermediateBow;

    private PlayerWeapon[,] weapons = new PlayerWeapon[3,4];
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

    protected override void Start()
    {
        base.Start();
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        stateMachine.currentState.Update();

        UpdateCooldowns();
        CheckWeaponSwap();
        UpdateUI();

        TestInputs();

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
                currentWeaponIndex = 2;

            else if (currentWeaponIndex > 2)
                currentWeaponIndex = 0;

            currentWeapon = weapons[currentWeaponIndex, indexToWeaponLevel[currentWeaponIndex]];
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
            "Health: " + CurrentHealth +
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
        indexToWeaponLevel.Add(0, swordLevel);
        indexToWeaponLevel.Add(1, bowLevel);
        indexToWeaponLevel.Add(2, magicLevel);

        weapons[0, 0] = basicSword;
        weapons[1, 0] = basicBow;
        weapons[2, 0] = basicMagic;

        weapons[0, 1] = basicSword;
        weapons[1, 1] = intermediateBow;
        weapons[2, 1] = basicMagic;

        weapons[0, 2] = basicSword;
        weapons[1, 2] = basicBow;
        weapons[2, 2] = basicMagic;

        weapons[0, 3] = basicSword;
        weapons[1, 3] = basicBow;
        weapons[2, 3] = basicMagic;

        currentWeaponIndex = 0;
        currentWeapon = weapons[currentWeaponIndex, indexToWeaponLevel[currentWeaponIndex]];
    }

    private void StatsInit()
    {
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

    public void TestInputs()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            bowLevel = Math.Min(bowLevel + 1, 3);
            indexToWeaponLevel.Remove(1);
            indexToWeaponLevel.Add(1, bowLevel);
            
            currentWeapon = weapons[currentWeaponIndex, indexToWeaponLevel[currentWeaponIndex]];
        }
    }
}
