using System;
using System.Reflection;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Extensions.Events
{
	/// <summary>
	///    Represents an inspector-assigned condition that checks a public, parameterless, bool-returning method
	///    on a component of a GameObject. Uses delegate caching for high performance at runtime.
	/// </summary>
	[Serializable]
	public class SerializableCondition
	{
		// The GameObject that holds the condition method
		[SerializeField]
		private GameObject _conditionSource;

		// The composite key set by the Editor: "Full.TypeName|Index|MethodName"
		[SerializeField]
		private string _methodName;

		[SerializeField]
		private bool _hideDefaultMethods = true;

		/// <summary>
		/// The cached delegate that points to the condition method that is called at runtime.
		/// Initialized only after CacheConditionDelegate() is called.
		/// </summary>
		private Func<bool> _cachedCondition;

		/// <summary>
		/// Flag to indicate whether the delegate has been initialized.
		/// This prevents redundant initialization and allows for a fallback initialization if IsMet() is called before CacheConditionDelegate().
		/// </summary>
		private bool _isInitialized;

		/// <summary>
		/// Indicates whether this SerializableCondition has valid data to attempt initialization.
		/// This is a quick check to avoid unnecessary errors during initialization.
		/// </summary>
		public bool IsValid => _conditionSource != null && !string.IsNullOrEmpty(_methodName);

		/// <summary>
		///    Initializes the delegate cache. Must be called once before IsMet() is called.
		///    This is the expensive step that runs only once (e.g., in Awake).
		/// </summary>
		public void CacheConditionDelegate()
		{
			if (_isInitialized || !IsValid)
				return;

			try
			{
				// 1. Parse the key string: "Full.TypeName|Index|MethodName"
				string[] parts = _methodName.Split('|');
				if (parts.Length != 3 || !int.TryParse(parts[1], out int componentIndex))
				{
					Debug.LogError(
						$"[EventCache] Invalid serialized method format: {_methodName}. Initialization skipped."
					);
					return;
				}

				string typeName = parts[0];
				string methodName = parts[2];

				// 2. Find the target component Type
				var componentType = Type.GetType(typeName);
				if (componentType == null)
				{
					Debug.LogError(
						$"[EventCache] Could not find Type '{typeName}'. Initialization skipped."
					);
					return;
				}

				// 3. Find the specific component instance using the index
				// GetComponents(Type) returns all components of that type in the order they appear in the Inspector/array.
				var components = _conditionSource.GetComponents(componentType);

				if (componentIndex < 0 || componentIndex >= components.Length)
				{
					Debug.LogError(
						$"[EventCache] Component index {componentIndex} is out of bounds for type {typeName} on {_conditionSource.name}. Initialization skipped."
					);
					return;
				}

				// The specific, unique component instance is found by index!
				var targetComponent = components[componentIndex];

				// 4. Get the MethodInfo
				var methodInfo = targetComponent
					.GetType()
					.GetMethod(
						methodName,
						BindingFlags.Public | BindingFlags.Instance,
						null,
						Type.EmptyTypes,
						null
					);

				if (methodInfo == null || methodInfo.ReturnType != typeof(bool))
				{
					Debug.LogError(
						$"[EventCache] Method '{methodName}' not found or signature mismatch on '{typeName}'. Initialization skipped."
					);
					return;
				}

				// 5. Create the optimized delegate and cache it
				_cachedCondition =
					(Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), targetComponent, methodInfo);
				_isInitialized = true;
			}
			catch (Exception e)
			{
				Debug.LogError(
					$"[EventCache] Failed to initialize condition for '{_methodName}' on '{_conditionSource.name}'. Error: {e.Message}"
				);
			}
		}

		/// <summary>
		///    Fast runtime check using the cached delegate.
		/// </summary>
		public bool IsMet()
		{
			if (!_isInitialized)
			{
				Debug.LogError(
					$"Condition for '{_methodName}' was not initialized before calling IsMet()! Calling Initialize() now (performance warning)."
				);
				// Fallback: If not initialized, attempt to initialize immediately
				CacheConditionDelegate();
				// If it still isn't initialized after the attempt, return false to avoid errors
				if (!_isInitialized)
					return false;
			}

			Debug.Log("Checking if condition is met: " + _methodName);
			return _cachedCondition();
		}
	}
}
