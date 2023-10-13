using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // Current Character Health
    float health;

    [SerializeField, Tooltip("Max Health The Character Can Have")]
    public float maxHealth;

    // Current Character Stamina
    public float stamina;

    [SerializeField, Tooltip("Max Stamina The Character Can Have")]
    public float maxStamina;

    [SerializeField, Tooltip("Damage Weaknesses & Resistances. Higher Values Offer More Protection")]
    public Dictionary<DamageType, float> damageTypeMultipliers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TakeDamage(float damage, DamageType type = DamageType.DEFAULT)
    {
        float totalDamage = damage;

        if (type != DamageType.DEFAULT)
        {
            // Multiply Damage by Resistances & Weaknesses
            foreach (KeyValuePair<DamageType, float> kvp in damageTypeMultipliers)
            {
                if (kvp.Key == type)
                {
                    totalDamage /= kvp.Value;
                }
            }
        }

        health -= totalDamage;

        if (health <= 0) Die();

        // This is needed for if this function is called to heal the character;
        if (health > maxHealth) health = maxHealth;
    }

    void Die()
    {
        // TODO:: Implement Die
    }
}

// Types of Status Effects 
public enum StatusEffect
{ 
    BURNING,
    FROZEN,
    CURSED,
    BLINDED
}
