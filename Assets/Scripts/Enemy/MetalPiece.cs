using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalPiece : MonoBehaviour
{
    public AudioClip getMetal;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) {

            SaveManager.Instance.state.metalAmount++;
            other.gameObject.GetComponentInParent<AudioSource>().clip = getMetal;
            other.gameObject.GetComponentInParent<AudioSource>().Play();
            Destroy(gameObject);
        }
    }
}
