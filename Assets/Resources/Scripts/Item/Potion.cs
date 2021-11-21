using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    [SerializeField] private PlayerHealth m_PlayerHealth;

    public void UsePotion(string sort)
    {
        ITEM_SORT eSort = ITEM_SORT.None;
        switch (sort)
        {
            case "Hp": eSort = ITEM_SORT.Hp; break;
            case "Mp": eSort = ITEM_SORT.Mp; break;
        }

        var potion = PlayerState.GetInstance.GetPotion(eSort) as HealthPack;

        if (null != potion)
        {
            switch (eSort)
            {
                case ITEM_SORT.Hp:
                    m_PlayerHealth.RestoreHealth(potion.RestoreRate);
                    break;
                case ITEM_SORT.Mp:
                    m_PlayerHealth.RestoreMp(potion.RestoreRate);
                    break;
            }

            StartCoroutine(MainCanvas.GetInstance.ActivateCoolTimeIcon(eSort, potion.Delay));
        }
    }
}
