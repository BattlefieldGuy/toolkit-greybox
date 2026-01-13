using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Samples.Scripts
{
	public class PlayerOrientator : MonoBehaviour
	{
		[SerializeField] private Camera _camera;
		[SerializeField] private Rigidbody _rigidbody;
		[SerializeField] private float _minPitch = -85f;
		[SerializeField] private float _maxPitch = 85f;

		private float _currentPitch;

		public void OnLook(Vector2 direction)
		{
			float yawInput = direction.x;
			float pitchInput = direction.y;

			// Rotate body (yaw)
			_rigidbody.MoveRotation(
				_rigidbody.rotation * Quaternion.Euler(0f, yawInput, 0f)
			);

			// Accumulate & clamp pitch
			_currentPitch = Mathf.Clamp(
				_currentPitch - pitchInput, // minus = natural mouse look
				_minPitch,
				_maxPitch
			);

			// Apply pitch to camera
			_camera.transform.localRotation = Quaternion.Euler(_currentPitch, 0f, 0f);
		}
	}
}
