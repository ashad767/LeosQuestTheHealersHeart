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

    public int playerComboScalingDamage;
    public float playerAttackCooldown;
    public float playerHealCooldown;
    public float playerComboGrace;

    [Space]

    public int bowChargeState = 0;
    public int playerComboCounter;
    [HideInInspector]public float playerComboTimer;


    [HideInInspector]public int swordLevel;
    private int bowLevel;
    private int magicLevel;
    public Dictionary<int, int> indexToWeaponLevel = new Dictionary<int, int>() {};


    #endregion

    #region Components

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public UIManager playerUIManager;

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
    public PlayerSword intermediateSword;
    public PlayerBow intermediateBow;
    [Space]
    public PlayerBow advancedBow;
    [Space]
    public PlayerBow expertBow;

    public PlayerWeapon[,] weapons = new PlayerWeapon[3,4];
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

        TestInputs();

        anim.SetFloat("BowChargeState", bowChargeState);
        if(swordLevel > 0)
            anim.SetFloat("ComboCounter", playerComboCounter);
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

        playerComboTimer -= Time.deltaTime;
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
            playerUIManager.UpdatePlayerUI();
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

        weapons[0, 1] = intermediateSword;
        weapons[1, 1] = intermediateBow;
        weapons[2, 1] = basicMagic;

        weapons[0, 2] = intermediateSword;
        weapons[1, 2] = advancedBow;
        weapons[2, 2] = basicMagic;

        weapons[0, 3] = intermediateSword;
        weapons[1, 3] = expertBow;
        weapons[2, 3] = basicMagic;

        currentWeaponIndex = 0;
        currentWeapon = weapons[currentWeaponIndex, indexToWeaponLevel[currentWeaponIndex]];

        playerUIManager.UpdatePlayerUI();
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

        if (Input.GetKeyDown(KeyCode.Insert))
        {
            swordLevel = Math.Min(swordLevel + 1, 3);
            indexToWeaponLevel.Remove(0);
            indexToWeaponLevel.Add(0, swordLevel);

            currentWeapon = weapons[currentWeaponIndex, indexToWeaponLevel[currentWeaponIndex]];
            playerUIManager.UpdatePlayerUI();
        }

        if (Input.GetKeyDown(KeyCode.Home))
        {
            bowLevel = Math.Min(bowLevel + 1, 3);
            indexToWeaponLevel.Remove(1);
            indexToWeaponLevel.Add(1, bowLevel);
            
            currentWeapon = weapons[currentWeaponIndex, indexToWeaponLevel[currentWeaponIndex]];
            playerUIManager.UpdatePlayerUI();
        }
    }
}
