using Google.Protobuf.Protocol;
using UnityEngine;

public class MyPlayerController : PlayerController
{
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
        if (Dir != MoveDir.None)
        {
            State = CreatureState.Moving;
            return;
        }
        
        if (Input.GetKey(KeyCode.Space))
        {
            State = CreatureState.Skill;
            // _skillRoutine = StartCoroutine(CoStartPunch());
            _skillRoutine = StartCoroutine(CoStartShootArrow());
        }
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
    
    protected override void MoveToNextPos()
    {
        var prevState = State;
        var prevCellPos = CellPos;
        
        base.MoveToNextPos();

        if (prevState != State || prevCellPos != CellPos)
        {
            var movePacket = new C_Move();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
        }
    }
}