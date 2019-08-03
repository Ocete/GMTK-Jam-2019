using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject player;

    void Update()
    {
        Vector3 player_pos = player.transform.position;
        Vector3 direction =  player_pos - transform.position;
        direction = direction.normalized;
        transform.position = Vector3.Lerp(transform.position, transform.position + direction, 5 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Object.Destroy(this);
        }
    }
}
