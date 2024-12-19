using System.Data.Common;
using System.Diagnostics;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class FlyController : MonoBehaviour
{
    public bool IsFlyingForward = false;
    
    [SerializeField,Self] private Rigidbody flyerRb;
    [SerializeField,Anywhere] private Camera mainVRCamera;
    
    [SerializeField, Anywhere] private Transform leftHandPosition;
    [SerializeField, Anywhere] private Transform rightHandPosition;
    [SerializeField, Anywhere] private Transform headPosition;
    
    [SerializeField] private float flyingUpwardSpeed;
    [SerializeField] private float flyingForwardSpeed = 300f;
    [SerializeField] private Vector3 rotationEulerAngleVelocity;

    
    private float currentSpeed =1;
    private Vector3 currentDirection;

    private Vector3 flightDirection;
    
    private bool isLeftHandRaised;
    private bool isRightHandRaised;
    private bool changingHeight;
    private bool isRotating;

    private const int Zero=0;
    private const int One=1;
    

    private void Update()
    {
        if (isLeftHandRaised && isRightHandRaised)
        {
            currentSpeed = flyingForwardSpeed * 2;
        }
        else if (isLeftHandRaised || isRightHandRaised)
        {
            currentSpeed = flyingForwardSpeed;
        }
        else if (changingHeight )
        {
            currentSpeed = flyingUpwardSpeed;
        }
        else
        {
            currentSpeed = One;
        }
        
        Debug.Log(currentSpeed);
    }
    
    private void FixedUpdate()
    {
        flyerRb.linearVelocity = currentDirection * (currentSpeed * Time.deltaTime);

        if (IsFlyingForward)
        {
            AdjustFlightDirectionFinger();
        }

        if (isRotating)
        {
            Debug.Log("turning");
            flyerRb.MoveRotation(flyerRb.rotation * Quaternion.Euler(rotationEulerAngleVelocity));
        }
    }
    
    public void ChangingHeight(bool isGoingUp)
    {
        Debug.Log("Flying upwards");
        flyerRb.useGravity = false;
        changingHeight = true;

        if (isGoingUp)
        {
            currentDirection = Vector3.up;
        }
        else
        {
            currentDirection = Vector3.down;
        }
    }
    

    
    public void ChangingHeightStop()
    {
        flyerRb.useGravity = true;
        
        changingHeight = false;
    }
    
    public void HandFlight(bool isLeftHand)
    {
        
        IsFlyingForward= true;

        if (isLeftHand)
        {
            isLeftHandRaised = true;
            flightDirection = leftHandPosition.position;
        }
        else
        {
            isRightHandRaised = true;
            flightDirection = rightHandPosition.position;
        }
        
    }
    
    public void HandFlightStop(bool isLeftHand)
    {
        
        if (isLeftHand)
        {
            isLeftHandRaised = false;
            if (isRightHandRaised)
            {
                flightDirection = rightHandPosition.position;
            }
            else
            {
                IsFlyingForward = false;
            }
        }
        else
        {
            isRightHandRaised = false;
            if (isLeftHandRaised)
            {
                flightDirection = leftHandPosition.position;
            }else
            {
                IsFlyingForward = false;
            }
        }
    }
    
    /*private void AdjustFlightDirection()
    {
        currentDirection = mainVRCamera.transform.forward;
    }*/
    
    private void AdjustFlightDirectionFinger()
    {
        if (isLeftHandRaised)
        {
            currentDirection = rightHandPosition.position - headPosition.transform.position;
        }
        
        if (isRightHandRaised)
        {
            currentDirection = leftHandPosition.position - headPosition.transform.position;
        }
    }

    public void RBrotation(bool IsRotationLeft)
    {
        isRotating = true;
        if (IsRotationLeft)
        {
            rotationEulerAngleVelocity= new Vector3(0,rotationEulerAngleVelocity.y,0);
        }
        else
        {
            rotationEulerAngleVelocity= new Vector3(0,-rotationEulerAngleVelocity.y,0);
        }
        
    }
    
    public void RBrotationStop()
    {
        isRotating = false;
    }
    
    public enum Hands
    {
        Left,Right
    }

}

