using System.Collections.Generic;
using System.Linq;
using NL.XRLab.ToolkitGreybox.Director;
using NL.XRLab.ToolkitGreybox.GameplayModules;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Logger = NL.XRLab.ToolkitGreybox.Utils.Logger;

namespace NL.XRLab.Toolkit.Greybox.Editor
{
	[CustomEditor(typeof(Director))]
	public class DirectorEditor : UnityEditor.Editor
	{
		[SerializeField] private VisualTreeAsset _visualTreeAsset;

		private Director _director;

		private void OnEnable()
		{
			_director = (Director)target;
		}

		public override VisualElement CreateInspectorGUI()
		{
			var root = new VisualElement();
			_visualTreeAsset.CloneTree(root);

			var syncBuildSettingsButton = root.Q<Button>("syncBuildSettingsButton");
			syncBuildSettingsButton.RegisterCallback<ClickEvent>(SyncBuildSettings);
			return root;
		}

		private void SyncBuildSettings(ClickEvent e)
		{
			Logger.Log("Syncing build settings with scenes from gameplay modules...");
			GameplayModuleData[] modules = _director.GameplayModules.ToArray();

			List<EditorBuildSettingsScene> scenesInBuildSettings = EditorBuildSettings.scenes.ToList();
			string[] scenePathsInBuildSettings = scenesInBuildSettings.Select(scene => scene.path).ToArray();

			foreach (GameplayModuleData module in modules)
			{
				if (module.SceneAsset == null)
				{
					Logger.Log($"Skipping module '{module.name}' because it has no SceneAsset assigned.");
					continue;
				}

				if (scenePathsInBuildSettings.Contains(module.ScenePath))
					continue;
				Logger.Log($"Adding scene '{module.ScenePath}' from module '{module.name}' to build settings.");
				var newScene = new EditorBuildSettingsScene(module.ScenePath, true);
				scenesInBuildSettings.Add(newScene);
			}

			EditorBuildSettings.scenes = scenesInBuildSettings.ToArray();
		}
	}
}
