using System.Collections;
using UnityEngine;
using static Define;

public class MonsterController : CreatureController
{
    private Coroutine _patrolRoutine;
    private Coroutine _searchRoutine;
    
    [SerializeField] private Vector3Int destCellPos;
    [SerializeField] private GameObject target;

    [SerializeField] private float searchRange = 5f;

    public override CreatureState State
    {
        get => state;
        set
        {
            if (state == value)
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
        Dir = MoveDir.None;

        moveSpeed = 3f;
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
        }

        var path = Managers.Map.FindPath(CellPos, destPos, ignoreDestCollision: true);
        if (path.Count < 2 || (target != null &&  path.Count > 10))
        {
            target = null;
            State = CreatureState.Idle;
            return;
        }

        Vector3Int nextPos = path[1];
        var moveCellDir = nextPos - CellPos;
        if (moveCellDir.x > 0)
        {
            Dir = MoveDir.Right;
        }
        else if (moveCellDir.x < 0)
        {
            Dir = MoveDir.Left;
        }
        else if (moveCellDir.y > 0)
        {
            Dir = MoveDir.Up;
        }
        else if (moveCellDir.y < 0)
        {
            Dir = MoveDir.Down;
        }
        else
        {
            Dir = MoveDir.None;
        }

        if (Managers.Map.CanGo(nextPos) && Managers.Object.Find(nextPos) == null)
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
        base.OnDamaged();
        var effect = Managers.Resource.Instantiate("Effect/DieEffect");
        effect.transform.position = gameObject.transform.position;
        effect.GetComponent<Animator>().Play("Start");
        Destroy(effect, 0.5f);

        Managers.Object.Remove(gameObject);
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

            if (Managers.Map.CanGo(randPos) && Managers.Object.Find(randPos) == null)
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
}