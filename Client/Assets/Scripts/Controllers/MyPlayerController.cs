using System.Collections;
using Google.Protobuf.Protocol;
using UnityEngine;

public class MyPlayerController : PlayerController
{
    private bool _isMoveKeyPressed;

    protected override void Init()
    {
        base.Init();
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

    protected override void UpdateIdle()
    {
        base.UpdateIdle();
        if (_isMoveKeyPressed)
        {
            State = CreatureState.Moving;
            return;
        }
        
        if (_inputCooltimeRoutine == null && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Skill !!");
            
            var skillPacket = new C_Skill
            {
                Info = new SkillInfo
                {
                    SkillId = 2
                }
            };
            Managers.Network.Send(skillPacket);
            
            _inputCooltimeRoutine = StartCoroutine(CoInputCooltime(0.2f));
        }
    }

    private Coroutine _inputCooltimeRoutine;
    private IEnumerator CoInputCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        _inputCooltimeRoutine = null;
    }

    private void LateUpdate()
    {
        Camera.main.transform.position =
            new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
    }
    
    private void GetDirInput()
    {
        _isMoveKeyPressed = true;
        
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
            _isMoveKeyPressed = false;
        }
    }
    
    protected override void MoveToNextPos()
    {
        if (_isMoveKeyPressed == false)
        {
            State = CreatureState.Idle;
            CheckUpdatedFlag();
            return;
        }

        Vector3Int destPos = CellPos;

        switch (Dir)
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
        
        CheckUpdatedFlag();
    }

    protected override void CheckUpdatedFlag()
    {
        if (_isUpdated)
        {
            var movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            _isUpdated = false;
        }
    }
}