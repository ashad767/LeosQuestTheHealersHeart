using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator a;
    [SerializeField] AnimationClip[] anim;
    [SerializeField] Transform MC;

    // Audio
    [SerializeField] AudioSource startJump;
    [SerializeField] AudioSource jumpLanding;
    [SerializeField] AudioSource whoosh;

    private enum States { idle, walk, attack, jump };
    private bool idle = true;
    private bool walk = false;
    private bool attack = false;
    private bool jump = false;
    
    // For the jump animation
    private Vector2 snapshotMCPosition; // snapshot of MC's position during boss' jump
    private Vector2 jumpStartPosition; // boss' position before beginning jump
    private float jumpHeight = 5f; // Adjust the jump height as needed
    float jumpDuration = 1.2f; // Adjust the duration of the jump as needed



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        a = GetComponent<Animator>();

        StartCoroutine(follow_MC());
    }

    // Update is called once per frame
    void Update()
    {
        // used for my Blend Tree
        Vector2 dir = MC.position - transform.position;
        dir.Normalize();
        a.SetFloat("dirX", dir.x);
        a.SetFloat("dirY", dir.y);

        if (idle && !attack)
        {
            a.SetInteger("state", (int)States.idle);
        }

        if (!idle && !jump)
        {
            transform.position = Vector2.MoveTowards(transform.position, MC.position, 4f * Time.deltaTime);
        }
    }

    private IEnumerator follow_MC()
    {
        while (true)
        {
            // For the 1st time the game runs, stay idle for the time it takes the jump animation + jump duration to take place, plus 0.2 seconds.
            // After the first run, stay idle for 0.2 seconds. This is because of StartCoroutine(jumpFunc()), and the way coroutines work
            yield return new WaitForSeconds((anim[2].length*2) + jumpDuration + 0.2f);
            idle = false; // start walking

            walk = true;
            a.SetInteger("state", (int)States.walk);
            yield return new WaitForSeconds(3f); // follow player for some time
            walk = false;

            // after walking for some time, do jump ability
            StartCoroutine(jumpFunc());

            // Control comes back here after the first 'yield' call runs in jumpFunc() coroutine.
            // This is why when the attack animation happens right before the jump animation, the attack animation triggers and finishes during the jump animation duration, and since after that attack=false and idle=true, the idle animation happens while jumping.
            idle = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "MC" && !jump) // don't wanna attack while jumping
        {
            attack = true;
            a.SetInteger("state", (int)States.attack);
            whoosh.Play();
            StartCoroutine(attackFunc());
        }
    }
    private IEnumerator attackFunc()
    {
        yield return new WaitForSeconds(anim[1].length);
        attack = false;
        
        // because walking is a looped animation, if boss attacks midway of the walking animation, I want to return back to the walking animation if boss was walking
        if (walk)
        {
            a.SetInteger("state", (int)States.walk);
        }
    }


    private IEnumerator jumpFunc()
    {
        // For the boss' jump animation, I want to first take a snapshot of the MC's position before jumping. I also need the jump duration as well as the elapsed time of the jump.
        // To make a parabolic jump from the boss' position to the snapshot of MC's position, I have to use a combination of the sine function and linear interpolation.
        // I'll explain each line and what its function is in the jump animation (I've commented the line #s)
        // line 1: Need a while loop to check if the time elapsed for the jump animation exceeds the jump duration (that I set)
        // line 2: Divide timeElapsed by jumpDuration, which will give me a percentage of how much of the jump duration has elapsed (will use this for jump height in sine function and linear interpolation).
        // line 3: The sine function goes from sin(pi*0) = sin(0) = 0 (start of jump), to sin(pi*1) = sin(pi) = 0 (landing). The halfway angle, or the highest point of the jump, is sin(pi*0.5) = sin(pi/2) = 1. This gives a parabolic path for my boss sprite. I multiply the resulting sine value by jumpHeight to give incremental increase/decrease in the y-value. If I want my jumpHeight = 4 for example, the height would start at sin(0)*4 = 0*4 = 0. Halfway (highest) point would be sin(pi/2)*4 = 1*4 = 4. Landing height would be sin(pi)*4 = 0*4 = 0.
        // line 4: Linear interpolation is used when you want to go from point A (start position) to point B (end position) on a straight line, advancing by a value determined by 't' (a value between 0 and 1). 't' is basically a percentage of how far you've come on the line between the 2 points. So if point A = (0,0) and B = (1,1), then when t = 0, you are 0% through the line, so it returns (0,0). When t = 0.5, you are 50% through the line, so it returns (0.5,0.5). When t = 1, you are 100% throught the line, so it returns (1,1). So the purpose of Vector2.Lerp() is to get the percentage of how far the boss has travelled the jump path. But we're not done. Since I want to have a parabolic jump, I first take the position of the boss from the jump path using linear interpolation, and then multiply the y-value (Vector2.up = (0,1)) by the yOffset.
        // line 5: I assign the position of the boss to be the calculated parabolic position
        // line 6: Increment timeElapsed by Time.deltaTime to be checked against jumpDuration in the while loop condition
        jump = true;
        a.SetInteger("state", (int)States.jump);
        startJump.Play();
        yield return new WaitForSeconds(anim[2].length);

        float timeElapsed = 0.0f; // time elapsed from the beginning of jump until the end
        snapshotMCPosition = MC.position;
        jumpStartPosition = transform.position;

        while (timeElapsed < jumpDuration) // line 1
        {
            float percentageElapsed = timeElapsed / jumpDuration; // line 2
            float yOffset = Mathf.Sin(Mathf.PI * percentageElapsed) * jumpHeight; // line 3
            Vector2 parabolicPosition = Vector2.Lerp(jumpStartPosition, snapshotMCPosition, percentageElapsed) + Vector2.up * yOffset; // line 4
            transform.position = parabolicPosition; // line 5

            timeElapsed += Time.deltaTime; // line 6
            yield return null; // Let the physics update. Just goes to next frame to render the boss' position incrementally.
        }
        // Trigger screen shake when the boss lands
        Camera.main.GetComponent<ScreenShake>().Shake();
        
        float originalRadius = GetComponent<CircleCollider2D>().radius;
        GetComponent<CircleCollider2D>().radius = originalRadius * 2; // splash damage from jump landing

        a.SetTrigger("land");
        jumpLanding.Play();
        yield return new WaitForSeconds(anim[2].length);
        
        GetComponent<CircleCollider2D>().radius = originalRadius;

        jump = false;
    }


}
