using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private Vector2 startTouchPosition, endTouchPosition;
    private float minSwipeDistance = 300f; // Minimum distance for a swipe to be recognized
    PlayerController playerController = new PlayerController();

    public Animator playerControllerAnimator;

   public float moveSpeed = 10f;

    float jumpForce = 8f;
    Rigidbody rb;
    bool isGrounded;
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        MoveForward();
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                DetectSwipe();
            }
        }
    }

    void DetectSwipe()
    {
        Vector2 swipeVector = endTouchPosition - startTouchPosition;

        if (swipeVector.magnitude >= minSwipeDistance)
        {
            float xSwipe = swipeVector.x;
            float ySwipe = swipeVector.y;

            if (Mathf.Abs(xSwipe) > Mathf.Abs(ySwipe))
            {
                // Horizontal swipe
                if (xSwipe > 0)
                {
                    RotateObject(90); // Swipe right
                }
                else
                {
                    RotateObject(-90); // Swipe left
                }
            }
            else
            {
                // Vertical swipe
                if (ySwipe > 0)
                {
                    Debug.Log("Swipe Up");
                    Jump();
                     StartCoroutine(playerController.jumpPlayer(playerControllerAnimator)); // sipe Up
                }
                else
                {
                    Debug.Log("Swipe Slide");
                    StartCoroutine(playerController.slidePlayer(playerControllerAnimator)); ; // sipe Down
                }
            }
        }
    }
    
    void RotateObject(float angle)
    {
        transform.Rotate(0, angle, 0, Space.Self);
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // Check if the object is grounded
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    // Check if the object leaves the ground
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
