using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;

    private GameObject attackTarget;
    private float lastAttackTime;
    private bool isDead;
    private float stopDistance;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stopDistance = agent.stoppingDistance;
    }
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStats);
    }

    void Update()
    {
        isDead = characterStats.CurrentHealth == 0;
        if (isDead)
        {
            //广播角色的死亡
            GameManager.Instance.NotifyObservers();
        }
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }
    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();//打断攻击
        if (isDead) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }
    private void EventAttack(GameObject target)
    {
        if (isDead) return;
        if (target != null)
        {
            attackTarget = target;
            //计算暴击
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;

        transform.LookAt(attackTarget.transform);
        
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;//表示暂缓一帧，在下一帧接着往下处理
        }
        agent.isStopped = true;
        //执行攻击
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>()&& attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
        
    }

}
