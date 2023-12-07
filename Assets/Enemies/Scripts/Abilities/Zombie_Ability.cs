using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie_Ability : Ability
{
    private Enemy enemy;
    public int chance;
    private AudioSource audioSource;

    private void Start()
    {
        enemy = GetComponent<Enemy>();
        audioSource = GetComponent<AudioSource>();
    }

    public override void OnAbility()
    {
        chance = Random.Range(0, 3);

        if (chance == 0)
        {
            //Debug.Log("heavy attack");
            enemy.animator.speed = 0.75f;
            enemy.damage *= 1.5f;
            audioSource.pitch = 0.6f;
        }
        else
        {
            enemy.damage = enemy.NormDamage;
            enemy.animator.speed = 1.35f;
            audioSource.pitch = 1f;
        }
    }
}
