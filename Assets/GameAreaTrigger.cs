using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAreaTrigger : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.gameObject.TryGetComponent(out Ball ball))
        {
            GameManager.instance.Lose();
        }
    }
}
