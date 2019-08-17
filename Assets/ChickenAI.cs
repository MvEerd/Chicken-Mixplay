using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform[] points;
    int destPoint = 0;
    private float timer;
    public float wanderRadius;
    public float wanderTimer;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
    }

    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        destPoint = (destPoint + 1) % points.Length;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;
        randomDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);
        return navHit.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            agent.updateRotation = true;
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos); //Walk to a random location
            timer = 0;

            GetComponent<Animator>().SetBool("Eat", false);
            GetComponent<Animator>().SetBool("Walk", true);
        }

        if (!agent.pathPending && agent.remainingDistance < 0.2f) {
            GetComponent<Animator>().SetBool("Eat", true);
            GetComponent<Animator>().SetBool("Walk", false);
            return; 
        }
    }
}
