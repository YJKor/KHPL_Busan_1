using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class NPC_Movement : MonoBehaviour
{
    public float wanderRadius = 10f;
    public float moveMinTime = 3f;
    public float moveMaxTime = 7f;
    public float stopMinTime = 4f;
    public float stopMaxTime = 5f;

    private Animator animator;
    private NavMeshAgent nav;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        nav.speed = 2f;
    }
    void Start()
    {
        StartCoroutine(MoveNPC());
    }

    void Update()
    {
        
    }

    private IEnumerator MoveNPC()
    {
        while (true)
        {
            float moveTime = Random.Range(moveMinTime, moveMaxTime);
            Vector3 dest = RandomNavmeshLocation(wanderRadius);

            nav.isStopped = false;
            nav.SetDestination(dest);
            SetWalking(true); //움직임 시작 시 걷기 애니메이션 ON

            float timer = 0f;
            while (timer < moveTime)
            {
                if(!nav.pathPending && nav.remainingDistance < 0.5f)
                {
                    dest = RandomNavmeshLocation(wanderRadius);
                    nav.SetDestination(dest);
                }
                timer += Time.deltaTime;
                yield return null;
            }

            nav.isStopped = true;
            SetWalking (false);
            float stopTime =Random.Range(stopMinTime, stopMaxTime);
            yield return new WaitForSeconds(stopTime);
        }
    }

    void SetWalking(bool isWalking)
    {
        if (animator != null)
            animator.SetBool("Walking", isWalking);
    }

    Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }
}
