using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    private AudioClip collisionClip;
    private AudioSource collisionSound;
    // Start is called before the first frame update

    

    void Start()
    {
        collisionClip = Resources.Load<AudioClip>("Bonk1") as AudioClip;
        collisionSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision col)
    {
        float audioLevel = col.relativeVelocity.magnitude / 30.0f;
        collisionSound.PlayOneShot(collisionClip, audioLevel);
    }

    
}
