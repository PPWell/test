using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [Header("Player Move")]
    public Rigidbody2D rb;
    private float xInput;
    public float moveSpeed=5;
    public Animator playerAnimator;
    public SpriteRenderer playerSprit;
    [HideInInspector] public bool isRigth;
    

    [Header("Player Jump")]
    public float jumpForce = 5;
    //public GameObject playerGO;
    public LayerMask groundLayer;
    [HideInInspector]public bool isGround = true;
    [HideInInspector] public bool isJump = false;
    [HideInInspector] public bool isAir = false;


    [Header("Player Dash")]
    public float dashSpeed = 15;
    public float dashTime = 0.1f;
    public float dashCoolDownTime = 1f;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool canDash = true;


    [Header("Player Attack")]
    public Transform attackPoint1;
    public Transform attackPoint2;
    public Transform attackPoint3;
    public Transform attackPoint1L;
    public Transform attackPoint2L;
    public Transform attackPoint3L;
    public GameObject attackBox;
    public float attackAddForce = 300f;
    [HideInInspector] public bool isAttack = false;
    [HideInInspector]public int combo = 1;
    public float[] attackSoundInvokeTime;

    [Header("Player Attributes")]
    public float HPMax = 100f;
    public float HPNow = 100f;
    public float ATK = 10f;

    [Header("Player Dead")]
    public bool isDead = false;

    [Header("Player GetHit")]
    public Slider hpSilde;
    public Text hpText;
    public RectTransform damageNumPoint;
    public GameObject damageNum;
    public GameObject hpCanvas;

    void Start()
    {
        Application.targetFrameRate = 60;
        HPNow = HPMax;
        hpText.text = HPNow.ToString() + "/" + HPMax.ToString();
    }

    void Update()
    {
        CheckGround();
        PlayerMove();
        PlayerJump();
        PlayerDash();
        PlayerAttack();
    }
    private void FixedUpdate()
    {
        FixPlayerMove();
    }


    public void PlayerMove()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if(!isAttack && !isDead)
        {
            if (xInput > 0)
            {
                isRigth = true;
                playerSprit.flipX = false;
                playerAnimator.SetBool("IsRun", true);
            }
            else if (xInput < 0)
            {
                isRigth = false;
                playerSprit.flipX = true;
                playerAnimator.SetBool("IsRun", true);
            }
            else if (!(xInput != 0) && playerAnimator.GetBool("IsRun") == true)
            {
                playerAnimator.SetBool("IsRun", false);
            }
        }
    }
    public void FixPlayerMove()
    {
        if (!isAttack && !isDead)
        {
            if (isDashing)
            {
                rb.linearVelocity = new Vector2(dashSpeed * (isRigth ? 1 : -1), 0);
            }
            else
                rb.linearVelocity = new Vector2(moveSpeed * xInput, rb.linearVelocityY);
        }
        else
            rb.linearVelocity = new Vector2(0,0);
    }

    public void PlayerJump()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (isGround && !isDead)
            {
                rb.AddForce(new Vector2(rb.linearVelocityX, jumpForce));
                playerAnimator.SetTrigger("IsJump");
                Invoke("SetIsJump", 0.1f);

            }
        }

        if (!isGround && rb.linearVelocityY<0 && !playerAnimator.GetBool("IsAir") && !isDashing)
        {
            playerAnimator.SetBool("IsAir", true);
            isAir = true;
        }


        if (isGround && (isJump||isAir))
        {
            playerAnimator.SetBool("IsGround", true);
            playerAnimator.SetBool("IsAir",false);
            isJump = false;
            isAir = false;
        }
    }

    public void CheckGround()
    {
        Vector2 startPos = transform.position;
        Vector2 endPos = transform.position + Vector3.down;
        RaycastHit2D hit = Physics2D.Linecast(startPos, endPos, groundLayer);

        if(hit.collider!=null)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
            playerAnimator.SetBool("IsGround", false);
        }

    }
    public void SetIsJump()
    {
        isJump = true;
    }

    public void PlayerDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDead)
        {
            if (!isDashing&&canDash)
            {
                isDashing = true;
                canDash = false;
                playerAnimator.SetBool("IsDash",true);
                Invoke("DashEnd", dashTime);
                Invoke(nameof(DashCoolDown), dashCoolDownTime);
            }
        }
    }

    public void DashEnd()
    {
        isDashing = false;
        playerAnimator.SetBool("IsDash", false);
    }

    public void DashCoolDown()
    {
        canDash = true;
    }

    public void PlayerAttack()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAttack && !isJump && !isDead)
        {
            isAttack = true;
            CancelInvoke(nameof(ComboReset));
            Invoke(nameof(ComboReset), 2f);
            switch (combo)
            {
                case 1:
                    {
                        playerAnimator.SetTrigger("Attack1");
                        Invoke(nameof(CreateCombo1AttackBox), 0.4f);
                        Invoke(nameof(AttackEnd), 0.5f);
                        Invoke(nameof(PlayAtackSound), attackSoundInvokeTime[0]);
                        rb.AddForce(new Vector2((isRigth ? 1 : -1) * attackAddForce, 0));
                        combo = 2;
                        return;
                    }
                case 2:
                    {
                        playerAnimator.SetTrigger("Attack2");
                        Invoke(nameof(CreateCombo2AttackBox), 0.25f);
                        Invoke(nameof(AttackEnd), 0.3f);
                        Invoke(nameof(PlayAtackSound), attackSoundInvokeTime[1]);
                        rb.AddForce(new Vector2((isRigth ? 1 : -1) * attackAddForce*0.8f, 0));
                        combo = 3;
                        return;
                    }
                case 3:
                    {
                        playerAnimator.SetTrigger("Attack3");
                        Invoke(nameof(CreateCombo3AttackBox), 0.25f);
                        Invoke(nameof(AttackEnd), 0.5f);
                        Invoke(nameof(PlayAtackSound), attackSoundInvokeTime[2]);
                        rb.AddForce(new Vector2((isRigth ? 1 : -1) * attackAddForce*1.2f, 0));
                        combo = 1;
                        return;
                    }
            }
        }
    }

    public void AttackEnd()
    {
        isAttack = false;
    }

    public void ComboReset()
    {
        combo = 1;
    }

    public void CreateCombo1AttackBox()
    {
        GameObject go;
        if (isRigth)
        {
            go = Instantiate(attackBox, attackPoint1.transform.position, attackPoint1.rotation, transform);
            go.transform.localScale = attackPoint1.localScale;
        }
        else
        {
            go = Instantiate(attackBox, attackPoint1L.transform.position, attackPoint1L.rotation, transform);
            go.transform.localScale = attackPoint1L.localScale;
        }
        PlayerAttackBox playerAttackBox = go.GetComponent<PlayerAttackBox>();
        playerAttackBox.damage = ATK * 1.0f;
    }

    public void CreateCombo2AttackBox()
    {
        GameObject go;
        if (isRigth)
        {
            go = Instantiate(attackBox, attackPoint2.transform.position, attackPoint2.rotation, transform);
            go.transform.localScale = attackPoint2.localScale;
        }
        else
        {
            go = Instantiate(attackBox, attackPoint2L.transform.position, attackPoint2L.rotation, transform);
            go.transform.localScale = attackPoint2L.localScale;
        }
        PlayerAttackBox playerAttackBox = go.GetComponent<PlayerAttackBox>();
        playerAttackBox.damage = ATK * 1.2f;
    }

    public void CreateCombo3AttackBox()
    {
        GameObject go;
        if (isRigth)
        {
            go = Instantiate(attackBox, attackPoint3.transform.position, attackPoint3.rotation, transform);
            go.transform.localScale = attackPoint3.localScale;
        }
        else
        {
            go = Instantiate(attackBox, attackPoint3L.transform.position, attackPoint3L.rotation, transform);
            go.transform.localScale = attackPoint3L.localScale;
        }
        PlayerAttackBox playerAttackBox = go.GetComponent<PlayerAttackBox>();
        playerAttackBox.damage = ATK * 1.5f;
    }

    public void GetHit(float damage)
    {
        if (!isDead)
        {
            playerAnimator.SetBool("IsGetHit",true);
            HPNow -= damage;
            Invoke(nameof(GetHitEnd), 0.5f);
            hpSilde.value = HPNow/HPMax;
            hpText.text = HPNow.ToString() + "/" + HPMax.ToString();

            GameObject go = Instantiate(damageNum, damageNumPoint.position, damageNumPoint.rotation,
                hpCanvas.transform);
            go.transform.localScale = damageNumPoint.localScale;
            go.GetComponent<Text>().text = damage.ToString();
        }
        if (HPNow <= 0) 
        {
            isDead = true;
            HPNow = 0;
            playerAnimator.SetTrigger("IsDead");
        }
        hpSilde.value = HPNow / HPMax;
        hpText.text = HPNow.ToString() + "/" + HPMax.ToString();
    }

    public void GetHitEnd()
    {
        playerAnimator.SetBool("IsGetHit", false);
    }

    public void PlayAtackSound()
    {
        SoundManger.Instance.PlayShortMusic(0);
    }
}
