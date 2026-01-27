using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace NL.XRLab.Toolkit.Greybox.Samples.Scripts
{
	public class FirstPersonPlayerController : MonoBehaviour
	{
		[SerializeField] private float _lookSensitivityPitch = .2f;
		[SerializeField] private float _lookSensitivityYaw = .2f;

		[SerializeField] private bool _invertPitch;
		[SerializeField] private bool _invertYaw;

		[SerializeField] private PlayerInput _playerInput;

		private InputAction _moveAction;

		private Vector2 LookSensitivity =>
			new(
				_invertYaw ? -_lookSensitivityYaw : _lookSensitivityYaw,
				_invertPitch ? -_lookSensitivityPitch : _lookSensitivityPitch
			);

		public UnityEvent<Vector2> OnMove { get; } = new();
		public UnityEvent<Vector2> OnLook { get; } = new();
		public UnityEvent OnInteract { get; } = new();
		public UnityEvent OnToggleCursorLock { get; } = new();

		private void Start()
		{
			_moveAction = _playerInput.actions.FindAction("Move");
		}

		private void Update()
		{
			PollMoveInput();
		}

		private void PollMoveInput()
		{
			var input = _moveAction.ReadValue<Vector2>();
			OnMove.Invoke(input);
		}

		public void OnToggleCursorLockAction(InputAction.CallbackContext context)
		{
			if (!context.started) return;
			OnToggleCursorLock.Invoke();
		}

		public void OnLookInputAction(InputAction.CallbackContext context)
		{
			var input = context.ReadValue<Vector2>();
			OnLook.Invoke(input * LookSensitivity);
		}

		public void OnInteractInputAction(InputAction.CallbackContext context)
		{
			if (!context.started) return;
			OnInteract.Invoke();
		}
	}
}
