using System;
using UnityEngine;
using static Define;

namespace Controllers
{
    public class CreatureController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;

        protected Animator _animator;
        protected SpriteRenderer _spriteRenderer;
        protected Vector3Int _cellPos = Vector3Int.zero;

        private CreatureState _state = CreatureState.Idle;

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

        private MoveDir _lastDir = MoveDir.Down;
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

                _dir = value;
                if (_dir != MoveDir.None)
                {
                    _lastDir = _dir;
                }
                
                UpdateAnimation();
            }
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
                    switch (_lastDir)
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
                    break;
                case CreatureState.Dead:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
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
            UpdatePosition();
        }

        private void Update()
        {
            UpdateController();
        }

        protected virtual void UpdateController()
        {
            UpdatePosition();
            UpdateIsMoving();
        }

        private void UpdatePosition()
        {
            var destPos = Managers.Map.CurrentGrid.CellToWorld(_cellPos) + new Vector3(0.5f, 0.5f);
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
                if(_dir == MoveDir.None)
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

        private void UpdateIsMoving()
        {
            if (State == CreatureState.Idle && _dir != MoveDir.None)
            {
                Vector3Int destPos = _cellPos;

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

                if (Managers.Map.CanGo(destPos))
                {
                    _cellPos = destPos;
                    State = CreatureState.Moving;
                }
            }
        }
    }
}