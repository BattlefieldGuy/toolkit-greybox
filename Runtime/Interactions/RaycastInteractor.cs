using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Interactions
{
	/// <summary>
	/// An interactor that uses raycasting to detect and interact with objects in the environment.
	/// </summary>
	public class RaycastInteractor : Interactor
	{
		/// <summary>
		/// The origin point from which the raycast will be emitted.
		/// </summary>
		[SerializeField]
		private Transform _interactionOrigin;

		/// <summary>
		/// The maximum distance the raycast will check for interactable objects.
		/// </summary>
		[SerializeField]
		private float _maxInteractionDistance = 5f;

#if UNITY_EDITOR
		/// <summary>
		/// Whether to draw debug rays in the editor to visualize the raycast.
		/// </summary>
		[SerializeField]
		private bool _drawDebugRay = true;
#endif

		public override void AttemptInteract()
		{
#if UNITY_EDITOR
			if (_drawDebugRay)
				Debug.DrawRay(
					_interactionOrigin.position,
					_interactionOrigin.forward * _maxInteractionDistance,
					Color.yellow,
					1f
				);
#endif
			if (
				!Physics.Raycast(
					_interactionOrigin.position,
					_interactionOrigin.forward,
					out var hit,
					_maxInteractionDistance,
					_interactionLayerMask
				)
			)
				return;

#if UNITY_EDITOR
			if (_drawDebugRay)
				Debug.DrawRay(
					_interactionOrigin.position,
					_interactionOrigin.forward * hit.distance,
					Color.green,
					1f
				);
#endif

			var interactable = hit.collider.GetComponent<IInteractable>();
			if (interactable != null)
				Interact(interactable);
		}
	}
}
