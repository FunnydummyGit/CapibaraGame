using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]

public class PlayerControllerHuesAndCues : MonoBehaviour
{
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float _turnSpeed = 360;

    [SerializeField] private float accelerationFactor = 5f;
    [SerializeField] private float decelerationFactor = 10f;


    [Header("Dash")]
    [SerializeField] private float dashingCooldown = 1.5f;
    [SerializeField] private float dashingTime = 0.2f;
    [SerializeField] private float dashingSpeed = 8f;

    private bool _canDash;
    private bool _isDashing;

    private bool _dashInput;

    private float _currentSpeed;
    private PIHuesAndCues _playerInputActions;
    private Vector3 _input;
    private CharacterController _characterController;
    private bool _isGrounded;

    private void Awake()
    {
        _playerInputActions = new PIHuesAndCues();
        _characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        _playerInputActions.Player.Enable();
        _canDash = true;
    }

    private void OnDisable()
    {
        _playerInputActions.Player.Disable();
    }

    private void Update()
    {

        GatherInput();

        Look();
        CalculareSpeed();

        Move();

        if (_dashInput && _canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;
        yield return new WaitForSeconds(dashingTime);
        _isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        _canDash = true;
    }

    private void CalculareSpeed()
    {
        if (_input == Vector3.zero && _currentSpeed > 0)
        {
            _currentSpeed -= decelerationFactor * Time.deltaTime;
        }
        else if (_input != Vector3.zero && _currentSpeed < maxSpeed)
        {
            _currentSpeed += accelerationFactor * Time.deltaTime;
        }

        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, maxSpeed);
    }

    private void Look()
    {
        if (_input == Vector3.zero) return;

        Quaternion rot = Quaternion.LookRotation(_input, Vector3.up);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTimes); // smooth rotation
        transform.rotation = rot;  //instant rotation, no smoothing
    }

    private void Move()
    {
        if(_isDashing)
        {
            _characterController.Move(transform.forward * _input.normalized.magnitude * dashingSpeed * Time.deltaTime);
            return;
        }
        _characterController.Move(transform.forward * _input.normalized.magnitude * _currentSpeed * Time.deltaTime);
    }

    private void GatherInput()
    {
        Vector2 input = _playerInputActions.Player.Move.ReadValue<Vector2>();
        _input = new Vector3(input.x, 0, input.y);
        _dashInput = _playerInputActions.Player.Sprint.IsPressed();
    }
}