using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using Fusion;

public class Projection : MonoBehaviour
{
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxPhysicsFrameIterations = 100;
    [SerializeField] private GameObject _ballPrefab;
   // private List<Transform> _obstaclesParent;
    [SerializeField]private Transform _obstaclesParent;
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;
    private readonly Dictionary<Transform, Transform> _spawnedObjects = new Dictionary<Transform, Transform>();
    [SerializeField] private float angle;
    private void Awake() {
        //_obstaclesParent = new List<Transform>();

        //assign the obstacles parent to the PhysicsInteractable object in the scene
        _obstaclesParent = GameObject.Find("PhysicsInteractable").transform;
        CreatePhysicsScene();
   }


    private void CreatePhysicsScene() {
        var simulationSceneMaybe = SceneManager.GetSceneByName("Simulation");
        if (simulationSceneMaybe.IsValid()) {
            _simulationScene = simulationSceneMaybe;
            _physicsScene = _simulationScene.GetPhysicsScene();
            return;
        } else {
            _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        }
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
                ghostObj.layer = 2;
                var children = ghostObj.GetComponentsInChildren<Transform>(includeInactive: true);
                foreach (var child in children)
                {
                    child.gameObject.layer = 2;
                }
            } catch {
                //this is so that the script doesn't crash if the object doesn't have a renderer
            }
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            if (!ghostObj.isStatic) _spawnedObjects.Add(obj, ghostObj.transform);
        }
    }

    public void SimulateTrajectory(float power, float angle) {
        if(_line == null) return;
        //set the steering angle to the transform of the angle of the touch input first and second position

        var ghostObj = Instantiate(_ballPrefab, this.transform);
        ghostObj.transform.parent = null;
        DontDestroyOnLoad(ghostObj.gameObject);
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);
        //use steering angle to create a vector3 for the direction of the ball

        _line.positionCount = _maxPhysicsFrameIterations;
    // ghostOzbj.Shoot();   
        //.AddForce( direction * power, ForceMode.Impulse);
        ghostObj.GetComponent<Rigidbody>().AddForce( Quaternion.AngleAxis( angle + 90, Vector3.up)  * new Vector3(1, 1, 0) * power * 500f);

        for (var i = 0; i < _maxPhysicsFrameIterations; i++) {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            _line.SetPosition(i, ghostObj.transform.position);
        }
        Destroy(ghostObj.gameObject);
    }
}
