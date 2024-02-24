using System;
using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;

    protected CreatureState _state = CreatureState.Idle;

    public CreatureState State
    {
        get => _state;
        set
        {
            if (_state == value)
            {
                return;
            }

            _state = value;

            UpdateAnimation();
        }
    }

    protected MoveDir _lastDir = MoveDir.Down;
    protected MoveDir _dir = MoveDir.Down;

    public MoveDir Dir
    {
        get => _dir;
        set
        {
            if (_dir == value)
            {
                return;
            }

            _dir = value;
            if (_dir != MoveDir.None)
            {
                _lastDir = _dir;
            }

            UpdateAnimation();
        }
    }

    public Vector3Int GetFrontCellPos()
    {
        var cellPos = CellPos;

        switch (_lastDir)
        {
            case MoveDir.Up:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.Down:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.Left:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.Right:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    protected virtual void UpdateAnimation()
    {
        switch (State)
        {
            case CreatureState.Idle:
            {
                switch (_lastDir)
                {
                    case MoveDir.Up:
                        _animator.Play("IdleBack");
                        _spriteRenderer.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play("IdleFront");
                        _spriteRenderer.flipX = false;
                        break;
                    case MoveDir.Left:
                        _animator.Play("IdleRight");
                        _spriteRenderer.flipX = true;
                        break;
                    case MoveDir.Right:
                        _animator.Play("IdleRight");
                        _spriteRenderer.flipX = false;
                        break;
                }
            }
                break;
            case CreatureState.Moving:
            {
                switch (_dir)
                {
                    case MoveDir.Up:
                        _animator.Play("WalkBack");
                        _spriteRenderer.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play("WalkFront");
                        _spriteRenderer.flipX = false;
                        break;
                    case MoveDir.Left:
                        _animator.Play("WalkRight");
                        _spriteRenderer.flipX = true;
                        break;
                    case MoveDir.Right:
                        _animator.Play("WalkRight");
                        _spriteRenderer.flipX = false;
                        break;
                }
            }
                break;
            case CreatureState.Skill:
                switch (_lastDir)
                {
                    case MoveDir.Up:
                        _animator.Play("AttackBack");
                        _spriteRenderer.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play("AttackFront");
                        _spriteRenderer.flipX = false;
                        break;
                    case MoveDir.Left:
                        _animator.Play("AttackRight");
                        _spriteRenderer.flipX = true;
                        break;
                    case MoveDir.Right:
                        _animator.Play("AttackRight");
                        _spriteRenderer.flipX = false;
                        break;
                }

                break;
            case CreatureState.Dead:
                break;
        }
    }

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        var initPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = initPos;

        UpdateMoving();
    }

    private void Update()
    {
        UpdateController();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                UpdateIdle();
                break;
            case CreatureState.Moving:
                UpdateMoving();
                break;
            case CreatureState.Skill:
                UpdateSkill();
                break;
            case CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
        if (_dir != MoveDir.None)
        {
            Vector3Int destPos = CellPos;

            switch (_dir)
            {
                case MoveDir.Up:
                    destPos += Vector3Int.up;
                    break;
                case MoveDir.Down:
                    destPos += Vector3Int.down;
                    break;
                case MoveDir.Left:
                    destPos += Vector3Int.left;
                    break;
                case MoveDir.Right:
                    destPos += Vector3Int.right;
                    break;
            }

            State = CreatureState.Moving;
            if (Managers.Map.CanGo(destPos))
            {
                if (Managers.Object.Find(destPos) == null)
                {
                    CellPos = destPos;
                }
            }
        }
    }

    protected virtual void UpdateMoving()
    {
        var destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        var moveDir = destPos - transform.position;

        if (State != CreatureState.Moving)
        {
            return;
        }

        // 도착 여부 체크
        var dist = moveDir.magnitude;
        moveDir.Normalize();

        if (dist < moveSpeed * Time.deltaTime)
        {
            transform.position = destPos;
            _state = CreatureState.Idle;
            if (_dir == MoveDir.None)
            {
                UpdateAnimation();
            }
        }
        else
        {
            transform.position += moveDir * (moveSpeed * Time.deltaTime);
            State = CreatureState.Moving;
        }
    }

    public virtual void UpdateSkill()
    {
        
    }

    public virtual void UpdateDead()
    {
        
    }

    public virtual void OnDamaged()
    {
        
    }
}