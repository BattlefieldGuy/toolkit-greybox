using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Interactions
{
	public class RaycastInteractor : Interactor
	{
		[SerializeField] private Transform _interactionOrigin;
		[SerializeField] private float _maxInteractionDistance = 5f;

#if UNITY_EDITOR
		[SerializeField] private bool _drawDebugRay = true;
#endif

		public override void AttemptInteract()
		{
#if UNITY_EDITOR
			if (_drawDebugRay)
				Debug.DrawRay(_interactionOrigin.position, _interactionOrigin.forward * _maxInteractionDistance,
					Color.yellow, 1f);
#endif
			if (!Physics.Raycast(
				    _interactionOrigin.position, _interactionOrigin.forward, out var hit, _maxInteractionDistance,
				    _interactionLayerMask)
			   )
				return;

#if UNITY_EDITOR
			if (_drawDebugRay)
				Debug.DrawRay(_interactionOrigin.position, _interactionOrigin.forward * hit.distance, Color.green, 1f);
#endif

			var interactable = hit.collider.GetComponent<IInteractable>();
			Debug.Log($"Found: {interactable}");
			if (interactable != null)
				Interact(interactable);
		}
	}
}
