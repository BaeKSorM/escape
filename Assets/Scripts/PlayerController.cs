using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D playerRB;
    public Rigidbody2D boxRB;
    CapsuleCollider2D playerCC;
    Animator anim;
    [SerializeField] internal enum eConditon { 아이들, 걷기, 밀기, 당기기, 수그리기 };
    [SerializeField] internal eConditon conditon;
    [SerializeField] internal float moveSpeed;
    [SerializeField] internal float runSpeed = 5.0f;
    [SerializeField] internal float crouchingSpeed = 2.0f;
    [SerializeField] internal float ppingSpeed = 1.0f;
    [SerializeField] internal float ppMass = 1.0f;
    [SerializeField] internal float jumpForce = 5.0f;
    [SerializeField] internal int jumpCount;
    [SerializeField] internal int maxJumpCount;
    [SerializeField] internal bool isCrouchingZone;
    [SerializeField] internal bool isCrouching;
    [SerializeField] internal bool isWall;
    [SerializeField] internal bool isJump;
    [SerializeField] internal bool isPush;
    [SerializeField] internal bool isPull;
    [SerializeField] internal bool isBox;
    [SerializeField] internal float gDistance = 0f;
    [SerializeField] internal float bDistance = 0f;
    [SerializeField] internal float wDistance = 0f;
    [SerializeField] internal LayerMask groundMask = 0;
    [SerializeField] internal LayerMask boxMask = 0;
    [SerializeField] internal LayerMask wallMask = 0;
    [SerializeField] internal LayerMask hidenMask = 0;
    [SerializeField] internal RaycastHit2D hit;
    [SerializeField] internal GameObject[] walls;
    [SerializeField] internal bool ip;
    [SerializeField] internal bool canUnLock;
    [SerializeField] internal Vector2 checkPoint;
    public VariableJoystick variableJoystick;
    public GameObject end;

    void Awake()
    {
        moveSpeed = runSpeed;
        playerRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerCC = GetComponent<CapsuleCollider2D>();
    }
    private void Update()
    {
        if (GameManager.instance.start)
        {
            Action();
            GroundCheck();
            BoxCheck();
            StickWall();
            WallCheck();
        }
    }
    void FixedUpdate()
    {
        Move();
    }
    void Move()
    {
        float xPos = variableJoystick.Horizontal;
        // #if UNITY_EDITOR
        if (Input.GetAxisRaw("Horizontal") != 0)
            xPos = Input.GetAxisRaw("Horizontal");
        // #endif
        if (xPos > 0) xPos = 1;
        else if (xPos < 0) xPos = -1;
        if (!isWall)
        {
            if (conditon == eConditon.아이들 || conditon == eConditon.수그리기)
            {
                if (xPos > 0)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                else if (xPos < 0)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }
            else if (conditon == eConditon.밀기)
            {
                if (transform.localScale.x > 0 && xPos < 0)
                {
                    return;
                }
                else if (transform.localScale.x < 0 && xPos > 0)
                {
                    return;
                }
            }
            else if (conditon == eConditon.당기기)
            {
                if (transform.localScale.x > 0 && xPos > 0)
                {
                    return;
                }
                else if (transform.localScale.x < 0 && xPos < 0)
                {
                    return;
                }
                if (isBox)
                {
                    boxRB = hitBox.transform.GetComponent<Rigidbody2D>();
                }
            }
        }
        if (isBox && conditon == eConditon.당기기)
        {
            boxRB.velocity = new Vector2(xPos * moveSpeed * 1.2f, boxRB.velocity.y);
        }
        playerRB.velocity = new Vector2(xPos * moveSpeed, playerRB.velocity.y);
        anim.SetFloat("speed", Mathf.Abs(xPos));
    }
    void StickWall()
    {
        if (!isWall)
        {
            anim.SetBool("isWallClimb", false);
        }
    }
    void Action()
    {
        if (conditon == eConditon.아이들)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
        if (Input.GetKeyDown(KeyCode.S) && conditon == eConditon.아이들)
        {
            conditon = eConditon.수그리기;
            anim.SetBool("isCrouch", true);
            playerCC.offset = new Vector2(playerCC.offset.x, -0.2f);
            playerCC.size = new Vector2(playerCC.size.x, 0.5f);
            isCrouching = true;
            moveSpeed = crouchingSpeed;
        }
        if (Input.GetKeyUp(KeyCode.S) && !isCrouchingZone && conditon == eConditon.수그리기)
        {
            conditon = eConditon.아이들;
            anim.SetBool("isCrouch", false);
            playerCC.offset = new Vector2(playerCC.offset.x, -0.05f);
            playerCC.size = new Vector2(playerCC.size.x, 0.9f);
            isCrouching = false;
            moveSpeed = runSpeed;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isWall && conditon == eConditon.아이들 && !isCrouching)
        {
            playerRB.velocity = Vector2.zero;
            conditon = eConditon.밀기;
            anim.SetBool("isPush", true);
            playerRB.velocity = Vector2.zero;
            anim.CrossFade("Player_Push_Blend", 0f);
            playerRB.mass = ppMass;
            isPush = true;
            moveSpeed = ppingSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && !isWall && conditon != eConditon.당기기 && !isCrouching)
        {
            conditon = eConditon.아이들;
            anim.SetBool("isPush", false);
            anim.enabled = false;
            anim.enabled = true;
            playerRB.mass = 1;
            isPush = false;
            moveSpeed = runSpeed;
        }
        if (Input.GetKeyDown(KeyCode.C) && !isWall && conditon == eConditon.아이들 && !isCrouching)
        {
            playerRB.velocity = Vector2.zero;
            conditon = eConditon.당기기;
            anim.SetBool("isPull", true);
            playerRB.velocity = Vector2.zero;
            anim.CrossFade("Player_Pull_Blend", 0f);
            playerRB.mass = ppMass;
            isPull = true;
            moveSpeed = ppingSpeed;
        }
        if (Input.GetKeyUp(KeyCode.C) && !isWall && conditon != eConditon.밀기 && !isCrouching)
        {
            conditon = eConditon.아이들;
            anim.SetBool("isPull", false);
            anim.enabled = false;
            anim.enabled = true;
            playerRB.mass = 1;
            isPull = false;
            moveSpeed = runSpeed;
        }
    }
    public void CrouchDown()
    {
        if (conditon == eConditon.아이들)
        {
            conditon = eConditon.수그리기;
            anim.SetBool("isCrouch", true);
            playerCC.offset = new Vector2(playerCC.offset.x, -0.2f);
            playerCC.size = new Vector2(playerCC.size.x, 0.5f);
            isCrouching = true;
            moveSpeed = crouchingSpeed;
        }
    }
    public void CrouchUp()
    {
        if (!isCrouchingZone && conditon == eConditon.수그리기)
        {
            conditon = eConditon.아이들;
            anim.SetBool("isCrouch", false);
            playerCC.offset = new Vector2(playerCC.offset.x, -0.05f);
            playerCC.size = new Vector2(playerCC.size.x, 0.9f);
            isCrouching = false;
            moveSpeed = runSpeed;
        }
    }
    public void PushDown()
    {
        if (!isCrouchingZone && conditon == eConditon.아이들)
        {
            playerRB.velocity = Vector2.zero;
            conditon = eConditon.밀기;
            anim.SetBool("isPush", true);
            playerRB.velocity = Vector2.zero;
            anim.CrossFade("Player_Push_Blend", 0f);
            playerRB.mass = ppMass;
            isPush = true;
            moveSpeed = ppingSpeed;
        }
    }
    public void PushUp()
    {
        if (!isCrouchingZone && conditon == eConditon.밀기)
        {
            conditon = eConditon.아이들;
            anim.SetBool("isPush", false);
            anim.enabled = false;
            anim.enabled = true;
            playerRB.mass = 1;
            isPush = false;
            moveSpeed = runSpeed;
        }

    }
    public void PullDown()
    {
        if (!isCrouchingZone && conditon == eConditon.아이들)
        {
            playerRB.velocity = Vector2.zero;
            conditon = eConditon.당기기;
            anim.SetBool("isPull", true);
            playerRB.velocity = Vector2.zero;
            anim.CrossFade("Player_Pull_Blend", 0f);
            playerRB.mass = ppMass;
            isPull = true;
            moveSpeed = ppingSpeed;
        }
    }
    public void PullUp()
    {
        if (!isCrouchingZone && conditon == eConditon.당기기)
        {
            conditon = eConditon.아이들;
            anim.SetBool("isPull", false);
            anim.enabled = false;
            anim.enabled = true;
            playerRB.mass = 1;
            isPull = false;
            moveSpeed = runSpeed;
        }
    }
    public void Jump()
    {
        if (jumpCount < maxJumpCount && !isCrouching)
        {
            playerRB.gravityScale = 1;
            playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
            ip = false;
            playerRB.velocity = Vector2.zero;
            anim.CrossFade("Player_Jump", 0f);
            playerRB.velocity = Vector2.up * jumpForce;
            if (jumpCount < 1)
            {
                anim.SetBool("isJump", true);
            }
            else if (jumpCount >= 1)
            {
                anim.SetBool("isDouble", true);
            }
            ++jumpCount;
        }
    }
    void GroundCheck()
    {
        if (playerRB.velocity.y < 0)
        {
            isJump = true;
            hit = Physics2D.Raycast(transform.position, Vector2.down, gDistance, groundMask | boxMask | hidenMask);
            if (hit)
            {
                anim.SetBool("isJump", false);
                anim.SetBool("isDouble", false);
                anim.SetBool("isWall", false);
                anim.SetBool("isWallClimb", false);
                playerRB.gravityScale = 5;
                ip = false;
                jumpCount = 0;
                isJump = false;
                foreach (var wall in walls)
                {
                    wall.GetComponent<WallChecker>().hitWall = false;
                }
            }
            else
            {
                playerRB.gravityScale = 1;
            }
        }
    }
    RaycastHit2D hitBox;
    RaycastHit2D hitWall;
    void BoxCheck()
    {
        hitBox = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, bDistance, boxMask);
        if (hitBox)
        {
            isBox = true;
        }
        else
        {
            isBox = false;
        }
    }
    void WallCheck()
    {
        hitWall = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, wDistance, wallMask);
        if (hitWall)
        {
            if (!hitWall.transform.GetComponent<WallChecker>().hitWall)
            {
                foreach (var wall in walls)
                {
                    wall.GetComponent<WallChecker>().hitWall = false;
                }
                isWall = true;
                hitWall.transform.GetComponent<WallChecker>().hitWall = true;
                StartCoroutine(WallHold());
            }
        }
    }
    IEnumerator WallHold()
    {
        jumpCount = 0;
        anim.SetBool("isWallLand", true);
        anim.CrossFade("Player_Wall_Land", 0f);
        float curTime = 0f;
        float holdTime = 0.5f;
        playerRB.constraints = RigidbodyConstraints2D.FreezeAll;
        while (curTime < holdTime)
        {
            curTime += Time.deltaTime;
            yield return new WaitForSeconds(0.001f);
        }
        anim.SetBool("isWallLand", false);
        anim.enabled = false;
        anim.enabled = true;
        isWall = false;
        playerRB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            isWall = false;
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Crouching Zone"))
        {
            conditon = eConditon.수그리기;
            isCrouching = true;
            isCrouchingZone = true;
            anim.SetBool("isCrouch", true);
            playerCC.offset = new Vector2(playerCC.offset.x, -0.2f);
            playerCC.size = new Vector2(playerCC.size.x, 0.5f);
            moveSpeed = crouchingSpeed;
        }
        if (other.CompareTag("DeadZone"))
        {
            transform.position = checkPoint;
        }
        if (other.CompareTag("CheckPoint"))
        {
            checkPoint = other.transform.position;
            conditon = eConditon.아이들;
            other.gameObject.SetActive(false);
        }
        if (other.CompareTag("KeyLock"))
        {
            GameManager.instance.jumpButton.SetActive(false);
            GameManager.instance.unLockButton.SetActive(true);
            canUnLock = true;
        }
        if (other.CompareTag("End"))
        {
            end.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Crouching Zone"))
        {
            conditon = eConditon.아이들;
            isCrouching = false;
            isCrouchingZone = false;
            playerCC.offset = new Vector2(playerCC.offset.x, -0.05f);
            playerCC.size = new Vector2(playerCC.size.x, 0.9f);
            anim.SetBool("isCrouch", false);
            moveSpeed = runSpeed;
        }
        if (other.CompareTag("KeyLock"))
        {
            GameManager.instance.jumpButton.SetActive(true);
            GameManager.instance.unLockButton.SetActive(false);
            canUnLock = false;
        }
    }
}