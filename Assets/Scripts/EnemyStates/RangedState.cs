using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedState : IEnemyState
{
    
    
    private Enemy enemy;
    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Execute()
    {
        ThrowKunai();
        
        if (enemy.Target != null)
        {
            if (!enemy.canThrow && !enemy.canAttack)
            {
                enemy.ChangeState(new PatrolState());
            }

            if (enemy.InMeleeRange)
            {
                enemy.ChangeState(new MeleeState());
            } 
            else
            {
                enemy.Move();
            }
        }
        else
        {
            enemy.ChangeState(new IdleState());
        }
    }

    public void Exit()
    {

    }

    public void OnTriggerEnter(Collider2D other)
    {

    }

    private void ThrowKunai()
    {
        enemy.throwTimer += Time.deltaTime;
        if(enemy.throwTimer >= enemy.throwCooldown)
        {
            enemy.canThrow = true;
            enemy.throwTimer = 0;
        }

        if (enemy.canThrow)
        {   
            enemy.canThrow = false;
            enemy.MyAnimator.SetTrigger("throw");
        }
    }
}
