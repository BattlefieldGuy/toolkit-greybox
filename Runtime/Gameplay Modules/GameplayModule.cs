using UnityEngine;
using UnityEngine.Events;

namespace NL.XRLab.Toolkit.Greybox.GameplayModules
{
	public class GameplayModule : MonoBehaviour
	{
		[SerializeField] public GameplayModuleData GameplayModuleData;

		public GameplaySequence GameplaySequence;

		public UnityEvent<GameplayModule> OnModuleCompleted = new();

		private void CompleteModule()
		{
			OnModuleCompleted?.Invoke(this);
		}
	}
}
