using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
     Animator animator;

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

    void Update()
    {
        // Check if the space key is pressed for jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed.");
            StartCoroutine(PlayAnimationAndReturnToRun("Jump"));
        }

        // Check if the down arrow key is pressed for sliding
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Down arrow key pressed.");
            StartCoroutine(PlayAnimationAndReturnToRun("Slide"));
        }

    }

    public void Jump()
    {
        Debug.Log("Jump Called");
        StartCoroutine(PlayAnimationAndReturnToRun("Jump"));
    }
    public void Slide()
    {
        Debug.Log("Slide Called");
        StartCoroutine(PlayAnimationAndReturnToRun("Slide"));
    }
    public IEnumerator PlayAnimationAndReturnToRun(string triggerName)
    {
        Debug.Log("PlayAnimationAndReturnToRun Called");
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
            Debug.Log($"{triggerName} animation triggered.");

            float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
            Debug.Log($"{triggerName} animation length: {animationLength}");

            yield return new WaitForSeconds(animationLength);
            animator.ResetTrigger(triggerName);
            animator.SetTrigger("Run");
            Debug.Log("Returning to RunningForward animation.");
        }
        else
        {
            Debug.LogError("Animator component is missing or not set!");
        }
    }

    public IEnumerator jumpPlayer(Animator playerAnimator)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Jump");
            Debug.Log($"{"Jump"} animation triggered.");

            float animationLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;
            Debug.Log($"{"Jump"} animation length: {animationLength}");

            yield return new WaitForSeconds(animationLength);
            playerAnimator.ResetTrigger("Jump");
            playerAnimator.SetTrigger("Run");
            Debug.Log("Returning to RunningForward animation.");
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
            Debug.Log($"{"Slide"} animation triggered.");

            float animationLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;
            Debug.Log($"{"Slide"} animation length: {animationLength}");

            yield return new WaitForSeconds(animationLength);
            playerAnimator.ResetTrigger("Slide");
            playerAnimator.SetTrigger("Run");
            Debug.Log("Returning to RunningForward animation.");
        }
        else
        {
            Debug.LogError("Animator component is missing or not set!");
        }
    }


}

