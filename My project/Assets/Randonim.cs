using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randonim : MonoBehaviour
{
    [SerializeField] private AnimationCurve animCurve ;
   
    void Update()
    {
        if (Input.GetMouseButtonUp (0))
	{
		float number = animCurve.Evaluate (Random.value);

		Debug.Log ((int)number);
	}
    }
}
