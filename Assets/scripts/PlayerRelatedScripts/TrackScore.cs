using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackScore : MonoBehaviour
{
    [SerializeField] private int pointValue;
    [SerializeField] private ScoreBoard scoreBoard;
    private DoDamage opponent;

    public void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.layer == 13) {
            opponent = other.gameObject.GetComponent<DoDamage>();

        }
    }
}
