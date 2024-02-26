using System.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MonsterController : CreatureController
{
    private Coroutine _patrolRoutine;
    private Coroutine _searchRoutine;
    private Coroutine _skillRoutine;

    [SerializeField] private Vector3Int destCellPos;
    [SerializeField] private GameObject target;

    [SerializeField] private float searchRange = 10f;
    [SerializeField] private float skillRange = 1f;

    [SerializeField] private bool isRangedSkill;

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
            if (_patrolRoutine != null)
            {
                StopCoroutine(_patrolRoutine);
                _patrolRoutine = null;
            }
        }
    }

    protected override void Init()
    {
        base.Init();
        
        State = CreatureState.Idle;
        Dir = MoveDir.Down;

        isRangedSkill = Random.Range(0, 2) == 0;

        skillRange = isRangedSkill ? 10f : 1f;
    }

    protected override void UpdateIdle()
    {
        base.UpdateIdle();

        if (_patrolRoutine == null)
        {
            _patrolRoutine = StartCoroutine(CoPatrol());
        }

        if (_searchRoutine == null)
        {
            _searchRoutine = StartCoroutine(CoSearch());
        }
    }

    protected override void MoveToNextPos()
    {
        Vector3Int destPos = destCellPos;
        if (target != null)
        {
            destPos = target.GetComponent<CreatureController>().CellPos;

            var dir = destPos - CellPos;
            if (dir.magnitude <= skillRange && (dir.x == 0 || dir.y == 0))
            {
                Dir = GetDirFromVec(dir);
                State = CreatureState.Skill;

                if (isRangedSkill)
                {
                    _skillRoutine = StartCoroutine(CoStartShootArrow());
                }
                else
                {
                    _skillRoutine = StartCoroutine(CoStartPunch());
                }
                
                return;
            }
        }

        var path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        if (path.Count < 2 || (target != null && path.Count > 20))
        {
            target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        var moveCellDir = nextPos - CellPos;
        Dir = GetDirFromVec(moveCellDir);

        if (Managers.Map.CanGo(nextPos) && Managers.Object.FindCreature(nextPos) == null)
        {
            CellPos = nextPos;
        }
        else
        {
            State = CreatureState.Idle;
        }
    }

    public override void OnDamaged()
    {
        Managers.Object.Remove(Id);
        Managers.Resource.Destroy(gameObject);
    }

    private IEnumerator CoPatrol()
    {
        var waitSeconds = Random.Range(1, 4);
        yield return new WaitForSeconds(waitSeconds);

        for (var i = 0; i < 10; i++)
        {
            var xRange = Random.Range(-5, 6);
            var yRange = Random.Range(-5, 6);
            var randPos = CellPos + new Vector3Int(xRange, yRange, 0);

            if (Managers.Map.CanGo(randPos) && Managers.Object.FindCreature(randPos) == null)
            {
                destCellPos = randPos;
                State = CreatureState.Moving;
                yield break;
            }
        }

        State = CreatureState.Idle;
    }

    private IEnumerator CoSearch()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (target != null)
            {
                continue;
            }

            target = Managers.Object.Find((go) =>
            {
                var playerController = go.GetComponent<PlayerController>();
                if (playerController == null)
                {
                    return false;
                }

                var dir = playerController.CellPos - CellPos;
                if (dir.magnitude > searchRange)
                {
                    return false;
                }

                return true;
            });
        }
    }

    private IEnumerator CoStartPunch()
    {
        var go = Managers.Object.FindCreature(GetFrontCellPos());
        if (go != null)
        {
            var creatureController = go.GetComponent<CreatureController>();
            if (creatureController != null)
            {
                creatureController.OnDamaged();
            }
        }

        yield return new WaitForSeconds(0.5f);
        State = CreatureState.Moving;
        _skillRoutine = null;
    }

    private IEnumerator CoStartShootArrow()
    {
        var go = Managers.Resource.Instantiate("Creature/Arrow");
        var arrowController = go.GetComponent<ArrowController>();
        arrowController.Dir = Dir;
        arrowController.CellPos = CellPos;

        yield return new WaitForSeconds(0.3f);
        State = CreatureState.Idle;
        _skillRoutine = null;
    }
}