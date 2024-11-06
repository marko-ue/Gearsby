using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageHealthBar : MonoBehaviour
{
    const float MAX_HEALTH = 100f;
	Image healthBar;
	TextMeshProUGUI healthText;
	public float health = MAX_HEALTH;
    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<Image>();
        healthText = GameObject.Find("healthBarText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health > 50) healthBar.color = Color.green;
        else if (health > 30) healthBar.color = Color.yellow;
        else healthBar.color = Color.red;
        if (health < 0) health = 0;
        if (health > 100) health = MAX_HEALTH;

        healthBar.fillAmount = health / MAX_HEALTH;
        healthText.text = "" + Mathf.Floor(health);
    }

    public void SetHealth(int newHealth)
    {
        health = 1.0f * newHealth;
    }
}
