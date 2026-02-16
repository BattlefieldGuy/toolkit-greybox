using System;
using System.Collections.Generic;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Extensions.Data
{
	/// <summary>
	/// A serializable dictionary that can be used in Unity's Inspector.
	/// It uses a list of key-value pairs to store the data, and provides methods to access the data as a regular dictionary.
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary. Must be serializable.</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary. Must be serializable.</typeparam>
	[Serializable]
	public class SerializableDictionary<TKey, TValue>
	{
		/// <summary>
		/// The serializable list of key-value pairs that represents the contents of the dictionary.
		/// This is what gets turned into a dictionary at runtime. It is serialized by Unity and can be edited in the Inspector.
		/// </summary>
		[SerializeField]
		private List<SerializableKeyValuePair<TKey, TValue>> _contents;

		/// <summary>
		/// Backing field for the lazily initialized dictionary. This is not serialized and is created from the contents when needed.
		/// </summary>
		private Dictionary<TKey, TValue> _dictionary;

		/// <summary>
		/// The dictionary that provides access to the key-value pairs. It is created lazily from the contents when first accessed.
		/// </summary>
		private Dictionary<TKey, TValue> Dictionary => _dictionary ?? CreateDictionaryFromContents();

		/// <summary>
		/// Creates a dictionary from the contents of the list of key-value pairs.
		/// This is done lazily.
		/// </summary>
		/// <returns>The resulting Dictionary.</returns>
		private Dictionary<TKey, TValue> CreateDictionaryFromContents()
		{
			_dictionary = new Dictionary<TKey, TValue>();
			foreach (var entry in _contents)
				_dictionary[entry.Key] = entry.Value;
			return _dictionary;
		}

		/// <summary>
		/// Mimics the TryGetValue method of a regular dictionary.
		/// </summary>
		/// <param name="key">The key to get.</param>
		/// <param name="TValue">The value associated with the key to retrieve.</param>
		/// <returns>Whether the dictionary has the value.</returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return Dictionary.TryGetValue(key, out value);
		}

		/// <summary>
		/// Mimics the ContainsKey method of a regular dictionary.
		/// </summary>
		/// <param name="key">The key to check for.</param>
		/// <returns>Whether the dictionary contains the key.</returns>
		public bool ContainsKey(TKey key)
		{
			return Dictionary.ContainsKey(key);
		}
	}
}
