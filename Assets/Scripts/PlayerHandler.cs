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
    private float targetXPosition; // Target X position for smooth movement

    public float jumpForce = 8f;
    public Animator playerControllerAnimator;
    public float moveSpeed = 10f;
    public float maxMoveDistance = 3f; // Max distance the player can move left or right on the X-axis
    public GameObject swipeObject; // Reference to the game object to detect swipe on
    public float smoothTime = 0.1f; // Smoothing time for movement
    private float velocity = 0.0f; // Used for smooth damp movement

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        targetXPosition = startPos.x;
    }

    void Update()
    {
        MoveForward();

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
                    MoveObject(currentTouchPosition - startTouchPosition);
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

        // Smoothly move towards the target X position
        float newXPosition = Mathf.SmoothDamp(transform.position.x, targetXPosition, ref velocity, smoothTime);
        transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);


        float yRotation = NormalizeAngle(transform.localEulerAngles.y);

        // Print the Y-axis rotation value
        Debug.Log("Y-axis rotation: " + ((int)yRotation));
    }

    float NormalizeAngle(float angle)
    {
        while (angle > 180) angle -= 360;
        while (angle < -180) angle += 360;
        return angle;
    }
    void MoveObject(Vector2 swipeDelta)
    {
        // Calculate horizontal movement based on swipe
        float xSwipe = swipeDelta.x * 0.01f; // Adjust sensitivity for smoother control

        // Calculate new position within bounds
        float newXPosition = Mathf.Clamp(transform.position.x + xSwipe, startPos.x - maxMoveDistance, startPos.x + maxMoveDistance);

        // Set the target X position for smooth movement
        targetXPosition = newXPosition;
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
