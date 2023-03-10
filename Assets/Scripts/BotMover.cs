using UnityEngine;
using UnityEngine.AI;

public class BotMover : MonoBehaviour
{
    public bool ControlledMovement = true;
    public float RotationSpeed = 3f;
    public float MovementSpeed = 8f;
    public float MinAnglesForForcedRotation = 1f;
    public float MinDistanceFromMoveTarget = 0.5f;
    public bool Ready = true;
    public Vector3 CurrentPosition;
    public Vector3 NextPos;

    private NavMeshAgent _navMeshAgent;
    private Vector3? _destination;
    
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(!ControlledMovement || !_destination.HasValue)
        {
            return;
        }

        var angle = SignedAngle(_destination.GetValueOrDefault());
        if(Mathf.Abs(angle) > MinAnglesForForcedRotation)
        {
            Debug.Log($"Signed delta angle = {angle}");
            var targetDirection = _destination.GetValueOrDefault() - transform.position;
            targetDirection.y = 0f;
            var targetRotation = Quaternion.LookRotation(targetDirection);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * RotationSpeed);
        }


        if(Mathf.Abs(angle) <= MinAnglesForForcedRotation)
        {
            var distance = Vector3.Distance(transform.position, _destination.GetValueOrDefault());
            Debug.Log($"Distance = {distance}");

            if (distance > MinDistanceFromMoveTarget)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    _destination.GetValueOrDefault(),
                    Time.deltaTime * MovementSpeed);
            }
            else
            {
                CurrentPosition = _destination.GetValueOrDefault();
                _destination = null;
                Ready = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (_destination.HasValue)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                transform.position,
                transform.position + transform.forward * 5);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(
                transform.position,
                _destination.GetValueOrDefault());
        }
    }

    public bool MoveTo(Vector3 destination)
    {
        if(ControlledMovement)
        {
            if (_destination != destination &&
                Ready)
            {
                Ready = false;
                _destination = destination;
                NextPos = destination;
                return true;
            }
        }
        else
        {
            _navMeshAgent.destination = destination;
            return true;
        }

        return false;
    }

    private float SignedAngle(Vector3 destination)
    {
        var angleToNext = Vector3.SignedAngle(
               destination - transform.position,
               transform.forward,
               Vector3.up);
        return angleToNext;
    }

    //private void PerformVirtualHorizontalAxis(float value)
    //{
    //    //Debug.Log($"VHAxis =  {value}");
    //    var horizontalAxis = value;
    //    var rotation = horizontalAxis * RotationSpeed;
    //    _desiredYRotation = rotation;
    //}

    //public void PerformVirtualVerticalAxis(float value)
    //{
    //    //Debug.Log($"VVAxis =  {value}");
    //    var verticalAxis = value;
    //    var movement = Time.deltaTime * (verticalAxis * MovementSpeed);
    //    _desiredZTranslation = movement;
    //}
}
