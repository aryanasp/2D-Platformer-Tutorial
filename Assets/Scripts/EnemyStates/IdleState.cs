using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IEnemyState
{

    private Enemy enemy;

    private float idleTimer;

    private float idleDuration;
    public void Enter(Enemy enemy)
    {
        idleDuration = UnityEngine.Random.Range(1, 10);
        this.enemy = enemy;
    }

    public void Execute()
    {
        if (enemy.Target != null)
        {
            if (enemy.InMeleeRange)
            {
                enemy.ChangeState(new MeleeState());
            }
            else if (enemy.InThrowRange)
            {
                enemy.ChangeState(new RangedState());
            }
            else
            {
                enemy.ChangeState(new PatrolState());
            }
        }
        Idle();
    }

    public void Exit()
    {
        
    }

    public void OnTriggerEnter(Collider2D other)
    {
        if(other.tag == "Kunai")
        {
            enemy.Target = Player.Instance.gameObject;
        }
    }

    private void Idle()
    {
        enemy.MyAnimator.SetFloat("speed", 0);

        idleTimer += Time.deltaTime;

        if (enemy.InMeleeRange)
        {
            enemy.ChangeState(new MeleeState());
        }
        else if (enemy.InThrowRange)
        {
            enemy.ChangeState(new RangedState());
        }
        else if(idleTimer >= idleDuration)
        {
            enemy.ChangeState(new PatrolState());
        }
    }
}
