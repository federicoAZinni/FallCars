using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.UI;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.OnScreen;


public class NewButtonController : OnScreenControl
{
    private Finger finger;
    private Image button;

    void  Start()
    {
        button=GetComponent<Image>();
    }
    
        
    

    protected override void OnEnable()
    {
        base.OnEnable();
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
        EnhancedTouch.Touch.onFingerMove += CheckPosition;
        EnhancedTouch.Touch.onFingerDown += CheckPosition;
        EnhancedTouch.Touch.onFingerUp += ReleaseButton;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EnhancedTouch.Touch.onFingerMove -= CheckPosition;
        EnhancedTouch.Touch.onFingerDown -= CheckPosition;
        EnhancedTouch.Touch.onFingerUp -= ReleaseButton;
        EnhancedTouchSupport.Disable();
        TouchSimulation.Disable();
        }

    private void CheckPosition(Finger finger)
    {
        Vector2 pointerPos = finger.screenPosition;
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            button.rectTransform,
            pointerPos,
            null,
            out localPos
        );
        if (button.rectTransform.rect.Contains(localPos))
        {
            PressButton(finger);
        }
        else
        {
            ReleaseButton(finger);
        }
    }

    private void PressButton(Finger finger)
    {
        this.finger = finger;
        
        SendValueToControl(1.0f);
    }

    private void ReleaseButton(Finger finger)
    {
        if (this.finger != finger) return;
        this.finger = null;
        
        SendValueToControl(0.0f);
    }

    [InputControl(layout = "Button")]
    [SerializeField]
    private string m_ControlPath;

    protected override string controlPathInternal
    {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }
}

// using System;
// using System.Collections;
// using UnityEngine;
// using UnityEngine.InputSystem.EnhancedTouch;
// using UnityEngine.UI;
// using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;


// public class NewButtonController : MonoBehaviour
// {

//     private Controls inputActions;
//     private Finger finger;
//     [SerializeField] private Image button;
//     [SerializeField] private ScreenControlsManager.ScreenControls screenControl;
//     private event Action<ScreenControlsManager.ScreenControls>OnButtonPressed;
//     private event Action<ScreenControlsManager.ScreenControls>OnButtonReleased;




//     void OnEnable()
//     {
//         inputActions = SuspensionCarController.GetInputActions();
//         if(inputActions == null)
//         {
//             Debug.LogError("no input actions found");
//             return;
//         }
        
        
//         EnhancedTouchSupport.Enable();
//         TouchSimulation.Enable();
//         EnhancedTouch.Touch.onFingerMove += CheckPosition;
//         EnhancedTouch.Touch.onFingerDown += CheckPosition;
//         EnhancedTouch.Touch.onFingerUp += ReleaseButton;
//         StartCoroutine(SuscribeScreenControlsManager());

//     }

    

//     void OnDisable()
//     {
//         EnhancedTouch.Touch.onFingerMove -= CheckPosition;
//         EnhancedTouch.Touch.onFingerDown -= CheckPosition;
//         EnhancedTouch.Touch.onFingerUp -= ReleaseButton;
//         OnButtonPressed -= ScreenControlsManager.Instance.ActivateAction;
//         OnButtonReleased -= ScreenControlsManager.Instance.DeActivateAction;
//         EnhancedTouchSupport.Disable();
//         TouchSimulation.Disable();
//     }
   
   

//     private void CheckPosition(Finger finger)
//     {
//         Vector2 pointerPos = finger.screenPosition;
//         Vector2 localPos;
//         RectTransformUtility.ScreenPointToLocalPointInRectangle(
//             button.rectTransform,
//             pointerPos,
//             null, 
//             out localPos
//         );
//         if (button.rectTransform.rect.Contains(localPos))
//         {
//             PressButton(finger);
//         }else 
//         {
//             ReleaseButton(finger);
//         }
        
//     }

//     private void PressButton(Finger finger)
//     {
        
//         this.finger = finger;
//         OnButtonPressed?.Invoke(screenControl);
//     }
    
//     private void ReleaseButton(Finger finger)
//     {
//         if (this.finger != finger) return;
//         this.finger = null;
//         OnButtonReleased?.Invoke(screenControl);
        
//     }
//     private IEnumerator SuscribeScreenControlsManager()
//     {
//         while (ScreenControlsManager.Instance == null)
//         {
//             yield return null;
//         }
//         OnButtonPressed += ScreenControlsManager.Instance.ActivateAction;
//         OnButtonReleased += ScreenControlsManager.Instance.DeActivateAction;
        
//     }
// }
