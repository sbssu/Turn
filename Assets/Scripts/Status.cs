using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Status
{
    public float hp;
    public float attack;
    public float defense;
    public float speed;

    public Status(float hp, float attack, float defense, float speed)
    {
        this.hp = hp;
        this.attack = attack;
        this.defense = defense;
        this.speed = speed;
    }
}
