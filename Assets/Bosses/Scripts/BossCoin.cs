using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCoin : Entity
{
    public Animator anim;
    //public bool enemyDead;
    public float timer = 1f;
    public Rigidbody2D Coin;
    public Material fade;
    public int value;

    public void Awake()
    {
        //enemy = GetComponentInParent<Enemy>();
        anim = GetComponent<Animator>();
        fade = GetComponent<SpriteRenderer>().sharedMaterial;
        //enemyDead = GameObject.FindGameObjectWithTag("EnemiesDead").GetComponent<EnemiesDead>();
    }

    // Start is called before the first frame update
    void Start()
    {
        fade.SetFloat("_Fade", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(IsDead)
        {

            timer -= 0.01f;
            fade.SetFloat("_Fade", timer);
            if (timer <= 0f)
            {
                fade.SetFloat("_Fade", 1f);
                Rigidbody2D rb = GameObject.Instantiate(Coin, transform.position, Quaternion.identity);
                rb.gameObject.GetComponent<Coin>().value = value;
                GameObject.Destroy(gameObject);
            }
        }
    }

    public void OnBossDeath()
    {
        IsDead = true;
        anim.speed = 0f;
    }
}
