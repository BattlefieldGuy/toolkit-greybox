using NL.XRLab.Toolkit.Greybox.Utils;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Interactions.Buttons
{
	/// <summary>
	/// A pushable button that visually moves when pressed and released.
	/// </summary>
	public class PushableButton : PressableButton
	{
		/// <summary>
		/// The Transform of the graphics that should move when the button is pressed and released.
		/// </summary>
		[SerializeField]
		private Transform _gfxToMove;

		/// <summary>
		/// The distance the button's graphics should move when pressed, in local space units.
		/// </summary>
		[SerializeField]
		private float _travelDistance = 0.2f;

		/// <summary>
		/// The duration of the movement when the button is pressed or released, in seconds.
		/// </summary>
		[SerializeField]
		private float _travelDuration = 0.75f;

		/// <summary>
		/// The local position of the button's graphics when the button is not pressed.
		/// </summary>
		private Vector3 _releasedPosition;

		/// <summary>
		/// The local position of the button's graphics when the button is pressed.
		/// </summary>
		private Vector3 _pressedPosition => _releasedPosition + Vector3.down * _travelDistance;

		private void Awake()
		{
			_releasedPosition = _gfxToMove.localPosition;
			if (_pressed)
				_gfxToMove.localPosition = _pressedPosition;
		}

		public void OnEnable()
		{
			OnPress.AddListener(MoveDown);
			OnRelease.AddListener(MoveUp);
		}

		public void OnDisable()
		{
			OnPress.RemoveListener(MoveDown);
			OnRelease.RemoveListener(MoveUp);
		}

		/// <summary>
		/// Moves the button's graphics down to the pressed position over the travel duration.
		/// </summary>
		private void MoveDown()
		{
			StartCoroutine(
				TransformExtensions.MoveToLocalPositionCoroutine(
					_gfxToMove,
					_pressedPosition,
					_travelDuration
				)
			);
		}

		/// <summary>
		/// Moves the button's graphics up to the released position over the travel duration.
		/// </summary>
		private void MoveUp()
		{
			StartCoroutine(
				TransformExtensions.MoveToLocalPositionCoroutine(
					_gfxToMove,
					_releasedPosition,
					_travelDuration
				)
			);
		}
	}
}
