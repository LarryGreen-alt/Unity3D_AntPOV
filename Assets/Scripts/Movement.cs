using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] Transform playerCamera;
    [SerializeField][Range(0.0f, 0.5f)] float mouseSmoothTime = 0.03f;
    [SerializeField] bool cursorLock = true;
    [SerializeField] float mouseSensitivity = 3.5f;
    [SerializeField] float Speed = 12.0f;
    [SerializeField][Range(0.0f, 0.5f)] float moveSmoothTime = 0.3f;
    [SerializeField] float gravity = -30f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;  
 
    public float jumpHeight = 6f;
    float velocityY;
    bool isGrounded;
    public float speed;
 
    float cameraCap;
    Vector2 currentMouseDelta;
    Vector2 currentMouseDeltaVelocity;
    
    CharacterController controller;
    Vector2 currentDir;
    Vector2 currentDirVelocity;
    Vector3 velocity;
    Vector3 velocityWall;

    public float range = 1f;
    public bool TouchingWall = false;
    public float UpwardSpeed = 3.3f;
    public Camera Cam;
 
    void Start()
    {
        controller = GetComponent<CharacterController>();
 
        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
        }
    }
 
    void Update()
    {
        UpdateMouse();
        UpdateMove();
    }
 
    void UpdateMouse()
    {
        Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
 
        currentMouseDelta = Vector2.SmoothDamp(currentMouseDelta, targetMouseDelta, ref currentMouseDeltaVelocity, mouseSmoothTime);
 
        cameraCap -= currentMouseDelta.y * mouseSensitivity;
 
        cameraCap = Mathf.Clamp(cameraCap, -90.0f, 90.0f);
 
        playerCamera.localEulerAngles = Vector3.right * cameraCap;
 
        transform.Rotate(Vector3.up * currentMouseDelta.x * mouseSensitivity);
    }

    // For the raycast laser to wall
    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, range))
        {

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                TouchingWall = true;
                Debug.Log("Touching wall...");
            }
        }
    }

    // Function to Pickup Objects in front of the player
    // Take input from 'P' and remain in front of player until drop or hole.

    private void OnCollisionEnter(Collision collision)
   {
      if (collision.gameObject.tag == "Object" && Input.GetKey("e"))
      {
            Destroy(collision.gameObject);
            Debug.Log("Object Destroyed.");
      }
       
   }

            // To "Eat" (Destroy) object from Key "e"
 
    void UpdateMove()
    {   
        Shoot();
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, ground);
 
        Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDir.Normalize();
 
        currentDir = Vector2.SmoothDamp(currentDir, targetDir, ref currentDirVelocity, moveSmoothTime);
 
        velocityY += gravity * 2f * Time.deltaTime;
 
        // Movement on the ground 
        Vector3 velocity = (transform.forward * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;

        // Movement on the wall 
        Vector3 velocityWall = (transform.up * currentDir.y + transform.right * currentDir.x) * Speed + Vector3.up * velocityY;
 
        controller.Move(velocity * Time.deltaTime);


        if(Input.GetKey("w") & TouchingWall == true)     
        {
            isGrounded = false;
            velocityY = velocityY * 0;
            controller.Move(velocityWall * Time.deltaTime);
            velocity = velocity * 0;
            GetComponent<Rigidbody>().isKinematic = true;
            TouchingWall = false;
            GetComponent<Rigidbody>().isKinematic = false;
        }

        if(Input.GetKeyUp("w"))
        {
            GetComponent<Rigidbody>().isKinematic = false;
            TouchingWall = false;
            velocityWall = velocityWall * 0;
        }
 
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocityY = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
 
        if(isGrounded! && controller.velocity.y < -1f)
        {
            velocityY = -8f;
        }
    }
}
