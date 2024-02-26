﻿using Google.Protobuf.Protocol;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public int Id { get; set; }

    private StatInfo _stat = new StatInfo();

    public virtual StatInfo Stat
    {
        get => _stat;
        set
        {
            if (_stat.Equals(value))
            {
                return;
            }

            _stat.Hp = value.Hp;
            _stat.MaxHp = value.MaxHp;
            _stat.Speed = value.Speed;
        }
    }

    public float Speed
    {
        get => Stat.Speed;
        set => Stat.Speed = value;
    }

    public virtual int Hp
    {
        get => Stat.Hp;
        set
        {
            if (Stat.Hp == value)
            {
                return;
            }

            Stat.Hp = value;
        }
    }

    protected bool _isUpdated = false;

    private PositionInfo _positionInfo = new();

    public PositionInfo PosInfo
    {
        get => _positionInfo;
        set
        {
            if (_positionInfo.Equals(value))
            {
                return;
            }

            CellPos = new Vector3Int(value.PosX, value.PosY, 0);
            State = value.State;
            Dir = value.MoveDir;
        }
    }

    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;

    public void SyncPos()
    {
        var destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = destPos;
    }

    public Vector3Int CellPos
    {
        get => new(PosInfo.PosX, PosInfo.PosY, 0);
        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
            {
                return;
            }

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            _isUpdated = true;
        }
    }

    public virtual CreatureState State
    {
        get => PosInfo.State;
        set
        {
            if (PosInfo.State == value)
            {
                return;
            }

            PosInfo.State = value;
            UpdateAnimation();
            _isUpdated = true;
        }
    }

    public MoveDir Dir
    {
        get => PosInfo.MoveDir;
        set
        {
            if (PosInfo.MoveDir == value)
            {
                return;
            }

            PosInfo.MoveDir = value;

            UpdateAnimation();
            _isUpdated = true;
        }
    }

    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
        {
            return MoveDir.Right;
        }

        if (dir.x < 0)
        {
            return MoveDir.Left;
        }

        if (dir.y > 0)
        {
            return MoveDir.Up;
        }

        if (dir.y < 0)
        {
            return MoveDir.Down;
        }

        return MoveDir.Down;
    }

    public Vector3Int GetFrontCellPos()
    {
        var cellPos = CellPos;

        switch (Dir)
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
                switch (Dir)
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
                switch (Dir)
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
                switch (Dir)
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

        State = CreatureState.Idle;
        Dir = MoveDir.Down;
        UpdateAnimation();
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

        if (dist < Speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir * (Speed * Time.deltaTime);
            State = CreatureState.Moving;
        }
    }

    protected virtual void MoveToNextPos()
    {
    }

    public virtual void UpdateSkill()
    {
    }

    public virtual void UpdateDead()
    {
    }
}