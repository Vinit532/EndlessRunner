using UnityEngine;

public class PlayerHandler : MonoBehaviour
{
    private Vector2 startTouchPosition, currentTouchPosition;
    private float minSwipeDistance = 300f; // Minimum distance for a swipe to be recognized
    private PlayerController playerController = new PlayerController();
    private Rigidbody rb;
    private bool isGrounded;
    private bool isTouchingObject = false; // Track if the initial touch was on the swipeObject
    private Vector3 startPos; // Starting position of the player
    private float targetZPosition; // Target Z position for smooth movement
    private float targetXPosition; // Target X position for smooth movement

    public float jumpForce = 8f;
    public Animator playerControllerAnimator;
    public float moveSpeed = 10f;
    public float maxMoveDistance = 3f; // Max distance the player can move forward or backward on the Z-axis
    public GameObject swipeObject; // Reference to the game object to detect swipe on
    public float smoothTime = 0.1f; // Smoothing time for movement
    private float velocity = 0.0f; // Used for smooth damp movement

    float yRotation; // to store value for changed rotation
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        
    }

    void Update()
    {
        MoveForward();
        yRotation = NormalizeAngle(transform.localEulerAngles.y);

        // Print the Y-axis rotation value
        Debug.Log("Y-axis rotation (normalized): " + yRotation);
        Debug.Log("Y-axis rotation (raw): " + transform.localEulerAngles.y);

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            currentTouchPosition = touch.position;

            // Convert touch position to world space
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            RaycastHit hit;

            if (touch.phase == TouchPhase.Began)
            {
                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == swipeObject)
                {
                    isTouchingObject = true;
                    startTouchPosition = touch.position;
                }
                else
                {
                    isTouchingObject = false;
                    startTouchPosition = touch.position;
                }
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                if (isTouchingObject)
                {
                    //  MoveObjectOnX(currentTouchPosition - startTouchPosition);
                    if (yRotation == 0 || yRotation == 180)
                    {
                        MoveObjectOnX(currentTouchPosition - startTouchPosition);
                    }
                    else
                    {
                        MoveObjectOnZ(currentTouchPosition - startTouchPosition);
                    }
                    
                    startTouchPosition = currentTouchPosition; // Update the start position for the next frame
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                if (isTouchingObject)
                {
                    isTouchingObject = false;
                }
                else
                {
                    DetectSwipe();
                }
            }
        }

        // Smoothly move towards the target Z position
       

        // Get local Y rotation
        
    }

    float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }

    void MoveObjectOnZ(Vector2 swipeDelta)
    {
        targetZPosition = startPos.z;
        // Calculate vertical movement based on swipe
        
        float zSwipe ; // Adjust sensitivity for smoother control
        if (yRotation < 92)
        {
            zSwipe = swipeDelta.x * - 0.01f; // Adjust sensitivity for smoother control
        }
        else
        {
            zSwipe = swipeDelta.x * 0.01f;
        }
        // Calculate new position within bounds
        float newZPosition = Mathf.Clamp(transform.position.z + zSwipe, startPos.z - maxMoveDistance, startPos.z + maxMoveDistance);

        // Set the target Z position for smooth movement
        targetZPosition = newZPosition;

        newZPosition = Mathf.SmoothDamp(transform.position.z, targetZPosition, ref velocity, smoothTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, newZPosition);
    }
    void MoveObjectOnX(Vector2 swipeDelta)
    {
        targetXPosition = startPos.x;
        // Calculate vertical movement based on swipe
        float xSwipe = swipeDelta.x * 0.01f; // Adjust sensitivity for smoother control
        if (yRotation < 2)
        {
            xSwipe = swipeDelta.x * 0.01f;
        }
        else
        {
            xSwipe = swipeDelta.x * - 0.01f;
        }
        // Calculate new position within bounds
        float newZPosition = Mathf.Clamp(transform.position.x + xSwipe, startPos.x - maxMoveDistance, startPos.x + maxMoveDistance);

        // Set the target X position for smooth movement
        targetXPosition = newZPosition;

        newZPosition = Mathf.SmoothDamp(transform.position.x, targetXPosition, ref velocity, smoothTime);
        transform.position = new Vector3(newZPosition, transform.position.y, transform.position.z);
    }

    void DetectSwipe()
    {
        Vector2 swipeVector = currentTouchPosition - startTouchPosition;

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
                    Jump();
                    StartCoroutine(playerController.jumpPlayer(playerControllerAnimator)); // Swipe up
                }
                else
                {
                    StartCoroutine(playerController.slidePlayer(playerControllerAnimator)); // Swipe down
                }
            }
        }
    }

    void RotateObject(float angle)
    {
        transform.Rotate(0, angle, 0, Space.Self);

        // Log rotation after rotation
        Debug.Log("After rotation Y-axis: " + transform.localEulerAngles.y);
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
