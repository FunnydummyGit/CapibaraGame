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

    [SerializeField] private MeshRenderer ColoredObject0, ColoredObject1, ColoredObject2;

    [SerializeField] private MeshRenderer Arrow;

    private bool _canDash;
    private bool _isDashing;

    private bool _dashInput;
    private bool _switch;

    private float _currentSpeed;
    private PIHuesAndCues _playerInputActions;
    private Vector3 _input;
    private CharacterController _characterController;

    private float[][] colorValues =
{
    new float[] { 1.0f, 1.0f, 1.0f },
    new float[] { 1.0f, 1.0f, 1.0f },
    new float[] { 1.0f, 1.0f, 1.0f }
};

    private int selectedObjectNumber = 0;


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

        if (_switch)
        {
            SwitchObject();
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

    private void SwitchObject()
    {
        selectedObjectNumber = (selectedObjectNumber + 1) % 3; // Cycle through 0, 1, 2

        switch (selectedObjectNumber)
        {
            case 0:
                Arrow.transform.position = ColoredObject0.transform.position + new Vector3(0, 0, 2);
                break;
            case 1:
                Arrow.transform.position = ColoredObject1.transform.position + new Vector3(0, 0, 2);
                break;
            case 2:
                Arrow.transform.position = ColoredObject2.transform.position + new Vector3(0, 0, 2);
                break;
        }
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
        _switch = _playerInputActions.Player.Switch.WasCompletedThisFrame();

    }

    // Expose Interact state for external components (e.g., picker controls)
    public bool IsInteractPressed()
    {
        if (_playerInputActions == null) return false;
        return _playerInputActions.Player.Interact.IsPressed();
    }

    public void ChangeColor(float hue, float sat, float val)
    {
        Color newColor = Color.HSVToRGB(hue, sat, val);
        switch (selectedObjectNumber)
        {
            case 0:
                ColoredObject0.material.SetColor("_BaseColor", newColor);
                break;
            case 1:
                ColoredObject1.material.SetColor("_BaseColor", newColor);
                break;
            case 2:
                ColoredObject2.material.SetColor("_BaseColor", newColor);
                break;
        }
        colorValues[selectedObjectNumber] = new float[] {hue, sat, val};
    } 

}