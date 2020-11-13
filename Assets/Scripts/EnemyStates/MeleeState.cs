using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeState : IEnemyState
{
    private Enemy enemy;
    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void Execute()
    {
        Attack();
        if (enemy.Target != null)
        {
            if (!enemy.InMeleeRange && enemy.InThrowRange)
            {
                enemy.ChangeState(new RangedState());
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

    private void Attack()
    {
        enemy.attackTimer += Time.deltaTime;
        if (enemy.attackTimer >= enemy.attackCooldown)
        {
            enemy.canAttack = true;
            enemy.attackTimer = 0;
        }
        if (enemy.canAttack)
        {
            enemy.MyAnimator.SetTrigger("attack");
            enemy.canAttack = false;
            enemy.ChangeState(new IdleState());
        }
    }
}
