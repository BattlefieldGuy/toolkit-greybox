using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Interactions
{
	/// <summary>
	/// A component that delegates interaction calls to another GameObject's IInteractable component.
	/// </summary>
	public class InteractionDelegator : MonoBehaviour, IInteractable
	{
		/// <summary>
		/// Reference to the target GameObject that has an IInteractable component.
		/// </summary>
		[SerializeField]
		private GameObject _targetInteractable;

		/// <summary>
		/// When this method is called, it will call the Interact method on the IInteractable component of the target GameObject.
		/// </summary>
		public void Interact()
		{
			_targetInteractable.GetComponent<IInteractable>().Interact();
		}
	}
}
