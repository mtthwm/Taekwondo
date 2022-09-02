using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowColliderHitboxes : MonoBehaviour
{
    public SpriteRenderer sprite;
    public BoxCollider2D collider;

    // Update is called once per frame
    void Update()
    {
        if (collider.enabled) {
            sprite.enabled = true;
        } else {
            sprite.enabled = false;
        }
    }
}
