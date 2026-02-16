using NL.XRLab.Toolkit.Greybox.Interactions;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Samples.Scripts
{
	/// <summary>
	/// A sample first-person player class for use in the Greybox samples.
	/// </summary>
	[RequireComponent(typeof(PlayerMotor))]
	[RequireComponent(typeof(FirstPersonPlayerController))]
	[RequireComponent(typeof(RaycastInteractor))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(PlayerOrientator))]
	public class FirstPersonPlayer : MonoBehaviour
	{
		/// <summary>
		/// Whether the cursor should start locked when the game begins.
		/// </summary>
		[SerializeField]
		private bool _startCursorLocked = true;

		/// <summary>
		/// Whether the cursor is currently locked. This is used to track the cursor state and update it when toggled.
		/// </summary>
		private bool _cursorLocked;

		/// <summary>
		/// Reference to the FirstPersonPlayerController component that handles player input and controls the player's movement and looking around.
		/// </summary>
		public FirstPersonPlayerController Controller { get; private set; }

		/// <summary>
		/// Reference to the RaycastInteractor component that allows the player to interact with objects in the environment using raycasting.
		/// </summary>
		public RaycastInteractor Interactor { get; private set; }

		/// <summary>
		/// Reference to the Rigidbody component that allows the player to be affected by physics and move around the environment using forces.
		/// </summary>
		public Rigidbody Rigidbody { get; private set; }

		/// <summary>
		/// Reference to the PlayerMotor component that handles the player's movement logic.
		/// </summary>
		public PlayerMotor Motor { get; private set; }

		/// <summary>
		/// Reference to the PlayerOrientator component that handles the player's orientation and looking around logic.
		/// </summary>
		public PlayerOrientator Orientator { get; private set; }

		private void Awake()
		{
			Controller = GetComponent<FirstPersonPlayerController>();
			Interactor = GetComponent<RaycastInteractor>();
			Rigidbody = GetComponent<Rigidbody>();
			Motor = GetComponent<PlayerMotor>();
			Orientator = GetComponent<PlayerOrientator>();
			SetCursorLock(_startCursorLocked);
		}

		private void OnEnable()
		{
			SubscribeControllerEvents();
		}

		private void OnDisable()
		{
			SubscribeControllerEvents(true);
		}

		/// <summary>
		/// Toggles the cursor lock state between locked and unlocked.
		/// </summary>
		private void ToggleCursorLock()
		{
			SetCursorLock(!_cursorLocked);
		}

		/// <summary>
		/// Sets the cursor lock state to either locked or unlocked based on the provided boolean value. This will update the Cursor.lockState accordingly.
		/// </summary>
		/// <param name="isLocked">Whether the cursor should be locked (true) or unlocked (false).</param>
		private void SetCursorLock(bool isLocked)
		{
			_cursorLocked = isLocked;
			UpdateCursorLockState();
		}

		/// <summary>
		/// Updates the Cursor.lockState based on the current value of _cursorLocked.
		/// If _cursorLocked is true, the cursor will be locked to the center of the screen; if false, it will be unlocked and free to move around.
		/// </summary>
		private void UpdateCursorLockState()
		{
			Cursor.lockState = _cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
		}

		/// <summary>
		/// Subscribes or unsubscribes the player's controller events to the appropriate methods in the PlayerMotor, PlayerOrientator, and RaycastInteractor components.
		/// </summary>
		/// <param name="unsubscribe">Whether to subscribe (false) or unsubscribe (true).</param>
		private void SubscribeControllerEvents(bool unsubscribe = false)
		{
			if (unsubscribe)
			{
				Controller.OnMove.RemoveListener(Motor.UpdateMoveDirection);
				Controller.OnLook.RemoveListener(Orientator.OnLook);
				Controller.OnInteract.RemoveListener(Interactor.AttemptInteract);
				Controller.OnToggleCursorLock.RemoveListener(ToggleCursorLock);
				return;
			}

			Controller.OnMove.AddListener(Motor.UpdateMoveDirection);
			Controller.OnLook.AddListener(Orientator.OnLook);
			Controller.OnInteract.AddListener(Interactor.AttemptInteract);
			Controller.OnToggleCursorLock.AddListener(ToggleCursorLock);
		}
	}
}
