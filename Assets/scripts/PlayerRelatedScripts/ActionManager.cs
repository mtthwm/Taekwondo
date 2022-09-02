using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ActionManager : MonoBehaviour
{
    
    public delegate void jumpEvent();
    public static event jumpEvent JumpEvent;
    public delegate void dodgeEvent();
    public static event dodgeEvent DodgeEvent;
    public delegate void kickEvent();
    public static event kickEvent KickEvent;
    public delegate void spinKickEvent();
    public static event spinKickEvent SpinKickEvent;
    public delegate void crouchEvent();
    public static event crouchEvent CrouchEvent;
    public delegate void uncrouchEvent();
    public static event uncrouchEvent UncrouchEvent;
    public delegate void blockEvent();
    public static event blockEvent BlockEvent;
    public delegate void walkEvent(float userx);
    public static event walkEvent WalkEvent;

    public void  jump () {
        JumpEvent();
    }

    public void  dodge () {
        if (DodgeEvent != null) {
            DodgeEvent();
        }
    }

    public void kick () {
        if (KickEvent != null) {
            KickEvent();
        }
    }

    public void spinKick () {
        if (SpinKickEvent != null) {
            SpinKickEvent();
        }
    }

    public void crouch () {
        if (CrouchEvent != null) {
            CrouchEvent();
        }
    }

    public void uncrouch () {
        if (UncrouchEvent != null) {
            UncrouchEvent();
        }
    }

    public void block () {
        if (BlockEvent != null) {
            BlockEvent();
        }
    }

    public void walk (float userx) {
        if (WalkEvent != null) {
            WalkEvent(userx);
        }
    }
}
