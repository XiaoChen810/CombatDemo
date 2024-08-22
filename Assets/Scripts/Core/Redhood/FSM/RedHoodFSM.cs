using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    // 处理玩家闪避，攻击，技能的状态切换
    public class RedHoodFSM : MonoBehaviour
    {
        private Dictionary<string, IRedHoodState> statesDictionary = new Dictionary<string, IRedHoodState>();
        private IRedHoodState currentState = null;
        private float duration = -1;

        private RedHood redHood;

        public RHStateType CurrentStateType => currentState.StateType;

        private void Start()
        {
            redHood = GetComponent<RedHood>();
            statesDictionary.Add("Idle", new RedHood_Idle(this, redHood));
            statesDictionary.Add("Air", new RedHood_Air(this, redHood));
            statesDictionary.Add("LightHit", new RedHood_LightHit(this, redHood));
            statesDictionary.Add("HeavyHit", new RedHood_HeavyHit(this, redHood));
            statesDictionary.Add("BowHit", new RedHood_BowHit(this, redHood));
            statesDictionary.Add("Sliding", new RedHood_Sliding(this, redHood));
            statesDictionary.Add("Dodge", new RedHood_Dodge(this, redHood)); 
            statesDictionary.Add("Hurt", new RedHood_Hurt(this, redHood));
            statesDictionary.Add("SpecialSkill", new RedHood_SpecialSkill(this, redHood));
            ChangeState("Idle");
        }

        public void ChangeState(string stateName)
        {
            if(!statesDictionary.ContainsKey(stateName))
            {
                Debug.LogWarning("不存在这个状态");
            }

            var next = statesDictionary[stateName];

            if (currentState != null) { currentState.OnExit(); }        
            currentState = next;   
            currentState.OnEnter();

            duration = currentState.MaxDuration;

            redHood.PreJump = false;
        }

        [SerializeField] private RHStateType currentStateType;
        public void Update()
        {
            // 仅用于Debug
            currentStateType = CurrentStateType;

            if (duration > 0)
            {
                duration -= Time.deltaTime;

                if(duration <= 0)
                {
                    ChangeState("Idle");
                }
            }

            currentState.OnUpdate();
        }
    }
}
