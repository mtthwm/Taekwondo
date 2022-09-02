using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaText : MonoBehaviour
{
    public void SetStamina (int amount) {
        Text textObj = GetComponent<Text>();
        textObj.text = amount.ToString();
    }
}
