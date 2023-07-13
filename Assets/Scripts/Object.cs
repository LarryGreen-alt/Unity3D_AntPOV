using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{

    // Reference to the rigidbody
    private Rigidbody rb;
    public Rigidbody Rb => rb;

    private void Awake()
    {
        // Get reference to the rigidbody
        rb = GetComponent<Rigidbody>();
    }



}
