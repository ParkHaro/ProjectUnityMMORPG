using System.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;

public class PlayerController : CreatureController
{
    protected Coroutine _skillRoutine;
    protected bool _isRangedSkill;

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
        base.UpdateController();
    }
    
    protected override void UpdateIdle()
    {
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }
    }
    
    protected IEnumerator CoStartPunch()
    {
        var go = Managers.Object.Find(GetFrontCellPos());
        if (go != null)
        {
            var creatureController = go.GetComponent<CreatureController>();
            if (creatureController != null)
            {
                creatureController.OnDamaged();
            }
        }

        _isRangedSkill = false;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _skillRoutine = null;
    }

    protected IEnumerator CoStartShootArrow()
    {
        var go = Managers.Resource.Instantiate("Creature/Arrow");
        var arrowController = go.GetComponent<ArrowController>();
        arrowController.Dir = _lastDir;
        arrowController.CellPos = CellPos;

        _isRangedSkill = true;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _skillRoutine = null;
    }
}