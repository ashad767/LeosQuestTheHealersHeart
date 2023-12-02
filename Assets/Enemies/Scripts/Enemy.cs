using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    [SerializeField]
    public Rigidbody2D RB { get; set; }
    [HideInInspector]
    public Animator animator { get; private set; }
    public bool IsFacingRight { get; set; } = true;
    public GameObject Player { get; set; }

    #region SM Variables

    public EnemySM enemySM { get; set; }
    public EnemyAttack AttackState { get; set; }
    public EnemyWalk WalkState { get; set; }
    public EnemyChase ChaseState { get; set; }

    #endregion

    #region Walk Variables

    public float MovementRange = 5f;
    public float MovementSpeed = 1f;

    #endregion

    #region

    public bool IsAggro { get; set; }
    public bool IsStrike { get; set; }

    #endregion

    private void Awake()
    {
        enemySM = new EnemySM();
        animator = GetComponentInChildren<Animator>();
        AttackState = new EnemyAttack(this, enemySM, "Attack");
        WalkState = new EnemyWalk(this, enemySM, "Walk");
        ChaseState = new EnemyChase(this, enemySM, "Chase");

        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        RB = GetComponent<Rigidbody2D>();
        enemySM.Initialize(WalkState);
    }

    // Update is called once per frame
    private void Update()
    {
        enemySM.CurrentEnemyState.FrameUpdate();
        Debug.Log(enemySM.CurrentEnemyState);
        Debug.Log("Strike: " + IsStrike);
    }

    private void FixedUpdate()
    {
        enemySM.CurrentEnemyState.PhysicsUpdate();
    }

    private void AnimationTriggerEvent(AnimationTriggerType type)
    {
        enemySM.CurrentEnemyState.AnimationTriggerEvent(type);
    }

    public enum AnimationTriggerType
    {
        EnemyDamaged,
        Footsteps
    } 

    public void MoveEnemy(Vector2 velocity)
    {
        RB.velocity = velocity;
        CheckLeftOrRight(velocity);
        animator.SetFloat("xInput", velocity.x);
        animator.SetFloat("yInput", velocity.y);
        Vector2 direction = (Player.transform.position - transform.position).normalized * MovementSpeed;
        animator.SetFloat("xPInput", direction.x);
        animator.SetFloat("yPInput", direction.y);
    }

    public void CheckLeftOrRight(Vector2 velocity)
    {
        /*if(IsFacingRight && velocity.x < 0f)
        {
            Vector3 rotate = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotate);
            IsFacingRight = !IsFacingRight;

        }
        else if(!IsFacingRight && velocity.x > 0f)
        {
            Vector3 rotate = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotate);
            IsFacingRight = !IsFacingRight;
        }*/
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
