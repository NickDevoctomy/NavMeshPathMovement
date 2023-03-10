using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Game : MonoBehaviour
{
    private static Game _instance;

    public GameObject BotPrefab;
    public GameObject ObstacleParent;
    public GameObject BotsParent;
    public GameObject DestinationMarkerPrefab;
    public Material ObstacleMaterial;
    public int Width = 50;
    public int Depth = 50;
    public int ObstacleCount;

    private GameObject _botGameObject;
    private Bot _bot;
    private NavMeshSurface _navMeshSurface;

    void Start()
    {
        if(_instance != null)
        {
            DestroyImmediate(this);
            return;
        }

        _instance = this;
        CreateObstacles();
        GenerateNavMesh();
        CreateBot();
    }

    void Update()
    {
    }

    public void MoveToRandom()
    {
        // chose a random location in the arena
        var pos = new Vector3(
        Random.Range(0, Width) - Width / 2,
        0.5f,
        Random.Range(0, Depth) - Depth / 2);
        if (NavMesh.SamplePosition(
            pos,
            out var closestHit,
            500f,
            NavMesh.AllAreas))
        {
            _bot.QueueNavigationTo(closestHit.position);
        }
    }

    private void CreateBot()
    {
        var pos = new Vector3(
                Random.Range(0, Width) - Width / 2,
                0.5f,
                Random.Range(0, Depth) - Depth / 2);
        if (NavMesh.SamplePosition(
            pos,
            out var closestHit,
            500f,
            NavMesh.AllAreas))
        {
            _botGameObject = GameObject.Instantiate(
                BotPrefab,
                closestHit.position,
                Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)),
                BotsParent.transform);
            _bot = _botGameObject.GetComponent<Bot>();
            _navMeshSurface.BuildNavMesh();
        }
    }

    private void GenerateNavMesh()
    {
        var ground = transform.Find("Ground");
        if(ground != null)
        {
            _navMeshSurface = ground.GetComponent<NavMeshSurface>();
            _navMeshSurface.BuildNavMesh();
        }
    }

    private void CreateObstacles()
    {
        for(var i = 0; i < ObstacleCount; i++)
        {
            var obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
            var pos = new Vector3(
                Random.Range(0, Width) - Width / 2,
                0.5f,
                Random.Range(0, Depth) - Depth / 2);
            var meshRenderer = obstacle.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = ObstacleMaterial;
            obstacle.transform.parent = ObstacleParent.transform;
            obstacle.transform.position = pos;
            meshRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
        }
    }
}
