using Unity.Netcode;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Emote : NetworkBehaviour
{
    private InputAction _actEmoteRed;
    private InputAction _actEmoteGreen;

    [SerializeField] private GameObject EmoteCubeRed;
    [SerializeField] private GameObject EmoteCubeGreen;

    void Start()
    {
        _actEmoteRed = InputSystem.actions.FindAction("Emote2");
        _actEmoteGreen = InputSystem.actions.FindAction("Emote1");
    }

    void Update()
    {
        EmoteRed();
        EmoteGreen();
    }
    private void EmoteRed()
    {
        if (_actEmoteRed.WasPressedThisFrame())
        {
            StartCoroutine("EmoteCubeRedAction");
        }
    }

    private void EmoteGreen()
    {
        if (_actEmoteGreen.WasPressedThisFrame())
        {
            StartCoroutine("EmoteCubeGreenAction");
        }
    }

    IEnumerator EmoteCubeRedAction()
    {
        //Do emote, enable and disable colored cube above player
        if (IsOwner)
        {
            EmoteCubeRedEnableRpc();
        }
        yield return new WaitForSeconds(1);
        if (IsOwner)
        {
            EmoteCubeRedDisableRpc();
        }
    }

    IEnumerator EmoteCubeGreenAction()
    {
        //Do emote, enable and disable colored cube above player
        if (IsOwner)
        {
            EmoteCubeGreenEnableRpc();
        }
        yield return new WaitForSeconds(1);
        if (IsOwner)
        {
            EmoteCubeGreenDisableRpc();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void EmoteCubeRedEnableRpc()
    {
        EmoteCubeRed.SetActive(true);
    }

    [Rpc(SendTo.Everyone)]
    private void EmoteCubeRedDisableRpc()
    {
        EmoteCubeRed.SetActive(false);
    }

    [Rpc(SendTo.Everyone)]
    private void EmoteCubeGreenEnableRpc()
    {
        EmoteCubeGreen.SetActive(true);
    }

    [Rpc(SendTo.Everyone)]
    private void EmoteCubeGreenDisableRpc()
    {
        EmoteCubeGreen.SetActive(false);
    }
}
