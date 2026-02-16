using NL.XRLab.Toolkit.Greybox.GameplayModules;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NL.XRLab.Toolkit.Greybox.Editor.GameplayDirector.GameplayModules.Graph
{
	/// <summary>
	/// A custom node for the GameplayModuleData in the Gameplay Modules Graph.
	/// It displays the module's name, allows editing of its properties, and provides input/output ports for connections with other nodes.
	/// </summary>
	public class GameplayModuleDataNode : Node
	{
		/// <summary>
		/// Constructor for the GameplayModuleDataNode.
		/// It initializes the node from the given GameplayModule Data and sets up the UI elements and ports.
		/// </summary>
		public GameplayModuleDataNode(GameplayModuleData module)
		{
			AssociatedModule = module; // Store the associated module
			Initialize(module);

			// Add a callback to focus the asset when the node is selected
			RegisterCallback<MouseDownEvent>(_ => FocusAssetInProjectBrowser());
		}

		/// <summary>
		/// The module associated with this node.
		/// </summary>
		public GameplayModuleData AssociatedModule { get; }

		/// <summary>
		/// The output port of the node, which can be connected to other nodes in the graph. It is of type GameplayModuleData to ensure type safety in connections.
		/// </summary>
		public Port Output { get; private set; }

		/// <summary>
		/// The input port of the node, which can receive connections from other nodes in the graph. It is of type GameplayModuleData to ensure type safety in connections.
		/// </summary>
		public Port Input { get; private set; }

		/// <summary>
		/// Initializes the node's UI elements, including the title, input/output ports, and property fields for editing the module's properties.
		/// </summary>
		/// <param name="module">The module to initialize</param>
		private void Initialize(GameplayModuleData module)
		{
			title = module.name;
			SetPosition(new Rect(Vector2.zero, new Vector2(200, 100)));

			// Create input and output ports with explicit portType
			Input = InstantiatePort(
				Orientation.Vertical,
				Direction.Input,
				Port.Capacity.Multi,
				typeof(GameplayModuleData)
			);
			Input.portName = "Input";
			Input.portType = typeof(GameplayModuleData);
			inputContainer.Add(Input);

			Output = InstantiatePort(
				Orientation.Vertical,
				Direction.Output,
				Port.Capacity.Multi,
				typeof(GameplayModuleData)
			);
			Output.portName = "Output";
			Output.portType = typeof(GameplayModuleData);
			outputContainer.Add(Output);

			// Add a toggle for UnloadSceneOnCompletion
			var unloadSceneToggle = new Toggle("Unload Scene On Completion")
			{
				value = module.UnloadSceneOnCompletion,
			};
			unloadSceneToggle.RegisterValueChangedCallback(evt =>
			{
				module.UnloadSceneOnCompletion = evt.newValue;
			});
			extensionContainer.Add(unloadSceneToggle);

			// Add a PropertyField for _sceneAsset
			var serializedObject = new SerializedObject(module);
			SerializedProperty sceneAssetProperty = serializedObject.FindProperty("_sceneAsset");
			var sceneAssetField = new PropertyField(sceneAssetProperty, "Scene Asset");
			sceneAssetField.Bind(serializedObject);
			extensionContainer.Add(sceneAssetField);

			RefreshExpandedState();
			RefreshPorts();
		}

		/// <summary>
		/// Focuses the associated GameplayModuleData asset in the Unity Project Browser when the node is selected.
		/// </summary>
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

		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			base.BuildContextualMenu(evt);

			// Remove the default delete action
			evt.menu.MenuItems()
				.RemoveAll(item => item is DropdownMenuAction action && action.name == "Delete");

			// Add a custom delete action with confirmation
			evt.menu.AppendAction(
				"Delete",
				_ =>
				{
					if (
						EditorUtility.DisplayDialog(
							"Delete Node and Asset",
							"Are you sure you want to delete this node? This will also delete the associated GameplayModuleData asset.",
							"Delete",
							"Cancel"
						)
					)
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
				},
				DropdownMenuAction.AlwaysEnabled
			);
		}
	}
}
