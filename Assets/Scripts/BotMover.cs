using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class BotMover : MonoBehaviour
{
    public float RotationSpeed = 12f;
    public float MovementSpeed = 2f;
    public bool Ready = true;
    public Vector3 CurrentPosition;
    public Vector3 NextPos;

    private NavMeshAgent _navMeshAgent;
    private Vector3? _destination;
    private float? _desiredYRotation;
    private float? _desiredZTranslation;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(!_destination.HasValue)
        {
            return;
        }


        var targetDirection = _destination.GetValueOrDefault() - transform.position;
        targetDirection.y = 0f;
        var targetRotation = Quaternion.LookRotation(targetDirection);
        var deltaAngle = Quaternion.Angle(transform.rotation, targetRotation);
        var rotate = deltaAngle != 0F;

        if(rotate)
        {
            Debug.Log("Rotating!");
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                RotationSpeed * Time.deltaTime / deltaAngle);
        }
        else
        {
            // Move toward
            var distance = Vector3.Distance(transform.position, _destination.GetValueOrDefault());
            if (distance > _navMeshAgent.radius)
            {
                Debug.Log("Moving!");
                transform.position = Vector3.Slerp(
                    transform.position,
                    _destination.GetValueOrDefault(),
                    MovementSpeed * Time.deltaTime / distance);
            }
            else
            {
                CurrentPosition = _destination.GetValueOrDefault();
                _destination = null;
                Ready = true;
                Debug.Log("Finished moving!");
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if(_destination.HasValue)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawLine(
    //            transform.position,
    //            transform.position + transform.forward * 5);

    //        Gizmos.color = Color.blue;
    //        Gizmos.DrawLine(
    //            transform.position,
    //            _destination.GetValueOrDefault());
    //    }
    //}

    public bool MoveTo(Vector3 destination)
    {
        if(_destination != destination &&
            Ready)
        {
            Ready = false;
            _destination = destination;
            NextPos = destination;
            return true;
        }

        return false;
    }

    private float Angle(Vector3 destination)
    {
        var angleToNext = Vector3.SignedAngle(
               destination - transform.position,
               transform.forward,
               Vector3.up);
        return angleToNext;
    }

    private void PerformVirtualHorizontalAxis(float value)
    {
        //Debug.Log($"VHAxis =  {value}");
        var horizontalAxis = value;
        var rotation = horizontalAxis * RotationSpeed;
        _desiredYRotation = rotation;
    }

    public void PerformVirtualVerticalAxis(float value)
    {
        //Debug.Log($"VVAxis =  {value}");
        var verticalAxis = value;
        var movement = Time.deltaTime * (verticalAxis * MovementSpeed);
        _desiredZTranslation = movement;
    }
}
