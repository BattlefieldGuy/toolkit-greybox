using UnityEngine;
using UnityEngine.Events;

namespace NL.XRLab.Toolkit.Greybox.Interactions.Buttons
{
	/// <summary>
	/// A simple pressable button that can be interacted with.
	/// It has two states: pressed and not pressed.
	/// It can be configured to allow or disallow pressing and releasing.
	/// When pressed or released, it invokes the corresponding UnityEvents.
	/// </summary>
	public class PressableButton : MonoBehaviour, IInteractable
	{
		/// <summary>
		/// Whether the button can be pressed. If false, interacting with the button will have no effect.
		/// </summary>
		[SerializeField]
		protected bool _canPush = true;

		/// <summary>
		/// Whether the button is currently pressed.
		/// </summary>
		[SerializeField]
		protected bool _pressed;

		/// <summary>
		/// Whether the button can be released after being pressed.
		/// </summary>
		[SerializeField]
		protected bool _canRelease = true;

		/// <summary>
		/// UnityEvent invoked when the button is pressed.
		/// </summary>
		[SerializeField]
		private UnityEvent _onPress = new();

		/// <summary>
		/// UnityEvent invoked when the button is released.
		/// </summary>
		[SerializeField]
		private UnityEvent _onRelease = new();

		/// <summary>
		/// Getter for the UnityEvent invoked when the button is pressed.
		/// </summary>
		public UnityEvent OnPress => _onPress;

		/// <summary>
		/// Getter for the UnityEvent invoked when the button is released.
		/// </summary>
		public UnityEvent OnRelease => _onRelease;

		/// <summary>
		///    If the button is pushable, invokes the OnPress event.
		/// </summary>
		public virtual void Interact()
		{
			Debug.Log($"Interacting with {name}");
			if (!_canPush)
				return;
			TogglePressed();
		}

		/// <summary>
		///    Sets whether the button is pushable.
		/// </summary>
		/// <param name="pushable">Whether the button is pushable.</param>
		public void SetPushable(bool pushable)
		{
			_canPush = pushable;
		}

		/// <summary>
		///    Sets whether the button is pressed.
		/// </summary>
		/// <param name="pressed">Whether the button is pressed.</param>
		public void Press(bool pressed)
		{
			if (!_canPush)
				return;
			if (!_canRelease && _pressed)
				return;
			_pressed = pressed;
			if (_pressed)
				_onPress.Invoke();
			else
				_onRelease.Invoke();
		}

		/// <summary>
		///    Toggles the pressed state of the button.
		/// </summary>
		public void TogglePressed()
		{
			Debug.Log("Toggling Pressed");
			Press(!_pressed);
		}

		/// <summary>
		///   Returns whether the button is currently pressed.
		/// </summary>
		/// <returns>True if pressed, false otherwise.</returns>
		public bool IsPressed()
		{
			return _pressed;
		}
	}
}
