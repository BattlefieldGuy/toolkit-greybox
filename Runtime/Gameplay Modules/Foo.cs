using UnityEngine;

namespace NL.XRLab.Toolkit.Greybox
{
	public class Foo : MonoBehaviour
	{
		[SerializeField] private bool _myBoolProp;

		public void PrintHello()
		{
			Debug.Log("Hello");
		}

		public bool MyTrueCondition()
		{
			return true;
		}

		public bool MyFalseCondition()
		{
			return false;
		}

		public bool MyBoolPropCondition()
		{
			return _myBoolProp;
		}
	}
}
