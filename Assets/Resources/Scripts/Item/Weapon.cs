using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string KorName { get; protected set; }
    public float Price { get; protected set; }
    public float Delay { get; protected set; }
    public ITEM_SORT Sort { get; protected set; }
    public float Range { get; protected set; } //공격 사거리 
    public float Power { get; protected set; } //공격 데미지 
    public string SkillName { get; protected set; }

    public void SetWeapon(WeaponData weaponData)
    {
        KorName = weaponData.KorName;
        Price = weaponData.Price;
        Range = weaponData.Range;
        Power = weaponData.Power;
        Sort = weaponData.Sort;
        SkillName = weaponData.SkillName;
    }

    public virtual IEnumerator AttackEffect(Vector3 hitPos)
    {
        yield return null;
    }
}
