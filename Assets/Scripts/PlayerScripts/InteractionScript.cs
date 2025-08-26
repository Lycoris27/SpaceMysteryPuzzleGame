using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
using System;

public class InteractionScript : MonoBehaviour
{
    public static event Action interactionEvent;

    #if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
    #endif

    private void Start()
    {
    #if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
    #else
		Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
    #endif
    }

    public void OnInteract(InputValue value)
    {
        if(value.isPressed)
        {
            interactionEvent?.Invoke();
        }
    }
}
