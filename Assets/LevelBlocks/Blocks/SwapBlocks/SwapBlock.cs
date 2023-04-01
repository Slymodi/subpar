using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class SwapBlock : SimulationBehaviour
{
    [SerializeField] private NetworkObject _swapBlock;

    public void Swap()
    {
        if(Object.HasStateAuthority == false) return;
        
        Runner.Spawn(_swapBlock, transform.position, transform.rotation);
        Runner.Despawn(Object);
    }

    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out PlayerInputConsumer playerIC))
        {
            Debug.Log("HOLE");
        }
    }
}
