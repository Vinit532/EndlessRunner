using System.Collections;
using UnityEngine;
using System.Collections.Generic;

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

    float rotationSpeed = 450f;
    bool isRotating = false;

    public AudioSource gamePlayAudioSource;
    public List<AudioClip> audioClips;
    public delegate void AudioSourceAction(); // To take action as parameter while calling methods to play Audio 
    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
        GamePlaySound(gamePlayAudioSource.Play);
    }

    void Update()
    {
        MoveForward();
        yRotation = NormalizeAngle(transform.localEulerAngles.y);


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
                     // MoveObjectOnX(currentTouchPosition - startTouchPosition);
                    if (transform.localEulerAngles.y <= 80 || (transform.localEulerAngles.y >= 150 && transform.localEulerAngles.y <= 200)) //(yRotation >= -40 || yRotation <= 45) || (yRotation >= 120 || yRotation <= 220)
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
        if (transform.localEulerAngles.y < 150)
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
        if (transform.localEulerAngles.y < 80)
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
                    StartCoroutine(RotateObject(90));  // Swipe right
                }
                else
                {
                   StartCoroutine( RotateObject(-90)); // Swipe left
                }
            }
            else
            {
                // Vertical swipe
                if (ySwipe > 0)
                {
                    playerControllerAnimator.ResetTrigger("Run");
                    Jump();
                    
                }
                else
                {
                    StartCoroutine(playerController.slidePlayer(playerControllerAnimator)); 
                }
            }
        }
    }
    
    IEnumerator RotateObject(float angle)
    {
        isRotating = true;

        float targetAngle = transform.localEulerAngles.y + angle;
        float startAngle = transform.localEulerAngles.y;
        float elapsedTime = 0f;

        while (elapsedTime < Mathf.Abs(angle) / rotationSpeed)
        {
            float currentAngle = Mathf.Lerp(startAngle, targetAngle, elapsedTime * rotationSpeed / Mathf.Abs(angle));
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, currentAngle, transform.localEulerAngles.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure final angle is set to target angle
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, targetAngle, transform.localEulerAngles.z);

        // Log rotation after rotation
        Debug.Log("After rotation Y-axis: " + transform.localEulerAngles.y);

        isRotating = false;
    }

    void MoveForward()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void Jump()
    {
        if (isGrounded)
        {
            GamePlaySound(gamePlayAudioSource.Stop);
            PlayJumpSound(gamePlayAudioSource.Play);
            StartCoroutine(playerController.jumpPlayer(playerControllerAnimator));
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        isGrounded = false;
    }

    void GamePlaySound(AudioSourceAction action)
    {
        gamePlayAudioSource.clip = audioClips[0];
        gamePlayAudioSource.loop = true;
        action.Invoke();
    }
    void PlayJumpSound(AudioSourceAction action)
    {
        gamePlayAudioSource.clip = audioClips[1];
        gamePlayAudioSource.loop = false;
        action.Invoke();
    }
    void PlayFallsIntoWaterSound(AudioSourceAction action)
    {
        gamePlayAudioSource.clip = audioClips[2];
        gamePlayAudioSource.loop = false;
        action.Invoke();
    }
    // Check if the object is grounded
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            playerControllerAnimator.SetTrigger("Run");
            if (!gamePlayAudioSource.isPlaying)
            {
                GamePlaySound(gamePlayAudioSource.Play);
            }
            Debug.Log("Player collied to ground");
        }

        if (collision.gameObject.CompareTag("Water"))
        {
            Debug.Log("Player Falls Into the Water");
            isGrounded = false;
            GamePlaySound(gamePlayAudioSource.Stop);
            PlayFallsIntoWaterSound(gamePlayAudioSource.Play);
            
        }

    }

    // Check if the object leaves the ground
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            
            Debug.Log("Player Out of ground");
        }
    }

}
