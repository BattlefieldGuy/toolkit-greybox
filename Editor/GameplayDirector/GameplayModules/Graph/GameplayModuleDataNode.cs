using NL.XRLab.Toolkit.Greybox.GameplayModules;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

// Added namespace for UI Toolkit

namespace NL.XRLab.Toolkit.Greybox.Editor.GameplayDirector.GameplayModules.Graph
{
	public class GameplayModuleDataNode : Node
	{
		public GameplayModuleDataNode(GameplayModuleData module)
		{
			AssociatedModule = module; // Store the associated module
			Initialize(module);

			// Add a callback to focus the asset when the node is selected
			RegisterCallback<MouseDownEvent>(_ => FocusAssetInProjectBrowser());
		}

		// Added a property to store the associated GameplayModuleData
		public GameplayModuleData AssociatedModule { get; }

		public Port Output { get; private set; }
		public Port Input { get; private set; }

		private void Initialize(GameplayModuleData module)
		{
			title = module.name;
			SetPosition(new Rect(Vector2.zero, new Vector2(200, 100)));

			// Create input and output ports with explicit portType
			Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi,
				typeof(GameplayModuleData));
			Input.portName = "Input";
			Input.portType = typeof(GameplayModuleData); // Explicitly set portType
			inputContainer.Add(Input);

			Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi,
				typeof(GameplayModuleData));
			Output.portName = "Output";
			Output.portType = typeof(GameplayModuleData); // Explicitly set portType
			outputContainer.Add(Output);

			// Add a toggle for UnloadSceneOnCompletion
			var unloadSceneToggle = new Toggle("Unload Scene On Completion")
			{
				value = module.UnloadSceneOnCompletion
			};
			unloadSceneToggle.RegisterValueChangedCallback(evt => { module.UnloadSceneOnCompletion = evt.newValue; });
			extensionContainer.Add(unloadSceneToggle);

			// Add a PropertyField for _sceneAsset
			var serializedObject = new SerializedObject(module);
			SerializedProperty sceneAssetProperty = serializedObject.FindProperty("_sceneAsset");
			var sceneAssetField = new PropertyField(sceneAssetProperty, "Scene Asset");
			sceneAssetField.Bind(serializedObject);
			extensionContainer.Add(sceneAssetField);

			RefreshExpandedState();
			RefreshPorts(); // Ensure ports are refreshed and recognized
		}

		private void FocusAssetInProjectBrowser()
		{
			if (AssociatedModule != null)
			{
				string assetPath = AssetDatabase.GetAssetPath(AssociatedModule);
				if (!string.IsNullOrEmpty(assetPath))
				{
					var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
					EditorGUIUtility.PingObject(asset);

					// Open the asset in the Inspector
					Selection.activeObject = asset;
				}
			}
		}

		// Override BuildContextualMenu to customize the context menu for the node
		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			base.BuildContextualMenu(evt);

			// Remove the default delete action
			evt.menu.MenuItems().RemoveAll(item => item is DropdownMenuAction action && action.name == "Delete");

			// Add a custom delete action with confirmation
			evt.menu.AppendAction("Delete", _ =>
			{
				if (EditorUtility.DisplayDialog(
					    "Delete Node and Asset",
					    "Are you sure you want to delete this node? This will also delete the associated GameplayModuleData asset.",
					    "Delete",
					    "Cancel"))
				{
					if (AssociatedModule != null)
					{
						string assetPath = AssetDatabase.GetAssetPath(AssociatedModule);
						if (!string.IsNullOrEmpty(assetPath))
						{
							AssetDatabase.DeleteAsset(assetPath);
							AssetDatabase.SaveAssets();
						}
					}

					// Remove this node from the graph
					RemoveFromHierarchy();
				}
			}, DropdownMenuAction.AlwaysEnabled);
		}
	}
}
