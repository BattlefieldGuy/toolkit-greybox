using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Interactions
{
	public class InteractionDelegator : MonoBehaviour, IInteractable
	{
		[SerializeField] private GameObject _targetInteractable;

		public void Interact()
		{
			_targetInteractable.GetComponent<IInteractable>().Interact();
		}
	}
}
