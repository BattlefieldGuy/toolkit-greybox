using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NL.XRLab.Toolkit.Greybox.Director.Loader;
using NL.XRLab.Toolkit.Greybox.GameplayModules;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NL.XRLab.Toolkit.Greybox.Editor.GameplayDirector.GameplayModules.Graph
{
	public class GameplayModuleGraphView : GraphView
	{
		private string _currentModulesHash;
		private bool _needsRefresh;

		public GameplayModuleGraphView()
		{
			// Setup zoom and manipulators
			SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
			this.AddManipulator(new ContentDragger());
			this.AddManipulator(new SelectionDragger());
			this.AddManipulator(new RectangleSelector());

			// Add grid background
			var grid = new GridBackground();
			Insert(0, grid);
			grid.style.flexGrow = 1;

			// Load and display all GameplayModuleData objects
			LoadAllGameplayModules();

			graphViewChanged += OnGraphViewChanged;
		}

		private GraphViewChange OnGraphViewChanged(GraphViewChange change)
		{
			if (change.edgesToCreate != null)
				foreach (Edge edge in change.edgesToCreate)
					if (edge.output.node is GameplayModuleDataNode outputNode &&
					    edge.input.node is GameplayModuleDataNode inputNode)
					{
						Debug.Log($"Attempting to link nodes: {outputNode.title} -> {inputNode.title}");

						GameplayModuleData outputModule = outputNode.AssociatedModule;
						GameplayModuleData inputModule = inputNode.AssociatedModule;

						if (outputModule != null && inputModule != null)
						{
							Debug.Log("Modules are valid. Creating connection.");

							outputModule.ConnectedModules.Add(new GameplayModuleDataEntry
							{
								GameplayModuleData = inputModule,
								LoadModuleMode = LoadModuleMode.Additive
							});

							EditorUtility.SetDirty(outputModule);
						}
						else
						{
							Debug.LogWarning("One or both modules are null. Cannot create connection.");
						}
					}
					else
					{
						Debug.LogWarning("Edge nodes are not of type GameplayModuleDataNode.");
					}

			if (change.elementsToRemove != null)
				foreach (GraphElement element in change.elementsToRemove)
					if (element is Edge edge &&
					    edge.output.node is GameplayModuleDataNode outputNode &&
					    edge.input.node is GameplayModuleDataNode inputNode)
					{
						GameplayModuleData outputModule = outputNode.AssociatedModule;
						GameplayModuleData inputModule = inputNode.AssociatedModule;

						if (outputModule != null && inputModule != null)
						{
							GameplayModuleDataEntry entryToRemove = outputModule.ConnectedModules
								.FirstOrDefault(entry => entry.GameplayModuleData == inputModule);

							if (entryToRemove != null)
							{
								outputModule.ConnectedModules.Remove(entryToRemove);
								EditorUtility.SetDirty(outputModule);
							}
						}
					}

			return change;
		}

		private string CalculateModulesHash()
		{
			string[] guids = AssetDatabase.FindAssets("t:GameplayModuleData");
			guids = guids.OrderBy(g => g).ToArray(); // Ensure consistent order

			using (var sha256 = SHA256.Create())
			{
				string concatenatedGuids = string.Join("", guids);
				byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(concatenatedGuids));
				return Convert.ToBase64String(hashBytes);
			}
		}


		// Corrected access modifier for BuildContextualMenu
		public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			base.BuildContextualMenu(evt);

			// Add "Add Module" action
			evt.menu.AppendAction("Add Module", _ => { OpenCreateModuleDialog(); }, DropdownMenuAction.AlwaysEnabled);

			// Add "Arrange Nodes" action
			evt.menu.AppendAction("Arrange Nodes", _ => { ArrangeNodes(); }, DropdownMenuAction.AlwaysEnabled);
		}


		private bool NeedsRefresh()
		{
			string newHash = CalculateModulesHash();
			Debug.Log("Current Modules Hash: " + _currentModulesHash);
			return _currentModulesHash != newHash;
		}

		public void RefreshGraphIfNeeded()
		{
			if (NeedsRefresh()) RefreshGraphView();
		}

		private void RefreshGraphView()
		{
			Debug.Log("Refreshing Gameplay Module Graph View...");
			ClearGraph();
			LoadAllGameplayModules();
			LinkNodes(); // Link nodes after loading modules

			ArrangeNodes(); // Arrange nodes after loading
		}

		private void LoadAllGameplayModules()
		{
			string[] guids = AssetDatabase.FindAssets("t:GameplayModuleData");
			foreach (string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				var module = AssetDatabase.LoadAssetAtPath<GameplayModuleData>(path);
				if (module != null) AddNode(module);
			}

			_currentModulesHash = CalculateModulesHash();
		}

		private void ClearGraph()
		{
			foreach (GraphElement element in graphElements.ToList()) RemoveElement(element);
		}

		private void AddNode(GameplayModuleData module)
		{
			var node = new GameplayModuleDataNode(module);
			AddElement(node);
		}


		// Reimplemented OpenCreateModuleDialog to handle module creation
		private void OpenCreateModuleDialog()
		{
			string path = EditorUtility.SaveFilePanelInProject("Create Gameplay Module", "NewGameplayModule", "asset",
				"Specify where to save the new Gameplay Module asset.");

			if (!string.IsNullOrEmpty(path))
			{
				var newModule = ScriptableObject.CreateInstance<GameplayModuleData>();
				AssetDatabase.CreateAsset(newModule, path);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();

				AddNode(newModule);
			}
		}

		private void LinkNodes()
		{
			Debug.Log("Linking nodes based on ConnectedModules...");
			// Iterate through all nodes in the graph
			foreach (GraphElement element in graphElements.ToList())
				if (element is GameplayModuleDataNode node)
				{
					GameplayModuleData module = node.AssociatedModule;


					Debug.Log("Linking for module: " + module.name);
					// Iterate through the ConnectedModules of the current module
					foreach (GameplayModuleDataEntry connectedModuleEntry in module.ConnectedModules)
					{
						GameplayModuleData connectedModule = connectedModuleEntry.GameplayModuleData;
						Debug.Log("  Connected to: " + (connectedModule != null ? connectedModule.name : "null"));

						// Find all nodes corresponding to the connected module
						var targetNodes = graphElements.OfType<GameplayModuleDataNode>()
							.Where(n => n.AssociatedModule == connectedModule); // Compare by reference

						foreach (GameplayModuleDataNode targetNode in targetNodes)
						{
							Debug.Log($"Linking '{module.name}' to '{connectedModule.name}'");
							// Create an edge between the current node and the target node
							Edge edge = node.Output.ConnectTo(targetNode.Input);
							AddElement(edge);
						}
					}
				}
		}

		private void ArrangeNodes()
		{
			// Dictionary to track visited nodes and their positions
			var nodePositions = new Dictionary<GameplayModuleDataNode, Vector2>();

			// Function to recursively position nodes
			void PositionNode(GameplayModuleDataNode node, float x, float y)
			{
				if (nodePositions.ContainsKey(node))
					return;

				nodePositions[node] = new Vector2(x, y);

				float childY = y;
				foreach (Edge edge in edges.ToList())
					if (edge.output.node == node && edge.input.node is GameplayModuleDataNode inputNode)
					{
						PositionNode(inputNode, x + 300, childY);
						childY += 150; // Adjust vertical spacing as needed
					}
			}

			// Position root nodes (nodes without inputs) at the far left
			float rootY = 0;
			foreach (GameplayModuleDataNode node in nodes.ToList().OfType<GameplayModuleDataNode>())
				if (!edges.ToList().Any(edge => edge.input.node == node))
				{
					PositionNode(node, 0, rootY);
					rootY += 150; // Adjust vertical spacing as needed
				}

			// Apply positions to nodes
			foreach (var kvp in nodePositions)
			{
				GameplayModuleDataNode node = kvp.Key;
				Vector2 position = kvp.Value;
				node.SetPosition(new Rect(position.x, position.y, 200, 100));
			}
		}

		public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
		{
			return ports.ToList().Where(port =>
				port != startPort &&
				port.node != startPort.node &&
				port.direction != startPort.direction &&
				port.portType == startPort.portType
			).ToList();
		}


		public void ForceRefreshGraph()
		{
			_currentModulesHash = string.Empty;
			RefreshGraphView();
		}
	}
}
