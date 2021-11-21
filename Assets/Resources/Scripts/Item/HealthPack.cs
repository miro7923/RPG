using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : BaseItemInfo
{
    public float RestoreRate { get; protected set; }

    public override void SetItem(string type, string engName, string korName, float price, float delay,
        ITEM_SORT sort, float restoreRate = 0, float range = 0, float pow = 0, string skillName = null)
    {
        base.SetItem(type, engName, korName, price, delay, sort);

        RestoreRate = restoreRate;
    }
}
