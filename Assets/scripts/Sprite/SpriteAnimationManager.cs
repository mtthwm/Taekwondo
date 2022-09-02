using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void OnEnable () {
        ActionManager.JumpEvent += receiveJump;
        ActionManager.WalkEvent += receiveWalk;
        // ActionManager.BlockEvent += receiveBlock;
        // ActionManager.DodgeEvent += receiveDodge;
        // ActionManager.CrouchEvent += receiveCrouch;
        // ActionManager.UncrouchEvent += receiveUncrouch;
        // ActionManager.KickEvent += receiveKick;
        // ActionManager.SpinKickEvent += receiveSpinKick;

    }

    void OnDisable () {
        ActionManager.JumpEvent -= receiveJump;
        ActionManager.WalkEvent += receiveWalk;
        // ActionManager.BlockEvent -= receiveBlock;
        // ActionManager.DodgeEvent -= receiveDodge;
        // ActionManager.CrouchEvent -= receiveCrouch;
        // ActionManager.UncrouchEvent -= receiveUncrouch;
        // ActionManager.KickEvent -= receiveKick;
        // ActionManager.SpinKickEvent -= receiveSpinKick;
    }

    void receiveJump () {
        animator.SetTrigger("RecieveJump");
    }

    void receiveWalk (float userx) {
        Debug.Log(Mathf.Abs(userx) + " " + gameObject.transform.parent.name);
        animator.SetFloat("UserX", Mathf.Abs(userx));
    }

    // void receiveDodge () {
    //     opponentDodge = true;
    // }
    
    // void receiveBlock () {
    //     opponentBlock = true;
    // }

    // void receiveKick () {
    //     opponentKick = true;
    // }

    // void receiveSpinKick () {
    //     opponentSpinKick = true;
    // }

    // void receiveCrouch () {
    //     opponentCrouch = true;
    // }

    // void receiveUncrouch () {
    //     opponentCrouch = false;
    // }
}
