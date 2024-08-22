using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public enum DirType
    {
        Forward, Backward, Top, Bottom
    }

    [System.Serializable]
    public class FixedForce
    {
        public RHStateType StateType;
        public DirType Dir = DirType.Forward;
        public float Force = 0;
    }

    public class RedHoodFixForce : MonoBehaviour
    {
        [SerializeField] private FixedForceSetting setting;
        [SerializeField] private RedHood rh;
        [SerializeField] private Rigidbody rb;

        private List<FixedForce> forceList;

        private void Start()
        {
            forceList = setting.ForceList;
        }

        private void FixedUpdate()
        {
            foreach (FixedForce force in forceList)
            {
                if (force.StateType == rh.CurrentStateType)
                {
                    // ������������ܵ��ж��������ʾ
                    if (force.StateType == RHStateType.DodgeLeft)
                    {
                        if (rb.velocity.x > 0) // ���X�����ٶȴ���0�������ʩ��������
                        {
                            rb.AddForce(new Vector3(force.Force, 0, 0), ForceMode.Force);
                        }
                        return;
                    }

                    if (force.StateType == RHStateType.DodgeRight)
                    {
                        if (rb.velocity.x < 0) // ���X�����ٶ�С��0�������ʩ��������
                        {
                            rb.AddForce(new Vector3(-force.Force, 0, 0), ForceMode.Force);
                        }
                        return;
                    }

                    switch (force.Dir)
                    {
                        case DirType.Forward:
                            rb.AddForce(new Vector3(force.Force * rh.Facing, 0, 0), ForceMode.Force);
                            break;
                        case DirType.Backward:
                            rb.AddForce(new Vector3(-force.Force * rh.Facing, 0, 0), ForceMode.Force);
                            break;
                        case DirType.Top:
                            rb.AddForce(new Vector3(0, force.Force, 0), ForceMode.Force);
                            break;
                        case DirType.Bottom:
                            rb.AddForce(new Vector3(0, -force.Force, 0), ForceMode.Force);
                            break;
                    }
                }
            }
        }

        //private void FixedUpdate()
        //{
        //    foreach (FixedForce force in forceList)
        //    {
        //        if(force.StateType == rh.CurrentStateType)
        //        {
        //            // ������������ܵ��ж��������ʾ
        //            if (force.StateType == RHStateType.DodgeLeft)
        //            {
        //                rb.AddForce(new Vector3(force.Force, 0, 0), ForceMode.Force);
        //                return;
        //            }

        //            if (force.StateType == RHStateType.DodgeRight)
        //            {
        //                rb.AddForce(new Vector3(-force.Force, 0, 0), ForceMode.Force);
        //                return;
        //            }

        //            switch (force.Dir)
        //            {
        //                case DirType.Forward:
        //                    rb.AddForce(new Vector3(force.Force * rh.Facing, 0, 0), ForceMode.Force);
        //                    break;
        //                case DirType.Backward:
        //                    rb.AddForce(new Vector3(-force.Force * rh.Facing, 0, 0), ForceMode.Force);
        //                    break;
        //                case DirType.Top:
        //                    rb.AddForce(new Vector3(0, force.Force, 0), ForceMode.Force);
        //                    break;
        //                case DirType.Bottom:
        //                    rb.AddForce(new Vector3(0, -force.Force, 0), ForceMode.Force);
        //                    break;
        //            }
        //        }
        //    }
        //}
    }
}