using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NL.XRLab.Toolkit.Greybox.GameplayModules
{
	/// <summary>
	///    ScriptableObject representing a gameplay module in the game.
	///    Contains data about the module's corresponding scene and its connections to other modules.
	/// </summary>
	[CreateAssetMenu(
		fileName = "new Gameplay Module Data",
		menuName = "XR-Lab/Gameplay Modules/Gameplay Module Data"
	)]
	public class GameplayModuleData : ScriptableObject
	{
		/// <summary>
		///    List of modules that are connected to this module.
		///    (e.g., loaded upon completion of this module)
		/// </summary>
		[Tooltip(
			"List of modules that are connected to this module. (e.g., loaded upon completion of this module)"
		)]
		public List<GameplayModuleDataEntry> ConnectedModules = new();

		/// <summary>
		///    Whether to unload this module's scene upon completion.
		/// </summary>
		public bool UnloadSceneOnCompletion = true;

		/// <summary>
		///    Serialized runtime scene path. This is safe to use at runtime (does not depend on UnityEditor).
		/// </summary>
		[Tooltip("Path to the scene asset (used at runtime).")]
		private string _scenePath = string.Empty;

		/// <summary>
		///    Public property for the scene path (runtime-friendly).
		/// </summary>
		public string ScenePath => _scenePath;

		/// <summary>
		///    Public helper that returns the scene name (filename without extension) derived from ScenePath.
		///    Use this to call SceneManager.LoadScene/LoadSceneAsync at runtime. Note: the scene must be
		///    included in Build Settings or loaded via Addressables/AssetReference for runtime loading to work.
		/// </summary>
		public string SceneName =>
			string.IsNullOrEmpty(_scenePath) ? string.Empty : Path.GetFileNameWithoutExtension(_scenePath);


#if UNITY_EDITOR
		/// <summary>
		///    Reference to the scene asset that corresponds to this module.
		///    This field is only available in the editor and is used by the GameplayModuleDataEditor to set the scene path.
		/// </summary>
		[Tooltip("Reference to the scene that corresponds to this module.")] [SerializeField]
		private SceneAsset _sceneAsset;

		/// <summary>
		///    Public getter for the SceneAsset,
		///    which is only available in the editor and is used by the GameplayModuleDataEditor to set the scene path.
		/// </summary>
		public SceneAsset SceneAsset => _sceneAsset;

		/// <summary>
		///    Ensure the serialized _scenePath is updated whenever the SceneAsset is changed in the inspector.
		///    This keeps the runtime ScenePath field in sync and avoids any dependency on UnityEditor at runtime.
		/// </summary>
		private void OnValidate()
		{
			string path = _sceneAsset != null ? AssetDatabase.GetAssetPath(_sceneAsset) : string.Empty;
			if (_scenePath != path)
			{
				_scenePath = path;
				EditorUtility.SetDirty(this);
			}
		}
#endif
	}
}
