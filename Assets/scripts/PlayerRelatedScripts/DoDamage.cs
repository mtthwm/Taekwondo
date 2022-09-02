using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    [HideInInspector]
    public float strength;
    
    public float isJumping;
    public float isSpinning;
    
    
    public int ScoreValue;

    private ControlMovement controlMovementScript;
    private AI aiMovementScript;

    void Update () {
        controlMovementScript = transform.parent.GetComponent<ControlMovement>();
        aiMovementScript = transform.parent.GetComponent<AI>();
        strength = controlMovementScript != null ? controlMovementScript.strength : aiMovementScript.strength;
    }
}
