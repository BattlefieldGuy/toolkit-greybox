using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.GameplayModules
{
	/// <summary>
	///    ScriptableObject representing a gameplay module in the game.
	///    Contains data about the module's corresponding scene and its connections to other modules.
	/// </summary>
	[CreateAssetMenu(fileName = "new Gameplay Module Data", menuName = "Greybox/Gameplay Module Data")]
	public class GameplayModuleData : ScriptableObject
	{
		/// <summary>
		///    The name of the scene associated with this gameplay module.
		///    Used internally to load the correct scene.
		/// </summary>
		[SerializeField] private string _scenePath;

		/// <summary>
		///    List of modules that are connected to this module.
		///    (e.g., loaded upon completion of this module)
		/// </summary>
		[Tooltip("List of modules that are connected to this module. (e.g., loaded upon completion of this module)")]
		public List<GameplayModuleDataEntry> ConnectedModules;

		/// <summary>
		///    Public readonly getter for the scene path.
		/// </summary>
		public string ScenePath => _scenePath;

#if UNITY_EDITOR
		/// <summary>
		///    Reference to the scene asset that corresponds to this module.
		///    This field is only available in the editor and is used by the GameplayModuleDataEditor to set the scene path.
		/// </summary>
		[Tooltip("Reference to the scene that corresponds to this module.")] [SerializeField]
		private SceneAsset _sceneAsset;

		/// <summary>
		///    Public readonly getter for the scene asset that corresponds to this module.
		/// </summary>
		public SceneAsset SceneAsset => _sceneAsset;
#endif
	}
}
