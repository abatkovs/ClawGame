using System;
using Scripts.Input;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private Vector3 movementVelocity;

    private CharacterController _characterController;
    private float _verticalVelocity;

    private Camera _mainCam;

    private InputReader _input;
    
    private void Awake()
    {
        _mainCam = Camera.main;
    }

    private void Start()
    {
        _input = InputReader.Instance;
        if (_characterController == null) _characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        SetVelocity(_input.MovementValue);
        CalculateMovement();
        
        if (!_characterController.isGrounded) 
            _verticalVelocity = gravity;
        else
            _verticalVelocity = gravity * 0.3f;
            
        movementVelocity += Vector3.up * (_verticalVelocity * Time.deltaTime);
            
        _characterController.Move(movementVelocity);
    }
        
    public void SetVelocity(Vector3 velocity)
    {
        movementVelocity.Set(velocity.x, 0, velocity.y);
    }

    public void SetCharacterController(CharacterController controller)
    {
        _characterController = controller;
    }

    private void CalculateMovement()
    {
        //Align to camera
        movementVelocity = Quaternion.Euler(0, _mainCam.transform.eulerAngles.y, 0) * movementVelocity;
        movementVelocity *= speed * Time.deltaTime;
    }
}
