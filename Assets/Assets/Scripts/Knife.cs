using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Knife : MonoBehaviour {

    public Rigidbody rb;
    public Transform menuTransform;

    //Force applied to swipe
    public float force = 5;

    //Mass
    public Text massText;

    //Gravity
    public Text gravityText;
    public Slider gravitySlider;

    //Torque
    public float torque = 20;
    public Text torqueText;
    public bool autoTorque = true;
    private float swipeTorque;

    //Wind
    public float wind = 0;
    private Vector2 windVector;
    public Text windText;

    //Time the knife started flying to prevent hitbox trigger while takeoff
    private float startTime;

    //Score handling
    private int currentScore = -1;
    public Text scoreText;

    //Resetproperties
    float resetStyle = 1;

    //Swipe Coordinates 
    private Vector2 startSwipe;
    private Vector2 endSwipe;

    //knife Position handling
    private Vector3 knifePos;
    private Quaternion knifeRot;

	// Use this for initialization
	void Start () {
        knifePos = rb.position;
        knifeRot = rb.rotation;
	}
	
	// Update is called once per frame
	void Update () {

        //Pause&Settings
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(menuTransform.gameObject.activeInHierarchy == false)
            {
                //Paused
                menuTransform.gameObject.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                //Unpaused
                menuTransform.gameObject.SetActive(false);
                Time.timeScale = 1;
            }
        }

        //Gameplay
        if (Time.timeScale == 1)
        {
            //Swiping
            if (Input.GetMouseButtonDown(0))
            {
                startSwipe = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            }
            if (Input.GetMouseButtonUp(0))
            {
                endSwipe = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                Swipe();
            }
        }
	}

    void Swipe()
    {
        rb.isKinematic = false;

        startTime = Time.time;

        Vector2 swipe = endSwipe - startSwipe;
        //Debug.Log(swipe);

        //Adds force of the players swipe
        rb.AddForce(swipe * force, ForceMode.Impulse);

        //Adds wind
        rb.AddForce(windVector, ForceMode.Impulse);

        //Adds torque
        if(autoTorque == true)
        {
            rb.AddTorque(0f, 0f, torque, ForceMode.Impulse);
        }else
        {
            swipeTorque = startSwipe.x - endSwipe.x;
            rb.AddTorque(0f, 0f, swipeTorque, ForceMode.Impulse);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Block")
        {
            rb.isKinematic = true;
            UpdateScore();
            knifePos = rb.transform.position;
            knifeRot = rb.transform.rotation;
        }
        else
        {
            Restart();
        }
    }

    void OnCollisionEnter()
    {
        float timeInAir = Time.time - startTime;

        if (!rb.isKinematic && timeInAir >= .05f)
        {
            //Debug.Log("Failed!");
            Restart();
        }
    }

    private void UpdateScore()
    {
        currentScore++;
        Debug.Log(currentScore);
        scoreText.text = "Score: " + currentScore;
    }

    public void Restart()
    {
        if(resetStyle == 1)
        {
            Debug.Log("Final Score: " + currentScore);
            currentScore = 0;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Time.timeScale = 1;
        }
        else
        {
            ResetPosition();
        }

    }

    public void ResetPosition()
    {
        rb.velocity = new Vector3(0, 0, 0);
        rb.transform.rotation = new Quaternion(0, -90, 0, 0);
        rb.transform.position = knifePos;
        rb.transform.rotation = knifeRot;
        currentScore--;
    }

    /// <summary>
    /// Handles the wind set by user - includes intensity factor
    /// </summary>
    /// <param name="newWind"></param>
    public void AdjustWind(float newWind)
    {
        float intensity = 0.3f;
        wind = newWind;
        windVector.x = wind * intensity;
        windText.text = "Wind: " + wind;
        Debug.Log("Wind changed to: " + wind);
    }

    /// <summary>
    /// Handles the Torque set on the slider which only takes action if on auto-torque
    /// </summary>
    /// <param name="newTorque"></param>
    public void AdjustTorque(float newTorque)
    {
        torque = newTorque;
        torqueText.text = "Drehmoment: " + torque;
        Debug.Log("Torque changed to: " + torque);
    }

    /// <summary>
    /// Toggles the auto-torque
    /// </summary>
    /// <param name="newAutoTorque"></param>
    public void ToggleAutoTorque(float newAutoTorque)
    {
        if(newAutoTorque == 0)
        {
            autoTorque = true;
            Debug.Log("Auto-Torque turned on!");
        }else
        {
            autoTorque = false;
            Debug.Log("Auto-Torque turned off!");
        }
    }

    /// <summary>
    /// Switches between reset options (soft=only knifepos, hard= reset all)
    /// </summary>
    /// <param name="newResetStyle"></param>
    public void ToggleResetStyle(float newResetStyle)
    {
        //0 == soft reset / 1 == hard reset
        resetStyle = newResetStyle;
        Debug.Log(resetStyle);
    }

    /// <summary>
    /// Adjusts the mass of the current object
    /// </summary>
    /// <param name="newMass"></param>
    public void AdjustMass(float newMass)
    {
        rb.mass = newMass;
        massText.text = "Masse: " + rb.mass;
        Debug.Log("Mass adjusted to: " + newMass);
    }

    /// <summary>
    /// Adjusts the gravity of the scene
    /// </summary>
    /// <param name="newGravity"></param>
    public void AdjustGravity(float newGravity)
    {
        Vector3 gravConverter = new Vector3(0, newGravity, 0);
        Physics.gravity = gravConverter;
        gravityText.text = "Gravitation: " + newGravity;
        Debug.Log("Gravity adjusted to: " + newGravity);
    }

    public void setGravityEarth()
    {
        Physics.gravity = new Vector3(0, -9.81f, 0);
        gravityText.text = "Gravitation: -9.81";
        gravitySlider.value = -9.81f;
    }

    public void setGravityDefault()
    {
        Physics.gravity = new Vector3(0, -3.5f, 0);
        GameObject slider = GameObject.Find("Slider-Gravity");
        gravityText.text = "Gravitation: -3.5";
        gravitySlider.value = -3.5f;
    }

    public void RestartButtonPressed()
    {
        resetStyle = 1;
        Restart();
    }
}
