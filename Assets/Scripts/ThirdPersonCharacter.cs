using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class ThirdPersonCharacter : MonoBehaviour
{
    //Input Action bindings
    private InputAction _actMovement;
    private InputAction _actLook;
    private InputAction _actJump;
    private InputAction _actDash;
    //private InputAction _actInteract;
    private InputAction _actAttack;
    private InputAction _actMenu;
    
    
    //Variable assignments
    [SerializeField] public CharacterController characterController;
    [SerializeField] private GameObject EmoteCube;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform headCheck;
    [SerializeField] private LayerMask enviromentMask;
    [SerializeField] private float characterSpeed = 5f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float gravity = -9.82f;
    //[SerializeField] private float groundCheckDistance = 0.4f;
    [SerializeField] private float headCheckDistance = 0.4f;
    [SerializeField] private float jumpHeight = 3f;
    [SerializeField] private float groundedGravity = -9.82f;
    [SerializeField] private float slopeLimit = 45f;
    [SerializeField] private float maxSlopeSlideSpeed = 50f;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float coyoteTime = 1;
    [SerializeField] private int maxAmountOfJumps = 1;
    private Vector3 _hitNormal;
    private Vector3 _velocity;
    private Vector3 _moveDir;
    private Vector3 _moveDirSloped;
    private Vector3 _slopeDecentSpeed;
    private float _turnSmoothVelocity;
    private float _slideSpeedBuffer = 4f;
    private float _coyoteCounter = 0;
    private float _jumpHold;
    private Vector3 _direction;
    private int _amountOfJumps;
    private bool _isGrounded;
    private bool _hittingCeiling;
    private bool _onSlope;
    private bool _isDashing;

    // Things to impliment
    // Coyote time - Done
    // Jump height based on key press length


    private void Start()
    {
        _actMovement = InputSystem.actions.FindAction("Movement");
        _actLook = InputSystem.actions.FindAction("Look");
        _actJump = InputSystem.actions.FindAction("Jump");
        _actDash = InputSystem.actions.FindAction("Dash");
        //_actInteract = InputSystem.actions.FindAction("Interact");
        _actAttack = InputSystem.actions.FindAction("Attack");
        _actMenu = InputSystem.actions.FindAction("Menu");
    }

    void Update()
    {
        //Debug.Log(_velocity.y);
        
        _onSlope = false;

        GroundChecks();
        HeadChecks();
        SlipperySlope();
        MoveCharacter();
        //Interact();
        Jump();
        Dash();
        //If you are grounded you will have a static gravity variable
        if (_isGrounded && _velocity.y < 0) _velocity.y = groundedGravity;
        Gravity();
    }

    //--------------------PlayerActions--------------------
    
    private void MoveCharacter()
    {
        float horizontal = _actMovement.ReadValue<Vector2>().x;
        float vertical = _actMovement.ReadValue<Vector2>().y;
        _direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (_direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            _moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(characterSpeed * Time.deltaTime * (_moveDir.normalized));
        }
    }


    
    private void Jump()
    {
        if (_actJump.WasPressedThisFrame() && _amountOfJumps > 0 && !_isDashing && !_onSlope)
        {
            _jumpHold += Time.deltaTime;
            //Debug.Log("Pressed");

            if (_amountOfJumps == maxAmountOfJumps && _actJump.WasCompletedThisFrame())
            {
                //Debug.Log("Complete");
            }

            else
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                _amountOfJumps--;
            }
        }
    }
    
    private void Dash()
    {
        if (_actDash.WasPressedThisFrame())
        {
            if (_direction != Vector3.zero)
            {
                StartCoroutine(DashAction(dashDuration));
            }
            // Debug.Log("You are not moving!");
        }
    }

    private IEnumerator DashAction(float duration)
    {
        _isDashing = true;
        float i = 0;
        while (i  < duration)
        {
            characterController.Move(dashSpeed * Time.deltaTime * _moveDir.normalized);
            yield return null;
            i += Time.deltaTime;
        }
        _isDashing = false;
    }
    
    //--------------------Checks & Gravity--------------------
    
    private void GroundChecks()
    {
        //Sphere casts at the location of the Groundcheck object on the player to see if they are on the ground.
        //_isGrounded = Physics.CheckCapsule(groundCheck.position + new Vector3(0, 0.475f, 0), (groundCheck.position + new Vector3(0, 1, 0)), 0.5f, enviromentMask);

        if (!Physics.CheckCapsule(groundCheck.position + new Vector3(0, 0.475f, 0), (groundCheck.position + new Vector3(0, 1, 0)), 0.5f, enviromentMask))
        {
            _coyoteCounter += Time.deltaTime;

            if (_coyoteCounter > coyoteTime) 
            {
                _isGrounded = false;
                if (_amountOfJumps == maxAmountOfJumps) _amountOfJumps--;
            }
        }

        else
        {
            _coyoteCounter = 0;
            _isGrounded = true;
            _isGrounded = (Vector3.Angle (Vector3.up, _hitNormal) <= slopeLimit); 
            if (!_isGrounded) _onSlope = true;
            if (!_onSlope) _amountOfJumps = maxAmountOfJumps;
        }
    }

    private void HeadChecks()
    {
        _hittingCeiling = Physics.CheckSphere(headCheck.position, headCheckDistance, enviromentMask);
        if (_hittingCeiling) _velocity.y += gravity/50;
    }

    private void SlipperySlope()
    {
        if (!_isGrounded) _moveDirSloped = Vector3.ProjectOnPlane(transform.up, _hitNormal);
    }
    
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        _hitNormal = hit.normal;
    }
    
    private void Gravity()
    {
        _velocity.y += gravity * Time.deltaTime;
        if (_onSlope)
        {
            if (_slideSpeedBuffer < maxSlopeSlideSpeed) _slideSpeedBuffer += 0.1f;
            _slopeDecentSpeed = _moveDirSloped * _slideSpeedBuffer;
        }
        else
        {
            _slopeDecentSpeed = Vector3.zero;
            _slideSpeedBuffer = 4f;
        }
        characterController.Move((_velocity - _slopeDecentSpeed) * Time.deltaTime);
    }
    
    
}