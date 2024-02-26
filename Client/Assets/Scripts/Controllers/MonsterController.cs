using Google.Protobuf.Protocol;
using UnityEngine;

public class MonsterController : CreatureController
{
    private Coroutine _skillRoutine;
    
    public override CreatureState State
    {
        get => PosInfo.State;
        set
        {
            if (PosInfo.State == value)
            {
                return;
            }

            base.State = value;
        }
    }

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
    }

    public override void OnDamaged()
    {
        // Managers.Object.Remove(Id);
        // Managers.Resource.Destroy(gameObject);
    }
    
    public override void UseSkill(int skillId)
    {
        if (skillId == 1)
        {
            State = CreatureState.Skill;
        }
    }
}