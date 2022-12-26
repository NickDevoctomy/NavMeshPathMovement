using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class BotMover : MonoBehaviour
{
    public float RotationSpeed = 8f;
    public float MovementSpeed = 4f;
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
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                RotationSpeed * Time.deltaTime / deltaAngle
            );
        }
        else
        {
            // Move toward
            var distance = Vector3.Distance(transform.position, _destination.GetValueOrDefault());
            if (distance > _navMeshAgent.radius)
            {
                var axisAmout = distance / _navMeshAgent.radius;
                axisAmout = Mathf.Clamp(axisAmout, 0f, 1f);
                PerformVirtualVerticalAxis(axisAmout);
                transform.Translate(new Vector3(0f, 0f, _desiredZTranslation.GetValueOrDefault()));
            }
            else
            {
                // reached destination
                _destination = null;
            }
        }

        //if(_destination.HasValue)
        //{
        //    var angleToDestination = Angle(_destination.GetValueOrDefault());
        //    var angleToDestinationAbs = Mathf.Abs(angleToDestination);
        //    if (angleToDestinationAbs > 5f)
        //    {
        //        Debug.Log($"We are {angleToDestinationAbs} degrees off target");
        //        if (angleToDestination < 0f)
        //        {
        //            var axisAmout = angleToDestinationAbs / 180f;
        //            axisAmout = Mathf.Clamp(axisAmout, 0f, 1f);
        //            PerformVirtualHorizontalAxis(axisAmout);
        //        }
        //        else
        //        {
        //            var axisAmout = angleToDestinationAbs / 180f;
        //            axisAmout = Mathf.Clamp(axisAmout, 0f, 1f);
        //            PerformVirtualHorizontalAxis(-axisAmout);
        //        }
        //    }

        //    var distance = Vector3.Distance(transform.position, _destination.GetValueOrDefault());
        //    if (distance > 0.5f)
        //    {
        //        var axisAmout = distance / _navMeshAgent.radius;
        //        axisAmout = Mathf.Clamp(axisAmout, 0f, 1f);
        //        PerformVirtualVerticalAxis(axisAmout);
        //    }
        //    else
        //    {
        //        // reached destination
        //        _destination = null;
        //    }
        //}

        //if (_desiredYRotation.HasValue)
        //{
        //    Debug.Log("Rotating...");
        //    transform.Rotate(new Vector3(0f, _desiredYRotation.GetValueOrDefault(), 0f));
        //    _desiredYRotation = null;
        //}
        //else
        //{
        //    if (_desiredZTranslation.HasValue)
        //    {
        //        Debug.Log("Translating...");
        //        transform.Translate(new Vector3(0f, 0f, _desiredZTranslation.GetValueOrDefault()));
        //        _desiredZTranslation = null;
        //    }
        //}

        //if(_destination == null &&
        //    !Ready)
        //{
        //    Debug.Log("Reached destination!");
        //    CurrentPosition = destination.GetValueOrDefault();
        //    Ready = true;
        //}
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
