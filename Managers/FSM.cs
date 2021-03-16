using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FSM
{
    public class FSM : MonoBehaviour
    {
       
    }
    public abstract class BaseState//抽象类
    {
        protected FsmManager _fsmManager;//
                                         //构造函数
        public BaseState(FsmManager fsmManager)
        {
            _fsmManager = fsmManager;
        }
        //进入状态
        public abstract void EnterState();
        //离开状态
        public abstract void ExitState();
        //更新状态
        public abstract void UpdateState();
    }

    public class FsmManager
    {
        private BaseState currentState;//当前状态
        private Dictionary<string, BaseState> enemyStateDic = new Dictionary<string, BaseState>();
        public FsmManager() { }
        //状态注册
        public void Region(string statename, BaseState state)
        {
            if (!enemyStateDic.ContainsKey(statename))
            {
                enemyStateDic.Add(statename, state);
            }
        }
        //设置默认状态
        public void SetDefaultState(string statename)
        {
            if (enemyStateDic.ContainsKey(statename))
            {
                currentState = enemyStateDic[statename];
                currentState.EnterState();
            }
        }
        //改变状态
        public void ChangeState(string statename)
        {
            if (enemyStateDic.ContainsKey(statename))
            {
                if (currentState != null)
                {
                    if (!(currentState == enemyStateDic[statename]))
                    {
                        currentState.ExitState();
                        currentState = enemyStateDic[statename];
                        currentState.EnterState();
                    }

                }
            }
        }
        //更新状态
        public void UpdateState()
        {
            if (currentState != null)
            {
                currentState.UpdateState();
            }
        }
    }
}
