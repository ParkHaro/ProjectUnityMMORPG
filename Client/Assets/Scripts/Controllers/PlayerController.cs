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
        if (_animator == null || _spriteRenderer == null)
        {
            return;
        }
        
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

    public void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            _skillRoutine = StartCoroutine(CoStartPunch());
        }
        else if (skillId == 2)
        {
            _skillRoutine = StartCoroutine(CoStartShootArrow());
        }
    }

    protected virtual void CheckUpdatedFlag()
    {
    }

    protected IEnumerator CoStartPunch()
    {
        _isRangedSkill = false;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Idle;
        _skillRoutine = null;
        CheckUpdatedFlag();
    }

    protected IEnumerator CoStartShootArrow()
    {
        _isRangedSkill = true;
        State = CreatureState.Skill;
        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _skillRoutine = null;
        CheckUpdatedFlag();
    }
}