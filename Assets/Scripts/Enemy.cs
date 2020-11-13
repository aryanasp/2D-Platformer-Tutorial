using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : Character
{
    private IEnemyState currentState;
    public GameObject Target { get; set; }
    public float throwTimer;
    public float throwCooldown;
    public bool canThrow = true;
    public float attackTimer;
    public double attackCooldown;
    public bool canAttack = true;
    private bool dropCoin = true;
    private Canvas healthCanvas;
    [SerializeField]
    private float meleeRange;
    [SerializeField]
    private float throwRange;
    [SerializeField]
    private Transform leftEdge;
    [SerializeField]
    private Transform rightEdge;
    [SerializeField]
    private Collider2D throwArea;

    public bool InMeleeRange
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= meleeRange;
            }
            return false;
        }
    }

    public bool InThrowRange
    {
        get
        {
            if (Target != null)
            {
                return Vector2.Distance(transform.position, Target.transform.position) <= throwRange;
            }
            return false;
        }
    }

    public override bool IsDead
    {
        get
        {
            return healthStat.CurrentVal <= 0;
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        Player.Instance.Dead += new DeadEventHandler(RemoveTarget);
            
        ChangeState(new PatrolState());

        healthCanvas = transform.GetComponentInChildren<Canvas>();

    }

    protected override void ChangeDirection()
    {
        Transform tmp = transform.Find("EnemyHealthBarCanvas").transform;
        Vector3 pos = tmp.position;
        tmp.SetParent(null);
        base.ChangeDirection();
        tmp.SetParent(transform);
        tmp.position = pos;
        //healthCanvas.transform.SetParent(null);
        //Vector3 pos = healthCanvas.transform.position;
        //base.ChangeDirection();
        //healthCanvas.transform.SetParent(transform);
        //healthCanvas.transform.position = pos;

    }
    public void RemoveTarget()
    {
        Target = null;
        ChangeState(new IdleState());
    }
    void LookAtTarget()
    {
        if (Target != null)
        {
            float xDir = Target.transform.position.x - transform.position.x; 

            if (xDir < 0 && rightFaced || xDir > 0 && !rightFaced)
            {
                
                
                ChangeDirection();
                
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsDead)
        {
            if (!TakingDamage)
            {
                currentState.Execute();
            }
            LookAtTarget();
        }
    }

    public void ChangeState(IEnemyState newState)
    {
        if(currentState != null)
        {
            currentState.Exit();
        }

        currentState = newState;
        currentState.Enter(this);
    }

    public void Move()
    {
        if ((GetDirection().x > 0 && transform.position.x < rightEdge.position.x) || (GetDirection().x < 0 && transform.position.x > leftEdge.position.x))
        {
            if (!Attack)
            {
                MyAnimator.SetFloat("speed", 1);
                transform.Translate(GetDirection() * (movementSpeed * Time.deltaTime));
            
            }
            if(currentState is RangedState)
            {
                if (!throwArea.IsTouching(Player.Instance.GetComponent<Collider2D>()))
                {
                    
                    ChangeState(new IdleState());
                }
            }
        }
        else if(currentState is PatrolState)
        {
            ChangeDirection();
        }
        else if (currentState is RangedState)
        {
            Target = null;
            ChangeState(new IdleState());
        }
    }


    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        currentState.OnTriggerEnter(other);
    }

    public override IEnumerator TakeDamage()
    {
        
        if (!healthCanvas.isActiveAndEnabled)
        {
            
            healthCanvas.enabled = true;
        }
        healthStat.CurrentVal -= 10;
        if (!IsDead)
        {
            MyAnimator.SetTrigger("damage");
        }
        else if (dropCoin)
        {
            dropCoin = false;
            GameObject coin = (GameObject) Instantiate(GameManager.Instance.CoinPrefab, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
            Physics2D.IgnoreCollision(coin.GetComponent<Collider2D>(), GetComponent<Collider2D>(), true); 
            MyAnimator.SetTrigger("dead");
            yield return null;
        }
    }

    public override void Death()
    {
        MyAnimator.ResetTrigger("dead");
        ChangeState(new IdleState());
        MyAnimator.SetTrigger("idle");
        healthStat.CurrentVal = healthStat.MaxVal;
        transform.position = startPos;
        healthCanvas.enabled = false;
        dropCoin = true;
    }
}
