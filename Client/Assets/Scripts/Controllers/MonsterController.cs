using UnityEngine;
using static Define;

namespace Controllers
{
    public class MonsterController : CreatureController
    {
        protected override void Init()
        {
            base.Init();
        }

        protected override void UpdateController()
        {
            // GetDirInput();
            base.UpdateController();
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
    }
}