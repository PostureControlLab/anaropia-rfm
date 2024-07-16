using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Doorbell : MonoBehaviour
{
    AudioSource bell;

    void Start()
    {
        //Fetch the AudioSource from the GameObject
        bell = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("bell pressed");
            bell.Play();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            bell.Stop();
        }
    }
}