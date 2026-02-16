using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Gameplay_Modules
{
	/// <summary>
	/// A ScriptableObject that serves as an identifier for gameplay events.
	/// By creating instances of this ScriptableObject,
	/// we can easily manage and reference different gameplay events throughout our codebase and Unity Inspector.
	/// </summary>
	[CreateAssetMenu(
		fileName = "Gameplay Event Identifier",
		menuName = "XR-Lab/Gameplay Modules/Gameplay Event Identifier"
	)]
	public class GameplayEventIdentifier : ScriptableObject { }
}
