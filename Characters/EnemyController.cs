using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD,PATROL,CHASE,DEAD}
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameObserver
{
    private EnemyStates enemystates;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;
    protected CharacterStats characterStats;
   
    [Header("Basic Settings")]
    public  float sightRadius;//怪物可视范围
    public bool isGuard;//是否是守卫型的敌人
    private float speed;//记录原有速度
    public GameObject attackTarget;//攻击目标
    public float lookAtTime;//停留时间
    private float remainLookAtTime;
    private float lastAttackTime;//攻击CD
    private Quaternion guardRotation;//原有站桩角度

    [Header("Patrol State")]
    public float patrolRange;//巡逻范围
    private Vector3 wayPoint;
    private Vector3 guardPos;

    //bool配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead;//初始默认false

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }
    void Start()
    {
        if (isGuard)
        {
            enemystates = EnemyStates.GUARD;

        }
        else
        {
            enemystates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
        //GameManager.Instance.AddObserver(this);
    }
    //先加载场景再加载怪物,否则可能找不到GameManager而报错
    void OnEnable()
    {
        GameManager.Instance.AddObserver(this);
    }
    void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);

        if(GetComponent<LootSpwaner>()&&isDead)
        {
            GetComponent<LootSpwaner>().Spawnloot();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        if (characterStats.CurrentHealth == 0)
        {
            isDead = true;
        }
        if (!playerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    
    }
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }
    void SwitchStates()
    {
        if (isDead)
        {
            enemystates = EnemyStates.DEAD;
        }
        //如果发现player则切换到chase
        else if (FoundPlayer())
        {
            enemystates = EnemyStates.CHASE;
            //Debug.Log("找到player");
        }
        switch (enemystates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        //差值,缓慢
                        transform.rotation = Quaternion.Lerp(transform.rotation,guardRotation,0.01f);
                    }
                }
                break;
            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                //判断是否走到了随机巡逻点
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                    }
                   
                }else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;

            case EnemyStates.CHASE:   
                isWalk = false;
                isChase = true;    
                agent.speed = speed;
                //脱战则回到上一个状态
                if (!FoundPlayer())
                {
                    
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if(isGuard)
                    {
                      
                        enemystates = EnemyStates.GUARD;
                    }
                    else
                    {
                        enemystates = EnemyStates.PATROL;
                    }
                    
                }
                else
                {
                    agent.destination=attackTarget.transform.position;
                    //在攻击范围内则攻击
                    if (TargetInAttackRange()||TargetInSkillRange())
                    {
                        
                        isFollow = false;
                        agent.isStopped = true;
                        if (lastAttackTime < 0)
                        {
                            lastAttackTime = characterStats.attackData.coolDown;
                            //判断暴击
                            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
                            //执行攻击                            
                            Attack();
                        }
                    }
                    else
                    {
                        isFollow = true;
                        agent.isStopped = false;
                    }
                    
                }
                break;
            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;//会导致空引用
                agent.radius = 0;
                //死亡后延迟两秒销毁
                Destroy(gameObject, 2f);
                break;
        }
    }
    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //近距离攻击动画
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("Skill");
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position,sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return (agent.remainingDistance) <= characterStats.attackData.attackRange;
        } 
        else
        {
            return false;
        }         
    }
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return (agent.remainingDistance) <= characterStats.attackData.skillRange;
        else
            return false;
    }

    void GetNewWayPoint()
    {
        //还原停留时间
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        //随机点不能是障碍物
        NavMeshHit hit;
        //1指的是navmesh area里walkable的cost
        wayPoint = NavMesh.SamplePosition(randomPoint,out hit, patrolRange, 1) ? hit.position : transform.position;
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget != null&&transform.IsFacingTarget(attackTarget.transform))
        {
            //UnityEngine.Debug.Log(agent.remainingDistance);
            if(agent.remainingDistance<=characterStats.attackData.attackRange)
            {
                var targetStats = attackTarget.GetComponent<CharacterStats>();
                targetStats.TakeDamage(characterStats, targetStats);
            } 
        }
        
    }

    public void EndNotify()
    {
        //获胜动画
        anim.SetBool("Win", true);
        playerDead = true;
        //停止所有移动
        //停止agent
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }

    public void PlayAgain()
    {
        anim.SetBool("Win", false);
        playerDead = false;
    }
}


