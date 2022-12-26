using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    private Queue<Vector3> _route = new Queue<Vector3>();
    private Vector3 _end;

    private NavMeshAgent _navMeshAgent;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _end = transform.position;
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        var path = _route.ToArray();
        if (path.Length > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < path.Length - 1; i++)
            {
                var curCorner = new Vector3(path[i].x, 0.5f, path[i].z);
                var nextCorner = new Vector3(path[i + 1].x, 0.5f, path[i + 1].z);
                Gizmos.DrawSphere(nextCorner, 0.1f);
                Gizmos.DrawLine(
                    curCorner,
                    nextCorner);
            }
        }
    }

    public void QueueNavigationTo(Vector3 destination)
    {
        var navMeshPath = new NavMeshPath();
        if(NavMesh.CalculatePath(
            _end,
            destination,
            NavMesh.AllAreas,
            navMeshPath))
        {
            foreach(var curCorner in navMeshPath.corners)
            {
                _route.Enqueue(new Vector3(curCorner.x, curCorner.y, curCorner.z));
            }
            _end = destination;
        }
    }

}
