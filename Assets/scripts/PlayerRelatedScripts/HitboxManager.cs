using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    // public hitboxScript;
    public BoxCollider2D[] colliders;

    private bool enabled;
    private int framesUntilStart;
    private int remainingFrames;
    private int[] lengths;
    private int currentColliderIndex;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (enabled) {
            if (framesUntilStart <= 0) {
                if (remainingFrames > 0) {
                    if (lengths[currentColliderIndex] > 0) {
                        colliders[currentColliderIndex].enabled = true;
                        lengths[currentColliderIndex] -= 1;
                    } else {
                        colliders[currentColliderIndex].enabled = false;
                        int maxIndex = colliders.Length-1;
                        if (currentColliderIndex < maxIndex) {
                            currentColliderIndex += 1;
                        } else {
                            enabled = false;
                        }
                    }

                    remainingFrames -= 1;
                }
            } else {
                framesUntilStart -= 1;
            }
        }
        
    }

    public void activate (int startDelay, int kickLength, int endDelay) {
        int colLength = colliders.Length;
        enabled = true;
        framesUntilStart = startDelay;
        remainingFrames = kickLength;
        int[] newLengthsArray = new int[colLength];
        int eachLength = kickLength / colLength;
        int remainder = kickLength % colLength;
        if (eachLength < 0) {
            Debug.LogError("Too many colliders for the kick duration!");
        }
        for (int i=0; i<colLength-1; i++) {
            newLengthsArray[i] = kickLength / colLength;
        }
        newLengthsArray[colLength-1] += remainder;
        lengths = newLengthsArray;
        currentColliderIndex = 0;
    }
}
