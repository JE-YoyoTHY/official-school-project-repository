using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManagerScript : MonoBehaviour
{

	public static InputManagerScript instance;


	public float moveInput = 0;
	public InputState jumpInput;
	public InputState fireballCastByKeyboardInput;
	public InputState fireballCastByMouseInput;
	public Vector2 fireballDirInput = Vector2.zero;

	private PlayerInput playerInput;
	//private InputAction moveAction;
	private InputAction moveLeftAction;
	private InputAction moveRightAction;
	private InputAction jumpAction;
	//private InputAction fireballCastByKeyboardAction;
	private InputAction fireballCastByMouseAction;
	//private InputAction fireballDirAction;
	private InputAction fireballDirUpAction;
	private InputAction fireballDirDownAction;
	private InputAction fireballDirLeftAction;
	private InputAction fireballDirRightAction;



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
		//moveAction = playerInput.actions["move"];
		moveLeftAction = playerInput.actions["moveLeft"];
		moveRightAction = playerInput.actions["moveRight"];
		jumpAction = playerInput.actions["jump"];
		//fireballCastByKeyboardAction = playerInput.actions["fireballCastByKeyboard"];
		fireballCastByMouseAction = playerInput.actions["fireballCastByMouse"];
		//fireballDirAction = playerInput.actions["fireballDir"];
		fireballDirUpAction = playerInput.actions["fireballDirUp"];
		fireballDirDownAction = playerInput.actions["fireballDirDown"];
		fireballDirLeftAction = playerInput.actions["fireballDirLeft"];
		fireballDirRightAction = playerInput.actions["fireballDirRight"];
	}

	private void updateInputs()
	{
		//moveInput = moveAction.ReadValue<float>();
		updateMoveInput();
		if (jumpAction.WasPressedThisFrame()) jumpInput = InputState.press;
		if (jumpAction.WasReleasedThisFrame()) jumpInput = InputState.release;
		//if (fireballCastByKeyboardAction.WasPressedThisFrame()) fireballCastByKeyboardInput = InputState.press;
		//if (fireballCastByKeyboardAction.WasReleasedThisFrame()) fireballCastByKeyboardInput = InputState.release;
		if (fireballCastByMouseAction.WasPressedThisFrame()) fireballCastByMouseInput = InputState.press;
		if (fireballCastByMouseAction.WasReleasedThisFrame()) fireballCastByMouseInput = InputState.release;
		//fireballDirInput = fireballDirAction.ReadValue<Vector2>();
		updateFireballDirInput();
		if (fireballDirInput.magnitude == 0) fireballCastByKeyboardInput = InputState.release;
		if (fireballCastByKeyboardInput == InputState.release && fireballDirInput.magnitude > 0) fireballCastByKeyboardInput = InputState.press;
	}

	private void updateMoveInput()
	{
		//if key released
		if (moveLeftAction.WasReleasedThisFrame() && moveRightAction.IsPressed()) moveInput = 1;
		if (moveRightAction.WasReleasedThisFrame() && moveLeftAction.IsPressed()) moveInput = -1;

		//reset move input
		if (!moveLeftAction.IsPressed() && !moveRightAction.IsPressed()) moveInput = 0;

		//override move input
		if (moveLeftAction.WasPressedThisFrame()) moveInput = -1;
		if (moveRightAction.WasPressedThisFrame()) moveInput = 1;


	}

	private void updateFireballDirInput()
	{
		//vertical
		//if key released
		if (fireballDirUpAction.WasReleasedThisFrame() && fireballDirDownAction.IsPressed()) fireballDirInput.y = -1;
		if (fireballDirDownAction.WasReleasedThisFrame() && fireballDirUpAction.IsPressed()) fireballDirInput.y = 1;

		//reset fireball dir input
		if (!fireballDirUpAction.IsPressed() && !fireballDirDownAction.IsPressed()) fireballDirInput.y = 0;

		//override fireball dir input
		if (fireballDirUpAction.WasPressedThisFrame()) fireballDirInput.y = 1;
		if (fireballDirDownAction.WasPressedThisFrame()) fireballDirInput.y = -1;


		//horizontal
		//if key released
		if (fireballDirRightAction.WasReleasedThisFrame() && fireballDirLeftAction.IsPressed()) fireballDirInput.x = -1;
		if (fireballDirLeftAction.WasReleasedThisFrame() && fireballDirRightAction.IsPressed()) fireballDirInput.x = 1;

		//reset fireball dir input
		if (!fireballDirRightAction.IsPressed() && !fireballDirLeftAction.IsPressed()) fireballDirInput.x = 0;

		//override fireball dir input
		if (fireballDirRightAction.WasPressedThisFrame()) fireballDirInput.x = 1;
		if (fireballDirLeftAction.WasPressedThisFrame()) fireballDirInput.x = -1;


		fireballDirInput.Normalize();
	}
}

public enum InputState
{
	press,
	hold,
	release,
	normal
}
