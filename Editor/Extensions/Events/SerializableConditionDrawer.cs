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
	[CustomPropertyDrawer(typeof(SerializableCondition))]
	public class SerializableConditionDrawer : PropertyDrawer
	{
		private const string ConditionSourcePropName = "_conditionSource";
		private const string MethodNamePropName = "_methodName";
		private const string ConditionSourceFieldName = "conditionSourcePropertyField";
		private const string MethodDropdownFieldName = "methodDropdownField";

		// NOTE: This field must be assigned manually in the Inspector for the drawer script asset.
		[SerializeField] private VisualTreeAsset _visualTreeAsset;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var root = new VisualElement();

			if (_visualTreeAsset == null)
			{
				root.Add(new Label("ERROR: The Visual Tree Asset for SerializableConditionDrawer is missing.")
				{
					style = { color = Color.red }
				});
				return root;
			}

			_visualTreeAsset.CloneTree(root);

			SerializedProperty sourceProp = property.FindPropertyRelative(ConditionSourcePropName);
			SerializedProperty methodNameProp = property.FindPropertyRelative(MethodNamePropName);

			var sourceField = root.Q<PropertyField>(ConditionSourceFieldName);
			sourceField.bindingPath = ConditionSourcePropName;

			var methodDropdown = root.Q<DropdownField>(MethodDropdownFieldName);

			// Store the current object reference to check for changes
			Object initialSourceValue = sourceProp.objectReferenceValue;

			RefreshMethodDropdown(sourceProp, methodNameProp, methodDropdown);

			// Set up the change callback for the GameObject field
			sourceField.RegisterCallback<SerializedPropertyChangeEvent>(evt =>
			{
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
				// Save the full PARSEABLE KEY (Full.TypeName|Index|MethodName) to the serialized property
				methodNameProp.stringValue = evt.newValue;
				methodNameProp.serializedObject.ApplyModifiedProperties();
			});

			root.Bind(property.serializedObject);

			return root;
		}

		private void RefreshMethodDropdown(SerializedProperty sourceProp, SerializedProperty methodNameProp,
			DropdownField dropdown)
		{
			var sourceGO = sourceProp.objectReferenceValue as GameObject;

			dropdown.choices.Clear();
			dropdown.SetEnabled(false);
			dropdown.value = string.Empty;

			if (sourceGO != null)
			{
				// 1. Collect all valid bool-returning methods from all MonoBehaviours on the GameObject
				var validMethods = new List<MethodOption>();
				MonoBehaviour[] components = sourceGO.GetComponents<MonoBehaviour>();

				// Dictionaries to track component indices for unique naming and serialization
				var componentCounts = new Dictionary<Type, int>();
				var componentIndices = new Dictionary<MonoBehaviour, int>();

				foreach (MonoBehaviour component in components)
				{
					if (component == null) continue;
					Type componentType = component.GetType();

					if (!componentCounts.ContainsKey(componentType))
						componentCounts[componentType] = 0;

					componentIndices[component] = componentCounts[componentType];
					componentCounts[componentType]++;
				}

				foreach (MonoBehaviour component in components)
				{
					if (component == null) continue;

					Type componentType = component.GetType();
					int typeIndex = componentIndices[component];

					// Determine the unique name to display
					string baseName = componentType.Name;
					string uniqueDisplayName;
					if (componentCounts[componentType] > 1)
						// Use 1-based indexing for user readability: "Foo (1)", "Foo (2)"
						uniqueDisplayName = $"{baseName} ({typeIndex + 1})";
					else
						uniqueDisplayName = baseName;

					IEnumerable<MethodOption> methods = componentType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
						.Where(m => m.ReturnType == typeof(bool) && m.GetParameters().Length == 0)
						.Select(m => new MethodOption
						{
							ComponentName = uniqueDisplayName,
							MethodName = m.Name,
							// The full key used for serialization and runtime lookup
							ParseableKey = $"{componentType.FullName}|{typeIndex}|{m.Name}"
						});

					validMethods.AddRange(methods);
				}

				// 2. Prepare the list of choice strings
				// Use the ParseableKey as the choice name, as it's what gets serialized
				List<string> choiceKeys = validMethods.Select(m => m.ParseableKey).ToList();
				string currentMethodKey = methodNameProp.stringValue;

				// 3. Handle placeholders and current value

				if (choiceKeys.Count == 0)
				{
					dropdown.choices.Add("No public bool methods found on components");
					dropdown.value = dropdown.choices[0];
					methodNameProp.stringValue = string.Empty;
				}
				else
				{
					// Map the unique key to the user-friendly display name for the dropdown label
					Dictionary<string, string> keyToDisplayName = validMethods.ToDictionary(
						m => m.ParseableKey,
						m => $"{m.ComponentName}.{m.MethodName}"
					);

					// Handle missing/invalid key
					if (!string.IsNullOrEmpty(currentMethodKey) && !choiceKeys.Contains(currentMethodKey))
					{
						choiceKeys.Insert(0, currentMethodKey);
						keyToDisplayName.Add(currentMethodKey, $"{currentMethodKey} (Missing or Invalid)");
					}

					// Handle "Select Method" placeholder
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
				dropdown.choices.Add("Select GameObject First");
				dropdown.value = dropdown.choices[0];
				methodNameProp.stringValue = string.Empty;
			}

			methodNameProp.serializedObject.ApplyModifiedProperties();
		}

		// Helper structure to hold method info needed for the dropdown
		private struct MethodOption
		{
			public string ComponentName;
			public string MethodName;
			public string ParseableKey;
			public MethodInfo Info;
		}
	}
}
