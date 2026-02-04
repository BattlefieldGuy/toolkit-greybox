using System.Linq;
using NL.XRLab.Toolkit.Greybox.GameplayModules;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Logger = NL.XRLab.Toolkit.Greybox.Utils.Logger;

namespace NL.XRLab.Toolkit.Greybox.Editor
{
	[CustomEditor(typeof(Director.GameplayDirector))]
	public class GameplayDirectorEditor : UnityEditor.Editor
	{
		[SerializeField] private VisualTreeAsset _visualTreeAsset;

		private Director.GameplayDirector _director;

		private void OnEnable()
		{
			_director = (Director.GameplayDirector)target;
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
			var modules = _director.GameplayModules.ToArray();

			var scenesInBuildSettings = EditorBuildSettings.scenes.ToList();
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
