using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ManagePlayerHealth : MonoBehaviour
{
    public static ManagePlayerHealth instance;

    public float alpha;
    public bool screenFlashBool;
    public int health = 100;
    int healthIncrement = 10;
    GameObject healthBar;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        alpha = 0;
        GameObject.Find("screenFlash").GetComponent<Image>().color = new Color(1, 0, 0, alpha);
        screenFlashBool = false;
        healthBar = GameObject.Find("greenHealthBar");
    }

    // Update is called once per frame
    void Update()
    {
        if (screenFlashBool)
        {
            alpha -= Time.deltaTime;
            GameObject.Find("screenFlash").GetComponent<Image>().color = new Color(1, 0, 0, alpha);
            if (alpha <= 0)
            {
                screenFlashBool = false;
                alpha = 0;
            }
        }

    }

    public void ResetHealth()
    {
        health = 100;
        healthBar.GetComponent<ManageHealthBar>().SetHealth(health);
    }

    public void AddHealth(int value, float delay)
    {
        StartCoroutine(AddHealthWithDelay(value, delay));
    }

    private IEnumerator AddHealthWithDelay(int value, float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay time
        health += value;
        healthBar.GetComponent<ManageHealthBar>().SetHealth(health);
        print("Current Health: " + health);
    }

    public void DecreaseHealth()
    {
        screenFlash();
        health -= healthIncrement;

        // Updating Health Bar
        healthBar.GetComponent<ManageHealthBar>().SetHealth(health);

        // Checking health to restart level
        if (health < 0) SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        // counting nbMLives
        if (health < 0)
        {
            // print("NB Lives:" + PlayerPrefs.GetInt("nbLives"));
            if (PlayerPrefs.GetInt("nbLives") > 0)
            {
                PlayerPrefs.SetInt("nbLives", PlayerPrefs.GetInt("nbLives") - 1);
                // GameObject.Find("nbLives").GetComponent<TextMeshProUGUI>().text = "" + PlayerPrefs.GetInt("nbLives");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            }
            else SceneManager.LoadScene("splashScreen");

        }
    }

    void screenFlash()
    {
        screenFlashBool = true;
        alpha = 1.0f;
        print("Screen Flash");
    }
}
