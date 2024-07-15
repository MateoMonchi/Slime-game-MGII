using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Peleador", menuName = "Peleador/Crear nuevos peleadores")]

public class PeleadoresBase : ScriptableObject
{
    [SerializeField] string name;
    [TextArea]
    [SerializeField] string description;
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    //Stats Bases
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int magicAttack;
    [SerializeField] int magicDefense;
    [SerializeField] int speed;
   
    public string Name
    {
        get { return name; }
    }
    public string Description
    {
        get { return description; }
    }
    public int MaxHp
    {
        get { return maxHp; }
    }
    public int Attack
    {
        get { return attack; }
    }
    public int Defense
    {
        get { return defense; }
    }
    public int MagicAttack
    {
        get { return magicAttack; }
    }
    public int MagicDefense
    {
        get { return magicDefense; }
    }
    public int Speed
    {
        get { return speed; }
    }
}
