using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Interactions
{
	/// <summary>
	/// A door that can be opened and closed by interacting with it.
	/// </summary>
	public class Door : MonoBehaviour, IInteractable
	{
		/// <summary>
		/// Reference to the Animator component that controls the door's opening and closing animations.
		/// </summary>
		[SerializeField]
		private Animator _animator;

		/// <summary>
		/// Whether the door is currently open. This is used to determine which animation state to play when toggling the door.
		/// </summary>
		[SerializeField]
		private bool _isOpen;

		/// <summary>
		/// Whether the door can be opened by interacting with it. If false, interacting with the door will have no effect.
		/// </summary>
		[SerializeField]
		private bool _isOpenableByInteraction = true;

		/// <summary>
		/// Toggles the door's open state if it is openable by interaction. This method is called when the door is interacted with.
		/// </summary>
		public void Interact()
		{
			if (!_isOpenableByInteraction)
				return;
			ToggleOpen();
		}

		/// <summary>
		/// Toggles the door's open state and updates the Animator parameter to play the corresponding animation.
		/// </summary>
		public void ToggleOpen()
		{
			_isOpen = !_isOpen;
			_animator.SetBool("isOpen", _isOpen);
		}
	}
}
