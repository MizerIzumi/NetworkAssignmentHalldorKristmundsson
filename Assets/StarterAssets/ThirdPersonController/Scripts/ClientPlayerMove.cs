using Unity.Netcode;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class ClientPlayerMove : NetworkBehaviour
{
    [SerializeField] CharacterController m_CharacterController;
    [SerializeField] ThirdPersonController m_ThirdPersonController;
    [SerializeField] ThirdPersonCharacter m_ThirdPersonCharacter;
    [SerializeField] PlayerInput m_PlayerInput;

    [SerializeField] Camera m_CameraFollow;

    private void Awake()
    {
        m_PlayerInput.enabled = false;
        m_CharacterController.enabled = false;
        m_ThirdPersonController.enabled = false;
        m_ThirdPersonCharacter.enabled = false;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        enabled = IsClient;
        if (!IsOwner)
        {
            enabled = false;
            m_PlayerInput.enabled = false;
            m_CharacterController.enabled = false;
            m_ThirdPersonController.enabled = false;
            m_ThirdPersonCharacter.enabled = false;
            return;
        }

        m_PlayerInput.enabled = true;
        m_CharacterController.enabled = true;
        m_ThirdPersonController.enabled = true;
        m_ThirdPersonCharacter.enabled = true;

    }
}
