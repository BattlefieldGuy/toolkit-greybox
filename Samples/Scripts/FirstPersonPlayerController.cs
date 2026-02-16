using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace NL.XRLab.Toolkit.Greybox.Samples.Scripts
{
	/// <summary>
	/// A simple first-person player controller that uses Unity's new Input System to handle movement, looking around, and interactions.
	/// </summary>
	public class FirstPersonPlayerController : MonoBehaviour
	{
		/// <summary>
		/// Sensitivity for looking around. Separate values for pitch (looking up/down) and yaw (looking left/right) to allow for different sensitivities on each axis.
		/// </summary>
		[SerializeField]
		private float _lookSensitivityPitch = .2f;

		/// <summary>
		/// Sensitivity for looking around. Separate values for pitch (looking up/down) and yaw (looking left/right) to allow for different sensitivities on each axis.
		/// </summary>
		[SerializeField]
		private float _lookSensitivityYaw = .2f;

		/// <summary>
		/// Whether to invert the pitch (looking up/down) input. This is a common option in first-person games to allow players to choose their preferred control scheme.
		/// </summary>
		[SerializeField]
		private bool _invertPitch;

		/// <summary>
		/// Whether to invert the yaw (looking left/right) input. This is less common than inverting pitch, but some players may prefer it, so it's included as an option.
		/// </summary>
		[SerializeField]
		private bool _invertYaw;

		/// <summary>
		/// Reference to the PlayerInput component that provides access to the input actions defined in the Input System.
		/// This is used to read the player's input for movement, looking around, and interactions.
		/// </summary>
		[SerializeField]
		private PlayerInput _playerInput;

		/// <summary>
		/// Backing field for the lazily initialized move action. This is not serialized and is created from the PlayerInput's actions when needed.
		/// </summary>
		private InputAction _moveAction;

		/// <summary>
		/// The sensitivity for looking around, taking into account the individual sensitivities for pitch and yaw, as well as whether each axis is inverted.
		/// This is calculated on the fly to allow for changes to the sensitivity and inversion settings at runtime.
		/// </summary>
		private Vector2 LookSensitivity =>
			new(
				_invertYaw ? -_lookSensitivityYaw : _lookSensitivityYaw,
				_invertPitch ? -_lookSensitivityPitch : _lookSensitivityPitch
			);

		/// <summary>
		/// Event that is invoked when the player provides input for movement.
		/// </summary>
		public UnityEvent<Vector2> OnMove { get; } = new();

		/// <summary>
		/// Event that is invoked when the player provides input for looking around.
		/// </summary>
		public UnityEvent<Vector2> OnLook { get; } = new();

		/// <summary>
		/// Event that is invoked when the player provides input to interact with something in the game world.
		/// /// </summary>
		public UnityEvent OnInteract { get; } = new();

		/// <summary>
		/// Event that is invoked when the player provides input to toggle the cursor lock state.		/// </summary>
		public UnityEvent OnToggleCursorLock { get; } = new();

		private void Start()
		{
			_moveAction = _playerInput.actions.FindAction("Move");
		}

		private void Update()
		{
			PollMoveInput();
		}

		/// <summary>
		/// Polls the move input action for the player's movement input and invokes the OnMove event with the input value.
		/// </summary>
		private void PollMoveInput()
		{
			var input = _moveAction.ReadValue<Vector2>();
			OnMove.Invoke(input);
		}

		/// <summary>
		/// Handles the input action for toggling the cursor lock state.
		/// </summary>
		/// <param name="context">The InputAction context.</param>
		public void OnToggleCursorLockAction(InputAction.CallbackContext context)
		{
			if (!context.started)
				return;
			OnToggleCursorLock.Invoke();
		}

		/// <summary>
		/// Handles the input action for looking around.
		/// </summary>
		/// <param name="context">The InputAction context.</param>
		public void OnLookInputAction(InputAction.CallbackContext context)
		{
			var input = context.ReadValue<Vector2>();
			OnLook.Invoke(input * LookSensitivity);
		}

		/// <summary>
		/// Handles the input action for interacting with something in the game world.
		/// </summary>
		/// <param name="context">The InputAction context.</param>
		public void OnInteractInputAction(InputAction.CallbackContext context)
		{
			if (!context.started)
				return;
			OnInteract.Invoke();
		}
	}
}
