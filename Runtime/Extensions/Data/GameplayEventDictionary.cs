using System;
using NL.XRLab.Toolkit.Greybox.Extensions.Events;
using NL.XRLab.Toolkit.Greybox.Gameplay_Modules;

namespace NL.XRLab.Toolkit.Greybox.Extensions.Data
{
	/// <summary>
	/// Serializable dictionary that maps GameplayEventIdentifiers to ConditionalUnityEvents.
	/// This allows for flexible event handling based on gameplay events, where each event can have its own set of conditions and responses defined in the Unity Inspector.
	/// </summary>
	[Serializable]
	public class GameplayEventDictionary
		: SerializableDictionary<GameplayEventIdentifier, ConditionalUnityEvent> { }
}
