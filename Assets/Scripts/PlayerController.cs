using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Animator _animator;

    private bool groundedPlayer = false;

    [SerializeField]
    private float playerSpeed = 10.0f;
    [SerializeField]
    private float jumpHeight = 30f;

    private Dictionary<Vector3, float> _rotationMap;
    private float movement = 0f;
    private float currentRotation = -1f;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        InitializeRotationMap();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        movement = Mathf.Abs(move.x) + Mathf.Abs(move.z);

        if (movement != 0)
        {
            var horizontalMovement = Input.GetAxis("Horizontal") > 0 ? Mathf.Ceil(Input.GetAxis("Horizontal")) : Mathf.Floor(Input.GetAxis("Horizontal"));
            var verticalMovement = Input.GetAxis("Vertical") > 0 ? Mathf.Ceil(Input.GetAxis("Vertical")) : Mathf.Floor(Input.GetAxis("Vertical"));

            _rotationMap.TryGetValue(new Vector3(horizontalMovement, 0, verticalMovement), out currentRotation);

            _rigidbody.rotation = Quaternion.Euler(0, currentRotation, 0);
        }

        move *= playerSpeed;
        move.y = _rigidbody.velocity.y;

        _rigidbody.velocity = move;

        if (Input.GetButton("Jump") && groundedPlayer)
        {
            groundedPlayer = false;

            _rigidbody.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }

        _animator.SetFloat("Movement", movement);
        _animator.SetBool("Grounded", groundedPlayer);

        Debug.Log($"Grounded: {groundedPlayer}");
    }

    private void InitializeRotationMap()
    {
        _rotationMap = new Dictionary<Vector3, float>();

        _rotationMap.Add(new Vector3(0, 0, 1), 0);
        _rotationMap.Add(new Vector3(0, 0, -1), 180);
        _rotationMap.Add(new Vector3(1, 0, 0), 90);
        _rotationMap.Add(new Vector3(-1, 0, 0), -90);
        _rotationMap.Add(new Vector3(-1, 0, 1), -45);
        _rotationMap.Add(new Vector3(1, 0, 1), 45);
        _rotationMap.Add(new Vector3(1, 0, -1), 135);
        _rotationMap.Add(new Vector3(-1, 0, -1), -135);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.transform.position.y <= transform.position.y)
            groundedPlayer = true;
    }
}