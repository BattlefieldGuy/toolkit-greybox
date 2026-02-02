using System;
using System.Collections.Generic;
using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox.Extensions.Data
{
	[Serializable]
	public class SerializableDictionary<TKey, TValue>
	{
		[SerializeField] private List<SerializableKeyValuePair<TKey, TValue>> _contents;

		private Dictionary<TKey, TValue> _dictionary;

		private Dictionary<TKey, TValue> Dictionary => _dictionary ?? CreateDictionaryFromContents();

		private Dictionary<TKey, TValue> CreateDictionaryFromContents()
		{
			_dictionary = new Dictionary<TKey, TValue>();
			foreach (var entry in _contents) _dictionary[entry.Key] = entry.Value;
			return _dictionary;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return Dictionary.TryGetValue(key, out value);
		}

		public bool ContainsKey(TKey key)
		{
			return Dictionary.ContainsKey(key);
		}
	}
}
