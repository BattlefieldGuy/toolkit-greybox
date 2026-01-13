namespace NL.XRLab.Toolkit.Greybox.Director.Loader
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
		Preload
	}
}
