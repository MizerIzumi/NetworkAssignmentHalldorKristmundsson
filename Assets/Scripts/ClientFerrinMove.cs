using Unity.Netcode;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class ClientFerrinMove : NetworkBehaviour
{
    [SerializeField] CharacterController m_CharacterController;
    [SerializeField] ThirdPersonCharacter m_ThirdPersonCharacter;
    [SerializeField] private CinemachineCamera m_vc;
    [SerializeField] private AudioListener m_Listener;

    private void Awake()
    {
        m_CharacterController.enabled = false;
        m_ThirdPersonCharacter.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsClient;
        if (!IsOwner)
        {
            enabled = false;
            m_CharacterController.enabled = false;
            m_ThirdPersonCharacter.enabled = false;
            m_Listener.enabled = false;
            m_vc.enabled = false;
            m_vc.Priority = 0;
            return;
        }

        m_CharacterController.enabled = true;
        m_ThirdPersonCharacter.enabled = true;
        m_vc.enabled = true;
        m_vc.Priority = 8;

    }
}

