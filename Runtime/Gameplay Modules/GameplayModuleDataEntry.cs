using System;
using NL.XRLab.Toolkit.Greybox.Director.Loader;

namespace NL.XRLab.Toolkit.Greybox.GameplayModules
{
	/// <summary>
	/// A serializable class that represents an entry for a gameplay module in the director's loading system.
	/// Used to specify how a gameplay module should be loaded, including the module data and the loading mode (e.g., additive, single, etc.).
	/// </summary>
	[Serializable]
	public class GameplayModuleDataEntry
	{
		/// <summary>
		/// Reference to the GameplayModuleData that contains information about the module's scene and connections.
		/// </summary>
		public GameplayModuleData GameplayModuleData;

		/// <summary>
		/// Specifies the mode in which the module's scene should be loaded (e.g., additive or preload).
		/// </summary>
		public LoadModuleMode LoadModuleMode;
	}
}
