using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Interactions
{
	public class RaycastInteractor : Interactor
	{
		[SerializeField] private float _maxInteractionDistance = 5f;

#if UNITY_EDITOR
		[SerializeField] private bool _drawDebugRay = true;
#endif

		public override void AttemptInteract()
		{
#if UNITY_EDITOR
			if (_drawDebugRay)
				Debug.DrawRay(transform.position, transform.forward * _maxInteractionDistance, Color.yellow, 1f);
#endif
			if (!Physics.Raycast(
				    transform.position, transform.forward, out var hit, _maxInteractionDistance, _interactionLayerMask)
			   )
				return;

#if UNITY_EDITOR
			if (_drawDebugRay)
				Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.green, 1f);
#endif

			var interactable = hit.collider.GetComponent<IInteractable>();
			if (interactable != null)
				Interact(interactable);
		}
	}
}
