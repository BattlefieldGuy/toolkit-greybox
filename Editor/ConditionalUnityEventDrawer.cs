using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NL.XRLab.Toolkit.Greybox.GameplayModules;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NL.XRLab.Toolkit.Greybox.Editor
{
	[CustomPropertyDrawer(typeof(ConditionalUnityEvent))]
	public class ConditionalUnityEventDrawer : PropertyDrawer
	{
		private DropdownField _componentDropdown;
		private List<Component> _componentsOnTarget = new();

		private GameObject _conditionTarget;

		private ObjectField _conditionTargetField;
		private DropdownField _methodDropdown;
		private Component _selectedComponent;
		[SerializeField] private VisualTreeAsset _visualTreeAsset;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var root = new VisualElement();

			if (_visualTreeAsset != null)
				_visualTreeAsset.CloneTree(root);

			// Query fields from UXML
			_conditionTargetField = root.Q<ObjectField>("conditionTargetField");
			_componentDropdown = root.Q<DropdownField>("componentDropdown");
			_methodDropdown = root.Q<DropdownField>("methodDropdown");

			// --- GameObject Field ---
			_conditionTargetField.objectType = typeof(GameObject);
			_conditionTargetField.allowSceneObjects = true;
			_conditionTargetField.value =
				property.FindPropertyRelative("_conditionTarget").objectReferenceValue as GameObject;
			_conditionTargetField.RegisterValueChangedCallback(evt =>
			{
				_conditionTarget = evt.newValue as GameObject;
				property.FindPropertyRelative("_conditionTarget").objectReferenceValue = _conditionTarget;
				property.serializedObject.ApplyModifiedProperties();

				// Reset Component and Method fields on target change
				_selectedComponent = null;
				_componentsOnTarget.Clear();
				_componentDropdown.choices = new List<string>();
				_componentDropdown.value = null;
				_methodDropdown.choices = new List<string>();
				_methodDropdown.value = null;

				UpdateComponentDropdown(property);
			});

			// --- Component Dropdown ---
			_componentDropdown.RegisterValueChangedCallback(evt =>
			{
				if (_componentsOnTarget.Count == 0) return;

				var selectedIndex = _componentDropdown.choices.IndexOf(evt.newValue);
				if (selectedIndex >= 0 && selectedIndex < _componentsOnTarget.Count)
				{
					_selectedComponent = _componentsOnTarget[selectedIndex];

					// Reset method field on component change
					_methodDropdown.choices = new List<string>();
					_methodDropdown.value = null;

					UpdateMethodDropdown(property);
				}
			});

			// --- Method Dropdown ---
			_methodDropdown.RegisterValueChangedCallback(evt =>
			{
				property.FindPropertyRelative("_methodName").stringValue = evt.newValue;
				property.serializedObject.ApplyModifiedProperties();
			});

			// Initialize dropdowns
			_conditionTarget = (GameObject)_conditionTargetField.value;
			UpdateComponentDropdown(property);

			return root;
		}

		private void UpdateComponentDropdown(SerializedProperty property)
		{
			_componentsOnTarget.Clear();
			_componentDropdown.choices = new List<string>();
			_methodDropdown.choices = new List<string>();

			if (_conditionTarget == null) return;

			_componentsOnTarget = _conditionTarget.GetComponents<Component>().ToList();

			// Build smart names: only add (index) if there are duplicates
			Dictionary<string, List<Component>> typeGroups = _componentsOnTarget
				.GroupBy(c => c.GetType().Name)
				.ToDictionary(g => g.Key, g => g.ToList());

			var componentNames = new List<string>();
			foreach (KeyValuePair<string, List<Component>> group in typeGroups)
				if (group.Value.Count == 1)
					componentNames.Add(group.Key);
				else
					// Add index only for duplicates
					for (var i = 0; i < group.Value.Count; i++)
						componentNames.Add($"{group.Key} ({i + 1})");

			_componentDropdown.choices = componentNames;

			// Auto-select first component if available
			if (_componentsOnTarget.Count > 0)
			{
				_componentDropdown.value = componentNames[0];
				_selectedComponent = _componentsOnTarget[0];
				UpdateMethodDropdown(property);
			}
		}

		private void UpdateMethodDropdown(SerializedProperty property)
		{
			_methodDropdown.choices = new List<string>();
			if (_selectedComponent == null) return;

			List<string> methodNames = _selectedComponent.GetType()
				.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				.Where(m => m.ReturnType == typeof(bool) && m.GetParameters().Length == 0)
				.Select(m => m.Name)
				.ToList();

			_methodDropdown.choices = methodNames;

			if (methodNames.Count > 0) _methodDropdown.value = methodNames[0];
		}
	}
}
