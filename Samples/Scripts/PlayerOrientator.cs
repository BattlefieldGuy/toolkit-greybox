using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Samples.Scripts
{
	/// <summary>
	/// Simple component that orients the player based on input.
	/// </summary>
	public class PlayerOrientator : MonoBehaviour
	{
		/// <summary>
		/// Reference to the camera that will be rotated for pitch (looking up and down).
		/// </summary>
		[SerializeField]
		private Camera _camera;

		/// <summary>
		/// Reference to the Rigidbody that will be rotated for yaw (looking left and right). The Rigidbody should be on the same GameObject as this component.
		/// </summary>
		[SerializeField]
		private Rigidbody _rigidbody;

		/// <summary>
		/// Minimum pitch angle.
		/// </summary>
		[SerializeField]
		private float _minPitch = -85f;

		/// <summary>
		/// Maximum pitch angle.
		/// </summary>
		[SerializeField]
		private float _maxPitch = 85f;

		/// <summary>
		/// The current pitch angle of the camera.
		/// </summary>
		private float _currentPitch;

		/// <summary>
		/// Updates the player's orientation based on the input direction.
		/// The x component of the direction is used for yaw (rotating left and right), and the y component is used for pitch (rotating up and down).
		/// </summary>
		/// <param name="direction">The input direction for looking around, where x is yaw and y is pitch.</param>
		public void OnLook(Vector2 direction)
		{
			float yawInput = direction.x;
			float pitchInput = direction.y;

			// Rotate body (yaw)
			_rigidbody.MoveRotation(_rigidbody.rotation * Quaternion.Euler(0f, yawInput, 0f));

			// Accumulate & clamp pitch
			_currentPitch = Mathf.Clamp(_currentPitch - pitchInput, _minPitch, _maxPitch);

			// Apply pitch to camera
			_camera.transform.localRotation = Quaternion.Euler(_currentPitch, 0f, 0f);
		}
	}
}
