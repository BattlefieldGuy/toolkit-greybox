using UnityEngine;
using UnityEngine.Events;

namespace NL.XRLab.ToolkitGreybox.GameplayModules
{
	public class GameplayModule : MonoBehaviour
	{
		[SerializeField] public GameplayModuleData GameplayModuleData;

		public UnityEvent<GameplayModule> OnModuleCompleted = new();

		private void CompleteModule()
		{
			OnModuleCompleted?.Invoke(this);
		}
	}
}
