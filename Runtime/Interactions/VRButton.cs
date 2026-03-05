using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class VRButton : MonoBehaviour
{
   /// <summary>
   /// Simple button script for the greyboxing toolkit.
   /// 
   /// Drag the necessary events into the inspector and make sure to use the "Player" prefab or add coliders and Rigidbody's to the controllers with the "Player" tag for the button to work.
   /// </summary>

   public UnityEvent OnPress;

   private bool isPressed = false;

   private void OnTriggerEnter(Collider other)
   {
	  if (!isPressed)
	  {
		 if (other.CompareTag("Player"))
		 {
			isPressed = true;
			StartCoroutine(Animation());
			OnPress.Invoke();
		 }
	  }
   }

   private IEnumerator Animation()
   {
	  this.transform.localPosition = new Vector3(0.0f, 0.45f, 0.0f);
	  yield return new WaitForSeconds(0.5f);
	  this.transform.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
	  isPressed = false;
   }

   #region --- DEBUG ---
   public void OnTrigger()
   {
	  print("Button was Pressed");
   }
   #endregion
}
