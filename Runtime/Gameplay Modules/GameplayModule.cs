using NL.XRLab.Toolkit.Greybox.Gameplay_Modules;
using UnityEngine;
using UnityEngine.Events;
using Logger = NL.XRLab.Toolkit.Greybox.Utils.Logger;

namespace NL.XRLab.Toolkit.Greybox.GameplayModules
{
	public class GameplayModule : MonoBehaviour
	{
		[SerializeField] public GameplayModuleData GameplayModuleData;

		// public GameplaySequenceLinear gameplaySequenceLinear;
		public GameplaySequenceMapped gameplaySequenceMapped;
		public UnityEvent<GameplayModule> OnModuleCompleted = new();

		[SerializeField] private bool _autoStart = true;
		[SerializeField] private bool _disableModuleOnComplete = true;

		private readonly bool _isCompleted = false;


		private void Awake()
		{
			// gameplaySequenceLinear.CacheConditionDelegates();
			// gameplaySequenceLinear.OnSequenceFinished.AddListener(CompleteModule);
			// gameplaySequenceLinear.BelongingModule = this;
		}

		private void Start()
		{
			if (_autoStart)
				StartSequence();
		}

		private void StartSequence()
		{
			if (!isActiveAndEnabled)
				return;
			// if (!gameplaySequenceLinear.HasEventLeft)
			// {
			// 	Logger.LogWarning(
			// 		$"Tried to start GameplaySequence for {GameplayModuleData.name}, but it has no events. Completing module immediately.");
			// 	CompleteModule();
			// 	return;
			// }
			//
			// gameplaySequenceLinear.TryInvokeCurrentEvent();
		}

		public void TryInvokeGameplayEvent(GameplayEventIdentifier eventIdentifier)
		{
			if (!isActiveAndEnabled)
				return;
			gameplaySequenceMapped.TryInvokeEvent(eventIdentifier);
		}


		public void TryInvokeCurrentEventInSequence()
		{
			if (!isActiveAndEnabled)
				return;
			// gameplaySequenceLinear.TryInvokeCurrentEvent();
		}

		public void TryInvokeEventInSequence(int eventIndex)
		{
			if (!isActiveAndEnabled)
				return;
			// gameplaySequenceLinear.TryInvokeEvent(eventIndex, false);
		}

		public void TryInvokeEventInSequenceUnlessPassed(int eventIndex)
		{
			if (!isActiveAndEnabled)
				return;
			// gameplaySequenceLinear.TryInvokeEvent(eventIndex, true);
		}

		public void CompleteModule()
		{
			Logger.Log("GameplayModule completed: " + GameplayModuleData.name);
			OnModuleCompleted?.Invoke(this);
			if (_disableModuleOnComplete)
				gameObject.SetActive(false);
		}
	}
}
