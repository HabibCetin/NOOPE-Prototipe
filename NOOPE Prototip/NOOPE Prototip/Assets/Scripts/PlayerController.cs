// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    
    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float playerSpeed = 2.0f;
    private float jumpHeight = 1.0f;
    private float gravityValue = -9.81f;
    float turnSmoothVelocity;
    float turnSmoothTime = 2f;

    public static float inputStrenght; //animasyon hýzý için

    private void Awake()
    {
        controller  = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0f, input.y);
        inputStrenght = move.magnitude;
        // Debug.Log(move);
        
        move = move.x * Camera.main.transform.right + move.z * Camera.main.transform.forward;
        move.y = 0f;


        if (move != Vector3.zero)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            controller.Move(move * Time.deltaTime * playerSpeed);
            // gameObject.transform.forward = move;
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
}
