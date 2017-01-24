using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour , IAmLiving
{
    [SerializeField]
    protected Stats userStats;
    protected int powerPoints;
    protected IAmLiving enemyCaster;
    
    protected bool dead;
    protected bool isLVLing = false;

    public event System.Action OnDeath;


    // Use this for initialization
    protected virtual void Start () {
	
	}

    #region UPDATE
    protected virtual void Update()
    {

    }
    #endregion

    #region INTERFACE
    public void TakeDamage()
    {

    }
    #endregion
}

#region USERSTATS

[System.Serializable]
public class Stats
{
    private float Level = 1;
    public float xp = 0;
    public float xpOnDeath;
    public float maxHealth = 100;
    public float health;
    public float healthRegen = 1;
    public float maxMana = 100;
    public float mana;
    public float manaRegen = 8;
    public float armor = 20;
    public float armorPen = 0;
    public float magicResist = 15;
    public float magicPen = 0;
    public float moveMentspeed = 10;
    public float sizeMod = 1;
    public float attackDamage = 10;
    public float magicDamage = 0;
    public float attackspeed = 1.2f;
    public float cooldownReduction = 0;
    public int powerPointsPerLVL = 10;
}

#endregion
