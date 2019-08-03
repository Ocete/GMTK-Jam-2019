using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum State
    {
        DIRECTION,
        AMMO,
        MOVING
    }
    private State state;

    private int health;
    private bool direction_selected;

    private Vector3 direction;
    private float angle; // absolute angle: 0 is north

    public float recoil_time;
    public float direction_speed;
    public float ammo_time;
    private float timer;

    public List<Color> colors;
    public List<float> distance;

    private Vector3 target_pos;

    private int i_ammo;
    private SpriteRenderer sprite_renderer;
    private const float THRESHOLD = 0.2f;

    //private LineRenderer aim_line;
    // Start is called before the first frame update
    void Start()
    {
        //aim_line = GetComponent<LineRenderer>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        i_ammo = 0;
        sprite_renderer.color = colors[i_ammo];
        health = 1;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.DIRECTION:
                angle = angle + direction_speed * Time.deltaTime; // if weird values may be overflowing
                direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                Debug.DrawLine(transform.position, transform.position + direction * 4);
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    state = State.AMMO;
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                    angle = transform.rotation.eulerAngles.z;
                }
                break;
            case State.AMMO:
                timer -= Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    direction_selected = false;
                    state = State.MOVING;
                    timer = ammo_time;
                    target_pos = transform.position + (direction * distance[i_ammo]);
                } else if (timer < 0) {
                    i_ammo = (i_ammo + 1) % colors.Count;
                    sprite_renderer.color = colors[i_ammo];
                    timer = ammo_time;
                }
                break;
            case State.MOVING:
                transform.position = Vector3.Lerp(transform.position, target_pos, recoil_time * Time.deltaTime);

                if (Input.GetKeyDown(KeyCode.Space) && Vector3.SqrMagnitude(transform.position - target_pos) < distance[i_ammo] * distance[i_ammo] / 2)
                {
                    direction_selected = true;
                }
                if (Vector3.SqrMagnitude(transform.position - target_pos) < THRESHOLD * THRESHOLD)
                {
                    state = direction_selected ? State.AMMO : State.DIRECTION;
                }
                break;
        }
    }

    private void GameOver() {
        Debug.Log("dep");
    }

    private void OnTriggerEnter(Collider other)
    {
        // TODO: ahora mismo te matan las paredes
        health--;
        if (health == 0)
        {
            GameOver();
        }
    }
}