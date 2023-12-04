using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomStrikeCheck : MonoBehaviour
{
    public Player player;
    public Animator anim;
    public int damage = 3;
    private float timer = 0;
    public bool dmg = false;
    public AudioSource audioSource;

    // Update is called once per frame
    void Update()
    {
        if (timer <= 3 && dmg)
        {
            timer += Time.deltaTime;
            player.TakeDamage(Time.deltaTime * damage);
        }
        else if(timer > 3)
            Destroy(gameObject);
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            dmg = true;
            anim.SetBool("Explode", true);
            audioSource.Play();
            player = col.gameObject.GetComponent<Player>();
        }
    }
}