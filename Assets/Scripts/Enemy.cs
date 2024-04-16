using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public override bool IsPlayable => false;
    public override bool SkillQ()
    {
        return false;
    }
    public override bool SkillE()
    {
        return false;
    }
    public override bool SkillR()
    {
        return false;
    }
}
