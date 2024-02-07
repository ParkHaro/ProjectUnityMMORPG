using System;
using UnityEngine;
using static Define;

public class PlayerController : MonoBehaviour
{
    private Grid _grid;
    private Animator _animator;

    [SerializeField] private float moveSpeed = 5f;

    private MoveDir _dir = MoveDir.Down;

    public MoveDir Dir
    {
        get => _dir;
        set
        {
            if (_dir == value)
            {
                return;
            }

            switch (value)
            {
                case MoveDir.None:
                    switch (_dir)
                    {
                        case MoveDir.Up:
                            _animator.Play("IdleBack");
                            transform.localScale = new Vector3(1f, 1f, 1f);
                            break;
                        case MoveDir.Down:
                            _animator.Play("IdleFront");
                            transform.localScale = new Vector3(1f, 1f, 1f);
                            break;
                        case MoveDir.Left:
                            _animator.Play("IdleRight");
                            transform.localScale = new Vector3(-1f, 1f, 1f);
                            break;
                        case MoveDir.Right:
                            _animator.Play("IdleRight");
                            transform.localScale = new Vector3(1f, 1f, 1f);
                            break;
                    }
                    break;
                case MoveDir.Up:
                    _animator.Play("WalkBack");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case MoveDir.Down:
                    _animator.Play("WalkFront");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
                case MoveDir.Left:
                    _animator.Play("WalkRight");
                    transform.localScale = new Vector3(-1f, 1f, 1f);
                    break;
                case MoveDir.Right:
                    _animator.Play("WalkRight");
                    transform.localScale = new Vector3(1f, 1f, 1f);
                    break;
            }

            _dir = value;
        }
    }

    private Vector3Int _cellPos = Vector3Int.zero;
    private bool _isMoving = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _grid = FindObjectOfType<Grid>();

        var pos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;
    }

    private void Update()
    {
        GetDirInput();
        UpdatePosition();
        UpdateIsMoving();
    }

    private void GetDirInput()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.Up;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.Down;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.Left;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.Right;
        }
        else
        {
            Dir = MoveDir.None;
        }
    }

    private void UpdatePosition()
    {
        var destPos = _grid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
        var moveDir = destPos - transform.position;
        
        // 도착 여부 체크
        var dist = moveDir.magnitude;
        moveDir.Normalize();
        
        if (dist < moveSpeed * Time.deltaTime)
        {
            transform.position = destPos;
            _isMoving = false;
        }
        else
        {
            transform.position += moveDir * (moveSpeed * Time.deltaTime);
            _isMoving = true;
        }
    }

    private void UpdateIsMoving()
    {
        if (_isMoving == false)
        {
            switch (_dir)
            {
                case MoveDir.Up:
                    _cellPos += Vector3Int.up;
                    _isMoving = true;
                    break;
                case MoveDir.Down:
                    _cellPos += Vector3Int.down;
                    _isMoving = true;
                    break;
                case MoveDir.Left:
                    _cellPos += Vector3Int.left;
                    _isMoving = true;
                    break;
                case MoveDir.Right:
                    _cellPos += Vector3Int.right;
                    _isMoving = true;
                    break;
            }
        }
    }
}