using System;
using Scripts.Input;
using Unity.Netcode;
using UnityEngine;

//TODO: use rigidBody instead of CharacterController
public class Character : NetworkBehaviour
{
    
    [SerializeField] private InputReader inputReader;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.8f;
    [SerializeField] private Vector3 movementVelocity;
    
    [SerializeField] private Animator animator;

    private CharacterController _characterController;
    private float _verticalVelocity;

    private Camera _mainCam;
    private GameManager _gameManager;

    private int _walkingAnimationHash = Animator.StringToHash("Walking");
    
    public override void OnNetworkSpawn()
    {
        Debug.Log("Character spawned OnNetworkSpawn");
        _gameManager = GameManager.Instance;
        if (_characterController == null) _characterController = GetComponent<CharacterController>();
        _mainCam = Camera.main;

        if (IsServer)
        {
            _gameManager.AddCharacter(this);
            transform.position = _gameManager.LevelSpawnPoints.GetRandomSpawnPoint();
        }
        
        //CharacterController prevents setting player position rigid bodies do not have this problem
        if(IsOwner) _characterController.enabled = true;
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;

        SetVelocity(inputReader.MovementValue);
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
        if(velocity.magnitude > 0f)
            animator.SetBool(_walkingAnimationHash, true);
        else
            animator.SetBool(_walkingAnimationHash, false);
        movementVelocity.Set(velocity.x, 0, velocity.y);
    }

    private void CalculateMovement()
    {
        //TODO: Fix getting main camera for first few frames
        if (_mainCam == null)
        {
            _mainCam = Camera.main;
            return;
        };
        //Align to camera
        movementVelocity = Quaternion.Euler(0, _mainCam.transform.eulerAngles.y, 0) * movementVelocity;
        movementVelocity *= speed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, _mainCam.transform.eulerAngles.y, 0);
    }
}
