using System;

namespace NL.XRLab.Toolkit.Greybox.Extensions.Data
{
	/// <summary>
	/// A serializable key-value pair that can be used in Unity's Inspector.
	/// This is used as the building block for the SerializableDictionary, allowing us to store key-value pairs in a list that Unity can serialize.
	/// </summary>
	/// <typeparam name="TKey">The type of the key in the key-value pair. Must be serializable.</typeparam>
	/// <typeparam name="TValue">The type of the value in the key-value pair. Must be serializable.</typeparam>
	[Serializable]
	public class SerializableKeyValuePair<TKey, TValue>
	{
		/// <summary>
		/// The key of the pair.
		/// </summary>
		public TKey Key;

		/// <summary>
		/// The value of the pair.
		/// </summary>
		public TValue Value;
	}
}
