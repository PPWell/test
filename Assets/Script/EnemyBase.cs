using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public enum EnemyState
{
    Idle,
    Patrol,
    Pursuit,
    Attack,
    GetHit,
    Death
}



public class EnemyBase : MonoBehaviour
{
    [Header("Idle")]
    public float idleTime = 1f;
    public SpriteRenderer spriteRenderer;
    public Animator enemyAnimator;
            
    [Header("Patrol")]
    public EnemyState currentState = EnemyState.Patrol;
    public Transform left;
    public Transform right;
    public Rigidbody2D rb;
    [HideInInspector] public bool isRight = false;
    public float speed = 1f;
    [HideInInspector] public bool canMove = true;

    [Header("Pursuit")]
    public float EnemyAttackDis = 1f;
    [HideInInspector]public GameObject player;
    

    [Header("Attack")]
    public float attackTime = 0.7f;
    public float attackCoolTime = 1.5f;
    public float attackCreateBoxTime = 0.3f;
    private bool canAttack = true;
    public bool isAttacking = false;
    public Transform attackPoint1;
    public Transform attackPoint1R;
    public GameObject attackBox;
    public float[] attackSoundInvokeTime;

    [Header("Attributes")]
    public float HPMax=100f;
    public float HPNow = 100f;
    public float ATK = 10f;


    [Header("GetHit")]
    public float getHitTime = 0.5f;
    public float getHitAddForce = 300f;
    [HideInInspector] public bool isGetHit = false;
    public Slider hpSilde;
    public Text hpText;
    public RectTransform damageNumPoint;
    public GameObject damageNum;

    [Header("Dead")]
    public GameObject EnemyAndPosition;
    public GameObject hpCanvas;

