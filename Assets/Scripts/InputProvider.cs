using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
public class InputProvider : SimulationBehaviour, INetworkRunnerCallbacks {
private NetworkRunner _runner;

  // creating a instance of the Input Action created  localNetworkInput localNetworkInput = new localNetworkInput();
    NetworkInputData localNetworkInput = new NetworkInputData();
  public void OnEnable() {
    if(Runner != null) {
        Runner.AddCallbacks( this );
    }
  }
    public void OnDisable(){
    if(Runner != null){
        Runner.RemoveCallbacks( this );
    }
  }
    private void OnGUI()
    {
    if (_runner == null)
    {
        if (GUI.Button(new Rect(0,0,200,40), "Host"))
        {
            StartGame(GameMode.Host);
        }
        if (GUI.Button(new Rect(0,40,200,40), "Join"))
        {
            StartGame(GameMode.Client);
        }
    }
    }
  public void Update()
  {
    if (Input.GetMouseButtonDown(0)) {
      localNetworkInput.NetworkButtons.Set(MyButtons.Shoot, true);
    }

    if (Input.GetKeyDown(KeyCode.Space)) {
      localNetworkInput.NetworkButtons.Set(MyButtons.Shoot, true);
    }

    CollectMouseInput();
  }

   [SerializeField] private float powerSensitivity = 1.0f;
   private Vector3 lastEndTouch = Vector3.zero;
    private bool justStarted = true;
    [SerializeField] private float aimSpeed = 10.0f;


  private void CollectMouseInput() {
    Vector3? nStartTouch = GetPlanePosition(TouchInput.Instance.pointerStartPosition);
    Vector3? nEndTouch = GetPlanePosition(TouchInput.Instance.pointerPosition);
    if (nStartTouch == null || nEndTouch == null) return;
    Vector3 startTouch = (Vector3)nStartTouch;
    Vector3 rawEndTouch = (Vector3)nEndTouch;
    var power = Mathf.Clamp((startTouch - rawEndTouch).magnitude * powerSensitivity, 0, 1);
    Vector3 goalEndTouch = startTouch + (rawEndTouch - startTouch).normalized * power;
    Vector3 endTouch = Vector3.Lerp(lastEndTouch, goalEndTouch, justStarted? 1: Mathf.Min(1, Time.deltaTime * aimSpeed));
    lastEndTouch = endTouch;
    float aimAngle = Vector3.SignedAngle(Vector3.forward, (endTouch - startTouch), Vector3.up);
    power = Mathf.Clamp((startTouch - endTouch).magnitude, 0, 1);
    
    if (TouchInput.Instance.pointerUp)
    {
        SignalShoot(power, aimAngle);
    }
  }

  private void SignalShoot(float power, float angle)
  {
    localNetworkInput.NetworkButtons.Set(MyButtons.Shoot, true);
    localNetworkInput.power = power;
    localNetworkInput.angle = angle;
  }

  public void OnInput(NetworkRunner runner, NetworkInput input) {

    input.Set(localNetworkInput);

    // Reset the input struct to start with a clean slate
    // when polling for the next tick
    localNetworkInput = default;
  }


  async void StartGame(GameMode mode)
		{
			// Create the Fusion runner and let it know that we will be providing user input
			_runner = gameObject.AddComponent<NetworkRunner>();
			_runner.ProvideInput = true;
	    
			// Start or join (depends on gamemode) a session with a specific name
			await _runner.StartGame(new StartGameArgs()
			{
				GameMode = mode, 
				SessionName = "TestRoom", 
				Scene = SceneManager.GetActiveScene().buildIndex,
				SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
			});
		}

		[SerializeField] private NetworkPrefabRef _playerPrefab; // Character to spawn for a joining player
		private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

[SerializeField] private ArrowHandler _arrowHandler;
		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			if (runner.IsServer)
			{
				Vector3 spawnPosition = new Vector3((player.RawEncoded%runner.Config.Simulation.DefaultPlayers)*3,1,0);
				NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
                var playerInputConsumer = networkPlayerObject.GetComponent<PlayerInputConsumer>();
                playerInputConsumer.playerRef = player;
				_spawnedCharacters.Add(player, networkPlayerObject);
                runner.SetPlayerObject(player, networkPlayerObject);

			}
      if (runner.LocalPlayer == player)
     {
        var playerInputConsumer = runner.GetPlayerObject(player).GetComponent<PlayerInputConsumer>();
        _arrowHandler.steeringPlaneForward = playerInputConsumer.steeringPlaneForward;
        _arrowHandler.steeringPlaneRear = playerInputConsumer.steeringPlaneRear;
      }
		}

		public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
			{
				runner.Despawn(networkObject);
				_spawnedCharacters.Remove(player);
			}
		}
		
		public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
		public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
		public void OnConnectedToServer(NetworkRunner runner) { }
		public void OnDisconnectedFromServer(NetworkRunner runner) { }
		public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
		public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
		public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
		public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
		public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
		public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
		public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
		public void OnSceneLoadDone(NetworkRunner runner) { }
		public void OnSceneLoadStart(NetworkRunner runner) { }

     
    public Vector3? GetPlanePosition(Vector2 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(screenPosition.x * Screen.width, screenPosition.y * Screen.height));
        RaycastHit hit;
        if (_inputRaycastPlane == null) return null;
        if (_inputRaycastPlane.Raycast(ray, out hit, 100.0f))
        {
            return hit.point;
        }
        else
        {
            return null;
        }
    }
    [SerializeField] public  Collider _inputRaycastPlane;


	}


