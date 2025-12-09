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
		[SerializeField] private List<ConditionalUnityEvent> _events;

		public UnityEvent OnSequenceFinished = new();


		private int _currentEventIndex = 0;

		public List<ConditionalUnityEvent> Events => _events;

		private ConditionalUnityEvent NextEvent => !HasNextEvent ? null : Events[_currentEventIndex + 1];

		private bool HasNextEvent => _currentEventIndex + 1 < Events.Count;

		public void CacheConditionDelegates()
		{
			foreach (ConditionalUnityEvent conditionalEvent in _events)
				conditionalEvent.CacheConditionDelegates();
		}

		public void TryInvokeNextEvent()
		{
			if (!HasNextEvent)
			{
				Logger.LogWarning("No more events to invoke in GameplaySequence, marking sequence as finished.");
				OnSequenceFinished.Invoke();
				return;
			}

			if (!NextEvent.TryInvoke()) Logger.LogWarning("TryInvokeNextEvent: Conditions for next event not met.");
		}
	}
}
