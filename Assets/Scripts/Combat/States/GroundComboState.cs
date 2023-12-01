using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundComboState : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {

        base.OnEnter(_stateMachine);

        attackIndex = 2;
        duration = 0.45f;
        anim.SetTrigger("Attack" + attackIndex);
        shouldCombo = false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        controller.AttackMove(comboCharacter.attack2Move.Evaluate(time));

        if (time >= duration)
        {
            if (shouldCombo)
            {
                stateMachine.SetNextState(new GroundCombo2State());
            }
            else
            {
                controller.canMove = true;
                comboCharacter.idleCombatState = true;
                stateMachine.SetNextStateToMain();
            }
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        comboCharacter.allowCombo = false;
    }
}
