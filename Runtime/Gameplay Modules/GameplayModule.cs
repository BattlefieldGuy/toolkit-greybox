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

		[SerializeField] private bool _autoStart = true;


		private void Awake()
		{
			GameplaySequence.CacheConditionDelegates();
			GameplaySequence.OnSequenceFinished.AddListener(CompleteModule);
			GameplaySequence.BelongingModule = this;
		}

		private void Start()
		{
			if (_autoStart)
				StartSequence();
		}

		private void StartSequence()
		{
			if (!GameplaySequence.HasEventLeft)
			{
				Logger.LogWarning(
					$"Tried to start GameplaySequence for {GameplayModuleData.name}, but it has no events. Completing module immediately.");
				CompleteModule();
				return;
			}

			GameplaySequence.TryInvokeCurrentEvent();
		}


		public void TryInvokeCurrentEventInSequence()
		{
			GameplaySequence.TryInvokeCurrentEvent();
		}

		public void TryInvokeEventInSequence(int eventIndex)
		{
			GameplaySequence.TryInvokeEvent(eventIndex, false);
		}

		public void TryInvokeEventInSequenceUnlessPassed(int eventIndex)
		{
			GameplaySequence.TryInvokeEvent(eventIndex, true);
		}

		private void CompleteModule()
		{
			Logger.Log("GameplayModule completed: " + GameplayModuleData.name);
			OnModuleCompleted?.Invoke(this);
		}
	}
}
