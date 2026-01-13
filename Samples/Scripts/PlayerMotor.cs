using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Samples.Scripts
{
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMotor : MonoBehaviour
	{
		[SerializeField] private float _torque = 5f;

		private Vector2 _direction;

		private Rigidbody _rigidbody;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		public void FixedUpdate()
		{
			MoveInDirection();
		}

		public void UpdateMoveDirection(Vector2 direction)
		{
			_direction = direction;
		}

		private void MoveInDirection()
		{
			// Debug.Log($"Moving in direction: {_direction}");
			var move =
				transform.right * _direction.x +
				transform.forward * _direction.y;
			move.y = _rigidbody.linearVelocity.y;

			_rigidbody.linearVelocity = move.normalized * _torque;
		}
	}
}
