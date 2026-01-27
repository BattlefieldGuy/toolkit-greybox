using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Interactions
{
	public class Door : MonoBehaviour, IInteractable
	{
		[SerializeField] private Animator _animator;

		[SerializeField] private bool _isOpen;
		[SerializeField] private bool _isOpenableByInteraction = true;

		public void Interact()
		{
			if (!_isOpenableByInteraction) return;
			ToggleOpen();
		}

		public void ToggleOpen()
		{
			_isOpen = !_isOpen;
			_animator.SetBool("isOpen", _isOpen);
		}
	}
}
