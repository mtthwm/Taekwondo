using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;


public class AI : MonoBehaviour
{   [SerializeField]
    private float maxSpeed = 100.0f;
    [SerializeField]
    private float thrust;
    [SerializeField]
    private float dodgeSpeed;
    [SerializeField]
    private GameObject groundCheckObj;
    [SerializeField]
    private GameObject ceilingCheckObj;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField] 
    private ActionManager actionManager;
    [SerializeField]
    private GameObject opponent;

    //Difficulty things
    [SerializeField]
    [Range(0.1f, 1.0f)]
    float difficultyLevel; 
    [SerializeField]
    float maxKickCooldownSeconds;
    private float kickCooldownSeconds;

    public bool player2;
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
    private String kickBtnStr;
    private bool isGround = false;

    //movement
    float speed = 0.0f;
    float inputX = 0.0f;
    [HideInInspector]
    public int direction;
    int prevDirection = 1;

    //stamina
    public float maxStamina;
    [HideInInspector]
    public float stamina;
    private float staminaPercent;

    //stats
    public float strength;
    public float weight;

    //timing
    private float NextDodge;
    [SerializeField]
    private float DodgeRate;
    private int frames;


    //timing for each kick type

    //normal kick
    private int normalStartDelay = 8;
    private int normalKickLength = 10;
    private int normalEndDelay = 5;

    //high kick
    private int highStartDelay = 8;
    private int highKickLength = 5;
    private int highEndDelay = 3;

    //spin kick
    private int spinStartDelay = 8;
    private int spinKickLength = 15;
    private int spinEndDelay = 3;

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

    //UI
    [SerializeField]
    private GameObject staminaUiObject;

    //whether moving or not
    bool moving;

    //Action Vars
    bool opponentJump;
    bool opponentCrouch;
    bool opponentBlock;
    bool opponentKick;
    bool opponentSpinKick;
    bool opponentDodge;
    bool damageTaken;


    //extra timing variables
    float timeToNextKick;
    float timeToUncrouch;
    float timeToCrouch;

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
    }

    void OnEnable () {
        kickCooldownSeconds = maxKickCooldownSeconds * (1f/difficultyLevel);
        ActionManager.JumpEvent += receiveJump;
        ActionManager.BlockEvent += receiveBlock;
        ActionManager.DodgeEvent += receiveDodge;
        ActionManager.CrouchEvent += receiveCrouch;
        ActionManager.UncrouchEvent += receiveUncrouch;
        ActionManager.KickEvent += receiveKick;
        ActionManager.SpinKickEvent += receiveSpinKick;

    }

    void OnDisable () {
        ActionManager.JumpEvent -= receiveJump;
        ActionManager.BlockEvent -= receiveBlock;
        ActionManager.DodgeEvent -= receiveDodge;
        ActionManager.CrouchEvent -= receiveCrouch;
        ActionManager.UncrouchEvent -= receiveUncrouch;
        ActionManager.KickEvent -= receiveKick;
        ActionManager.SpinKickEvent -= receiveSpinKick;
    }

    void receiveJump () {
        opponentJump = true;
    }

    void receiveDodge () {
        opponentDodge = true;
    }
    
    void receiveBlock () {
        opponentBlock = true;
    }

    void receiveKick () {
        opponentKick = true;
    }

    void receiveSpinKick () {
        opponentSpinKick = true;
    }

    void receiveCrouch () {
        opponentCrouch = true;
    }

    void receiveUncrouch () {
        opponentCrouch = false;
    }

    void resetActionVars () {
        moving = false;
        opponentBlock = false;
        opponentKick = false;
        opponentSpinKick = false;
        opponentDodge = false;
        opponentJump = false;
    }

    void resetActionVarsFixed () {

    }

    float opponentXDist() {
        return opponent.transform.position.x - transform.position.x;
    }

    float opponentYDist() {
        return opponent.transform.position.y - transform.position.y;
    }

    // Update is called once per frame
    void Update() {
        frames++;
        if (frames % 60 == 0 && stamina < maxStamina) {
            AdujstStamina(1);
        }

    }

    
    void normalKick () {
        StartCoroutine(Kick(normalKickManager, normalKickStaminaCost, normalStartDelay, normalKickLength, normalEndDelay));
    }
    void highKick () {
        Jump();
        StartCoroutine(Kick(highKickManager, highKickStaminaCost, highStartDelay, highKickLength, highEndDelay));
    }
    void spinKick () {
        StartCoroutine(Kick(spinKickManager, spinKickStaminaCost, spinStartDelay, spinKickLength, spinEndDelay));
    }
    void spinJumpKick () {
        Jump();
        StartCoroutine(Kick(spinKickManager, jumpSpinKickStaminaCost, spinStartDelay, spinKickLength, spinEndDelay));
    }

    void decreaseInputX () {
        if (inputX > 0.2f) {
            inputX -= 0.2f;
        }
    }

    bool randomBool(float probabilityOfTrue) {
        return (float)UnityEngine.Random.value < probabilityOfTrue;
    }

    bool randomBoolDifficultyAware () {
        return (float)UnityEngine.Random.value < difficultyLevel;
    }

    IEnumerator Block (int duration) {
        for (int i = 0; i < duration-1; i++) {
            isBlocking = true;
            canMove = false;
            yield return null;
        }
        isBlocking = false;
        canMove = true;
    }

    public void TakeDamage (int staminaAmount) {
        AdujstStamina(staminaAmount);
        damageTaken = true;
    }

    IEnumerator Delay (int duration) {
        yield return new WaitForSeconds(duration);
    }

    public void AdujstStamina (int amount) {
        stamina += amount;
        staminaUiObject.GetComponent<StaminaText>().SendMessage("SetStamina", stamina);
    }

    IEnumerator Kick (HitboxManager manager, int staminaCost, int start, int len, int end) {
        if (canKick && stamina > staminaCost && timeToNextKick < Time.time) {
            canKick = false;
            AdujstStamina(-staminaCost);
            for (int i = 0; i < start-1; i++) {
                yield return null;
            }
            manager.activate(start, len, end);
            for (int i = 0; i < len-1; i++) {
                yield return null;
            }
            for (int i = 0; i < end-1; i++) {
                yield return null;
            }
            timeToNextKick = Time.time + kickCooldownSeconds;
            canKick = true;
        }
    }

    void FixedUpdate()
    {
        // Debug.Log(rb2D.velocity + " VELOCITY");
        ceilingCheck = Physics2D.OverlapCircleAll(ceilingCheckObj.transform.position, 0.2f, groundLayer);
        ableToStand = (ceilingCheck.Length < 1);


        
        staminaPercent = ((float)stamina / (float)maxStamina) * 100.0f;

        if (opponent != null) {
            if (opponentXDist() > 3f && canMove) {
                Move("right");
            } else if (opponentXDist() < -3f && canMove) {
                Move("left");
            } else {
                //When within kicking range

                if (opponentXDist() < 0) {
                    direction = -1;
                } else {
                    direction = 1;
                }

                decreaseInputX();
                if (timeToNextKick < Time.time) { 
                    //Can Kick
                    if (staminaPercent > 50.0f) {
                        if (opponentJump) {
                            spinKick();
                            opponentJump = false;
                        } else {
                            if (randomBoolDifficultyAware()) {
                                spinKick();
                            } else {
                                if (opponent.GetComponent<ControlMovement>().StaminaPercent < 0.05) {
                                    spinKick();
                                } else {
                                    normalKick();
                                }
                            }
                        }
                    } else if (stamina > 25.0f) {
                        if (opponentJump) {
                            highKick();
                            opponentJump = false;
                        } else {
                            if (opponent.GetComponent<ControlMovement>().StaminaPercent < 0.05) {
                                highKick();
                            } else {
                                normalKick();
                            }
                        }
                    } else {
                        if (opponentJump) {
                            highKick();
                            opponentJump = false;
                        } else {
                            if (opponent.GetComponent<ControlMovement>().StaminaPercent < 0.05) {
                                highKick();
                            } else {
                                normalKick();
                            }
                        }
                    }
                } else {
                    //Can't Kick
                    if (staminaPercent > 50.0f) {
                        if (opponentJump) {
                            if (opponentKick || opponentSpinKick) {
                                if (timeToCrouch < Time.time && !crouching) {
                                    opponentJump = false;
                                    // Crouch(1f);
                                }
                            }
                        }
                    } 
                    // else if (stamina > 25.0f) {
                    //     if (opponentJump) {
                    //         if (timeToCrouch < Time.time) {
                    //             Crouch(2);
                    //         }
                    //     }
                    // } else {
                    //     if (opponentJump) {
                    //         if (timeToCrouch < Time.time) {
                    //             Crouch(2);
                    //         }
                    //     }             
                    // }
                }
            }
        }

        if (timeToUncrouch < Time.time && crouching) {
            Uncrouch();
        }

        if (canKick) {   
            speed = 2*(Math.Abs(inputX)/(Math.Abs(inputX)+1));
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
            speed *= 10f;
            Vector2 vectorSpeed = rb2D.velocity;
            vectorSpeed.x = speed;
            rb2D.velocity = vectorSpeed;   
        }
    }

    void Jump () {
        if (canMove) {
            isGrounded = Physics2D.OverlapCircleAll(groundCheckObj.transform.position, 0.2f, groundLayer);

            if (isGrounded.Length > 0) {   
                rb2D.AddForce(transform.up * (float)thrust * Time.deltaTime); 
            }
        }
        jump = false;
    }

    void Dodge () {
        if (canMove) {
            AdujstStamina(-5);
            NextDodge = Time.time + DodgeRate;
            rb2D.AddForce(transform.right * (float)dodgeSpeed * Time.deltaTime); 
        }
    }

    void Uncrouch () {
        BoxCollider2D topCollider = GetComponent<BoxCollider2D>();

        if (ableToStand) {
            topCollider.enabled = false;  
            crouching = true; 
        } else {
            topCollider.enabled = true;
            crouching = false;
        }

        timeToCrouch = Time.time + 8f;
    }

    void Crouch (float duration) {
        BoxCollider2D topCollider = GetComponent<BoxCollider2D>();

        topCollider.enabled = false; 
        crouching = true;
        timeToUncrouch = Time.time + duration;
    }

    void Move(String dir) {
        direction = dir == "right" ? 1 : -1;
        if (inputX < 1.0f) {
            inputX += 0.2f;
        }
    }
}