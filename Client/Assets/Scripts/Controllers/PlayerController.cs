using System.Collections;
using UnityEngine;
using static Define;

public class PlayerController : CreatureController
{
    private Coroutine skillRoutine;
    private bool _isRangedSkill;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateAnimation()
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
                        _animator.Play(_isRangedSkill ? "AttackWeaponBack" : "AttackBack");
                        _spriteRenderer.flipX = false;
                        break;
                    case MoveDir.Down:
                        _animator.Play(_isRangedSkill ? "AttackWeaponFront" : "AttackFront");
                        _spriteRenderer.flipX = false;
                        break;
                    case MoveDir.Left:
                        _animator.Play(_isRangedSkill ? "AttackWeaponRight" : "AttackRight");
                        _spriteRenderer.flipX = true;
                        break;
                    case MoveDir.Right:
                        _animator.Play(_isRangedSkill ? "AttackWeaponRight" : "AttackRight");
                        _spriteRenderer.flipX = false;
                        break;
                }

                break;
            case CreatureState.Dead:
                break;
        }
    }

    protected override void UpdateController()
    {
        switch (State)
        {
            case CreatureState.Idle:
                GetDirInput();
                break;
            case CreatureState.Moving:
                GetDirInput();
                break;
        }

        base.UpdateController();
    }

    private void LateUpdate()
    {
        Camera.main.transform.position =
            new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
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

    protected override void UpdateIdle()
    {
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            // skillRoutine = StartCoroutine(CoStartPunch());
            skillRoutine = StartCoroutine(CoStartShootArrow());
        }
    }
    
    private IEnumerator CoStartPunch()
    {
        var go = Managers.Object.Find(GetFrontCellPos());
        if (go != null)
        {
            Debug.Log(go.name);
        }

        _isRangedSkill = false;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        skillRoutine = null;
    }

    private IEnumerator CoStartShootArrow()
    {
        var go = Managers.Resource.Instantiate("Creature/Arrow");
        var arrowController = go.GetComponent<ArrowController>();
        arrowController.Dir = _lastDir;
        arrowController.CellPos = CellPos;

        _isRangedSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        skillRoutine = null;
    }
}