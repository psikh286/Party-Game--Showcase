using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using MoreMountains.Feedbacks;
using _Common;
using Party;
using Party.Movement.Interfaces;

public class PhysicsMovement : MonoBehaviour, IHittable
{
    private int _playerIndex;
    public static Action<PhysicsMovement, int> OnNewPlayerSpawned;
    [SerializeField] private float currentSpeed;

    #region References

    [Header("References")] 
    
    [SerializeField] private Transform cameraTransform;

    public Rigidbody rb;

    [SerializeField] private Transform meshParent;

    [SerializeField] private Animator animator;

    [Header("Feedbacks")] 
    
    [SerializeField] private MMF_Player jumpFeedback;
    [SerializeField] private MMF_Player landFeedback;
    [SerializeField] private MMF_Player hitFeedback;
    [SerializeField] private MMF_Player knockBackFeedback;
    [SerializeField] private MMF_Player dashFeedback;

    #endregion

    #region Movement Settings

    [Header("Movement Settings")]
    [Space]

    #region Grounded Movement Settings

    [Header("Grounded Movement Settings")]
    
    public bool canMove = true;
    
    [SerializeField] private float maxSpeed = 10f;

    [SerializeField] private float maxAcceleration = 20f;

    [SerializeField] private AnimationCurve accelerationFromDot;

    [SerializeField] private float maxAccelerationForce = 100f;

    [SerializeField] private AnimationCurve accelerationForceFromDot;

    [SerializeField] private float inAirSpeedMultiplier = 0.5f;

    #endregion

    #region Jump Movement Settings

    [Header("Jump Movement Settings")] 
    
    [SerializeField] private float jumpBuffer = 0.1f;

    [SerializeField] private float jumpForce = 250f;

    [SerializeField] private float fallMultiplier = 2.5f;

    [SerializeField] private float lowJumpMultiplier = 2f;

    [SerializeField] private float coyoteTime = 0.25f;

    #endregion
    
    #endregion

    #region Movement Values

    #region Grounded Movement Values

    private bool _knockedBack;

    private Vector3 _knockBackForce;

    #endregion

    #region Falling/Jump Values

    private bool _isGrounded = true;
    
    private bool _jumped;

    private float _jumpedButtonPressedTime;
    
    private Vector3 _gravitationalForce;
    
    private float _startedFallingTime;
    
    private const float JumpRayBuffer = 0.75f;

    #endregion

    #region Rotation

    private Quaternion _targetRotation;

    #endregion

    #endregion

    #region Inputs

    private Vector2 _movementInput;

    private bool _jumpButtonPressed;

    #endregion

    #region Height Spring Settings

    [Header("Height Spring Settings")] 
    
    [SerializeField] private float rideHeight = 1.5f;

    [SerializeField] private float downRayOffset = 1f;
    
    [SerializeField] private float downRayDistance = 2.5f;
    
    [SerializeField] private float rideSpringStrength = 4f;

    [SerializeField] private float rideSpringDamper = 0.2f;

    #endregion

    #region Upright Joint Settings
    
    [Header("Upright Joint Settings")]

    [SerializeField] private float uprightJointSpringStrength = 4f;
    
    [SerializeField] private float uprightJointSpringDamper = 0.6f;

    #endregion

    #region Hit Settings

    [Header("Hit Settings")]
    
    [SerializeField] private float hitForce = 5f;

    #endregion

    #region Dash Settings

    [Header("Dash Settings")]
    
    [SerializeField] private float dashForce = 20f;

    [SerializeField] private float dashCooldown = 1f;

    private float _lastDashTime;

    #endregion

    #region Animator Values

    private float _animatorSpeed;
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int IsGrounded = Animator.StringToHash("isGrounded");

    #endregion
    
    #region Input Functions

    public void SetMovementInput(InputAction.CallbackContext context)
    {
        if(SceneController.GameState != SceneControllerState.Active) return;
        
        _movementInput = context.ReadValue<Vector2>();
    }

    public void SetJumpInput(InputAction.CallbackContext context)
    {
        if(SceneController.GameState != SceneControllerState.Active) return;
        
        _jumpButtonPressed = context.ReadValueAsButton();

        if(context.started) _jumpedButtonPressedTime = Time.time;
    }

