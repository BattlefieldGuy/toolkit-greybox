using NL.XRLab.Toolkit.Greybox.GameplayModules;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Samples.Samples.PuzzleDependentDoor.Scripts
{
	public class FinishModuleArea : MonoBehaviour
	{
		[SerializeField] private GameplayModule _gameplayModule;
		private BoxCollider _finishAreaCollider;

		private void Awake()
		{
			_finishAreaCollider = GetComponent<BoxCollider>();
		}

		private void OnTriggerEnter(Collider other)
		{
			Debug.Log("Trigger enter");
			_gameplayModule.GameplaySequence.TryInvokeCurrentEvent();
		}
	}
}