    public virtual void Start()
    {
        PatrolEnter();
        HPNow = HPMax;
        hpText.text = HPNow.ToString() + "/" + HPMax.ToString();
    }

    
    public virtual void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:IdleUpdate(); break;
            case EnemyState.Patrol: PatrolUpdate(); break;
            case EnemyState.Pursuit: PursuitUpdate(); break;
            case EnemyState.Attack: AttackUpdate(); break;
            case EnemyState.GetHit: GetHitUpdate(); break;
            case EnemyState.Death: DeathUpdate(); break;
        }
    }

    public virtual void FixedUpdate()
    {
        if (canMove)
        {
            rb.linearVelocityX = (isRight ? 1 : -1) * speed;
        }
        else
            rb.linearVelocityX = 0;
    }

    #region ×´Ì¬»ú
    //¾²Ö¹
    public virtual void IdleEnter()
    {
        canMove = false;
        enemyAnimator.SetBool("IsRun",false);
        Invoke(nameof(ChangeIdleToPatrol), 1f);
    }
    public virtual void IdleUpdate()
    {

    }
    public virtual void IdleExit()
    {

    }
    //Ñ²Âß
    public virtual void PatrolEnter()
    {
        canMove = true;
        enemyAnimator.SetBool("IsRun", true);
    }
    public virtual void PatrolUpdate()
    {
        if(transform.position.x <= left.position.x && !isRight)
        {
            ChangecurrentState(EnemyState.Idle);
        }
        else if(transform.position.x >= right.position.x && isRight)
        {
            ChangecurrentState(EnemyState.Idle);
        }
    }
    public virtual void PatrolExit()
    {

    }
    //×·»÷
    public virtual void PursuitEnter()
    {
        CancelInvoke(nameof(ChangeIdleToPatrol));
        canMove = true;
        enemyAnimator.SetBool("IsRun", true);
    }
    public virtual void PursuitUpdate()
    {
        if (player != null)
        {

            if (player.transform.position.x <= transform.position.x)
            {
                isRight = false;
                spriteRenderer.flipX = isRight;
            }
            else
            {
                isRight = true;
                spriteRenderer.flipX = isRight;
            }

            if(Mathf.Abs(player.transform.position.x-transform.position.x)<EnemyAttackDis)
            {
                if(player.transform.position.x <= transform.position.x && !isRight)
                {
                    ChangecurrentState(EnemyState.Attack);
                }
                else if (player.transform.position.x > transform.position.x && isRight)
                { 
                    ChangecurrentState(EnemyState.Attack); 
                }
                    
            }
        }
    }
    public virtual void PursuitExit()
    {

    }

    //¹¥»÷
    public virtual void AttackEnter()
    {
        canMove = false;
        enemyAnimator.SetBool("IsRun", false);

    }
    public virtual void AttackUpdate()
    {
        if (canAttack && !isAttacking && !isGetHit)
        {
            canAttack = false;
            isAttacking = true; 
            enemyAnimator.SetTrigger("Attack1");
            Invoke(nameof(CreateAttackBox), attackCreateBoxTime);
            Invoke(nameof(SetCanAttack), attackCoolTime);
            Invoke(nameof(SetIsAttacking), attackTime);
            Invoke(nameof(PlayAttackSound), attackSoundInvokeTime[0]);
        }

        if(!isAttacking)
        {
            if (Mathf.Abs(player.transform.position.x - transform.position.x) > EnemyAttackDis)
            {
                ChangecurrentState(EnemyState.Pursuit);
            }
            else
            {
                if (player.transform.position.x >= transform.position.x && !isRight)
                {
                    ChangecurrentState(EnemyState.Pursuit);
                }
                else if (player.transform.position.x < transform.position.x && isRight)
                {
                    ChangecurrentState(EnemyState.Pursuit);
                }
            }
        }
    }
    public virtual void AttackExit()
    {

    }

    //ÊÜ»÷
    public virtual void GetHitEnter()
    {
        canMove = false;
        CancelInvoke(nameof(CreateAttackBox));
        CancelInvoke(nameof(ChangeStateToPursuit));
        enemyAnimator.SetBool("GetHit", true);
        enemyAnimator.SetBool("IsRun", false);
        Invoke(nameof(ChangeStateToPursuit), getHitTime);

        if(player!=null)
        {
            if(player.transform.position.x <= transform.position.x)
            {
                rb.AddForce(new Vector2(getHitAddForce, 0));
            }
            else if(player.transform.position.x > transform.position.x)
            {
                rb.AddForce(new Vector2(-getHitAddForce, 0));
            }
        }
    }
    public virtual void GetHitUpdate()
    {
        if(!isGetHit)
        {
            isGetHit = true;
        }
    }
    public virtual void GetHitExit()
    {
        isGetHit = false;
        enemyAnimator.SetBool("GetHit", false);
    }


    //ËÀÍö
    public virtual void DeathEnter()
    {
        CancelInvoke(nameof(ChangeStateToPursuit));
        enemyAnimator.SetBool("IsRun", false);
        enemyAnimator.SetTrigger("IsDead");
        canMove = false;
        Destroy(EnemyAndPosition, 3f);
    }
    public virtual void DeathUpdate()
    {

    }
    public virtual void DeathExit()
    {

    }
    #endregion

    public virtual void ChangecurrentState(EnemyState newState)


    {
        switch (currentState)
        {
            case EnemyState.Idle: IdleExit(); break;
            case EnemyState.Patrol: PatrolExit(); break;
            case EnemyState.Pursuit: PursuitExit(); break;
            case EnemyState.Attack: AttackExit(); break;
            case EnemyState.GetHit: GetHitExit(); break;
            case EnemyState.Death: DeathExit(); break;
        }
        currentState = newState;
        switch (newState)
        {
            case EnemyState.Idle: IdleEnter(); break;
            case EnemyState.Patrol: PatrolEnter(); break;
            case EnemyState.Pursuit: PursuitEnter(); break;
            case EnemyState.Attack: AttackEnter(); break;
            case EnemyState.GetHit: GetHitEnter(); break;
            case EnemyState.Death: DeathEnter(); break;
        }
    }

    public virtual void ChangeIdleToPatrol()
    {
        isRight = !isRight;
        spriteRenderer.flipX = isRight;
        ChangecurrentState(EnemyState.Patrol);
    }

    public virtual void FindPlayer(GameObject mainPlayer)
    {
        if (currentState != EnemyState.Death)
        {
            player = mainPlayer;
            ChangecurrentState(EnemyState.Pursuit);
        }
    }

    public virtual void PlayerOut()
    {
        if (currentState != EnemyState.Death)
            ChangecurrentState(EnemyState.Patrol);
    }

    public virtual void SetCanAttack()
    {
        canAttack = true;
    }

    public virtual void SetIsAttacking()
    {
        isAttacking = false;
    }

    public virtual void GetHit(float damage)
    {
        if (currentState != EnemyState.Death)
        {
            ChangecurrentState(EnemyState.GetHit);
            HPNow -= damage;
            if(HPNow <= 0)
            {
                HPNow = 0;
                ChangecurrentState(EnemyState.Death);
                Destroy(hpCanvas,0.5f);
            }
            hpSilde.value = HPNow/HPMax;
            hpText.text = HPNow.ToString() + "/" + HPMax.ToString();
            GameObject go = Instantiate(damageNum, damageNumPoint.position, damageNumPoint.rotation,
                hpCanvas.transform);
            go.transform.localScale = damageNumPoint.localScale;
            go.GetComponent<Text>().text = damage.ToString();
        }
    }

    public virtual void ChangeStateToPursuit()
    {
        if (currentState != EnemyState.Death)
        {
            ChangecurrentState(EnemyState.Pursuit);
        }
    }


    public virtual void CreateAttackBox()
    {
        if (currentState != EnemyState.Death && currentState != EnemyState.GetHit)
        {
            GameObject go;
            if (isRight)
            {
                go = Instantiate(attackBox, attackPoint1R.position, attackPoint1R.rotation, transform);
                go.transform.localScale = attackPoint1R.localScale;
            }
            else
            {
                go = Instantiate(attackBox, attackPoint1.position, attackPoint1.rotation, transform);
                go.transform.localScale = attackPoint1.localScale;
            }
            EnemyAttackBox enemyAttackBox = go.GetComponent<EnemyAttackBox>();
            enemyAttackBox.damage =Mathf.Round( ATK * Random.Range(0.8f, 1.4f));
        }

    }

    public virtual void PlayAttackSound()
    {
        if(currentState != EnemyState.Death && currentState != EnemyState.GetHit )
        {
            SoundManger.Instance.PlayShortMusic(1);
        }
    }
}