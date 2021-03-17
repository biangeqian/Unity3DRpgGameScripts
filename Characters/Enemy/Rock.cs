using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer,HitEnemy,HitNothing};
    public RockStates rockStates;

    private Rigidbody rb;
    [Header("BasicSettings")]
    public float force;
    public int damage;
    public GameObject target;
    public Vector3 direction;
    //获得石头破碎特效
    public GameObject breakEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //给个初始速度防止出bug
        rb.velocity = Vector3.one;
        rockStates = RockStates.HitPlayer;
        FlyToTarget();
    }
    //物理判断时用fixupdate
    void FixedUpdate()
    {
        //Debug.Log(rb.velocity.sqrMagnitude);
        if (rb.velocity.sqrMagnitude <1f)
        {
            
             rockStates = RockStates.HitNothing;
                
        }
    }

    public void FlyToTarget()
    {
        if (target == null)
        {
            target = FindObjectOfType<PlayerController>().gameObject;
        }
        direction = (target.transform.position - transform.position+Vector3.up*2).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);

    }
    void OnCollisionEnter(Collision other)
    {
        //可能有点不合理
        if (other.gameObject.CompareTag("Ground"))
        {
            rb.velocity = Vector3.zero;
        }
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    //弹开
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;
                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    //伤害
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());
                    rockStates = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
        }
    }
}
