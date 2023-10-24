using UnityEngine;

[CreateAssetMenu(fileName = "SO_Weapon", menuName = "Inventory/Weapon")]
public class SO_Weapon : SO_Item
{
    public float damage;
    public DamageType damageType;
}
