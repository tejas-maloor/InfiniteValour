using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEntryState : MeleeBaseState
{
    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        attackIndex = 1;
        duration = 0.85f;
        anim.SetTrigger("Attack" + attackIndex);
        shouldCombo = false;

    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        controller.AttackMove(comboCharacter.attack1Move.Evaluate(time));

        if(time >= duration)
        {
            if(shouldCombo)
            {
                stateMachine.SetNextState(new GroundComboState());
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
