using System;
using System.Collections.Generic;
using NL.XRLab.Toolkit.Greybox.Extensions.Events;
using UnityEngine;
using UnityEngine.Events;
using Logger = NL.XRLab.Toolkit.Greybox.Utils.Logger;

namespace NL.XRLab.Toolkit.Greybox.GameplayModules
{
	[Serializable]
	public class GameplaySequence
	{
		[SerializeField] private List<ConditionalUnityEvent> _events = new();

		public UnityEvent OnSequenceFinished = new();

		private int _currentEventIndex;

		public List<ConditionalUnityEvent> Events => _events;

		private ConditionalUnityEvent CurrentEvent => HasEventLeft ? Events[_currentEventIndex] : null;

		public bool HasEventLeft => _currentEventIndex < Events.Count;

		public GameplayModule BelongingModule { get; set; }

		public void CacheConditionDelegates()
		{
			foreach (var conditionalEvent in _events)
				conditionalEvent.CacheConditionDelegates();
		}

		public void TryInvokeCurrentEvent()
		{
			Logger.Log(
				$"Trying to invoke current event in GameplaySequence for {BelongingModule.GameplayModuleData.name}. (Total events: {Events.Count}, Current index: {_currentEventIndex})");
			if (!HasEventLeft)
			{
				CompleteSequence();
				return;
			}

			if (!CurrentEvent.TryInvoke())
			{
				Logger.LogWarning("TryInvokeNextEvent: Conditions for next event not met.");
				return;
			}

			CompleteEvent();
		}

		private void CompleteEvent()
		{
			_currentEventIndex++;
			if (!HasEventLeft)
				CompleteSequence();
		}

		private void CompleteSequence()
		{
			Logger.Log($"GameplaySequence completed for {BelongingModule.GameplayModuleData.name}.");
			OnSequenceFinished.Invoke();
		}
	}
}
