using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if NEW_INPUT_SYSTEM_INSTALLED
using UnityEngine.InputSystem.UI;
#endif

public class TempUIScript : MonoBehaviour
{

    [SerializeField]
    Button m_StartHostButton;
    [SerializeField]
    Button m_StartClientButton;
    [SerializeField]
    Image m_Image;


    void Awake()
    {
        if (!FindAnyObjectByType<EventSystem>())
        {
            var inputType = typeof(StandaloneInputModule);
#if ENABLE_INPUT_SYSTEM && NEW_INPUT_SYSTEM_INSTALLED
                inputType = typeof(InputSystemUIInputModule);                
#endif
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), inputType);
            eventSystem.transform.SetParent(transform);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_StartHostButton.onClick.AddListener(StartHost);
        m_StartClientButton.onClick.AddListener(StartClient);
    }

    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        DeactivateButtons();
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        DeactivateButtons();
    }

    void DeactivateButtons()
    {
        m_StartHostButton.interactable = false;
        m_StartClientButton.interactable = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
