namespace NL.XRLab.ToolkitGreybox.GameplayModules
{
	/// <summary>
	///    Defines how a gameplay module should be loaded.
	/// </summary>
	public enum LoadModuleMode
	{
		/// <summary>
		///    Load the module additively, keeping existing scenes loaded.
		/// </summary>
		Additive,

		/// <summary>
		///    Load the module additively but keep it inactive until explicitly activated.
		/// </summary>
		Preload,

		/// <summary>
		///    Unload the current module before loading the new one.
		/// </summary>
		Single
	}
}
