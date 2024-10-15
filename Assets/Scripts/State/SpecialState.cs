using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpecialState : State
{


    public Slider slider;
    public GameObject sliderCanvas;
    private float targetValue; // target speed 
    private bool slidingForward = true; // direction
    public float easeInSpeed = 1.0f;  // Speed during the ease-in phase
    public float constantSpeed = 2f;
    public float currentSpeed;
    public float easeInDuration = 2.0f; // How long to ease in before switching to constant speed
    private float easeInTimer;  // Tracks time during ease-in phase

    public SpecialState(CharacterMovement character) : base(character) { }

    public override void Enter()
    {
        Debug.Log("Entering Special State");
        Debug.Log("character" + character);
        this.sliderCanvas = character.gameObject.transform.Find("PowerBarCanvas").gameObject;
        this.slider = sliderCanvas.GetComponentInChildren<Slider>();
        sliderCanvas.SetActive(true);
        targetValue = slider.maxValue;
        slider.value = slider.minValue;
        currentSpeed = easeInSpeed;
        slidingForward = true;
        easeInTimer = 0f;
    }

    public override void Update()
    {
        if (!Input.GetMouseButton(0)) 
        {
            character.TransitionToState(character.moveState);
        }

        VisualizePower();
    }

    public void VisualizePower(){
        easeInTimer += Time.deltaTime;

        float t = Mathf.Clamp01(easeInTimer / easeInDuration); // normalized time 

        // ease from current value to target (max) value
        currentSpeed = Mathf.Lerp(easeInSpeed, constantSpeed, t);  // Interpolates the speed
        slider.value = Mathf.MoveTowards(slider.value, targetValue, currentSpeed * Time.deltaTime);


        if (slidingForward)
            targetValue = slider.maxValue;  // Slide to max
        else
            targetValue = slider.minValue;  // Slide to min
        

        // check if the slider has reached the target value with a small threshold
        if (Mathf.Abs(slider.value - targetValue) < 0.01f)
        {
            // once the target is reached, reverse the direction
            slidingForward = !slidingForward;
        }
        // Debug.Log("slider.value:" + slider.value);
    }
    
    public override void Exit()
    {
        // slider.value = 0;
        sliderCanvas.SetActive(false);
        Debug.Log("Exiting Special State");
    }
}
