using System.Collections;
using Assets.Scripts.Characters;
using UnityEngine;
using UnityEngine.UI;
// using Assets.Scripts.Character.Customer;

public class SpecialState : State
{


    private Slider slider;
    private GameObject sliderCanvas;
    private float targetValue; // target speed 
    private bool slidingForward = true; // direction
    private float easeInSpeed = 1.0f;  // Speed during the ease-in phase
    private float constantSpeed = 2f;
    private float currentSpeed;
    private float easeInDuration = 2.0f; // How long to ease in before switching to constant speed
    private float easeInTimer;  // Tracks time during ease-in phase

    public SpecialState(CharacterMovement character) : base(character) { }
    public Camera mainCamera;  // Reference to the main camera
  
    private Quaternion originalCanvasRotation; //canvas's rotation

    private bool grappling; // Indicates whether the player is currently grappling
    private float maxDistance = 25f; // Max distance for the grappling hook
    private RaycastHit hit;
    private Transform grappleTarget;  // Object we are grappling towards
    private LineRenderer lineRenderer;  // Reference to the LineRenderer

    public override void Enter()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        // Debug.Log("Entering Special State");
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
        originalCanvasRotation = mainCamera.transform.rotation;

        // Reference the LineRenderer from the character or instantiate one
        lineRenderer = character.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("No LineRenderer attached to the character!");
        }

        // Initially disable the LineRenderer
        lineRenderer.enabled = false;
        lineRenderer.positionCount = 2;  // We need two points: start (character) and end (target)

    }

    public override void Update()
    {
        sliderCanvas.transform.rotation = originalCanvasRotation; // does not rotate canvas
        Vector3 forward = character.transform.TransformDirection(Vector3.forward) * 100;
        Debug.DrawRay(character.transform.position, forward, Color.green);

        if (grappling){return;}
        if (!Input.GetMouseButton(0)) 
        {
            FireGrapplingHook();  // Fire grappling hook when mouse button is released
        }

        VisualizePower();
        CalculatePlayerFacing();
    }

    private void CalculatePlayerFacing()
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


    private void VisualizePower(){
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

    private void FireGrapplingHook()
    {
        // Freeze character while firing the grappling hook
        // character.GetComponent<Rigidbody>().isKinematic = true;
        Vector3 pos = character.transform.position;
        pos.y += 1;

        Debug.Log("FireGrapplingHook");
        // Fire a ray from the player's position in the forward direction
        if (Physics.Raycast(pos, character.transform.forward, out hit, maxDistance)  
            && hit.collider.CompareTag("Customer"))
        {
                GameObject hitObject = hit.collider.gameObject;
                grappleTarget = hitObject.transform;
                character.StartCoroutine(HandleHit(hitObject));
        }
        else
        {
            character.StartCoroutine(HandleHit(null));
            Debug.Log("Grappling hook missed.");
        }
    }

    private IEnumerator HandleHit(GameObject hitObject)
    {
        // if hit, check slider.value to determine behavior
            // kill - animate to target + kill signal
            // grapple - animate to target + move target
            // nudge - animate to target + move target
        // else. just animate from max distance

        grappling = true;  // Prevent further input while grappling
        lineRenderer.enabled = true;  // Enable the LineRenderer to start visualizing the grapple


        if (hitObject != null) // if hit
        {
            Customer c = hitObject.GetComponent<Customer>();
            c.DisableCustomer();
            Rigidbody rb = hitObject.GetComponent<Rigidbody>();
            if ( slider.value > 0.5){
                Debug.Log(slider.value + "Killed");
                // Kill code comes here 
                c.Die();
                yield return character.StartCoroutine(AnimateTentacle(hitObject.transform.position));
            }else if (slider.value <= 0.5 && slider.value > 0.3){
                Debug.Log(slider.value + "Grappled");
                while (Vector3.Distance(hitObject.transform.position, character.transform.position) > 1f)
                {
                    // Move the object toward the player over time
                    Vector3 direction = (character.transform.position - hitObject.transform.position).normalized;
                    rb.MovePosition(hitObject.transform.position + direction * Time.deltaTime * 20f);  // Adjust speed as needed
                    if (lineRenderer.enabled && grappleTarget != null)
                    {
                        Vector3 pos = character.transform.position;
                        pos.y += 1;
                        Vector3 grapplepos = grappleTarget.position;
                        grapplepos.y += 1;
                        lineRenderer.SetPosition(0, pos);  // Start point at the player
                        lineRenderer.SetPosition(1, grapplepos);        // End point at the target object
                    }
                    yield return null;  // Wait for the next frame
                }
                c.EnableCustomer();
            }else{
                Debug.Log(slider.value + "Vaguely Nudged");
                // Nudge code comes here 
                Vector3 nudgeDirection = (hitObject.transform.position - character.transform.position).normalized;
                rb.AddForce(nudgeDirection * 5f, ForceMode.Impulse);  // Adjust force as needed
                c.EnableCustomer();
                yield return character.StartCoroutine(AnimateTentacle(hitObject.transform.position));
            }
        }else{
            // nothing!
            Vector3 target = Vector3.MoveTowards(character.transform.position,
                            character.transform.TransformDirection(Vector3.forward)*100,maxDistance);
            yield return character.StartCoroutine(AnimateTentacle(target));
        }

        Debug.Log("Special complete.");
        lineRenderer.enabled = false;
        grappling = false;  // Reset grappling state
        character.TransitionToState(new MoveState(character));
    }

    private IEnumerator AnimateTentacle(Vector3 target){
        while(Vector3.Distance(target, character.transform.position) > 1f)
        {
            Vector3 direction = (character.transform.position - target).normalized;
            target = target + direction * Time.deltaTime * 40f;
            if (lineRenderer.enabled){
                Vector3 pos = character.transform.position;
                pos.y += 1;
                Vector3 targetPos = target;
                targetPos.y += 1;
                lineRenderer.SetPosition(0, pos);  // Start point at the player
                lineRenderer.SetPosition(1, targetPos);        // End point at the target object
            }
            yield return null;  // Wait for the next frame
        }
    }

    public override void Exit()
    {
        sliderCanvas.SetActive(false);
        // Debug.Log("Exiting Special State");
    }
}
