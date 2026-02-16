using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
		///    Public property for the scene path, derived from the SceneAsset.
		/// </summary>
		public string ScenePath =>
			_sceneAsset != null ? AssetDatabase.GetAssetPath(_sceneAsset) : string.Empty;

#if UNITY_EDITOR
		/// <summary>
		///    Reference to the scene asset that corresponds to this module.
		///    This field is only available in the editor and is used by the GameplayModuleDataEditor to set the scene path.
		/// </summary>
		[Tooltip("Reference to the scene that corresponds to this module.")]
		[SerializeField]
		private SceneAsset _sceneAsset;

		/// <summary>
		/// Public getter for the SceneAsset,
		/// which is only available in the editor and is used by the GameplayModuleDataEditor to set the scene path.
		/// </summary>
		public SceneAsset SceneAsset => _sceneAsset;
#endif
	}
}
