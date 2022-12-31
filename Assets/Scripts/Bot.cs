using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    private Queue<Vector3> _route = new Queue<Vector3>();
    private Vector3 _end;

    private BotMover _botMover;
    private NavMeshAgent _navMeshAgent;

    void Start()
    {
        _botMover = GetComponent<BotMover>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _end = transform.position;
    }

    void Update()
    {
        if(!_botMover.ControlledMovement)
        {
            return;
        }

        if(_route.TryPeek(out var nextPos))
        {
            if(_botMover.Ready)
            {
                if (_botMover.CurrentPosition != nextPos)
                {
                    Debug.Log($"Move to {nextPos}");
                    _botMover.MoveTo(nextPos);
                }
                else
                {
                    Debug.Log($"Proceeding to next on path...");
                    _route.Dequeue();
                }
            }

        }
    }

    private void OnDrawGizmos()
    {
        var path = _botMover.ControlledMovement ?
            _route.ToArray() :
            _navMeshAgent.path.corners;
        if (path != null && path.Length > 0)
        {
            Gizmos.color = Color.green;

            var startPos = new Vector3(transform.position.x, 0.5f, transform.position.z);
            var firstCorner = new Vector3(path[0].x, 0.5f, path[0].z);
            Gizmos.DrawLine(
                startPos,
                firstCorner);
            Gizmos.DrawSphere(firstCorner, 0.1f);

            if (path.Length > 1)
            {
                var nextCorner = new Vector3(path[1].x, 0.5f, path[1].z);
                Gizmos.DrawLine(
                    firstCorner,
                    nextCorner);

                for (int i = 1; i < path.Length - 1; i++)
                {
                    var curCorner = new Vector3(path[i].x, 0.5f, path[i].z);
                    nextCorner = new Vector3(path[i + 1].x, 0.5f, path[i + 1].z);
                    Gizmos.DrawSphere(nextCorner, 0.1f);
                    Gizmos.DrawLine(
                        curCorner,
                        nextCorner);
                }
            }
        }

    }

    public void QueueNavigationTo(Vector3 destination)
    {
        if(_botMover.ControlledMovement)
        {
            var navMeshPath = new NavMeshPath();
            if (NavMesh.CalculatePath(
                _end,
                destination,
                NavMesh.AllAreas,
                navMeshPath))
            {
                for (int i = 0; i < navMeshPath.corners.Length; i++)
                {
                    var curCorner = navMeshPath.corners[i];
                    if (i == 0 && _route.Count == 0)
                    {
                        var levelCorner = new Vector3(curCorner.x, transform.position.y, curCorner.z);
                        var distance = Vector3.Distance(transform.position, levelCorner);
                        if (distance == 0)
                        {
                            continue;
                        }
                    }

                    _route.Enqueue(new Vector3(curCorner.x, transform.position.y, curCorner.z));
                }
                _end = new Vector3(
                    destination.x,
                    transform.position.y,
                    destination.z);
            }
            return;
        }

        _botMover.MoveTo(destination);
    }

}
