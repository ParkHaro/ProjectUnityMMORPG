using Google.Protobuf.Protocol;
using UnityEngine;

public class CreatureController : BaseController
{
    private HpBar _hpBar;
    
    public override StatInfo Stat
    {
        get => base.Stat;
        set
        {
            base.Stat = value;
            UpdateHpBar();
        }
    }
    
    public override int Hp
    {
        get => base.Hp;
        set
        {
            base.Hp = value;
            UpdateHpBar();
        }
    }
    
    protected void AddHpBar()
    {
        GameObject go = Managers.Resource.Instantiate("UI/HpBar", transform);
        go.transform.localPosition = new Vector3(0, 0.5f, 0f);
        go.name = "HpBar";
        _hpBar = go.GetComponent<HpBar>();
        UpdateHpBar();
    }

    private void UpdateHpBar()
    {
        if (_hpBar == null)
        {
            return;
        }

        var ratio = 0f;
        if (Stat.MaxHp > 0)
        {
            ratio = ((float)Hp) / Stat.MaxHp;
        }

        Debug.Log($"Ratio : {ratio}");
        _hpBar.SetHpBar(ratio);
    }

    protected override void Init()
    {
        base.Init();
        AddHpBar();
    }

    public virtual void OnDamaged()
    {
    }

    public virtual void OnDead()
    {
        State = CreatureState.Dead;
        
        var effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = gameObject.transform.position;
        effect.GetComponent<Animator>().Play("Start");
        Destroy(effect, 0.5f);
    }

    public virtual void UseSkill(int skillId)
    {
    }
}