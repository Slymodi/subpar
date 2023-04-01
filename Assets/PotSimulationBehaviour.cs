using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Fusion;
public class PotSimulationBehaviour : SimulationBehaviour
{
    private static Sprite[] _potSprites;
    [SerializeField] private string texturePath;
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        if (_potSprites == null)
        {
             var sprites = Resources.LoadAll(texturePath, typeof(Sprite));
                _potSprites = new Sprite[sprites.Length];
             for (int i = 0; i < sprites.Length; i++)
             {
                 _potSprites[i] = (Sprite) sprites[i];
             }

        }
         var randomSprite = Random.Range(0, _potSprites.Length);
         var sprite = _potSprites[randomSprite];
         spriteRenderer.sprite = sprite;
        
    }
}
