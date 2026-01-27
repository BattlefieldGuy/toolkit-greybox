using NL.XRLab.Toolkit.Greybox.Utils;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Interactions.Buttons
{
	public class PushableButton : PressableButton
	{
		[SerializeField] private Transform _gfxToMove;

		[SerializeField] private float _travelDistance = 0.2f;
		[SerializeField] private float _travelDuration = 0.75f;

		private Vector3 _releasedPosition;

		private Vector3 _pressedPosition =>
			_releasedPosition + Vector3.down * _travelDistance;


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

		private void MoveDown()
		{
			Debug.Log("Move Down");
			StartCoroutine(TransformExtensions.MoveToLocalPositionCoroutine(
				_gfxToMove,
				_pressedPosition,
				_travelDuration
			));
		}

		private void MoveUp()
		{
			Debug.Log("Move Up");
			StartCoroutine(TransformExtensions.MoveToLocalPositionCoroutine(
				_gfxToMove,
				_releasedPosition,
				_travelDuration
			));
		}
	}
}
