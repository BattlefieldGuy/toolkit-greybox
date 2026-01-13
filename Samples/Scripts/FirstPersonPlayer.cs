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
		}

		private void OnEnable()
		{
			SubscribeControllerEvents();
		}

		private void OnDisable()
		{
			SubscribeControllerEvents(true);
		}

		private void SubscribeControllerEvents(bool unsubscribe = false)
		{
			if (unsubscribe)
			{
				Controller.OnMove.RemoveListener(Motor.UpdateMoveDirection);
				Controller.OnLook.RemoveListener(Orientator.OnLook);
				return;
			}

			// Subscribe to movement input
			Controller.OnMove.AddListener(Motor.UpdateMoveDirection);
			Controller.OnLook.AddListener(Orientator.OnLook);
		}
	}
}
