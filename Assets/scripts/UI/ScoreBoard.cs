using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour
{
    private int score;
    public int Score {
        get {
            return score;
        }
    }
    public Text ScoreUiObject;

    void Start () {
        score = 0;
        AdjustScore(0);
    }

    public void AdjustScore (int amount) {
        score += amount;
        if (ScoreUiObject != null) {
            ScoreUiObject.text = score.ToString();
        }
    }
}
