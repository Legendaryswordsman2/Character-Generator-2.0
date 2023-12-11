using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    Vector3 _rotation = new(0, 0, -150);

    private void Update()
    {
        transform.Rotate(_rotation * Time.deltaTime);
    }
}