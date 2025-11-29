using System;
using System.Collections.Generic;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.GameplayModules
{
	[Serializable]
	public class GameplaySequence
	{
		[SerializeField] private List<ConditionalUnityEvent> _events;

		public List<ConditionalUnityEvent> Events => _events;
	}
}
