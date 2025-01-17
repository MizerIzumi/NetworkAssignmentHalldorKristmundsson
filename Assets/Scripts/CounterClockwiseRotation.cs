using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
public class CounterClockwiseRotation : NetworkBehaviour
{
    [SerializeField] private GameObject Wheel;
    private bool IsInteractable = false;

    void Update()
    {
        CheckIfShouldRotate();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsInteractable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsInteractable = false;
        }
    }

    private void CheckIfShouldRotate()
    {
        if (IsInteractable) RotateWheelClockwiseRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void RotateWheelClockwiseRpc()
    {
        Wheel.transform.Rotate(0, 0, 0.5f);
    }
}
