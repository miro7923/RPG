using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : BaseItemInfo
{
    public float Range { get; protected set; } //공격 사거리 
    public float Power { get; protected set; } //공격 데미지 
    public string SkillName { get; protected set; }

    public override void SetItem(string type, string engName, string korName, float price, float delay,
        ITEM_SORT sort, float restoreRate = 0, float range = 0, float pow = 0, string skillName = null)
    {
        base.SetItem(type, engName, korName, price, delay, sort);
        Range = range;
        Power = pow;
        SkillName = skillName;
    }
}
