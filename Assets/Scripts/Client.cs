using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.AI;

public class Client : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    [HideInInspector] public Vector3 startLocation;
    [HideInInspector] public Vector3 randomDestination;
    [SerializeField] private Transform[] destinations;

    [HideInInspector] public bool clientAvailable = true;

    private bool isWaiting = true;
    public bool isOnCar = false;
    private bool isFarFromHome = false;

    private void Start()
    {
        startLocation = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void RandomDestination()
    {
        if (isFarFromHome)
        {
            randomDestination = startLocation;
            isFarFromHome = false;
            clientAvailable = false;
            return;
        }

        int random = Random.Range(0, destinations.Length);
        randomDestination = destinations[random].position;
        isFarFromHome = true;
        clientAvailable = false;
    }

    public void SetClientDestination(Vector3 destination)
    {
        navMeshAgent.SetDestination(destination);
        isWaiting = false;
    }

    public void DropClient(Vector3 doorPosition)
    {
        navMeshAgent.enabled = false;
        transform.position = doorPosition;
        isOnCar = false;

        navMeshAgent.enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        navMeshAgent.SetDestination(randomDestination);
    }

    private void Update()
    {
        if (!isWaiting 
            && !isOnCar 
            && Vector3.Distance(transform.position, navMeshAgent.destination) <= navMeshAgent.stoppingDistance)
        {
            isOnCar = true;
            GetComponent<MeshRenderer>().enabled = false;
        }

        if (!clientAvailable && Vector3.Distance(transform.position, randomDestination) <= navMeshAgent.stoppingDistance)
        {
            clientAvailable = true;
            isWaiting = true;
        }
    }
}
