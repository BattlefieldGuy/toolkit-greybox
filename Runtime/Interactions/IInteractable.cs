namespace NL.XRLab.Toolkit.Greybox.Interactions
{
	/// <summary>
	/// Interface for interactable objects in the Greybox toolkit.
	/// </summary>
	public interface IInteractable
	{
		/// <summary>
		/// Defines the interaction behavior for the object. When an object that implements this interface is interacted with,
		/// this method will be called to perform the appropriate action, such as opening a door, pressing a button, or triggering an event.
		/// </summary>
		void Interact();
	}
}
