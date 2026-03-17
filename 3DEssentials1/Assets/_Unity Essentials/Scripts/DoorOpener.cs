using UnityEngine;


public class DoorOpener : MonoBehaviour
{
   private Animator doorAnimator;


   void Start()
   {
       // Get the Animator component attached to the same GameObject as this script
       doorAnimator = GetComponent<Animator>();
   }


private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetBool("IsOpen", true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (doorAnimator != null)
            {
                doorAnimator.SetBool("IsOpen", false);
            }
        }
    }
}