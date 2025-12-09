using UnityEngine;
using UnityEngine.Events;
using Logger = NL.XRLab.Toolkit.Greybox.Utils.Logger;

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
			GameplaySequence.OnSequenceFinished.AddListener(CompleteModule);
		}

		private void Start()
		{
			StartSequence();
		}

		private void StartSequence()
		{
			if (GameplaySequence.Events.Count == 0)
			{
				Logger.LogWarning(
					$"Tried to start GameplaySequence for {GameplayModuleData.name}, but it has no events. Completing module immediately.");
				CompleteModule();
				return;
			}

			GameplaySequence.TryInvokeNextEvent();
		}

		private void CompleteModule()
		{
			Logger.Log("GameplayModule completed: " + GameplayModuleData.name);
			OnModuleCompleted?.Invoke(this);
		}
	}
}
