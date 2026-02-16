using System.Linq;
using NL.XRLab.Toolkit.Greybox.GameplayModules;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Logger = NL.XRLab.Toolkit.Greybox.Utils.Logger;

namespace NL.XRLab.Toolkit.Greybox.Editor
{
	/// <summary>
	/// Custom editor for the GameplayDirector class. It provides a button to sync the build settings with the scenes from the gameplay modules.
	/// </summary>
	[CustomEditor(typeof(Director.GameplayDirector))]
	public class GameplayDirectorEditor : UnityEditor.Editor
	{
		/// <summary>
		/// The VisualTreeAsset that defines the UI layout for the custom inspector.
		/// </summary>
		[SerializeField]
		private VisualTreeAsset _visualTreeAsset;

		/// <summary>
		/// Reference to the GameplayDirector being edited.
		/// </summary>
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

		/// <summary>
		/// Syncs the build settings with the scenes from the gameplay modules. It adds any scenes that are not already in the build settings.
		/// </summary>
		private void SyncBuildSettings(ClickEvent e)
		{
			Logger.Log("Syncing build settings with scenes from gameplay modules...");
			var modules = _director.GameplayModules.ToArray();

			var scenesInBuildSettings = EditorBuildSettings.scenes.ToList();
			string[] scenePathsInBuildSettings = scenesInBuildSettings
				.Select(scene => scene.path)
				.ToArray();

			foreach (GameplayModuleData module in modules)
			{
				if (module.SceneAsset == null)
				{
					Logger.Log(
						$"Skipping module '{module.name}' because it has no SceneAsset assigned."
					);
					continue;
				}

				if (scenePathsInBuildSettings.Contains(module.ScenePath))
					continue;
				Logger.Log(
					$"Adding scene '{module.ScenePath}' from module '{module.name}' to build settings."
				);
				var newScene = new EditorBuildSettingsScene(module.ScenePath, true);
				scenesInBuildSettings.Add(newScene);
			}

			EditorBuildSettings.scenes = scenesInBuildSettings.ToArray();
		}
	}
}
