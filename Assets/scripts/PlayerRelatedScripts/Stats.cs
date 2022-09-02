using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public float weight = 0.5f;
    public float strength = 0.5f;
    public float stamina = 100;
    private float cur_stamina;
    
    //UI
    public GameObject staminaUiObject;

    // Start is called before the first frame update
    void Start()
    {
        staminaUiObject.GetComponent<StaminaText>().SendMessage("SetStamina", stamina);  
        cur_stamina = stamina; 
    }

    public float GetStamina () {
        return stamina;
    }

    public float GetStrength () {
        return strength;
    }

    public float GetCurrentStamina () {
        return cur_stamina;
    }

    public float GetWeight () {
        return weight;
    }

    public void AdjustStamina (int amount) {
        Debug.Log("ADJUSTING STAMINA");
        cur_stamina += amount;
        staminaUiObject.GetComponent<StaminaText>().SendMessage("SetStamina", stamina);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
