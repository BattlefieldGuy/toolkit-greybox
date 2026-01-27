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
			foreach (ConditionalUnityEvent conditionalEvent in _events)
				conditionalEvent.CacheConditionDelegates();
		}

		public void TryInvokeCurrentEvent()
		{
			Logger.Log(
				$"Trying to invoke current event in GameplaySequence for {BelongingModule.GameplayModuleData.name}. (Total events: {Events.Count}, Current index: {_currentEventIndex})");
			if (!AttemptCompleteSequence()) return;

			if (!CurrentEvent.TryInvoke())
			{
				Logger.LogWarning("TryInvokeNextEvent: Conditions for next event not met.");
				return;
			}

			CompleteCurrentEvent();
		}

		private void CompleteCurrentEvent()
		{
			MoveToNextEvent();
			AttemptCompleteSequence();
		}

		private void MoveToNextEvent()
		{
			_currentEventIndex++;
		}

		private bool AttemptCompleteSequence()
		{
			if (HasEventLeft)
				return false;
			CompleteSequence();
			return true;
		}


		private void CompleteSequence()
		{
			Logger.Log($"GameplaySequence completed for {BelongingModule.GameplayModuleData.name}.");
			OnSequenceFinished.Invoke();
		}

		public void TryInvokeEvent(int eventIndex, bool ignoreIfPassedIndex)
		{
			if (eventIndex == _currentEventIndex)
			{
				TryInvokeCurrentEvent();
				return;
			}

			if (eventIndex < _currentEventIndex && ignoreIfPassedIndex)
				return;
			if (eventIndex >= Events.Count)
			{
				Logger.LogWarning(
					$"Tried to invoke event at index {eventIndex} in GameplaySequence for {BelongingModule.GameplayModuleData.name}, but index is out of range.");
				return;
			}

			ConditionalUnityEvent eventToInvoke = Events[eventIndex];
			if (!eventToInvoke.TryInvoke()) Logger.Log("TryInvokeEvent: Conditions for event not met.");
		}
	}
}
