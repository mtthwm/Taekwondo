using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityManager : MonoBehaviour
{
    public int invincibilityFrames;
    public delegate void invincibilityEvent();
    public static event invincibilityEvent OnInvincibility;

    public void activateInvincibility () 
    {
        OnInvincibility();
    }
}
