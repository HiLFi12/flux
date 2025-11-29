using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float lerpvel;
    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, player.position, lerpvel * Time.deltaTime);
    }

    
}
