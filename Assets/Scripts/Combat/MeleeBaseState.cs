using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBaseState : State
{
    public float duration;

    protected Animator anim;

    protected ThirdPersonController controller;

    protected bool shouldCombo = false;

    protected int attackIndex;

    protected ComboCharacter comboCharacter;

    float attackPressedTimer = 0;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);
        comboCharacter = GetComponent<ComboCharacter>();
        controller = comboCharacter.controller;
        anim = comboCharacter.anim;
        controller.canMove = false;
        comboCharacter.idleCombatState = false;
        //anim.applyRootMotion = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        attackPressedTimer -= Time.deltaTime;

        if(Input.GetMouseButtonDown(0))
        {
            attackPressedTimer = 2f;
        }

        if(attackPressedTimer > 0 && comboCharacter.allowCombo) 
        {
            shouldCombo = true;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        controller.canMove = true;
        //anim.applyRootMotion = false;
    }
}
