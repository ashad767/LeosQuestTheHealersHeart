using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField]
    public Rigidbody2D RB { get; set; }
    [HideInInspector]
    public Animator animator { get; private set; }
    public GameObject Player { get; set; }
    public HitBox[] hitboxes;
    private Vector2 direction; 


    #region SM Variables

    public EnemySM enemySM { get; set; }
    public EnemyAttack AttackState { get; set; }
    public EnemyWalk WalkState { get; set; }
    public EnemyChase ChaseState { get; set; }
    public EnemyDead DeadState { get; set; }
    public EnemyRangedAttack AttackRangedState { get; set; }

    #endregion

    #region Walk Variables

    public float MovementRange = 5f;
    public float MovementSpeed = 1f;

    #endregion

    #region Radius

    public bool IsAggro { get; set; }
    public bool IsStrike { get; set; }

    #endregion

    #region Sound Clips

    public AudioSource audioSource;
    public AudioClip WalkSound;
    public AudioClip AttackSound;

    #endregion

    #region Ranged Enemies

    public bool isRanged = false;
    public Rigidbody2D MM_Projectile;

    #endregion

    private void Awake()
    {
        enemySM = new EnemySM();
        animator = GetComponentInChildren<Animator>();
        AttackState = new EnemyAttack(this, enemySM, "Attack");
        WalkState = new EnemyWalk(this, enemySM, "Walk");
        ChaseState = new EnemyChase(this, enemySM, "Chase");
        DeadState = new EnemyDead(this, enemySM, "Dead");
        AttackRangedState = new EnemyRangedAttack(this, enemySM, "RangedAttack");

        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        RB = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        enemySM.Initialize(WalkState);
    }

    // Update is called once per frame
    protected override void Update()
    {
        enemySM.CurrentEnemyState.FrameUpdate();
        if(CurrentHealth == 0)
        {
            enemySM.ChangeState(DeadState);
        }
        Debug.Log(enemySM.CurrentEnemyState);
    }

    private void FixedUpdate()
    {
        enemySM.CurrentEnemyState.PhysicsUpdate();
    }

    /*private void AnimationTriggerEvent(AnimationTriggerType type)
    {
        enemySM.CurrentEnemyState.AnimationTriggerEvent(type);
    }

    public enum AnimationTriggerType
    {
        EnemyDamaged,
        Footsteps,
        Die
    } */

    public void MoveEnemy(Vector2 velocity)
    {
        RB.velocity = velocity;
        CheckDirection();
        animator.SetFloat("xInput", velocity.x);
        animator.SetFloat("yInput", velocity.y);

        direction = (Player.transform.position - transform.position).normalized;
        animator.SetFloat("xPInput", direction.x);
        animator.SetFloat("yPInput", direction.y);
    }

    public void EnemyAttack(List<GameObject> entities)
    {
        foreach (GameObject obj in entities)
        {
            Player ply = obj.GetComponent<Player>();

            if (ply != null)
            {
                ply.TakeDamage(3);
            }
        }
    }

    public int CheckDirection()
    {
        if(Mathf.Abs(direction.x) < Mathf.Abs(direction.y))
        {
            if (direction.y < 0f)
                return 3;
            else
                return 1;
        }
        else
        {
            if (direction.x < 0f)
                return 4;
            else
                return 2;
        }
    }

    public void SetAggroStatus(bool isAggro)
    {
        IsAggro = isAggro;        
    }

    public void SetStrikeStatus(bool isStrike)
    {
        IsStrike = isStrike;
    }
}
