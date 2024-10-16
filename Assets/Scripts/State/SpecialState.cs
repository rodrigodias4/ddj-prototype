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
    public Camera mainCamera;  // Reference to the main camera
  
    private Quaternion originalCanvasRotation; //canvas's rotation

    public override void Enter()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
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
        // characterBody = character.transform;
        originalCanvasRotation = sliderCanvas.transform.rotation;
    }

    public override void Update()
    {
        if (!Input.GetMouseButton(0)) 
        {
            character.TransitionToState(character.moveState);
        }

        VisualizePower();
        CalculatePlayerFacing();
    }

    public void CalculatePlayerFacing()
    {
        // mouse position in the world space
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // horizontal plane at character's height
        Plane groundPlane = new Plane(Vector3.up, character.transform.position);

        float rayDistance;

        // https://docs.unity3d.com/ScriptReference/Plane.Raycast.html
        if (groundPlane.Raycast(ray, out rayDistance)) // dist from starting ray to plane
        {
            // ray plane intersection
            Vector3 targetPosition = ray.GetPoint(rayDistance);
            Vector3 direction = (targetPosition - character.transform.position).normalized;
            character.transform.rotation = Quaternion.LookRotation(direction); 
            sliderCanvas.transform.rotation = originalCanvasRotation; // does not rotate canvas
        }
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
