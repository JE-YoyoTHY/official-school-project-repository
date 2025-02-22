using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerScript : MonoBehaviour
{

	public static InputManagerScript instance;


	public float moveInput = 0;
	public InputState jumpInput = InputState.normal;
	public InputState fireballCastByKeyboardInput = InputState.normal;
	public InputState fireballCastByMouseInput = InputState.normal;
	public Vector2 fireballDirInput = Vector2.zero;

	private PlayerInput playerInput;
	private InputAction moveAction;
	private InputAction jumpAction;
	private InputAction fireballCastByKeyboardAction;
	private InputAction fireballCastByMouseAction;
	private InputAction fireballDirAction;
	


	private void Awake()
	{
		//singleton
		if (instance != null && instance != this)
		{
			Destroy(this);
		}
		else
		{
			instance = this;
		}

		playerInput = GetComponent<PlayerInput>();

		setupInputAction();
	}

    // Update is called once per frame
    void Update()
    {
        updateInputs();
    }

	private void setupInputAction()
	{
		moveAction = playerInput.actions["move"];
		jumpAction = playerInput.actions["jump"];
		fireballCastByKeyboardAction = playerInput.actions["fireballCastByKeyboard"];
		fireballCastByMouseAction = playerInput.actions["fireballCastByMouse"];
		fireballDirAction = playerInput.actions["fireballDir"];
	}

	private void updateInputs()
	{
		moveInput = moveAction.ReadValue<float>();
		if (jumpAction.WasPressedThisFrame()) jumpInput = InputState.press;
		if (jumpAction.WasReleasedThisFrame()) jumpInput = InputState.release;
		if (fireballCastByKeyboardAction.WasPressedThisFrame()) fireballCastByKeyboardInput = InputState.press;
		if (fireballCastByKeyboardAction.WasReleasedThisFrame()) fireballCastByKeyboardInput = InputState.release;
		if (fireballCastByMouseAction.WasPressedThisFrame()) fireballCastByMouseInput = InputState.press;
		if (fireballCastByMouseAction.WasReleasedThisFrame()) fireballCastByMouseInput = InputState.release;
		fireballDirInput = fireballDirAction.ReadValue<Vector2>();
	}
}

public enum InputState
{
	press,
	hold,
	release,
	normal
}
