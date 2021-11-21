using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObjects : MonoBehaviour
{
    [SerializeField] private List<Weapon> m_ListWeapons; //모든 무기 오브젝트
    public List<Weapon> Weapons { get { return m_ListWeapons; } }

    private void Awake()
    {
        SetWeapons();
        WeaponManager.GetInstance.SetWeaponObjs(this);
    }

    private void SetWeapons()
    {
        for (int i = 0; ItemManager.GetInstance.Items["Weapon"].Count > i; i++)
        {
            WeaponData data = ItemManager.GetInstance.Items["Weapon"][i] as WeaponData;
            string weaponName = data.EngName;
            //무기 리스트에서 무기 코드(예:Axe0)와 같은 이름을 가진 오브젝트를 찾아서 정보 세팅 
            Weapon weapon = m_ListWeapons.Find((obj) => obj.name.Equals(weaponName));
            weapon.SetWeapon(data);
        }
    }
}
