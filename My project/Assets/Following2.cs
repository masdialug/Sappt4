using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Following2 : MonoBehaviour

{
    public Transform PlayerPrefab;
    public float moveSpeed = 5f;
    public int Delay = 2;
    private Vector2 targetPosition;
//private float targetDelay = 1f;
private float currTime;
public Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        TargetPlayer();
//currTime = targetDelay;
rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
	Invoke("MoveTowards", 2);
	

        // If the enemy reaches the target position, set a new random target position
        if ((Vector2)transform.position == targetPosition)
        {
        	
        }
	//currTime -= Time.fixedDeltaTime;
	//if (currTime <= 0)
	//{
		targetPosition = new Vector2(PlayerPrefab.position.x, transform.position.y);
//currTime = targetDelay;
	//}
	

    }
    void TargetPlayer()
    {
        	
        	targetPosition = new Vector2(PlayerPrefab.position.x, transform.position.y);
    }
    void MoveTowards()
    {
//targetPosition = new Vector2(PlayerPrefab.position.x, transform.position.y);

	//transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
if(Vector2.Distance(transform.position, targetPosition) > 0.5f)
{
rb.AddForce(targetPosition * moveSpeed);

}
else
{
rb.velocity=Vector2.zero;
}


	
    }
}
