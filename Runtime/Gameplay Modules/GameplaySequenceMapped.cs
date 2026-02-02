using System;
using NL.XRLab.Toolkit.Greybox.Extensions.Data;
using NL.XRLab.Toolkit.Greybox.Extensions.Events;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Gameplay_Modules
{
	[Serializable]
	public class GameplaySequenceMapped
	{
		[SerializeField] private SerializableDictionary<GameplayEventIdentifier, ConditionalUnityEvent> _map;

		public bool TryInvokeEvent(GameplayEventIdentifier eventIdentifier)
		{
			if (_map.TryGetValue(eventIdentifier, out ConditionalUnityEvent conditionalEvent))
				return conditionalEvent.TryInvoke();

			Debug.LogWarning($"No event found for identifier: {eventIdentifier}");
			return false;
		}
	}
}
