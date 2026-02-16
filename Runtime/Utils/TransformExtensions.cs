using System.Collections;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Utils
{
	/// <summary>
	/// Extension methods for the Transform class.
	/// </summary>
	public static class TransformExtensions
	{
		/// <summary>
		/// Coroutine that smoothly moves a Transform to a target local position over a specified duration.
		/// </summary>
		/// <param name="transformToMove">The Transform that should be moved.</param>
		/// <param name="targetPosition">The target local position to move to.</param>
		/// <param name="inDuration">The duration of the movement in seconds.</param>
		/// <returns>An IEnumerator for use in a coroutine.</returns>
		public static IEnumerator MoveToLocalPositionCoroutine(
			Transform transformToMove,
			Vector3 targetPosition,
			float inDuration
		)
		{
			var elapsedTime = 0f;
			var startingPos = transformToMove.localPosition;
			while (elapsedTime < inDuration)
			{
				transformToMove.localPosition = Vector3.Lerp(
					startingPos,
					targetPosition,
					elapsedTime / inDuration
				);
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			transformToMove.localPosition = targetPosition;
		}
	}
}
