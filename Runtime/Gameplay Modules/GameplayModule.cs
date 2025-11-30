using UnityEngine;
using UnityEngine.Events;

namespace NL.XRLab.Toolkit.Greybox.GameplayModules
{
	public class GameplayModule : MonoBehaviour
	{
		[SerializeField] public GameplayModuleData GameplayModuleData;

		public GameplaySequence GameplaySequence;

		public UnityEvent<GameplayModule> OnModuleCompleted = new();


		private void Awake()
		{
			GameplaySequence.CacheConditionDelegates();
		}

		private void Start()
		{
			GameplaySequence.Events[0].TryInvoke();
		}

		private void CompleteModule()
		{
			OnModuleCompleted?.Invoke(this);
		}
	}
}
