using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class StarSpawnerController : NetworkBehaviour 
{

    [SerializeField] private GameObject starSpawner;
    public GameObject particle;
    [SerializeField] private Game _game;

    [SerializeField] private NetworkObject _star;

    [SerializeField] private NetworkObject _hole;
   // public event Action<PlayerRef, StarSpawnerController> collidedWithPlayer;
    // Start is called before the first frame update
    void Awake()
    {
        _game = GameObject.Find("Game").GetComponent<Game>();
    }
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag != "Player") return;

        PlayerInputConsumer playerIC = other.gameObject.GetComponent<PlayerInputConsumer>();
        ParticleSpawner.Instance.SpawnHitParticle(particle, transform.position, transform.rotation);

        //initalize star

        var starObj = Runner.Spawn(_star, transform.position, transform.rotation);
        StarController star = starObj.GetComponent<StarController>();
        _game.AddScore(this, star, playerIC.playerRef); 
        star.ChangeColor(playerIC.color);
        Runner.Despawn(Object);


    }

    public void BecomeHole(Transform parent)
    {
        
        //var hole = Runner.Spawn(_hole, transform.position, transform.rotation);
        
        //move into before spawnn call
        //hole.transform.parent = parent;


        //raycast down and get from the layer Ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("SwapBlock")))
        {

            //TODO: REWRITE THIS TO SWAP ASSETS TO HOLE VERSION
            //move the star spawner to the hit point
            hit.collider.transform.parent.transform.parent.gameObject.GetComponent<SwapBlock>()?.Swap();
        } else
        {
            Debug.Log("No hit");
        }
        Runner.Despawn(Object);
    }

    private Transform _holeParent;

    private void setHoleParent(NetworkRunner runner, NetworkObject obj)
    {
        obj.transform.parent = _holeParent;
    }
}
