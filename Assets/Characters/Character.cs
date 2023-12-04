using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    // Character Stats
    float currentHealth;
    float CurrentStamina;

    [Header("Character Preset")]
    public SO_CharacterInfo characterInfo;

    [SerializeField, Tooltip("The Amount Of Stamina Used Per Second While Sprinting")]
    public float sprintStaminaUsage;

    GameObject model;

    [Space(20)]
    [Header("Events")]
    [SerializeField, Tooltip("Initializes Health")]
    UnityEvent<float> OnInitHealth = new();

    [SerializeField, Tooltip("Initializes Stamina")]
    UnityEvent<float> OnInitStamina = new();

    [SerializeField, Tooltip("Passes Current Health")]
    UnityEvent<float> OnTakeDamage = new();

    [SerializeField]
    UnityEvent OnDie = new();

    [SerializeField]
    UnityEvent OnAttack = new();

    [SerializeField, Tooltip("Passes Current Stamina")]
    UnityEvent<float> OnStaminaUsed = new();


    // Start is called before the first frame update
    void Start()
    {
        // Spawn Character
        SpawnCharacter();
        // Initialize Stats
        InitStats();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnCharacter()
    {
        if (characterInfo.characterModel)
        {
            model = Instantiate(characterInfo.characterModel, transform);
        }
    }

    void InitStats()
    {
        // Set stats based on character info

        currentHealth = characterInfo.maxHealth;
        CurrentStamina = characterInfo.maxStamina;
        OnInitHealth.Invoke(currentHealth);
        OnTakeDamage.Invoke(currentHealth);

        OnInitStamina.Invoke(CurrentStamina);
        OnStaminaUsed.Invoke(CurrentStamina);
    }

    public void TakeDamage(float damage, DamageType type = DamageType.DEFAULT)
    {
        float totalDamage = damage;

        if (type != DamageType.DEFAULT)
        {
            // Multiply Damage by Resistances & Weaknesses
            foreach (KeyValuePair<DamageType, float> kvp in characterInfo.damageTypeMultipliers)
            {
                if (kvp.Key == type)
                {
                    totalDamage /= kvp.Value;
                }
            }
        }

        currentHealth -= totalDamage;
        OnTakeDamage.Invoke(currentHealth);

        if (currentHealth <= 0) Die();

        // This is needed for if this function is called to heal the character;
        if (currentHealth > characterInfo.maxHealth) currentHealth = characterInfo.maxHealth;
    }

    public void Die()
    {
        OnDie.Invoke();
        // TODO:: Implement Die
    }

    public void UseStamina(float staminaUsed)
    {
        CurrentStamina -= staminaUsed * 0.05f;
        OnStaminaUsed.Invoke(CurrentStamina);
    }

    public void Attack()
    {
        OnAttack.Invoke();
        // TODO:: Implement Attack
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
