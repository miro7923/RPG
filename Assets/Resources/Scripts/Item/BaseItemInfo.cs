using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseItemInfo
{
    //공통 필요 정보
    public string Type { get; protected set; }
    public string EngName { get; protected set; }
    public string KorName { get; protected set; }
    public float Price { get; protected set; }
    public float Delay { get; protected set; }
    public ITEM_SORT Sort { get; protected set; }

    virtual public void SetItem(string type, string engName, string korName, float price, float delay,
        ITEM_SORT sort, float restoreRate = 0, float range = 0, float pow = 0, string skillName = default)
    {
        Type = type;
        EngName = engName;
        KorName = korName;
        Price = price;
        Delay = delay;
        Sort = sort;
    }
}
