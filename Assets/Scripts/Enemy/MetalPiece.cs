using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalPiece : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) {
            SaveManager.Instance.state.metalAmount++;

            Destroy(gameObject);
        }
    }
}
