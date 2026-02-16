using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Samples.Scripts
{
	/// <summary>
	/// A simple player motor that moves the player in a given direction using Rigidbody physics.
	/// </summary>
	[RequireComponent(typeof(Rigidbody))]
	public class PlayerMotor : MonoBehaviour
	{
		/// <summary>
		/// The torque applied to the player when moving. This controls how fast the player moves in the given direction.
		/// </summary>
		[SerializeField]
		private float _torque = 5f;

		/// <summary>
		/// The current movement direction for the player.
		/// </summary>
		private Vector2 _direction;

		/// <summary>
		/// Reference to the Rigidbody component that is used to move the player using physics.
		/// </summary>
		private Rigidbody _rigidbody;

		private void Awake()
		{
			_rigidbody = GetComponent<Rigidbody>();
		}

		public void FixedUpdate()
		{
			MoveInDirection();
		}

		/// <summary>
		/// Updates the movement direction for the player. This method should be called by the player controller when it receives input for movement.
		/// </summary>
		/// <param name="direction">The updated direction.</param>
		public void UpdateMoveDirection(Vector2 direction)
		{
			_direction = direction;
		}

		/// <summary>
		/// Moves the player in the current direction by applying a force to the Rigidbody.
		/// The movement is based on the player's local right and forward directions, allowing for movement relative to the player's orientation.
		/// </summary>
		private void MoveInDirection()
		{
			// Debug.Log($"Moving in direction: {_direction}");
			var move = transform.right * _direction.x + transform.forward * _direction.y;
			move.y = 0;

			_rigidbody.linearVelocity = move.normalized * _torque;
		}
	}
}
