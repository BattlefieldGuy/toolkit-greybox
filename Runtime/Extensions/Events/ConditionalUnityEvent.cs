using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace NL.XRLab.Toolkit.Greybox.Extensions.Events
{
	[Serializable]
	public class ConditionalUnityEvent
	{
		// List of inspector-assignable condition methods
		[SerializeField] private List<SerializableCondition> _conditions = new();

		// The event to be invoked
		[SerializeField] private UnityEvent _onConditionsAreMet;
		[SerializeField] private UnityEvent _onConditionsAreNotMet;

		/// <summary>
		///    Initializes all SerializableConditions within this event, building the delegate cache.
		///    Should be called once at runtime setup (e.g., in Awake).
		/// </summary>
		public void CacheConditionDelegates()
		{
			foreach (SerializableCondition condition in _conditions) condition.CacheConditionDelegate();
		}

		/// <summary>
		///    Checks all conditions. Returns true if all conditions are met.
		/// </summary>
		private bool ConditionsAreMet()
		{
			foreach (SerializableCondition condition in _conditions)
				// Stop and return false as soon as one condition is not met
				if (!condition.IsMet())
					return false;

			// If the list is empty or all conditions returned true, the event can be invoked
			return true;
		}

		/// <summary>
		///    Invokes the underlying UnityEvent if all conditions are met.
		/// </summary>
		public bool TryInvoke()
		{
			if (!ConditionsAreMet())
			{
				_onConditionsAreNotMet.Invoke();
				return false;
			}

			_onConditionsAreMet.Invoke();
			return true;
		}
	}
}
