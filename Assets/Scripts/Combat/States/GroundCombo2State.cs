using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCombo2State : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        attackIndex = 3;
        duration = 0.7964f;
        anim.SetTrigger("Attack" + attackIndex);
        shouldCombo = false;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        controller.AttackMove(comboCharacter.attack3Move.Evaluate(time));

        if (time >= duration)
        {
            if (shouldCombo)
            {
                stateMachine.SetNextState(new GroundFinisherState());
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
