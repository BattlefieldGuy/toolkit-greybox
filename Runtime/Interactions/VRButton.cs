using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple button script for the greyboxing toolkit.
///
/// Drag the necessary events into the inspector and make sure to use the "Player" prefab or
/// add coliders and Rigidbody's to the controllers with the "Player" tag for the button to work.
/// </summary>
namespace nl.xr_lab.toolkit_greybox.Interactions
{
	public class VRButton : MonoBehaviour
	{
		/// <summary> Unity event used to call other scripts when the button is pressed. </summary>
		[
			 SerializeField,
			 Tooltip("Unity event used to call other scripts when the button is pressed.")
		]
		private UnityEvent _onPress;

		/// <summary> The distance that the button moves on press, Default is 0.05 </summary>
		[SerializeField, Tooltip("The distance that the button moves on press, Default is 0.05")]
		private float _pressDepth = 0.05f;

		/// <summary> The duration of the buttons 'animation' time. </summary>
		[SerializeField, Tooltip("The duration of the buttons 'animation' time")]
		private float _pressDuration = 0.5f;

		/// <summary> Rest heigt where the button always moves back to, Always starting position of object on start. </summary>
		private float _restHeight;

		/// <summary> Used to prevent a button from being pressed multiple times at once, which could cause issues with the animation and event invocation. </summary>
		private bool _isPressed = false;

		private void Start()
		{
			//  Start is only used for setting the restHeight in this script.
			_restHeight = this.transform.localPosition.y;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				TriggerButton();
			}
		}

		/// <summary>
		/// Triggers button animation and events, also checks if the button is already pressed to prevent multiple presses at once.
		/// </summary>
		private void TriggerButton()
		{
			if (_isPressed)
				return;

			_isPressed = true;
			StartCoroutine(ButtonAnimation());
			_onPress.Invoke();
		}

		/// <summary>
		/// Basic button animation for visual feedback.
		/// The Depth of press and duration of the animation can be set in the inspector.
		/// </summary>
		private IEnumerator ButtonAnimation()
		{
			// Moves button down to give visual feedback of being pressed, and wait the set duration.
			this.transform.localPosition = new Vector3(0.0f, _pressDepth, 0.0f);
			yield return new WaitForSeconds(_pressDuration);

			// Resets button and bool to be activated again.
			this.transform.localPosition = new Vector3(0.0f, _restHeight, 0.0f);
			_isPressed = false;
		}
	}
}
