using System;
using Unity.VisualScripting;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class SuspensionCarController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody carRb;
    [SerializeField] Transform[] rayPoints;
    [SerializeField] LayerMask drivable;
    [SerializeField] Transform accelerationPoint;
    [SerializeField] Transform backJumpForcePoint;
    [SerializeField] Transform frontJumpForcePoint;
    [SerializeField] GameObject[] tires = new GameObject[4];
    [SerializeField] GameObject[] frontTireParents = new GameObject[2];
    [SerializeField] CarVFXManager carVFXManager;
    

    [Header("Suspension Settings")]
    [SerializeField] float springStiffness;
    [Range(1100, 8000)]
    [SerializeField] float damperStiffness;
    [SerializeField] float restLenght;
    [SerializeField] float springTravel;
    [SerializeField] float wheelRadius;

    [Header("Input")]
    float moveInput;
    float steerInput;
    public bool handBrake;
    bool boost;
    [SerializeField] float keyboardInputSmoothing = 5f; 
    

    [Header("Car Settings")]
    
    [SerializeField] float acceleration = 100f;
    [SerializeField] float deAcceleration = 25f;
    [SerializeField] float maxSpeed = 10f;
    [SerializeField] float turnStrength = 15f;
    [SerializeField] AnimationCurve turningCurve;
    [SerializeField] AnimationCurve handBreakGripLossCurve;
    [Range (0,5)]
    [SerializeField] float turningTorque;
    [SerializeField] float handBreakTimeToStop = 1f;
    [SerializeField] float jumpColdDownTime = 3f;
    [SerializeField] float TurnInAirForce = 1000f;
    [SerializeField] float jumpStrenght = 1000f;
    [Range(0, 90)]
    [SerializeField] float frontJumpAngle = 45f;


    [Header("DragConstants")]
    [SerializeField] float dragCofficientForTurning = 1f;
    [SerializeField] float baseDragGrounded = 2f;
    [SerializeField] float baseDragNotGrounded = 2f;

    [Header("Visuals")]
    [SerializeField] float tireRotSpeed = 3000f;
    [SerializeField] float maxTurningAngle = 30f; 
    
    #region Parameters
    Vector3 currentVelocity = Vector3.zero;
    float carVelocityRatio;
    int [] wheelIsGrounded = new int[4];
    [HideInInspector]public bool isGrounded;
    [HideInInspector]public bool isBoosting;
    float originalTurnStrength;
    float originalAcceleration;
    float timeOfHandBreak;
    float jumpTimer = 3;
    public event Action OnBoost;
    private static Controls inputActions;
    private float accelerateInput;
    private enum ControlTypes
    {
        Keyboard,
        Controller,
        Screen
    };
    private ControlTypes controlType = ControlTypes.Keyboard;   
    private Vector2 smoothInput;
    #endregion
    #region Delegates
    public delegate void Accelerate();
    Accelerate accelerateDelegate;
    public delegate void DeAccelerate();
    Accelerate deAccelerateDelegate;
    #endregion
    void Awake()
    {
        if (inputActions == null)
        {
            InitializeInputActions();
        }
        accelerateDelegate = AccelerateKeyboard;
        deAccelerateDelegate = DeAccelerateKeyboard;
    }
    void Start()
    {
        carRb = GetComponent<Rigidbody>();
        originalTurnStrength = turnStrength;
        originalAcceleration = acceleration;
    }
    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Jump.performed+= PerformJump;
        inputActions.Player.Movement.performed+= GetControlType;
        inputActions.Player.Accelerate.performed+= GetControlType;
        inputActions.Player.Break.performed+= GetControlType;
        
    }
    void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.Jump.performed-= PerformJump;
        inputActions.Player.Movement.performed-= GetControlType;
        inputActions.Player.Accelerate.performed-= GetControlType;
        inputActions.Player.Break.performed-= GetControlType;
    }

    void Update()
    {
        GetInput();
        
    }

  
    void FixedUpdate()
    {
        Suspension();
        GroudCheck();
        CalculateVelocity();
        Movement();
        Visuals();
    }

    void GroudCheck()
    {
        int groundedWheels = 0;
        for (int i = 0; i < wheelIsGrounded.Length; i++)
        {
            groundedWheels += wheelIsGrounded[i];
        }

        if (groundedWheels > 0)
        {
            isGrounded = true;
            carRb.drag=baseDragGrounded;

            if (jumpTimer<=jumpColdDownTime)
            {
                jumpTimer += Time.deltaTime;
            }
        }
        else
        {
            carRb.drag=baseDragNotGrounded;
            isGrounded = false;
            jumpTimer = 0;
        }
    }

    void CalculateVelocity()
    {
        
        currentVelocity = transform.InverseTransformDirection(carRb.velocity);
        carVelocityRatio = currentVelocity.z / maxSpeed;
    
    }
    void Suspension()

    {
        for (int i=0; i<rayPoints.Length; i++)
        {
            RaycastHit hit;
            float maxLenght = restLenght + springTravel;

            if (Physics.Raycast(rayPoints[i].position, -rayPoints[i].up, out hit, maxLenght+wheelRadius, drivable))
            {
                wheelIsGrounded[i] = 1;

                float currentSpringLenght = hit.distance - wheelRadius;
                float springCompression = (restLenght - currentSpringLenght) / springTravel;
                float springVelocity = Vector3.Dot(carRb.GetPointVelocity(rayPoints[i].position), rayPoints[i].up);
                float damperForce = springVelocity * damperStiffness;
                float springForce = springCompression * springStiffness;
                float netForce = springForce - damperForce;
                carRb.AddForceAtPosition(rayPoints[i].up * netForce, rayPoints[i].position);
                SetTirePosition(tires[i], hit.point+wheelRadius*rayPoints[i].up);
                Debug.DrawLine(rayPoints[i].position, hit.point, Color.red);
            }else
            {
                wheelIsGrounded[i] = 0;
                SetTirePosition (tires[i], rayPoints[i].position - rayPoints[i].up*maxLenght);
                Debug.DrawLine(rayPoints[i].position, rayPoints[i].position +(wheelRadius+ maxLenght)*-rayPoints[i].up, Color.green);
            }
        }

    }
    void Visuals()
    {
        TireVisuals();
    }
    void TireVisuals()
    {

        float turningAngle = maxTurningAngle * steerInput;
        for (int i = 0; i < tires.Length; i++)
        {
            if (i < 2)
            {
                tires[i].transform.Rotate(Vector3.right, tireRotSpeed * carVelocityRatio * Time.deltaTime,Space.Self);
                frontTireParents[i].transform.localEulerAngles = new Vector3 (frontTireParents[i].transform.rotation.x,turningAngle,frontTireParents[i].transform.rotation.z);
            }
            else
            {
                tires[i].transform.Rotate(Vector3.right, tireRotSpeed * moveInput * Time.deltaTime,Space.Self);
            }

        }
    }
    void SetTirePosition(GameObject tire, Vector3 targetPosition)
    {
        tire.transform.position = targetPosition;
    }
    
    private void GetControlType(InputAction.CallbackContext context)
    {
        if (context.control.device is Keyboard)
        {
            if (controlType==ControlTypes.Keyboard) return;
            controlType = ControlTypes.Keyboard;
            accelerateDelegate = AccelerateKeyboard;
            deAccelerateDelegate = DeAccelerateKeyboard;
        }
        else if (context.control.device is Gamepad)
        {
            if (controlType==ControlTypes.Controller) return;
            controlType = ControlTypes.Controller;
            accelerateDelegate = AccelerateController;
            deAccelerateDelegate = DeAccelerateController;
        }
    }
    void GetInput()
    {
        Vector2 inputVector = inputActions.Player.Movement.ReadValue<Vector2>();

        if (controlType==ControlTypes.Keyboard)
        {
            smoothInput = new Vector2 (
                Mathf.Lerp(smoothInput.x, inputVector.x, Time.deltaTime * keyboardInputSmoothing),
                Mathf.Lerp(smoothInput.y, inputVector.y, Time.deltaTime * keyboardInputSmoothing)
                );
            if (inputVector.y==0)
            {
                smoothInput.y = 0;
            }
            moveInput = smoothInput.y;
            steerInput = smoothInput.x;
        }
        else
        {
            moveInput = inputVector.y;
            steerInput = inputVector.x;
            accelerateInput = inputActions.Player.Accelerate.ReadValue<float>()-inputActions.Player.Break.ReadValue<float>();
        }
      
       
        handBrake = inputActions.Player.HandBreak.ReadValue<float>()>0.5f;
        if (inputActions.Player.Boost.ReadValue<float>()>0.5f)
        {
            OnBoost?.Invoke();
        }else
        {
            isBoosting=false;
        }
        

        
   
    }

    
    void Movement()
    {
        if (isGrounded)
        {   
            Turn();
            SidewaysDrag();
            HandBrake();
           
            if (isBoosting)return;
            deAccelerateDelegate();
            accelerateDelegate();

        }else
        {
            TurnInAir();
        }
    }

    //Se llaman desde el delegado DeAccelerate y Accelerate, dependiendo el tipo de input
    void AccelerateKeyboard()
    {
        // if (currentVelocity.z<maxSpeed)
        // {
        // carRb.AddForceAtPosition(acceleration*moveInput*transform.forward, accelerationPoint.position, ForceMode.Acceleration);
        // }
        carRb.AddForceAtPosition(acceleration*moveInput*transform.forward, accelerationPoint.position, ForceMode.Acceleration);
        
    }
    void DeAccelerateKeyboard()
    {
        
        // carRb.AddForceAtPosition(deAcceleration*Mathf.Abs(carVelocityRatio)*-transform.forward, accelerationPoint.position, ForceMode.Acceleration);
        carRb.AddForceAtPosition(deAcceleration*moveInput*-transform.forward, accelerationPoint.position, ForceMode.Acceleration);
        
    }
    void AccelerateController()
    {
        carRb.AddForceAtPosition(acceleration*accelerateInput*transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }
    void DeAccelerateController()
    {
        carRb.AddForceAtPosition(deAcceleration*accelerateInput*-transform.forward, accelerationPoint.position, ForceMode.Acceleration);
    }
    void Turn()
    {
       
       
        // Vector3 torqueLikeForce = steerInput 
        //     * turnStrength
        //     * turningCurve.Evaluate(Mathf.Abs(carVelocityRatio))
        //     * Mathf.Sign(carVelocityRatio)
        //     * transform.right;

       
        // carRb.AddForceAtPosition(torqueLikeForce*0.66f, rotationPivot.transform.position, ForceMode.Acceleration);
        carRb.AddTorque(steerInput * turnStrength * turningCurve.Evaluate(Mathf.Abs(carVelocityRatio))*Mathf.Sign(carVelocityRatio)*transform.up, ForceMode.Acceleration);
        carRb.AddTorque(steerInput * turnStrength *turningTorque* turningCurve.Evaluate(Mathf.Abs(carVelocityRatio))*Mathf.Sign(carVelocityRatio)*transform.forward, ForceMode.Acceleration);
    }
    void HandBrake()
    {
        if (handBrake)
        {

            
            timeOfHandBreak += Time.deltaTime;
            turnStrength = Mathf.Lerp(originalTurnStrength * 2f,originalTurnStrength * 3f, 1-carVelocityRatio);;
            carRb.drag = Mathf.Lerp(baseDragGrounded, baseDragGrounded * 0.25f, handBreakGripLossCurve.Evaluate(Mathf.Clamp(timeOfHandBreak/(handBreakTimeToStop/2),0,1)));
            acceleration = Mathf.Lerp(acceleration, deAcceleration*1.5f, timeOfHandBreak/handBreakTimeToStop);
           

        }
        else
        {
            timeOfHandBreak = 0;
            acceleration = originalAcceleration;
            turnStrength = originalTurnStrength;
            carRb.drag = baseDragGrounded;
           
        }
    }
    void PerformJump(InputAction.CallbackContext context)
    {
        if (!(jumpTimer >= jumpColdDownTime))return;
       
        jumpTimer = 0;
        Debug.Log("Jump");
        if (moveInput>-0.5f&&moveInput<0)
        {
            if (moveInput>0)
            {
            
            Quaternion rotation = transform.rotation * Quaternion.Euler(Math.Sign(moveInput) * frontJumpAngle, 0, 0);
            carRb.velocity = Vector3.zero;
            carRb.AddForceAtPosition(rotation * Vector3.up * jumpStrenght,backJumpForcePoint.transform.position,ForceMode.VelocityChange);
            }
            else if (moveInput<-0.5f)
            {
            Quaternion rotation = transform.rotation * Quaternion.Euler(Math.Sign(moveInput) * frontJumpAngle, 0, 0);
            carRb.velocity = Vector3.zero;
            carRb.AddForceAtPosition(rotation * Vector3.up * jumpStrenght*1.06f,frontJumpForcePoint.transform.position,ForceMode.VelocityChange);
            }
        
        }
        else
        {
            carRb.AddForce(transform.up*jumpStrenght*0.5f, ForceMode.VelocityChange);
        }
        
    }
    void TurnInAir()
    {
        carRb.AddTorque(-steerInput*transform.forward*TurnInAirForce, ForceMode.Acceleration);
        carRb.AddTorque(moveInput*transform.right*TurnInAirForce, ForceMode.Acceleration);
    }
    void SidewaysDrag()
    {
        float currentSidewaysSpeed = currentVelocity.x;

        float dragMagnitude = -currentSidewaysSpeed * dragCofficientForTurning;

        Vector3 dragForce = dragMagnitude * transform.right;

        carRb.AddForceAtPosition(dragForce, carRb.worldCenterOfMass, ForceMode.Acceleration);

    }

    private static void InitializeInputActions()
    {
       inputActions = new Controls();
    }

    public static Controls GetInputActions()
    {
        if (inputActions == null)
        {
            InitializeInputActions();
        }
        return inputActions;
    }

    
}
