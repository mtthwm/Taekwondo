using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class TakeDamage : MonoBehaviour
{
    public int baseDamageTaken;
    [SerializeField] private int scoreValue;
    [SerializeField] private InvincibilityManager invincibilityManager;
    [SerializeField] private Timer timer;
    
    private Rigidbody2D rb2d;
    private AudioManager audioManager;
    private ScoreBoard scoreBoard;
    private ControlMovement selfMovementScript;
    private AI aiMovementScript;
    private float weight;
    private float stamina;
    private float staminaNorm; 
    private int invincibilityFrames;
    private int remainingInvincibilityFrames = 0;   
    public CameraShake cameraShake;

    void Start()
    {
        
        rb2d = gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>();
        selfMovementScript = gameObject.transform.parent.gameObject.GetComponent<ControlMovement>();
        aiMovementScript = gameObject.transform.parent.gameObject.GetComponent<AI>();
        stamina = selfMovementScript != null ? selfMovementScript.stamina : aiMovementScript.stamina;
        invincibilityFrames = invincibilityManager.invincibilityFrames;
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
    }

    void Update () {
        if (remainingInvincibilityFrames > 0) {
            remainingInvincibilityFrames -= 1;
        }
    }

    void OnEnable () {
        if (timer == null) {
            timer = GameObject.Find("TimeManager").GetComponent<Timer>();
        }
        InvincibilityManager.OnInvincibility += invincible;
    }

    void Disable () {
        InvincibilityManager.OnInvincibility -= invincible;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        DoDamage opponent;
        ControlMovement collisionMovementScript;
        AI collisionAiScript;
        float j; 
        float s;
        float x; //opponent strength
        float w = weight;
        float damageMult;
        bool isGrounded;

        if (other.gameObject.layer == 10 && other.transform.root != transform.root) {
            opponent = other.gameObject.GetComponent<DoDamage>();
            if (opponent != null && remainingInvincibilityFrames == 0) {
                scoreBoard = other.transform.parent.gameObject.GetComponent<ScoreBoard>();
                collisionMovementScript = other.gameObject.transform.parent.gameObject.GetComponent<ControlMovement>();
                collisionAiScript = other.gameObject.transform.parent.gameObject.GetComponent<AI>();
                isGrounded = selfMovementScript != null ? (selfMovementScript.isGrounded.Length > 0) : (aiMovementScript.isGrounded.Length > 0);
                stamina = selfMovementScript != null ? selfMovementScript.stamina : aiMovementScript.stamina;
                // j = 2 if jumping, s = 2 if spinning
                j = opponent.isJumping;
                s = opponent.isSpinning;
                x = opponent.strength;

                staminaNorm = stamina/100;
                damageMult = (((j*s)/5f)*((2f*x)/(w+1.2f))) + 1;
                int finalDamage = (int)((float)baseDamageTaken * damageMult); 
                bool blocking = selfMovementScript ? selfMovementScript.IsBlocking : aiMovementScript.IsBlocking;    
                if (blocking) {
                    finalDamage = (int)(finalDamage / 1.5f);
                }
                Vector2 forceToAdd;     
                if (!isGrounded) {
                    forceToAdd = new Vector2(((collisionMovementScript != null ? collisionMovementScript.direction : collisionAiScript.direction) * 100000f * s * 3f), 20000f * (j > 1 ? 0.3f : 0.2f));
                } else {
                    forceToAdd = new Vector2(((collisionMovementScript != null ? collisionMovementScript.direction : collisionAiScript.direction) * 100000f * s), 20000f * (j > 1 ? 1.15f : 0.5f));
                }
                if (!blocking){
                    rb2d.AddForce(forceToAdd * Time.deltaTime, ForceMode2D.Force);
                }
                audioManager.PlayRandomFromSoundGroup("Oof");
                invincibilityManager.activateInvincibility();
                if (damageMult-1 >= 2*staminaNorm+0.2f) {
                    StartCoroutine(timeSlow(120));
                    cameraShake.shake(90, (float) Math.PI/4f, 1);
                } else {
                    if (selfMovementScript != null) {
                        selfMovementScript.SendMessage("AdujstStamina", stamina > finalDamage ? -finalDamage : -stamina);
                    } else {
                        aiMovementScript.SendMessage("TakeDamage", stamina > finalDamage ? -finalDamage : -stamina);
                    }
                    scoreBoard.AdjustScore(scoreValue);
                }
            }
        }
    }

    IEnumerator knockout () {
        if (selfMovementScript != null) {
            if (selfMovementScript.player2) {
                PlayerPrefs.SetInt("playerTwoTempScore", scoreBoard.Score);
            } else {
                PlayerPrefs.SetInt("playerOneTempScore", scoreBoard.Score);
            }
            selfMovementScript.CanMove = false;
        } else {
            if (aiMovementScript.player2) {
                PlayerPrefs.SetInt("playerTwoTempScore", scoreBoard.Score);
            } else {
                PlayerPrefs.SetInt("playerOneTempScore", scoreBoard.Score);
            }
            aiMovementScript.CanMove = false;
        }
        // Debug.Log("SOMETHING");
        if (timer) {
            timer.SendMessage("StopTimer");
        }
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("Results");
    }

    IEnumerator timeSlow (int duration) {
        for (int i = 0; i < duration; i++) {
            if (i == duration-1) {
                Time.timeScale = 1f;
            } else {
                Time.timeScale = 0.3f;
            }
            yield return null;
        }
        StartCoroutine(knockout());
    }

    void invincible () {
        remainingInvincibilityFrames = invincibilityFrames;
    }
}
