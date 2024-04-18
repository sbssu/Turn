using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayerRange : Playable
{
    protected override void SkillQ()
    {

        if (isLocked)
            return;

        isLocked = true;
        anim.SetTrigger("onAttack");
    }
}
