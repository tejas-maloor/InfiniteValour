using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundFinisherState : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        attackIndex = 4;
        duration = 2f;
        anim.SetTrigger("Attack" + attackIndex);
        shouldCombo = false;
        ComboCharacter.knockHit = true;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        controller.AttackMove(comboCharacter.attack4Move.Evaluate(time));

        if (time >= 1f)
        {
            controller.canMove = true;
        }

        if (time >= duration)
        {
            controller.canMove = true;
            comboCharacter.idleCombatState = true;
            stateMachine.SetNextStateToMain();
        }

    }

    public override void OnExit()
    {
        base.OnExit();


        comboCharacter.allowCombo = false;
        ComboCharacter.knockHit = false;
    }
}
