using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_CharacterInfo", menuName = "Characters/CharacterInfo")]
public class SO_CharacterInfo : ScriptableObject
{
    [Header("Combat Stats")]

    public float maxHealth;
    public float maxStamina;

    public float healthRegenRate;
    public float staminaRegenRate;

    public float attackDamage;
    public float attackSpeed;

    public Dictionary<DamageType, float> damageTypeMultipliers;

    [Header("Animations")]
    public Animation idleAnimation;
    public Animation deathAnimation;
    public Animation attackAnimation;

    [Header("Gameplay")]
    public GameObject characterModel;

    // public BehaviourTree behaviour


}
