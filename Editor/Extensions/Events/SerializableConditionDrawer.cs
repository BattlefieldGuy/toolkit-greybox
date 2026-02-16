using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NL.XRLab.Toolkit.Greybox.Extensions.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NL.XRLab.Toolkit.Greybox.Editor.Extensions.Events
{
	/// <summary>
	/// Custom property drawer for SerializableCondition,
	/// which allows users to select a GameObject
	/// and then choose from its public bool methods with no parameters.
	/// </summary>
	[CustomPropertyDrawer(typeof(SerializableCondition))]
	public class SerializableConditionDrawer : PropertyDrawer
	{
		/// <summary>
		/// Property name for the condition source GameObject reference in SerializableCondition.
		/// </summary>
		private const string ConditionSourcePropName = "_conditionSource";

		/// <summary>
		/// Property name for the method name string in SerializableCondition,
		/// which will store a parseable key (Full.TypeName|Index|MethodName)
		/// for runtime lookup.
		/// </summary>
		private const string MethodNamePropName = "_methodName";

		/// <summary>
		/// Name of the condition source field in the Visual Tree Asset.
		/// This must match the name of the PropertyField element in the UXML file
		/// that is used to select the GameObject.
		/// </summary>
		private const string ConditionSourceFieldName = "conditionSourcePropertyField";

		/// <summary>
		/// Name of the method dropdown field in the Visual Tree Asset.
		/// This must match the name of the DropdownField element in the UXML file
		/// that is used to display the list of methods.
		/// </summary>
		private const string MethodDropdownFieldName = "methodDropdownField";

		/// <summary>
		/// Reference to the UI Toolkit Visual Tree Asset that defines the layout of the property drawer.
		/// </summary>
		[SerializeField]
		private VisualTreeAsset _visualTreeAsset;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var root = new VisualElement();

			if (_visualTreeAsset == null)
			{
				root.Add(
					new Label("ERROR: The Visual Tree Asset for SerializableConditionDrawer is missing.")
					{
						style = { color = Color.red },
					}
				);
				return root;
			}

			// Clone the Visual Tree Asset to create the UI for this property drawer
			_visualTreeAsset.CloneTree(root);

			// Find the relevant serialized properties
			SerializedProperty sourceProp = property.FindPropertyRelative(ConditionSourcePropName);
			SerializedProperty methodNameProp = property.FindPropertyRelative(MethodNamePropName);

			// Query the GameObject field and bind it to the _conditionSource property
			var sourceField = root.Q<PropertyField>(ConditionSourceFieldName);
			sourceField.bindingPath = ConditionSourcePropName;

			// Query the method dropdown field
			var methodDropdown = root.Q<DropdownField>(MethodDropdownFieldName);

			// Store the current object reference to check for changes
			Object initialSourceValue = sourceProp.objectReferenceValue;

			RefreshMethodDropdown(sourceProp, methodNameProp, methodDropdown);

			// Set up the change callback for the GameObject field
			sourceField.RegisterCallback<SerializedPropertyChangeEvent>(evt =>
			{
				// When the source GameObject changes, we need to:
				// 1. Clear the method name property since it's no longer valid
				// 2. Refresh the method dropdown choices based on the new GameObject
				if (sourceProp.objectReferenceValue != initialSourceValue)
				{
					methodNameProp.stringValue = string.Empty;
					methodNameProp.serializedObject.ApplyModifiedProperties();

					initialSourceValue = sourceProp.objectReferenceValue;
				}

				RefreshMethodDropdown(sourceProp, methodNameProp, methodDropdown);
			});

			// Set up the change callback for the Method dropdown
			methodDropdown.RegisterValueChangedCallback(evt =>
			{
				// Save the full parseable key (Full.TypeName|Index|MethodName)
				// to the serialized property
				methodNameProp.stringValue = evt.newValue;
				methodNameProp.serializedObject.ApplyModifiedProperties();
			});

			root.Bind(property.serializedObject);

			return root;
		}

		/// <summary>
		/// Refreshes the choices in the method dropdown based on
		/// the currently selected GameObject.
		/// </summary>
		/// <param name="sourceProp">The propery that defines the condition source.</param>
		/// <param name="methodNameProp">The propery that defines the name of the validation method.</param>
		/// <param name="dropdown">The dropdown field which will list the available validation methods.</param>
		private void RefreshMethodDropdown(
			SerializedProperty sourceProp,
			SerializedProperty methodNameProp,
			DropdownField dropdown
		)
		{
			var sourceGO = sourceProp.objectReferenceValue as GameObject;

			// Clear existing choices and disable the dropdown until we populate it
			dropdown.choices.Clear();
			dropdown.SetEnabled(false);
			dropdown.value = string.Empty;

			if (sourceGO != null)
			{
				// 1. Collect all valid bool-returning methods from all MonoBehaviours on the GameObject
				var validMethods = new List<MethodOption>();
				var components = sourceGO.GetComponents<MonoBehaviour>();

				// Create dictionaries to track uniquieness of component types
				// in case there are multiple components of the same type
				// on the GameObject.
				var componentCounts = new Dictionary<Type, int>();
				var componentIndices = new Dictionary<MonoBehaviour, int>();

				// First pass to count components of each type and assign indices
				foreach (MonoBehaviour component in components)
				{
					if (component == null)
						continue;
					Type componentType = component.GetType();

					if (!componentCounts.ContainsKey(componentType))
						componentCounts[componentType] = 0;

					componentIndices[component] = componentCounts[componentType];
					componentCounts[componentType]++;
				}

				// Second pass to collect methods, using the counts and indices to create unique display names
				foreach (MonoBehaviour component in components)
				{
					if (component == null)
						continue;

					Type componentType = component.GetType();
					int typeIndex = componentIndices[component];

					// Determine the unique name to display
					string baseName = componentType.Name;
					string uniqueDisplayName;
					// If there are multiple components of the same type, append an index to differentiate them
					if (componentCounts[componentType] > 1)
						// Use 1-based indexing for user readability: "Foo (1)", "Foo (2)"
						uniqueDisplayName = $"{baseName} ({typeIndex + 1})";
					else
						uniqueDisplayName = baseName;

					// Get all public instance methods that return bool and have no parameters
					var methods = componentType
						.GetMethods(BindingFlags.Public | BindingFlags.Instance)
						.Where(m => m.ReturnType == typeof(bool) && m.GetParameters().Length == 0)
						.Select(m => new MethodOption
						{
							ComponentName = uniqueDisplayName,
							MethodName = m.Name,
							// The full key used for serialization and runtime lookup
							ParseableKey = $"{componentType.FullName}|{typeIndex}|{m.Name}",
						});

					validMethods.AddRange(methods);
				}

				// 2. Prepare the list of choice strings
				// Use the ParseableKey as the choice name, as it's what gets serialized
				var choiceKeys = validMethods.Select(m => m.ParseableKey).ToList();
				string currentMethodKey = methodNameProp.stringValue;

				// 3. Fill the dropdown choices and handle edge cases
				if (choiceKeys.Count == 0)
				{
					// No valid methods found, show a placeholder and disable the dropdown
					dropdown.choices.Add("No public bool methods found on components");
					dropdown.value = dropdown.choices[0];
					methodNameProp.stringValue = string.Empty;
				}
				else
				{
					// Map the paserable key to the user-friendly display name for the dropdown label
					var keyToDisplayName = validMethods.ToDictionary(
						m => m.ParseableKey,
						m => $"{m.ComponentName}.{m.MethodName}"
					);

					// Handle missing/invalid key
					// This can happen if the user had a method selected,
					// then changed the GameObject or modified components
					// such that the previously selected method is no longer valid.
					if (
						!string.IsNullOrEmpty(currentMethodKey) && !choiceKeys.Contains(currentMethodKey)
					)
					{
						choiceKeys.Insert(0, currentMethodKey);
						keyToDisplayName.Add(
							currentMethodKey,
							$"{currentMethodKey} (Missing or Invalid)"
						);
					}

					// If the current method key is empty or invalid,
					// add a placeholder option at the top
					// to prompt the user to select a method,
					// and set the dropdown value to it.
					if (string.IsNullOrEmpty(currentMethodKey) || !choiceKeys.Contains(currentMethodKey))
					{
						const string placeholderKey = "SELECT_METHOD_PLACEHOLDER";
						choiceKeys.Insert(0, placeholderKey);
						keyToDisplayName.Add(placeholderKey, "Select Method...");

						dropdown.value = placeholderKey;
						methodNameProp.stringValue = string.Empty;
					}
					else
					{
						dropdown.value = currentMethodKey;
					}

					// Set the actual choices and provide the display name mapping
					dropdown.choices = choiceKeys;
					dropdown.formatListItemCallback = itemKey =>
						keyToDisplayName.ContainsKey(itemKey) ? keyToDisplayName[itemKey] : itemKey;
					dropdown.formatSelectedValueCallback = itemKey =>
						keyToDisplayName.ContainsKey(itemKey) ? keyToDisplayName[itemKey] : itemKey;
					dropdown.SetEnabled(true);
				}
			}
			else
			{
				// No GameObject selected, show a placeholder that prompts
				// the user to select one, and disable the dropdown
				dropdown.choices.Add("Select GameObject First");
				dropdown.value = dropdown.choices[0];
				methodNameProp.stringValue = string.Empty;
			}

			methodNameProp.serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// Helper struct to store information about valid methods for display and serialization.
		/// </summary>
		private struct MethodOption
		{
			/// <summary>
			/// The unique display name for the component, which may include an index if there are multiple components of the same type.
			/// </summary>
			public string ComponentName;

			/// <summary>
			/// The name of the method itself.
			/// </summary>
			public string MethodName;

			/// <summary>
			/// The full parseable key that will be stored in the serialized property and used for runtime lookup.
			/// Format: "Full.TypeName|Index|MethodName"
			/// </summary>
			public string ParseableKey;

			/// <summary>
			/// The actual MethodInfo for the method, which can be used at runtime to invoke the method.
			/// This is not serialized, but can be looked up using the ParseableKey when needed.
			/// </summary>
			public MethodInfo Info;
		}
	}
}
