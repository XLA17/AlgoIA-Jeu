using UnityEngine;

[CreateAssetMenu(fileName = "NewUnit", menuName = "Scriptable Objects/Unit")]
public class UnitScriptableObject : ScriptableObject
{
    public int maxHealth;
    public float speed;
    public int damageDealt;
}
