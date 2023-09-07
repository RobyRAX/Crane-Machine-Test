using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeController : MonoBehaviour
{
    public static event Action<GameObject> OnPrizeGranted;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == PrizeSpawner.Instance.respawnCol)
        {
            if (GameManager.Instance.currentState == GameState.GameInit)
                transform.position = PrizeSpawner.Instance.spawnPoints[UnityEngine.Random.Range(0, PrizeSpawner.Instance.spawnPoints.Count)];
            else
            {
                OnPrizeGranted(this.gameObject);
                Destroy(this.gameObject, 20f);
            }                
        }
    }
}
