using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowImp_Ability : Ability
{
    public float timer = 0f;
    [SerializeField] private Animator animator;
    public Sprite sprite;
    public bool isReady = false;
    [SerializeField] private AudioSource audioSource;
    public AudioClip vanish;
    //private BoxCollider2D cc;
    private CapsuleCollider2D cc;
    private bool isVanished;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
        cc = GetComponent<CapsuleCollider2D>();
    }

    public override void OnAbility()
    {
        if(cooldown <= 0 && isReady && !isVanished)
        {
            isVanished = true;
            animator.GetComponent<SpriteRenderer>().sprite = null;
            animator.enabled = false;
            cc.enabled = false;
            gameObject.tag = "Untagged";
            audioSource.PlayOneShot(vanish, 0.3f);
        }

        if(isVanished)
        {
            timer += Time.deltaTime;
        }

        if (timer >= 3)
        {
            cc.enabled = true;

            //while(isVanished)
            //RandomPos();

            RandomPos();

            animator.enabled = true;
            //cc.enabled = true;
            gameObject.tag = "Enemy";
            animator.GetComponent<SpriteRenderer>().sprite = sprite;
            cooldown = 5f;
            timer = 0f;
            isReady = false;
            isVanished = false;
            audioSource.PlayOneShot(vanish, 0.3f);
        }
    }

    /*public void OnTriggerEnter2D(Collider2D col)
    {
        if(!col.CompareTag("collisionTilemap") && isVanished)
        {
            //RandomPos();
            isVanished=false;
            Debug.Log("collided");
            Debug.Log(col.gameObject.name);
        }
    }*/

    public void RandomPos()
    {
        int ranPos = Random.Range(0, 4);

        if (ranPos == 0)
        {
            transform.position = new Vector2(PlayerTransform.position.x, PlayerTransform.position.y + 0.8f);
        }
        else if (ranPos == 2)
        {
            transform.position = new Vector2(PlayerTransform.position.x, PlayerTransform.position.y - 1);
        }
        else if (ranPos == 1)
        {
            transform.position = new Vector2(PlayerTransform.position.x + 1, PlayerTransform.position.y);
        }
        else
            transform.position = new Vector2(PlayerTransform.position.x - 0.8f, PlayerTransform.position.y);

        //isVanished = false;
    }
}