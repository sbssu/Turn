using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playable : Character
{

    public override bool IsPlayable => true;
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
