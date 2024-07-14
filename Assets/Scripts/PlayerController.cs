using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("Animator component not found!");
        }
        else
        {
            animator.SetTrigger("Run");
            Debug.Log("Running animation set.");
        }
    }

    public IEnumerator jumpPlayer(Animator playerAnimator)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Jump");

            float animationLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;
            
            yield return new WaitForSeconds(animationLength);
            playerAnimator.ResetTrigger("Jump");
        }
        else
        {
            Debug.LogError("Animator component is missing or not set!");
        }
    }
    
    public IEnumerator slidePlayer(Animator playerAnimator)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Slide");
            
            float animationLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;
           
            yield return new WaitForSeconds(animationLength);
            playerAnimator.ResetTrigger("Slide");
            playerAnimator.SetTrigger("Run");
        }
        else
        {
            Debug.LogError("Animator component is missing or not set!");
        }
    }

    public IEnumerator RollAfterJump(Animator playerAnimator)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Roll");

            float animationLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;

            yield return new WaitForSeconds(animationLength);
            playerAnimator.ResetTrigger("Roll");
            Debug.Log("Roll And Run");
        }
        else
        {
            Debug.LogError("Animator component is missing or not set!");
        }
    }
}

