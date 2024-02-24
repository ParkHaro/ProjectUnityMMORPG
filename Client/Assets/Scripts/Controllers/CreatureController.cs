﻿using UnityEngine;
using static Define;

public class CreatureController : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 5f;

    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    public Vector3Int CellPos { get; set; } = Vector3Int.zero;

    [SerializeField] protected CreatureState state = CreatureState.Idle;

    public virtual CreatureState State
    {
        get => state;
        set
        {
            if (state == value)
            {
                return;
            }

            state = value;

            UpdateAnimation();
        }
    }

    protected MoveDir _lastDir = MoveDir.Down;
    [SerializeField] protected MoveDir dir = MoveDir.Down;

    public MoveDir Dir
    {
        get => dir;
        set
        {
            if (dir == value)
            {
                return;
            }

            dir = value;
            if (dir != MoveDir.None)
            {
                _lastDir = dir;
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
                switch (dir)
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
    }

    protected virtual void UpdateMoving()
    {
        var destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        var moveDir = destPos - transform.position;

        // 도착 여부 체크
        var dist = moveDir.magnitude;
        moveDir.Normalize();

        if (dist < moveSpeed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir * (moveSpeed * Time.deltaTime);
            State = CreatureState.Moving;
        }
    }

    protected virtual void MoveToNextPos()
    {
        if (dir == MoveDir.None)
        {
            State = CreatureState.Idle;
            return;
        }

        Vector3Int destPos = CellPos;

        switch (dir)
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

        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.Find(destPos) == null)
            {
                CellPos = destPos;
            }
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