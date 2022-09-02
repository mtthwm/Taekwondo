using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadHurtbox : MonoBehaviour
{
    [SerializeField] 
    private BoxCollider2D PhysicsHeadCollider;
    [SerializeField]
    private BoxCollider2D HeadHurtboxCollider;

    // Update is called once per frame
    void FixedUpdate()
    {
        HeadHurtboxCollider.enabled = PhysicsHeadCollider.enabled;
    }   
}
