using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;

public delegate void DeadEventHandler();
public class Player : Character
{
    public event DeadEventHandler Dead;

    private IUseable useable;
    private static Player instance;

    public static Player Instance 
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<Player>();
            }
            return instance;
        }
        
    }

    [SerializeField] 
    private Transform[] groundPoints;
    [SerializeField]
    private float climbSpeed;
    [SerializeField] 
    private bool airControl;
    [SerializeField] 
    private float groundRadius;
    [SerializeField] 
    private LayerMask whatIsGround;
    [SerializeField] 
    private float jumpForce;
    [SerializeField]
    private float immortalTime;

    
    private bool immortal = false;
    public bool IsClimbing { get; set; }

    public bool OnLadder { get; set; }

    public bool Slide { get; set; }
    public bool Jump { get; set; }
    public bool OnGround { get; set; }
    public bool IsFalling 
    { 
        get 
        {
            return MyRigidBody.velocity.y < 0; 
        } 
    }

    public override bool IsDead
    {
        get
        {
            if(healthStat.CurrentVal <= 0)
            {
                OnDead();
            }
            return healthStat.CurrentVal <= 0;
        }
    }

    private Vector2 vec;
    private float btnDirection;
    private bool btnMove;
    private float btnHorizontal;
    

    // Start is called before the first frame update

    protected override void Start()
    {
        base.Start();
        airControl = false;
        IsClimbing = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!TakingDamage && !IsDead)
        {
            if (transform.position.y < -14f)
            {
                MyRigidBody.velocity = Vector2.zero;
                healthStat.CurrentVal = 0;
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("dead");
            }
            HandleInput();
        }
        if ((MyRigidBody.velocity.y != 0) && OnLadder)
        {
            MyAnimator.SetTrigger("reset");
        }
        else if (!OnLadder || (OnLadder && (MyRigidBody.velocity.y == 0)))
        {
            
            MyAnimator.ResetTrigger("reset");
        }

    }
    void FixedUpdate()
    {
        if (!TakingDamage && !IsDead)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            OnGround = IsGrounded();
            if (btnMove)
            {
                btnHorizontal = Mathf.Lerp(btnHorizontal, btnDirection, Time.deltaTime * 2);
                HandleMovement(btnHorizontal);
                Flip(btnDirection);
            }
            else
            {

                HandleMovement(horizontal, vertical);
                Flip(horizontal);
            }
            HandleAniamtorLayers();
        }
        
    }

    public void OnDead()
    {
        if (Dead != null)
        {
            Dead();
        }
    }

    private void HandleMovement(float horizontal, float vertical = 1)
    {
        if(IsFalling)
        {
            gameObject.layer = 12;
            MyAnimator.SetBool("landing", true);
        }

        if(!Attack && (OnGround || airControl))
        {
            //MyRigidBody.velocity = new Vector2(horizontal * movementSpeed, MyRigidBody.velocity.y);
            if (OnGround && Slide)
            {
                vec.x = (float)-0.000000001;
                vec.y = 0;
                MyRigidBody.AddForce(vec);
            }
            else
            {
                MyRigidBody.velocity = new Vector2(horizontal * movementSpeed, MyRigidBody.velocity.y);
            }
        }

     
        if (Jump && MyRigidBody.velocity.y == 0 && !IsClimbing && !OnLadder)
        {
            MyRigidBody.AddForce(new Vector2(0, jumpForce));
        }

        if (IsClimbing)
        {
            MyAnimator.speed = vertical!= 0 ? Mathf.Abs(vertical) : Mathf.Abs(horizontal);
            MyRigidBody.velocity = new Vector2(horizontal * climbSpeed, vertical * climbSpeed);
        }
        MyAnimator.SetFloat("speed", Mathf.Abs(horizontal));

    }
    
    
    
    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !IsClimbing && !IsFalling)
        {

            MyAnimator.SetTrigger("jump");
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            MyAnimator.SetTrigger("attack");
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if(MyAnimator.GetFloat("speed") > 0.01 && OnGround)
            {
                MyAnimator.SetTrigger("slide");
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            MyAnimator.SetTrigger("throw");
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Use();
        }
    }


    private void Flip(float horizontal)
    {
        if (!this.MyAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && !this.MyAnimator.GetCurrentAnimatorStateInfo(0).IsTag("Slide"))
        {
            if (horizontal > 0 && !rightFaced || horizontal < 0 && rightFaced)
            {
                ChangeDirection();
            }
        } 

    }

    private bool IsGrounded()
    {
        
        if (MyRigidBody.velocity.y <= 0)
        {
            foreach (Transform point in groundPoints)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(point.position, groundRadius, whatIsGround);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if(colliders[i].gameObject != gameObject)
                    {
                        return true;
                    }   
                }
            }
        }
        return false;
    }

    private void HandleAniamtorLayers()
    {
        if (!OnGround)
        {
            MyAnimator.SetLayerWeight(1, 1);
        } 
        else
        {
            MyAnimator.SetLayerWeight(1, 0);
        }
    }

    protected override void ThrowKunai(int value)
    {
        if (OnGround && value == 0 || !OnGround && value == 1)
        {
            base.ThrowKunai(value);
        }
    }

    private IEnumerator IndicateImmortal()
    {
        while (immortal)
        {
            MySpriteRenderer.enabled = false;
            yield return new WaitForSeconds(.03f);
            MySpriteRenderer.enabled = true;
            yield return new WaitForSeconds(.03f);
        }
    }

    public override IEnumerator TakeDamage()
    {
        if (!immortal)
        {
            healthStat.CurrentVal -= 10;
            if (!IsDead)
            {
                MyAnimator.SetTrigger("damage");
                immortal = true;
                StartCoroutine(IndicateImmortal());
                yield return new WaitForSeconds(immortalTime);
                immortal = false;
            }
            else
            {
                MyAnimator.SetLayerWeight(1, 0);
                MyAnimator.SetTrigger("dead");
            }
        }
    }

    public override void Death()
    {
        GameManager.Instance.CollectedCoins /= 2;
        MyAnimator.SetTrigger("idle");
        healthStat.CurrentVal = healthStat.MaxVal;
        transform.position = startPos;
    }

    private void Use()
    {
        if(useable != null)
        {
            useable.Use();
        }
    }

    public void BtnJump()
    {
        MyAnimator.SetTrigger("jump");
    }

    public void BtnAttack()
    {
        MyAnimator.SetTrigger("attack");
    }

    public void BtnThrow()
    {
        MyAnimator.SetTrigger("throw");
    }

    public void BtnSlide()
    {
        if (MyAnimator.GetFloat("speed") > 0.01 && OnGround)
        {
            MyAnimator.SetTrigger("slide");
        }
    }

    public void BtnMove(float direction)
    {
        this.btnDirection = direction;
        btnMove = true;
    }

    public void BtnStopMove()
    {
        btnDirection = 0;
        btnHorizontal = 0;
        btnMove = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Coin")
        {
            GameManager.Instance.CollectedCoins++; 
            Destroy(other.gameObject);
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Useable")
        {
            useable = other.GetComponent<IUseable>();
        }
        base.OnTriggerEnter2D(other);
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Useable" && useable != null)
        {
            useable.ExitFromUseArea();
            useable = null;
        }
        base.OnTriggerExit2D(other);
    }
}
