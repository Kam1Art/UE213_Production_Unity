using PathCreation;
using PathCreation.Examples;
using System;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public CollectibleType type;
    public float heightOffset;
    public Int32 beat;
    public float offset;
    public Vector3 rotationNeeded;
    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

#if UNITY_EDITOR
    private void OnValidate() => UnityEditor.EditorApplication.delayCall += _OnValidate;

    private void _OnValidate()
    {
        UnityEditor.EditorApplication.delayCall -= _OnValidate;
        if (this == null) return;

        if (transform.parent != null)
        {
            CollectibleCreator creator = transform.parent.GetComponent<CollectibleCreator>();
            if (creator != null)
            {
                creator.updateCollectible(this);
            }
        }
    }
#endif

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (type == CollectibleType.Star)
            {
                ScoreManager.instance.AddPoint();
                GetComponent<Renderer>().enabled = false;
                audioSource.Play();
                Destroy(gameObject, audioSource.clip.length);
            }

            if (type == CollectibleType.Boost)
            {
                ScoreManager.instance.AddPoint();
                GetComponent<Renderer>().enabled = false;
                other.GetComponent<PathFollower>().SpeedUp();
                audioSource.Play();
                Destroy(gameObject, audioSource.clip.length);
            }
            
        }
    }
}


public enum CollectibleType { 
    Star,
    Boost

};