    public void SetHitInput(InputAction.CallbackContext context)
    {
        if(SceneController.GameState != SceneControllerState.Active) return;
        
        if (context.started)
        {
            HitPlayer();
        }
    }

    public void ResetInputVariables()
    {
        _movementInput = Vector2.zero;
        _jumpButtonPressed = false;
    }

    public void DashInput(InputAction.CallbackContext context)
    {
        if(SceneController.GameState != SceneControllerState.Active) return;
        
        if (context.started)
        {
            Dash();
        }
    }

    #endregion

    #region Lifetime Functions

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _gravitationalForce = Physics.gravity * rb.mass;
    }
    private void Start()
    {
        _playerIndex = gameObject.GetComponent<Player>().playerIndex;
        OnNewPlayerSpawned.Invoke(this, _playerIndex);
    }

    public void SetMeshParent(Transform newMeshParent)
    {
        meshParent = newMeshParent;

        animator = meshParent.GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        _isGrounded = SetIsGrounded();

        HeightSpring();
        
        UprightJointSpring();
        
        ApplyMovementForce();

        ApplyJumpForce();
        
        ApplyKnockBackForce();

        SetCurrentSpeed();
        
        SetAnimatorParameters();
    }

    #endregion
    
    private bool  SetIsGrounded()
    {
        Ray groundedRay = new Ray(transform.position + Vector3.up * 0.2f, Vector3.down);
        
        if (Physics.Raycast(groundedRay, (rideHeight - 1f) + JumpRayBuffer))
        {
            if (_jumped && rb.velocity.y > 0) return false;
            
            if (_jumped) _jumped = false;
            
            _startedFallingTime = 0f;

            if (!_isGrounded)
            {
                if(landFeedback) landFeedback.PlayFeedbacks();
            }
            
            return true;
        }
        
        if (_startedFallingTime == 0f) _startedFallingTime = Time.time;

        if (_isGrounded)
        {
            Vector3 currentVelocity = rb.velocity;
            rb.velocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
        }
        
        return false;
    }

    private void SetCurrentSpeed()
    {
        currentSpeed = rb.velocity.magnitude;
    }

    private void ApplyMovementForce()
    {
        //Convert movement input Vector2 to camera oriented Vector3
        Vector3 moveDirection = MovementFunctions.AlignMovementInputToCamera(_movementInput, cameraTransform);

        moveDirection = canMove ? moveDirection : Vector3.zero;
        
        SetTargetRotation(moveDirection);

        //Calculate Dot Vector between current and target velocity
        float velocityDot = Vector3.Dot(moveDirection, rb.velocity.normalized);

        //Calculate target acceleration using Max Acceleration and Acceleration Dot curve
        float acceleration = maxAcceleration * accelerationFromDot.Evaluate(velocityDot);
        
        //Calculate the target final velocity of the character
        Vector3 targetVelocity = moveDirection * maxSpeed;

        //limit final velocity if the character is in the air
        if (!_isGrounded) targetVelocity *= inAirSpeedMultiplier;

        //Calculate the possible increase in velocity from current to target based on acceleration in this physics frame
        Vector3 currentVelocityStep = Vector3.MoveTowards(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        //Calculate the target acceleration for this physics frame
        Vector3 targetAcceleration = (currentVelocityStep - rb.velocity) / Time.fixedDeltaTime;

        //Calculate the acceleration force limit based on curve
        float accelerationForce = maxAccelerationForce * accelerationForceFromDot.Evaluate(velocityDot);

        //clamp target acceleration to max acceleration force
        targetAcceleration = Vector3.ClampMagnitude(targetAcceleration, accelerationForce);

        //0 any force that can be applied in the vertical axis
        targetAcceleration.y = 0f;

        //add movement force based on mass
        rb.AddForce(targetAcceleration * rb.mass);
    }

    private void ApplyJumpForce()
    {
        if(!canMove) return;
        
        if (_jumped)
        {
            JumpModifier();
            return;
        }
        
        if (_jumpedButtonPressedTime > Time.time - jumpBuffer)
        {
            if(!_isGrounded)
            {
                if (!CoyoteTimeCheck()) return;
            }

            Jump();
        }
    }

    private void SetTargetRotation(Vector3 moveDirection)
    {
        if (_movementInput.sqrMagnitude > 0.1f)
        {
            _targetRotation = moveDirection == Vector3.zero ? _targetRotation : MovementFunctions.GetLookRotation(moveDirection);

            _targetRotation = Quaternion.Euler(0f, _targetRotation.eulerAngles.y, 0f);
        }
    }

    private void Jump()
    {
        Vector3 currentVelocity = rb.velocity;

        _jumped = true;

        rb.velocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);
            
        rb.AddForce(Vector3.up * (jumpForce * rb.mass));
        
        if(jumpFeedback) jumpFeedback.PlayFeedbacks();
    }

    private void JumpModifier()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * (Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime);
        }
        else if(rb.velocity.y > 0 && !_jumpButtonPressed)
        {
            rb.velocity += Vector3.up * (Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime);
        }
    }

    private bool CoyoteTimeCheck()
    {
        return _startedFallingTime + coyoteTime >= Time.time;
    }

    private void HeightSpring()
    {
        if(!_isGrounded) return;
        
        if(_jumped) return;
        
        Vector3 rayStart = transform.position + (Vector3.up * downRayOffset);

        Ray downRay = new Ray(rayStart, Vector3.down);

        bool rayHit = Physics.Raycast(downRay, out var downRayHit, downRayDistance);
        
        SetMeshPosition(downRayHit);

        if (!rayHit) return;
        
        rb.AddForce(GetHeightSpringForce(downRayHit, downRay));
    }

    private Vector3 GetHeightSpringForce(RaycastHit downRayHit, Ray downRay)
    {
        float mass = rb.mass;
        
        Vector3 vel = rb.velocity;
        
        Vector3 rayDir = Vector3.down;

        Vector3 otherVel = Vector3.zero;
        Rigidbody hitBody = downRayHit.rigidbody;

        if (hitBody)
        {
            otherVel = hitBody.velocity;
        }

        float rayDirVel = Vector3.Dot(rayDir, vel);
        float otherDirVel = Vector3.Dot(rayDir, otherVel);

        float relVel = rayDirVel - otherDirVel;

        float x = downRayHit.distance - rideHeight;

        float springForce = (x * (rideSpringStrength * mass)) - (relVel * (rideSpringDamper * mass));

        Vector3 maintainHeightForce = -_gravitationalForce + springForce * rayDir;

        Debug.DrawLine(downRay.origin, downRay.origin + (rayDir * springForce), Color.yellow);
        
        if (hitBody)
        {
            hitBody.AddForceAtPosition(rayDir * -springForce, downRayHit.point);
        }

        return maintainHeightForce;
    }

    private void SetMeshPosition(RaycastHit downRayHit)
    {
        Vector3 currentPosition = transform.position;
        float meshParentOffset;

        if (downRayHit.point.y > currentPosition.y - rideHeight)
        {
            meshParentOffset = currentPosition.y - downRayHit.point.y;
        }
        else
        {
            meshParentOffset = rideHeight;
        }

        Vector3 targetMeshPosition = currentPosition + Vector3.down * meshParentOffset;
        Vector3 smoothedTargetMeshPosition = Vector3.Lerp(meshParent.transform.position, targetMeshPosition, 0.1f);
        
        meshParent.position = smoothedTargetMeshPosition;
    }

    private void UprightJointSpring()
    {
        float mass = rb.mass;
        
        Quaternion currentRotation = transform.rotation;
        Quaternion toGoal = ShortestRotation(_targetRotation, currentRotation);
        
        toGoal.ToAngleAxis(out var rotDegrees, out var rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        Vector3 torqueForce = (rotAxis * (rotRadians * (uprightJointSpringStrength * mass)) -
                               (rb.angularVelocity * (uprightJointSpringDamper * mass)));
        
        rb.AddTorque(torqueForce);
    }

    private void SetAnimatorParameters()
    {
        _animatorSpeed = LevelFunctions.Map(currentSpeed, 0f, maxSpeed, 0f, 1f);
        
        if(animator) animator.SetFloat(Speed, _animatorSpeed);
        if(animator) animator.SetBool(IsGrounded, _isGrounded);
    }

    private void ApplyKnockBackForce()
    {
        if (_knockBackForce.magnitude < 0.1f) return;
        
        rb.velocity = Vector3.zero;
        
        rb.AddForce(_knockBackForce * 100f);

        _knockBackForce = Vector3.zero;
    }

    IEnumerator CancelKnockBack()
    {
        yield return new WaitUntil(KnockBackOver);
        
        _knockedBack = false;
        if (knockBackFeedback) knockBackFeedback.StopFeedbacks();
        SetCanMove(true);
    }

    //might be inconsistent, need further testing 
    private bool KnockBackOver()
    {
        Vector3 currentVelocity = rb.velocity;
        currentVelocity.y = 0f;

        float knockBackSpeed = currentVelocity.magnitude;

        //print(knockBackSpeed);
        if (knockBackSpeed < 1f)
        {
            return true;
        }

        return false;
    }

    private void HitPlayer()
    {
        if(!canMove) return;
        
        //line trace forward
        Ray forwardRay = new Ray(transform.position, _targetRotation * Vector3.forward);
        
        PlayHitAnimation();

        if (Physics.SphereCast(forwardRay, 0.3f, out var forwardRayHit, 2f))
        {
            if(forwardRayHit.transform.TryGetComponent(out IHittable hitObject)) hitObject.OnHit(_playerIndex);
            
            if (!forwardRayHit.rigidbody) return;
            
            if(hitFeedback) hitFeedback.PlayFeedbacks();

            Vector3 position = transform.position;
            Vector3 hitDirection = (forwardRayHit.point - position).normalized;
            forwardRayHit.rigidbody.AddForceAtPosition(hitDirection * hitForce, position, ForceMode.Impulse);
        }
    }

    public void OnHit(int i)
    {
        if(_knockedBack) return;
        
        _knockedBack = true;
        
        SetCanMove(false);
        
        if(animator) animator.Play("KnockBack");
        
        if(knockBackFeedback) knockBackFeedback.PlayFeedbacks();

        StartCoroutine(CancelKnockBack());
    }
    public void OnHit(Vector3 knockBackDirection)
    {
        if(_knockedBack) return;

        _knockedBack = true;
        
        SetCanMove(false);
        
        _knockBackForce = knockBackDirection;
        
        if(animator) animator.Play("KnockBack");
        
        if(knockBackFeedback) knockBackFeedback.PlayFeedbacks();

        StartCoroutine(CancelKnockBack());
    }

    private void PlayHitAnimation()
    {
        if(animator) animator.Play("Attack");
    }
    public void PlayHitAnimation(bool shouldPlayParticles)
    {
        if(animator) animator.Play("Attack");
        
        if(!shouldPlayParticles) return;
        
        if(hitFeedback) hitFeedback.PlayFeedbacks();
    }
    

    private void Dash()
    {
        if(!canMove) return;
        
        if(!DashCooldownCheck()) return;

        _lastDashTime = Time.time;
        
        Vector3 dashDirection = rb.velocity.normalized;
        dashDirection.y = 0;
        dashDirection = dashDirection.magnitude < 1f ? (_targetRotation * Vector3.forward).normalized : dashDirection;

        rb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        
        if(animator) animator.Play("Dash");
        
        if(dashFeedback) dashFeedback.PlayFeedbacks();
    }

    private bool DashCooldownCheck()
    {
        if (_lastDashTime + dashCooldown > Time.time) return false;

        return true;
    }

    public void Die()
    {
        SetCanMove(false);
        
        if(animator) animator.Play("Defeat");
    }

    #region Helper Functions

    public void SetCanMove(bool newCanMove)
    {
        canMove = newCanMove;

        //maxSpeed = canMove ? startingMaxSpeed : 0f;
    }

    public void SetMaxSpeed(float speed)
    {
        maxSpeed = speed;
    }

    public float GetMaxSpeed()
    {
        return maxSpeed;
    }

    private static Quaternion ShortestRotation(Quaternion a, Quaternion b)
    {
        if (Quaternion.Dot(a, b) < 0)
        {
            return a * Quaternion.Inverse(Multiply(b, -1));
        }

        else return a * Quaternion.Inverse(b);
    }
    
    private static Quaternion Multiply(Quaternion input, float scalar)
    {
        return new Quaternion(input.x * scalar, input.y * scalar, input.z * scalar, input.w * scalar);
    }

    public bool HasMovementInput()
    {
        return _movementInput.sqrMagnitude > 0f;
    }

    public void SetCamera(Transform cam)
    {
        cameraTransform = cam;
    }
    #endregion
}
