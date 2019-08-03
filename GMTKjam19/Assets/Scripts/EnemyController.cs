using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public LayerMask avoidableObjects;
    public GameObject player;

    // Parameters for steering
    public float speed = 5;
    public float max_force;
    public float mass;
    private Vector3 velocity;


    // distance from where the enemy will start following the player
    public float interest_distance_in;
    public float interest_distance_out;

    private const float THRESHOLD = 0.2f;

    // parameter for wandering
    public float wandering_time;
    private float wand_timer;
    Vector3 wander_target;

    // simple state machine
    private enum State {
        CHASING,
        WANDERING
    }
    private State state;

    private void Start()
    {
        velocity = transform.forward;
    }

    void Update()
    {
        Vector3 player_pos = player.transform.position;
        Vector3 direction = player_pos - transform.position;
        switch (state) {
            case State.CHASING:
                if (direction.sqrMagnitude > interest_distance_out * interest_distance_out)
                {
                    state = State.WANDERING;
                    break;
                }
                
                break;
            case State.WANDERING:
                if (direction.sqrMagnitude < interest_distance_in * interest_distance_in)
                {
                    state = State.CHASING;
                    break;
                }
                wand_timer -= Time.deltaTime;
                // override direction to reuse code found below
                direction = wander_target - transform.position;

                if (wand_timer < 0 || direction.sqrMagnitude < THRESHOLD * THRESHOLD)
                {
                    float angle = Random.value * 360f;
                    wander_target = transform.position + 4 * new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                    direction = wander_target - transform.position;
                    wand_timer = wandering_time;
                }

                break;
        }

        Vector3 steering_avoid = Vector3.zero;
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = avoidableObjects;
        Physics2D.CircleCast(transform.position, 3, Vector2.up, filter, hits);
        foreach (RaycastHit2D hit in hits)
        {
            Vector3 avoid = hit.point;
            Vector3 desired_velocity = direction.normalized * speed;
            steering_avoid += desired_velocity - velocity;
        }

        Debug.DrawLine(transform.position, transform.position + direction, Color.red);
        if (direction.sqrMagnitude > THRESHOLD * THRESHOLD)
        {
            Vector3 desired_velocity = direction.normalized * speed;
            Vector3 steering = desired_velocity - velocity;
            steering += steering_avoid;
            steering = Vector3.ClampMagnitude(steering, max_force);
            steering = steering / mass;
            velocity = Vector3.ClampMagnitude(velocity + steering, speed);

            transform.position = transform.position + velocity;
        } // if it is next to the target dont try to keep moving, looks glitchy


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Object.Destroy(this);
        }
    }
}
