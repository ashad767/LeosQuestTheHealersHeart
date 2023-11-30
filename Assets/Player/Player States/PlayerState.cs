using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerState 
{
    #region Components

    protected PlayerStateMachine stateMachine;
    protected Player player;

    protected Rigidbody2D rb;

    #endregion

    protected Vector3 mousePosition;
    public int facingDirection;

    protected float xInput;
    protected float yInput;
    protected string animBoolName;

    protected float stateTimer;

    public PlayerState(PlayerStateMachine playerStateMachine, Player player, string animBoolName)
    {
        this.stateMachine = playerStateMachine;
        this.player = player;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        player.anim.SetBool(animBoolName, true);
        rb = player.rb;
    }

    public virtual void Update()
    {

        stateTimer -= Time.deltaTime;
        mousePosition = Input.mousePosition;

    }

    public virtual void Exit()
    {
        player.anim.SetBool(animBoolName, false);
    }

    protected int GetFacingDirection()
    {
        var angleVector = mousePosition - Camera.main.WorldToScreenPoint(player.transform.position);
        var facingAngle = MathF.Atan2(angleVector.x, angleVector.y) * Mathf.Rad2Deg;

        if(facingAngle > -45 && facingAngle <= 45)
        {
            return 1;
        }

        else if(facingAngle > 45 && facingAngle <= 135)
        {
            return 2;
        }

        else if(facingAngle > 135 || facingAngle < -145)
        {
            return 3;
        }

        else
        {
            return 4;
        }
    }
}
