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
    public float currentShield;

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


    [HideInInspector] public int swordLevel;
    [HideInInspector] public int bowLevel;
    [HideInInspector] public int magicLevel;
    public Dictionary<int, int> indexToWeaponLevel = new Dictionary<int, int>() {};

    [HideInInspector] public float coins;

    public float invincibleTimer;

    private float coinMultiplier = 1;
    private float armour = 1;

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
    public PlayerDeathState deathState;

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
    public PlayerBow basicBow;
    public PlayerMagic basicMagic;
    [Space]
    public PlayerSword intermediateSword;
    public PlayerBow intermediateBow;
    public PlayerMagic intermediateMagic;
    [Space]
    public PlayerSword advancedSword;
    public PlayerBow advancedBow;
    public PlayerMagic advancedMagic;
    [Space]
    public PlayerSword expertSword;
    public PlayerBow expertBow;
    public PlayerMagic expertMagic;

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

        if (stateMachine.currentState is PlayerGroundState)
        {
            CheckWeaponSwap();
        }

        TestInputs();

        playerUIManager.UpdatePlayerUI();
        AnimatorVariablesUpdate();

        if(CurrentHealth == 0)
            stateMachine.ChangeState(deathState);
    }

    private void AnimatorVariablesUpdate()
    {
        anim.SetInteger("Weapon", currentWeaponIndex);
        anim.SetFloat("BowChargeState", bowChargeState);
        if (swordLevel > 1)
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
        TimersUpdate();
        ShieldUpdate();
    }

    private void TimersUpdate()
    {
        dashTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;
        healTimer -= Time.deltaTime;
        playerComboTimer -= Time.deltaTime;
        invincibleTimer -= Time.deltaTime;
    }

    private void ShieldUpdate()
    {
        currentShield -= Time.deltaTime / 4;
        if (currentShield < 0)
            currentShield = 0;
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
        deathState = new PlayerDeathState(stateMachine, this, "Death");
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
        weapons[2, 1] = intermediateMagic;

        weapons[0, 2] = advancedSword;
        weapons[1, 2] = advancedBow;
        weapons[2, 2] = advancedMagic;

        weapons[0, 3] = expertSword;
        weapons[1, 3] = expertBow;
        weapons[2, 3] = expertMagic;

        currentWeaponIndex = 0;
        currentWeapon = weapons[currentWeaponIndex, indexToWeaponLevel[currentWeaponIndex]];

        playerUIManager.UpdatePlayerUI();
    }

    private void StatsInit()
    {
        currentEnergy = MaxEnergy;
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

    public void AddShield(int ammount)
    {
        currentShield += ammount;

        if(currentShield > MaxShield)
        {
            currentShield = MaxShield;
        }
    }

    public void Heal(int ammount)
    {
        CurrentHealth += ammount;

        if (CurrentHealth > maxHealth)
        {
            CurrentHealth = maxHealth;
        }
    }

    public void RegenEnergy()
    {
        if(currentEnergy < MaxEnergy) 
            currentEnergy += Time.deltaTime * 2;

        if(currentEnergy > MaxEnergy)
            currentEnergy = MaxEnergy;
    }

    public override void TakeDamage(float damage)
    {
        if(invincibleTimer < 0)
        {
            if (currentShield > 0)
            {
                if (currentShield >= damage * 1 / armour)
                {
                    currentShield -= damage * 1/armour;
                }
                else
                {
                    base.TakeDamage(damage - currentShield * 1 / armour);
                    currentShield = 0;
                }
            }
            else
            {
                base.TakeDamage(damage * 1 / armour);
            }
        }

    }

    public void UpgradeSkill(int skill)
    {
            if(skill == 0 && swordLevel != 3)
            {
                swordLevel = Math.Min(swordLevel + 1, 3);
                indexToWeaponLevel.Remove(0);
                indexToWeaponLevel.Add(0, swordLevel);
            }

            else if(skill == 1 && bowLevel != 3)
            {
                bowLevel = Math.Min(bowLevel + 1, 3);
                indexToWeaponLevel.Remove(1);
                indexToWeaponLevel.Add(1, bowLevel);
            }

            else if (skill == 2 && magicLevel != 3)
            {
                magicLevel = Math.Min(magicLevel + 1, 3);
                indexToWeaponLevel.Remove(2);
                indexToWeaponLevel.Add(2, magicLevel);
            }

            currentWeapon = weapons[currentWeaponIndex, indexToWeaponLevel[currentWeaponIndex]];
            playerUIManager.UpdatePlayerUI();
    }

    public void AddCoins(int amt)
    {
        coins += amt * coinMultiplier;
    }

    public bool RemoveCoins(int amt)
    {
        if(coins < amt)
        {
            return false;
        }

        coins -= amt;
        return true;
    }

    public void AddMaxHealth(int amt)
    {
        maxHealth += amt;
        Heal(amt);
    }

    public void AddMaxEnergy(int amt)
    {
        MaxEnergy += amt;
        currentEnergy += amt;
    }

    public void ChangeSpeed(float speedIncrease)
    {
        //Base speed is 4
        moveSpeed += speedIncrease;
    }

    public void ChangeCoinMultiplier(float coinIncrease)
    {
        //base is 1
        coinMultiplier += coinIncrease;
    }

    public void ChangeArmourMultiplier(float coinIncrease)
    {
        //base is 1
        armour += coinIncrease;
    }

    public void TestInputs()
    {
        if (Input.GetKeyDown(KeyCode.Insert))
            UpgradeSkill(0);
        if (Input.GetKeyDown(KeyCode.Home))
            UpgradeSkill(1);
        if (Input.GetKeyDown(KeyCode.PageUp))
            UpgradeSkill(2);

        if (Input.GetKeyDown(KeyCode.PageDown))
            AddCoins(1);
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
            TakeDamage(5);
    }
}
