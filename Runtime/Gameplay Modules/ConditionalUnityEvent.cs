using System;
using UnityEngine;
using UnityEngine.Events;

namespace NL.XRLab.Toolkit.Greybox.GameplayModules
{
	[Serializable]
	public class ConditionalUnityEvent
	{
		[SerializeField] private GameObject _conditionTarget;
		[SerializeField] private string _methodName;

		[SerializeField] private UnityEvent _unityEvent;

		private Func<bool> _cachedCondition;

		public bool IsConditionMet()
		{
			if (_cachedCondition != null) return false;

			if (_conditionTarget == null || string.IsNullOrEmpty(_methodName))
				return false;

			return _cachedCondition?.Invoke() ?? false;
		}

		public void TryTrigger()
		{
			if (IsConditionMet())
				_unityEvent.Invoke();
		}
	}
}
