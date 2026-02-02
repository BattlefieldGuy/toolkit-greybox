using System;
using NL.XRLab.Toolkit.Greybox.Extensions.Events;
using NL.XRLab.Toolkit.Greybox.Gameplay_Modules;

namespace NL.XRLab.Toolkit.Greybox.Extensions.Data
{
	[Serializable]
	public class GameplayEventDictionary : SerializableDictionary<GameplayEventIdentifier, ConditionalUnityEvent>
	{
	}
}
