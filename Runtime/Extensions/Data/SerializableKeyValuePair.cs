using System;

namespace NL.XRLab.Toolkit.Greybox.Extensions.Data
{
	[Serializable]
	public class SerializableKeyValuePair<TKey, TValue>
	{
		public TKey Key;
		public TValue Value;
	}
}
