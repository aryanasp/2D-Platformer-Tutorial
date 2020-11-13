using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Character : MonoBehaviour
{
    public Animator MyAnimator { get; private set; }
    [SerializeField]
    protected float movementSpeed;
    protected bool rightFaced;
    [SerializeField]
    protected GameObject kunaiPrefab;
    [SerializeField]
    protected Transform kunaiPos;
    [SerializeField]
    protected Stat healthStat;
    [SerializeField]
    private EdgeCollider2D katanaCollider;
    [SerializeField]
    private List<string> damageSources;
    private bool isInKatanaPlace;
    public abstract void Death();
    public Vector3 startPos;
    public abstract bool IsDead
    {
        get;
    }
    public Rigidbody2D MyRigidBody { get; private set; }
    public SpriteRenderer MySpriteRenderer { get; set; }
    public bool Attack { get; set; }
    public bool TakingDamage { get; set; }
    public EdgeCollider2D KatanaCollider 
    { 
        get => katanaCollider;
    }

    protected void Awake()
    {
        
    }
    // Start is called before the first frame update
    protected virtual void Start()
    {
        healthStat.Initialize();
        startPos = transform.position ;
        rightFaced = true;
        MyRigidBody = GetComponent<Rigidbody2D>();
        MyAnimator = GetComponent<Animator>();
        MySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isInKatanaPlace);
    }

    public abstract IEnumerator TakeDamage();

    protected virtual void ChangeDirection()
    {
        rightFaced = !rightFaced;
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, 1);
    }

    protected virtual void ThrowKunai(int value)
    {
        if (rightFaced)
        {
            GameObject tmp = (GameObject)Instantiate(kunaiPrefab, kunaiPos.position, Quaternion.Euler(new Vector3(0, 0, -90)));
            tmp.GetComponent<Kunai>().Initialize(Vector2.right);
        }
        else
        {
            GameObject tmp = (GameObject)Instantiate(kunaiPrefab, kunaiPos.position, Quaternion.Euler(new Vector3(0, 0, +90)));
            tmp.GetComponent<Kunai>().Initialize(Vector2.left);
        }
    }

    public Vector2 GetDirection()
    {
        return rightFaced ? Vector2.right : Vector2.left;
    }

    public void MeleeAttack()
    {
        KatanaCollider.enabled = true;
        if (isInKatanaPlace)
        {
            StartCoroutine(TakeDamage());
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(damageSources.Contains(other.tag))
        {
            StartCoroutine(TakeDamage());
            isInKatanaPlace = true;
        }
        
    }


    public virtual void OnTriggerExit2D(Collider2D other)
    {
        if (damageSources.Contains(other.tag))
        {
            isInKatanaPlace = false;
            
        }

    }

}
