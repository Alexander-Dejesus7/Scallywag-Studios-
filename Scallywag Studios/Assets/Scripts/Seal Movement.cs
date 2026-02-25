using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SealMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float waterDrag = 4f;
    public float airDrag = 0f;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Water Check")]
    public LayerMask whatIsWater;
    bool isInWater;

    [Header("Ground Check")]
    public LayerMask whatIsGround;
    bool isGrounded;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public string jumpButton = "Jump";

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

    }
    private void Update()
    {
        MyInput();

        if (isInWater)
            rb.drag = waterDrag;
        else
            rb.drag = airDrag;

        SpeedControl();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if ((Input.GetKey(jumpKey) || Input.GetButtonDown(jumpButton)) && readyToJump && isGrounded)
        {
            Jump();
            readyToJump = false;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        if (isInWater)
        {
            moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else if (isGrounded)
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & whatIsWater) != 0)
            isInWater = true;

        if (((1 << other.gameObject.layer) & whatIsGround) != 0)
            isGrounded = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & whatIsWater) != 0)
        {
            isInWater = false;
        }

        if (((1 << other.gameObject.layer) & whatIsGround) != 0)
        {
            isGrounded = false;
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}

