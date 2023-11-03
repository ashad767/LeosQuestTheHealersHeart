using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [SerializeField] Transform MC;
    private Rigidbody2D rb;
    private Animator a;
    [SerializeField] AnimationClip[] anim;

    private enum States { idle, walk, attack };
    private bool idle = true;
    private bool walk = false;
    private bool attack = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        a = GetComponent<Animator>();

        StartCoroutine(follow_MC());
        //StartCoroutine(shootAnim());
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = MC.position - transform.position;
        dir.Normalize();
        a.SetFloat("dirX", dir.x);
        a.SetFloat("dirY", dir.y);

        if (!walk && !attack)
        {
            a.SetInteger("state", (int)States.idle);
        }

        if (!idle)
        {
            transform.position = Vector2.MoveTowards(transform.position, MC.position, 4f * Time.deltaTime);
        }
    }

    private IEnumerator follow_MC()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            idle = false;

            if (!attack)
            {
                walk = true;
                a.SetInteger("state", (int)States.walk);
                yield return new WaitForSeconds(4f);
                walk = false;
            }
            
            
            idle = true;
        }
    }

    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "MC")
        {
            attack = true;
            a.SetInteger("state", (int)States.attack);
            yield return new WaitForSeconds(anim[1].length);
            attack = false;
        }
    }
}
