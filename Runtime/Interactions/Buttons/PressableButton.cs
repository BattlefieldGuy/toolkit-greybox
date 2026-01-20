using UnityEngine;
using UnityEngine.Events;

namespace NL.XRLab.Toolkit.Greybox.Interactions.Buttons
{
	public class PressableButton : MonoBehaviour, IInteractable
	{
		[SerializeField] protected bool _canPush = true;
		[SerializeField] protected bool _pressed;
		[SerializeField] protected bool _canRelease = true;

		[SerializeField] private UnityEvent _onPress = new();
		[SerializeField] private UnityEvent _onRelease = new();

		public UnityEvent OnPress => _onPress;
		public UnityEvent OnRelease => _onRelease;

		/// <summary>
		///    If the button is pushable, invokes the OnPress event.
		/// </summary>
		public virtual void Interact()
		{
			Debug.Log($"Interacting with {name}");
			if (!_canPush) return;
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

		public bool IsPressed()
		{
			return _pressed;
		}
	}
}
