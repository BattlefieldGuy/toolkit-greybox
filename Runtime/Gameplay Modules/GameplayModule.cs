using NL.XRLab.Toolkit.Greybox.Gameplay_Modules;
using UnityEngine;
using UnityEngine.Events;
using Logger = NL.XRLab.Toolkit.Greybox.Utils.Logger;

namespace NL.XRLab.Toolkit.Greybox.GameplayModules
{
	/// <summary>
	///   Represents a gameplay module in the game. Each module has its own sequence of events and can be completed independently.
	/// </summary>
	public class GameplayModule : MonoBehaviour
	{
		/// <summary>
		/// The data asset that defines the properties and configuration of this gameplay module.
		/// It should be assigned in the Unity Inspector and contains information such as the module's name, description, and the
		/// relevant scene for this module.
		/// </summary>
		[SerializeField]
		public GameplayModuleData GameplayModuleData;

		/// <summary>
		/// The GameplaySequenceMapped which defines the gameplay events by mapping GameplayEventIdentifiers to UnityEvents.
		/// </summary>
		public GameplaySequenceMapped gameplaySequenceMapped;

		/// <summary>
		/// Event triggered when this gameplay module is completed. It passes a reference to the completed GameplayModule as a parameter.
		/// </summary>
		public UnityEvent<GameplayModule> OnModuleCompleted = new();

		/// <summary>
		/// Determines whether the module should be automatically disabled (set inactive) when it is completed.
		/// This can be useful for modules that should no longer be active or interactable after completion,
		/// such as a tutorial module that should disappear once the player has finished it.
		/// If set to true, the GameObject this script is attached to will be deactivated when CompleteModule() is called.
		/// </summary>
		[SerializeField]
		private bool _disableModuleOnComplete = true;

		/// <summary>
		///   Attempts to invoke a gameplay event based on the provided GameplayEventIdentifier.
		///   If the module is not active or enabled, the method will return early and not attempt to invoke the event.
		/// </summary>
		/// <param name="eventIdentifier">The identifier that is associated with this GameplayEvent.</param>
		public void TryInvokeGameplayEvent(GameplayEventIdentifier eventIdentifier)
		{
			if (!isActiveAndEnabled)
				return;
			gameplaySequenceMapped.TryInvokeEvent(eventIdentifier);
		}

		/// <summary>
		///  Marks this gameplay module as completed. This method should be called when the player has fulfilled the objectives of the module.
		/// </summary>
		public void CompleteModule()
		{
			Logger.Log("GameplayModule completed: " + GameplayModuleData.name);
			OnModuleCompleted?.Invoke(this);
			if (_disableModuleOnComplete)
				gameObject.SetActive(false);
		}
	}
}
