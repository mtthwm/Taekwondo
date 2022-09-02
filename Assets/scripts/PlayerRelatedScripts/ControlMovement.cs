using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ControlMovement : MonoBehaviour
{
    public float maxSpeed = 100.0f;
    public float thrust;
    public float dodgeSpeed;
    public GameObject groundCheckObj;
    public GameObject ceilingCheckObj;
    public LayerMask groundLayer;
    public bool player2;
    [SerializeField] 
    private ActionManager actionManager;
    // [SerializeField]
    // private Timer timer;

    [SerializeField] Animator animator;

    private bool isBlocking = false;
    [HideInInspector]
    public bool IsBlocking {
        get {
            return isBlocking;
        }
    }

    private bool canMove = true;
    [HideInInspector]
    public bool CanMove {
        get {
            return canMove;
        }
        set {
            canMove = value;
        }
    }

    [HideInInspector]
    public Collider2D[] isGrounded;
    Collider2D[] ceilingCheck;
    bool ableToStand = true;
    bool jump;
    float momentum;
    float momentumStep;
    private Rigidbody2D rb2D;
    bool crouching;
    bool crouch = false;
    bool dodge = false;
    private String player2AxisModifier;
    private String kickBtnStr;
    private bool isGround = false;

    //movement
    float speed = 0.0f;
    float UserX = 0.0f;
    [HideInInspector]
    public int direction;
    int prevDirection = 1;

    //stamina
    public float maxStamina;
    [HideInInspector]
    public float stamina;
    private float staminaPercent;
    [HideInInspector]
    public float StaminaPercent {
        get {
            return staminaPercent;
        }
    }

    //stats
    public float strength;
    public float weight;

    //timing
    private float NextDodge;
    public float DodgeRate;
    private int frames;
        //timing for each kick type

        //normal kick
        private int normalStartDelay = 20;
        private int normalKickLength = 20;
        private int normalEndDelay = 5;

        //high kick
        private int highStartDelay = 20;
        private int highKickLength = 20;
        private int highEndDelay = 7;

        //spin kick
        private int spinStartDelay = 20;
        private int spinKickLength = 37;
        private int spinEndDelay = 7;

    private int startDelay;
    private int kickLength; 
    private int endDelay;

    //kicking
    private HitboxManager spinKickManager;
    private HitboxManager normalKickManager;
    private HitboxManager highKickManager;
    //kick stamina costs
    private int normalKickStaminaCost = 3;
    private int highKickStaminaCost = 5;
    private int spinKickStaminaCost = 8;
    private int jumpSpinKickStaminaCost = 10;

    private bool canKick = true;
    private int kickBtnHoldLength;
    private int framesToSpinKick = 10;
    private AudioManager audioManager;

    //UI
    public GameObject staminaUiObject;

    //Camera
    private CameraShake cameraShake;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        spinKickManager = transform.Find("spinKickHitbox").gameObject.GetComponent<HitboxManager>();
        normalKickManager = transform.Find("normalKickHitbox").gameObject.GetComponent<HitboxManager>();
        highKickManager = transform.Find("highKickHitbox").gameObject.GetComponent<HitboxManager>();
        momentum = 0f;
        crouching = false;
        stamina = maxStamina;
        staminaUiObject.GetComponent<StaminaText>().SendMessage("SetStamina", stamina);
        frames = 0;
        player2AxisModifier = player2 ? " 2" : "";
        kickBtnStr = player2 ? "m" : "c";
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void OnEnable () {
        canMove = false;
        cameraShake = GameObject.Find("Match Camera").GetComponent<CameraShake>();
        Timer.TimeStart += TimerStart;
        Timer.TimeStop += TimerStop;
    }

    void OnDisable () {
        Timer.TimeStart -= TimerStart;
        Timer.TimeStop -= TimerStop;
    }

    void TimerStop () {
        canMove = false;
    }

    void TimerStart () {
        canMove = true;
    }

    // Update is called once per frame
    void Update() {
        frames++;
        if (frames % 60 == 0 && stamina < maxStamina) {
            AdujstStamina(1);
        }

        staminaPercent = ((float)stamina / (float)maxStamina) * 100.0f;

        //dodge
        dodge = (Input.GetAxis("Dodge" + player2AxisModifier) > 0) && (NextDodge != null ? (Time.time > NextDodge) : true) && (stamina >= 5) && !crouching && canMove;

        jump = canMove ? Input.GetAxis("Jump" + player2AxisModifier) > 0 : false;
        UserX = canMove && !crouching ? Input.GetAxis("Horizontal" + player2AxisModifier) : 0;
        if (actionManager != null) {
            actionManager.walk(UserX);
        }
        if (animator != null) {
            animator.SetFloat("UserX", Mathf.Abs(UserX));
        }
        if (canMove) {
            crouch = canMove && Input.GetAxis("Vertical" + player2AxisModifier) < 0;
        }

        //blocking
        if (canMove && !isBlocking) {
            if (Input.GetAxis("Block"+player2AxisModifier) > 0) {
                StartCoroutine(Block(30));
            }
        }
        
        //crouching
        ceilingCheck = Physics2D.OverlapCircleAll(ceilingCheckObj.transform.position, 0.2f, groundLayer);
        ableToStand = (ceilingCheck.Length < 1);

        if (crouch) {
            if (actionManager != null) {
                actionManager.crouch();
            }
            if (animator != null) {
                animator.SetBool("isCrouching", true);
            }
            crouching = true;
        } else {
            if (ableToStand) {
                if (actionManager != null) {
                    actionManager.uncrouch();
                }   
                if (animator != null) {
                    animator.SetBool("isCrouching", false);
                }
                crouching = false;
            } else {
                crouching = true;
                if (actionManager != null) {
                    actionManager.crouch();
                }  
                if (animator != null) {
                    animator.SetBool("isCrouching", true);
                }
            }
        }

        if (crouching) {
            GetComponent<BoxCollider2D>().enabled = false;
        } else {
            GetComponent<BoxCollider2D>().enabled = true;
        }

        //kicking
        if (Input.GetKeyUp(kickBtnStr) && canKick && !crouching && canMove) {
            if (kickBtnHoldLength <= framesToSpinKick && (isGrounded.Length > 0) && stamina >= normalKickStaminaCost) {
                if (animator != null) {
                    animator.SetBool("isNormalKicking", true);
                }
                Kick(normalKickManager, normalKickStaminaCost, normalStartDelay, normalKickLength, normalEndDelay);
            } else if (kickBtnHoldLength <= framesToSpinKick && !(isGrounded.Length > 0) && stamina >= highKickStaminaCost) {
                if (animator != null) {
                    animator.SetBool("isHighKicking", true);
                }
                Kick(highKickManager, highKickStaminaCost, highStartDelay, highKickLength, highEndDelay);
            } else if (kickBtnHoldLength > framesToSpinKick && (isGrounded.Length > 0) && stamina >= spinKickStaminaCost) {
                Kick(spinKickManager, spinKickStaminaCost, spinStartDelay, spinKickLength, spinEndDelay);
                if (animator != null) {
                    animator.SetBool("isSpinKicking", true);
                }
            } else if (kickBtnHoldLength > framesToSpinKick && !(isGrounded.Length > 0) && stamina >= jumpSpinKickStaminaCost) {
                if (animator != null) {
                    animator.SetBool("isSpinKicking", true);
                }   
                Kick(spinKickManager, jumpSpinKickStaminaCost, spinStartDelay, spinKickLength, spinEndDelay);
            }
        }

        if (Input.GetAxis("Kick" + player2AxisModifier) != 0) {
            kickBtnHoldLength += 1;
        } else if (kickBtnHoldLength > 0 ) {
            kickBtnHoldLength = 0;
        }

        manageKickTiming();
    }

    void manageKickTiming () {
        if (startDelay > 0) {
            startDelay -= 1;
        } else {
            if (kickLength > 0) {
                kickLength -= 1;
            } else {
                if (endDelay > 0) {
                    endDelay -= 1;
                } else {
                    canKick = true;
                    if (animator != null) {
                        animator.SetBool("isNormalKicking", false);
                        animator.SetBool("isHighKicking", false);
                        animator.SetBool("isSpinKicking", false);
                    }
                }
            }
        }
    }

    IEnumerator Block (int duration) {
        if (actionManager != null) {
            actionManager.block();
        }
        for (int i = 0; i < duration-1; i++) {
            isBlocking = true;
            canMove = false;
            yield return null;
        }
        isBlocking = false;
        canMove = true;
    }

    public void AdujstStamina (int amount) {
        stamina += amount;
        staminaUiObject.GetComponent<StaminaText>().SendMessage("SetStamina", stamina);
    }

    void Kick (HitboxManager manager, int staminaCost, int start, int len, int end, bool isSpin = false) {
        rb2D.velocity = new Vector2(0f, rb2D.velocity.y);
        if (audioManager != null) {
            audioManager.PlayRandomFromSoundGroup("Woosh");
        }
        if (actionManager != null) {
            if (isSpin) { 
                actionManager.spinKick();
            } else {
                actionManager.kick();
            }
        }
        canKick = false;
        AdujstStamina(-staminaCost);
        kickBtnHoldLength = 0;
        startDelay = start;
        kickLength = len;
        endDelay = end;
        manager.activate(start, len, end);
    }

    private bool airborne = false;

    void FixedUpdate()
    {
        //jump
        isGrounded = Physics2D.OverlapCircleAll(groundCheckObj.transform.position, 0.2f, groundLayer);

        if (isGrounded.Length > 0) {
            if (animator != null) {
                animator.SetBool("isJumping", false);
            }
        }

        if (jump && (isGrounded.Length > 0)) {   
            if (actionManager != null) {
                actionManager.jump();
            }
            if (animator != null) {
                animator.SetBool("isJumping", true);
            }
            if (audioManager != null) {
                audioManager.Play("Jump");
            }
            rb2D.AddForce(transform.up * (float)thrust * Time.deltaTime); 
        }

        //movement
        if (UserX != 0) {
            direction = UserX > 0 ? 1 : -1;
        }
        if (canKick)
        {
            speed = 2*(Math.Abs(UserX)/(Math.Abs(UserX)+1));
            speed *= maxSpeed;
            speed *= Time.deltaTime;
            if (isGrounded.Length < 1) {
                speed *= 0.1f;
            }

            if (direction > 0) {
                transform.eulerAngles = new Vector3(0, 0, 0);
            } else {
                transform.eulerAngles = new Vector3(0, 180, 0);
            }

            prevDirection = direction;
            speed *= direction;
            Vector2 vectorSpeed = rb2D.velocity;
            vectorSpeed.x = speed;
            rb2D.velocity = vectorSpeed;   
        }


        //dodge

        if (dodge) {
            // Debug.Log("DODGE");
            dodge = false;
            if (actionManager != null) {
                actionManager.dodge();
            }
            AdujstStamina(-5);
            NextDodge = Time.time + DodgeRate;
            Vector2 dodgeForce = transform.right * (float)dodgeSpeed * Time.deltaTime;
            // Debug.Log("DODGEFORCE: " + dodgeForce + " VELOCITY: " + rb2D.velocity);
            rb2D.AddForce(dodgeForce); 
        }

        CheckCameraShake();
    }

    void CheckCameraShake () {
        if (rb2D.velocity != null && rb2D.velocity.x > 5.0f) {
            Debug.Log(cameraShake);
            if (cameraShake != null) {
                cameraShake.shake(90, (float) Math.PI/4f, 1);
            }
        }
    }
}
