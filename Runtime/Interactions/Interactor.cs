using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Interactions
{
	public abstract class Interactor : MonoBehaviour
	{
		/// <summary>
		///    The layer mask defining which layers can be interacted with.
		/// </summary>
		[SerializeField] protected LayerMask _interactionLayerMask;

		/// <summary>
		///    Interact with the given interactable object.
		/// </summary>
		/// <param name="interactable">The interactable object to interact with.</param>
		protected static void Interact(IInteractable interactable)
		{
			interactable.Interact();
		}

		/// <summary>
		///    Attempt to interact with an interactable object.
		/// </summary>
		public abstract void AttemptInteract();
	}
}
