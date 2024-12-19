using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Events;

public class DeliveryEndPoint : MonoBehaviour
{
    [SerializeField, Self]private BoxCollider deliveryPointCollider;

    public GameObject currentPresent;
    public UnityEvent OnDeliveryFinished;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == currentPresent)
        {
            Debug.Log("Delivery Done!");
            OnDeliveryFinished.Invoke();
        }
    }
}
