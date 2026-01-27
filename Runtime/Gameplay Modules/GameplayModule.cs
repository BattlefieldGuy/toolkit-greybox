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
		[SerializeField] private bool _disableModuleOnComplete = true;

		private readonly bool _isCompleted = false;


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
			if (!isActiveAndEnabled)
				return;
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
			if (!isActiveAndEnabled)
				return;
			GameplaySequence.TryInvokeCurrentEvent();
		}

		public void TryInvokeEventInSequence(int eventIndex)
		{
			if (!isActiveAndEnabled)
				return;
			GameplaySequence.TryInvokeEvent(eventIndex, false);
		}

		public void TryInvokeEventInSequenceUnlessPassed(int eventIndex)
		{
			if (!isActiveAndEnabled)
				return;
			GameplaySequence.TryInvokeEvent(eventIndex, true);
		}

		private void CompleteModule()
		{
			Logger.Log("GameplayModule completed: " + GameplayModuleData.name);
			OnModuleCompleted?.Invoke(this);
			if (_disableModuleOnComplete)
				gameObject.SetActive(false);
		}
	}
}
