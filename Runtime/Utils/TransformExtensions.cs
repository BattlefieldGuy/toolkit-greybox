using System.Collections;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Utils
{
	public static class TransformExtensions
	{
		public static IEnumerator MoveToLocalPositionCoroutine(Transform gfxToMove, Vector3 pressedPosition,
			float inDuration)
		{
			var elapsedTime = 0f;
			var startingPos = gfxToMove.localPosition;
			while (elapsedTime < inDuration)
			{
				gfxToMove.localPosition = Vector3.Lerp(startingPos, pressedPosition,
					elapsedTime / inDuration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}

			gfxToMove.localPosition = pressedPosition;
		}
	}
}
