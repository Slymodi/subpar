using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


public class Projection : MonoBehaviour
{
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxPhysicsFrameIterations = 100;
    [SerializeField] private Ball _ballPrefab;
   // private List<Transform> _obstaclesParent;
    [SerializeField]private Transform _obstaclesParent;
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();

    private void Start() {
        //_obstaclesParent = new List<Transform>();
        CreatePhysicsScene();
   }


    private void CreatePhysicsScene() {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();





        //get every folder with the name generated-meshes and add its contents to ObstaclesParent
     /*   var generatedMeshes = Resources.FindObjectsOfTypeAll<GameObject>().Where(obj => obj.name == "[generated-meshes]");
        foreach (var generatedMesh in generatedMeshes)
        {
            foreach (Transform child in generatedMesh.transform)
            {
                
                _obstaclesParent.Add(child);
            }
        }*/



        foreach (Transform obj in _obstaclesParent) {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            //disable the renderer of the ghost object but the ghost object may not have a renderer
            try {
                ghostObj.GetComponent<Renderer>().enabled = false;
            } catch {
                //this is so that the script doesn't crash if the object doesn't have a renderer
            }
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
        }
    }

    private void Update() {
        foreach (var item in _spawnedObjects) {
            item.Value.position = item.Key.position;
            item.Value.rotation = item.Key.rotation;
        }
    }

    public void SimulateTrajectory( Transform pos, float steeringAngle, float power) {

        var ghostObj = Instantiate(_ballPrefab, pos);
        ghostObj.transform.parent = null;
        DontDestroyOnLoad(ghostObj.gameObject);
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);
        ghostObj.steeringAngle = steeringAngle;
        ghostObj.isGhost = true;
        ghostObj.power = 1;
        ghostObj.powerMultiplier = 500f;

        _line.positionCount = _maxPhysicsFrameIterations;
        ghostObj.Shoot();   

        for (var i = 0; i < _maxPhysicsFrameIterations; i++) {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghostObj.transform.position);
        }
        Destroy(ghostObj.gameObject);
    }
}
