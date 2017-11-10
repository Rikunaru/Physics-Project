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

    //Swipe Coordinates 
    private Vector2 startSwipe;
    private Vector2 endSwipe;


	// Use this for initialization
	void Start () {
        
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
            Debug.Log("Final Score: " + currentScore);
            currentScore = 0;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
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
    /// Adjusts the mass of the current object
    /// </summary>
    /// <param name="newMass"></param>
    public void AdjustMass(float newMass)
    {
        rb.mass = newMass;
        massText.text = "Masse: " + rb.mass;
        Debug.Log("Mass adjusted to: " + newMass);
    }
}
