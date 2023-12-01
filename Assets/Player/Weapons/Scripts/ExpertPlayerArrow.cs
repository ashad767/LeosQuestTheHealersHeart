using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExpertPlayerArrow : AdvancedPlayerArrow
{
    public EffectParticles effect;
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        Entity hitObject = collision.GetComponentInParent<Entity>();

        if (!collision.CompareTag("Player") && hitObject != null)
        {
            EffectParticles addedEffect = Instantiate(effect);
            addedEffect.transform.SetParent(collision.transform);
        }

        base.OnTriggerEnter2D(collision);
    }
}
