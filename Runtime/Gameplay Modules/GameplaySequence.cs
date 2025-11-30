using System;
using System.Collections.Generic;
using NL.XRLab.Toolkit.Greybox.Extensions.Events;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.GameplayModules
{
	[Serializable]
	public class GameplaySequence
	{
		[SerializeField] private List<ConditionalUnityEvent> _events;

		public List<ConditionalUnityEvent> Events => _events;

		public void CacheConditionDelegates()
		{
			foreach (ConditionalUnityEvent conditionalEvent in _events)
				conditionalEvent.CacheConditionDelegates();
		}
	}
}
