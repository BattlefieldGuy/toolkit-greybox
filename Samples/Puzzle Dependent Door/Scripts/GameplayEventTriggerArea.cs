using NL.XRLab.Toolkit.Greybox.Gameplay_Modules;
using NL.XRLab.Toolkit.Greybox.GameplayModules;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Samples.Puzzle_Dependent_Door.Scripts
{
	/// <summary>
	/// A component that can be attached to a trigger area in the scene.
	/// When a collider enters the trigger area, it will attempt to invoke a specified gameplay event using the GameplayModule.
	/// </summary>
	public class GameplayEventTriggerArea : MonoBehaviour
	{
		/// <summary>
		/// Reference to the GameplayModule that will be used to invoke the gameplay event when a collider enters the trigger area.
		/// </summary>
		[SerializeField]
		private GameplayModule _gameplayModule;

		/// <summary>
		/// The GameplayEventIdentifier that will be used to invoke a GameplayEvent when a collider enters the trigger area.
		/// </summary>
		[SerializeField]
		private GameplayEventIdentifier _eventToTryInvoke;

		/// <summary>
		/// Reference to the BoxCollider component that defines the trigger area. This collider should be set as a trigger in the Unity Inspector.
		/// </summary>
		private BoxCollider _finishAreaCollider;

		private void Awake()
		{
			_finishAreaCollider = GetComponent<BoxCollider>();
		}

		private void OnTriggerEnter(Collider other)
		{
			Debug.Log("Trigger enter");
			_gameplayModule.TryInvokeGameplayEvent(_eventToTryInvoke);
		}
	}
}
