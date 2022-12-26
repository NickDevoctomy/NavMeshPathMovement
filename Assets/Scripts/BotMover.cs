using UnityEngine;

public class BotMover : MonoBehaviour
{
    private Vector3? _destination;

    void Start()
    {
        
    }

    void Update()
    {
        
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

    public void MoveTo(Vector3 destination)
    {
        if(_destination != destination)
        {
            _destination = destination;

            var angleToDestination = Angle(destination);
            var distanceToDestination = Vector3.Distance(transform.position, destination);

            Debug.Log($"Angle to destination = {angleToDestination}");
            Debug.Log($"Distiance to destination = {distanceToDestination}");
        }
    }

    private float Angle(Vector3 destination)
    {
        var angleToNext = Vector3.SignedAngle(
               destination - transform.position,
               transform.forward,
               Vector3.up);
        return angleToNext;
    }
}
