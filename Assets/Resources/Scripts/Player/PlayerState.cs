using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//player 정보 저장 & 출력 & level up
public class PlayerState : MonoBehaviour
{
    private static PlayerState m_Instance = null;
    public static PlayerState GetInstance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType<PlayerState>();
                if (!m_Instance) m_Instance = new GameObject("PlayerState").AddComponent<PlayerState>();
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }

    //level, 체력, 마력, 공격력, 방어력, 경험치, 돈
    public int Level { get; private set; }
    public float Hp { get; private set; }
    public float Mp { get; private set; }
    public float Power { get; private set; }
    public float Deffense { get; private set; } 
    public float CurExp { get; private set; } 
    public float MaxExp { get; private set; }
    public float Gold { get; private set; }

    public List<KeyValuePair<BaseItemInfo, int>> PlayerItems { get; private set; }
        = new List<KeyValuePair<BaseItemInfo, int>>(new KeyValuePair<BaseItemInfo, int>[1]);

    private PlayerHealth m_PlayerHealth;

    private void Awake()
    {
        if (!m_Instance)
        {
            Level = 1; //플레이어 초기 능력치 설정 
            Hp = 30f;
            Mp = 20f;
            Power = 5f;
            Deffense = 2f;
            CurExp = 0;
            MaxExp = 10f;
            Gold = 100;
        }

        if (this != GetInstance)
            Destroy(gameObject);
    }

    public void Init()
    {
        MainCanvas.GetInstance.UpdateLvText(Level);
        MainCanvas.GetInstance.InitExpSlider(MaxExp, CurExp);
    }

    public void SetPlayerHealth(PlayerHealth playerHealth)
    {
        m_PlayerHealth = playerHealth;
    }

    public void GetExp(float exp)
    {
        CurExp += exp;

        MainCanvas.GetInstance.UpdateExpValue(CurExp);

        //경험치 찼으면 레벨업 
        if (MaxExp <= CurExp)
            LevelUp();
    }

    private void LevelUp()
    {
        //lv up
        ++Level; 
        //체력, 마력 증가
        Hp += Hp * 0.12f;
        Mp += Mp * 0.1f;
        //공격력, 방어력 증가
        Power += Level * 0.5f;
        Deffense += Level * 0.3f;
        //현재 경험치에서 레벨업에 필요했던 경험치 빼기  
        CurExp -= MaxExp;
        //레벨업 경험치 증가 
        MaxExp *= 1.5f;

        m_PlayerHealth.InitHealth(Hp, Mp);

        //UI 갱신 
        MainCanvas.GetInstance.UpdateLvText(Level);
        MainCanvas.GetInstance.InitExpSlider(MaxExp, CurExp);
        MainCanvas.GetInstance.InitHealthSlider(Hp, Mp);
    }

    public void GetGold(float gold)
    {
        Gold += gold;
    }

    public void BuyItem(float price)
    {
        Gold -= price;
    }

    public void SellItem(float income)
    {
        Gold += income;
    }

    public void SetWeaponInInventory(BaseItemInfo weapon)
    {
        var pair = new KeyValuePair<BaseItemInfo, int>(weapon, 1);
        //0번에는 항상 현재 착용중인 무기 저장
        PlayerItems[0] = pair;
    }

    public void SwapWeapon(int index)
    {
        KeyValuePair<BaseItemInfo, int> curWeapon = PlayerItems[0];
        PlayerItems[0] = PlayerItems[index];
        PlayerItems[index] = curWeapon;
    }

    public void AddItemInInvevtory(BaseItemInfo item, int count, bool isAdd)
    {
        var newItem = new KeyValuePair<BaseItemInfo, int>(item, count);
        switch (isAdd)
        {
            case true:
                for (int i = 0; PlayerItems.Count > i; i++)
                {
                    if (PlayerItems[i].Key.EngName.Equals(item.EngName))
                    {
                        newItem = new KeyValuePair<BaseItemInfo, int>(item, count + PlayerItems[i].Value);
                        PlayerItems[i] = newItem;
                    }
                }
                break;
            case false:
                PlayerItems.Add(newItem);
                break;
        }
    }

    public void RemoveItemInInventory(int index, int quantity = 1)
    {
        if (quantity == PlayerItems[index].Value)
            PlayerItems.RemoveAt(index);
        else
        {
            var newVal = new KeyValuePair<BaseItemInfo, int>(PlayerItems[index].Key, PlayerItems[index].Value - quantity);
            PlayerItems[index] = newVal;
        }
    }

    public BaseItemInfo GetPotion(ITEM_SORT eSort)
    {
        switch (eSort)
        {
            case ITEM_SORT.Hp:
                if (m_PlayerHealth.Health >= Hp)
                    return null;
                break;
            case ITEM_SORT.Mp:
                if (m_PlayerHealth.CurMp >= Mp)
                    return null;
                break;
        }

        for (int i = 0; PlayerItems.Count > i; i++)
        {
            if (PlayerItems[i].Key.Sort.Equals(eSort) && 0 < PlayerItems[i].Value)
            {
                PlayerItems[i] = new KeyValuePair<BaseItemInfo, int>(PlayerItems[i].Key, PlayerItems[i].Value - 1);

                MainCanvas.GetInstance.ShowPotionCount(eSort, PlayerItems[i].Value);

                return PlayerItems[i].Key;
            }
        }

        return null;
    }
}
