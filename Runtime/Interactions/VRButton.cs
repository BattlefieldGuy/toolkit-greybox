using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Simple button script for the greyboxing toolkit.
///
/// Drag the necessary events into the inspector and make sure to use the "Player" prefab or
/// add coliders and Rigidbody's to the controllers with the "Player" tag for the button to work.
/// </summary>
public class VRButton : MonoBehaviour
{
	/// <summary> Unity event used to call other scripts when the button is pressed. </summary>
	[SerializeField]
	private UnityEvent _onPress;

	[SerializeField, Tooltip("The distance that the button moves on press, Default is 0.05")]
	private float _pressDepth = 0.05f;

	[SerializeField, Tooltip("The duration of the buttons 'animation' time")]
	private float _pressDuration = 0.5f;

	/// <summary> Rest heigt where the button always moves back to, Always starting position of object on start. </summary>
	private float _restHeight;

	/// <summary> Used to prevent a button from being pressed multiple times at once, which could cause issues with the animation and event invocation. </summary>
	private bool _isPressed = false;

	/// <summary> Start is only used for setting the restHeight in this script. </summary>
	private void Start()
	{
		_restHeight = this.transform.localPosition.y;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isPressed)
			return;

		if (other.CompareTag("Player"))
		{
			_isPressed = true;
			StartCoroutine(ButtonAnimation());
			_onPress.Invoke();
		}
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
