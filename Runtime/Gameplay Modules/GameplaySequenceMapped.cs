using System;
using NL.XRLab.Toolkit.Greybox.Extensions.Data;
using NL.XRLab.Toolkit.Greybox.Extensions.Events;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Gameplay_Modules
{
	/// <summary>
	/// A serializable class that maps GameplayEventIdentifiers to ConditionalUnityEvents.
	/// </summary>
	[Serializable]
	public class GameplaySequenceMapped
	{
		/// <summary>
		/// A serializable dictionary that maps GameplayEventIdentifiers to ConditionalUnityEvents.
		/// </summary>
		[SerializeField]
		private SerializableDictionary<GameplayEventIdentifier, ConditionalUnityEvent> _map;

		/// <summary>
		/// Tries to invoke the ConditionalUnityEvent associated with the given GameplayEventIdentifier.
		/// </summary>
		/// <param name="eventIdentifier">The identifier of the event to invoke.</param>
		/// <returns>Whether the event was invoked (true when conditions are met).<returns>
		public bool TryInvokeEvent(GameplayEventIdentifier eventIdentifier)
		{
			if (_map.TryGetValue(eventIdentifier, out ConditionalUnityEvent conditionalEvent))
				return conditionalEvent.TryInvoke();

			Debug.LogWarning($"No event found for identifier: {eventIdentifier}");
			return false;
		}
	}
}
