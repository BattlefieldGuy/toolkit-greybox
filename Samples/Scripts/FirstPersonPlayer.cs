using NL.XRLab.Toolkit.Greybox.Interactions;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Samples.Scripts
{
	[RequireComponent(typeof(PlayerMotor))]
	[RequireComponent(typeof(FirstPersonPlayerController))]
	[RequireComponent(typeof(RaycastInteractor))]
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(PlayerOrientator))]
	public class FirstPersonPlayer : MonoBehaviour
	{
		[SerializeField] private bool _startCursorLocked = true;

		private bool _cursorLocked;

		public FirstPersonPlayerController Controller { get; private set; }

		public RaycastInteractor Interactor { get; private set; }

		public Rigidbody Rigidbody { get; private set; }

		public PlayerMotor Motor { get; private set; }
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

		private void ToggleCursorLock()
		{
			SetCursorLock(!_cursorLocked);
		}

		private void SetCursorLock(bool isLocked)
		{
			_cursorLocked = isLocked;
			UpdateCursorLockState();
		}

		private void UpdateCursorLockState()
		{
			Cursor.lockState = _cursorLocked ? CursorLockMode.Locked : CursorLockMode.None;
			Debug.Log("Cursor lock state: " + Cursor.lockState);
		}

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
