using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class ItemManager : MonoBehaviour
{
    private static ItemManager m_Instance;
    public static ItemManager GetInstance
    {
        get
        {
            if (!m_Instance)
            {
                m_Instance = FindObjectOfType<ItemManager>();
                if (!m_Instance) m_Instance = new GameObject("ItemManager").AddComponent<ItemManager>();
                DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }

    private Sprite[] Icons; //아이템 아이콘들을 저장할 배열

    public Dictionary<string, List<BaseItemInfo>> Items { get; private set; }
        = new Dictionary<string, List<BaseItemInfo>>();

    public void Awake()
    {
        if (!m_Instance)
            Init();

        if (this != GetInstance)
            Destroy(gameObject);
    }

    private void Init()
    {
        Icons = Resources.LoadAll<Sprite>("Icons");
        LoadWeaponList();
        MakeHealthPacks();
        RegistBaseItems();
    }

    private void LoadWeaponList()
    {
        Items.Add("Weapon", new List<BaseItemInfo>());

        TextAsset textAsset = Resources.Load<TextAsset>("WeaponList");
        //텍스트파일에서 내용을 불러온 뒤 줄 단위로 잘라서 배열에 저장한다. 
        string[] lines = textAsset.text.Split('\n');

        foreach (string line in lines)
        {
            //줄 단위로 저장된 텍스트를 띄어쓰기 단위로 잘라서 배열에 저장한다.
            //각 인덱스에는
            //0: 무기 종류
            //1: 무기 영문 이름  
            //2: 무기 한글 이름
            //3: 무기 공격력
            //4: 무기 가격
            //5: 무기 공격 범위
            //6: 무기 공격 속도 
            string[] words = line.Split(' ');

            string skillName = "";
            ITEM_SORT eWeapon = ITEM_SORT.None;
            switch (words[0])
            {
                case "Sword":
                    skillName = "소드 슬래시";
                    eWeapon = ITEM_SORT.Sword;
                    break;
                case "Hammer":
                    skillName = "해머 어택";
                    eWeapon = ITEM_SORT.Hammer;
                    break;
                case "Wand":
                    skillName = "메테오";
                    eWeapon = ITEM_SORT.Wand;
                    break;
            }

            //무기 정보 저장 
            BaseItemInfo weapon = new WeaponData();
            weapon.SetItem("Weapon", words[1], words[2], int.Parse(words[4]), float.Parse(words[6]), eWeapon,
                0, float.Parse(words[5]), float.Parse(words[3]), skillName);

            Items["Weapon"].Add(weapon);
        }
    }

    private void MakeHealthPacks()
    {
        Items.Add("Item", new List<BaseItemInfo>());

        BaseItemInfo Hp = new HealthPack();
        Hp.SetItem("Item", "Hp", "빨간 포션", 10f, 1f, ITEM_SORT.Hp, 0.2f);

        BaseItemInfo Mp = new HealthPack();
        Mp.SetItem("Item", "Mp", "파란 포션", 20f, 1f, ITEM_SORT.Mp, 0.1f);

        Items["Item"].Add(Hp);
        Items["Item"].Add(Mp);
    }

    private void RegistBaseItems()
    {
        //기본 무기 등록 
        PlayerState.GetInstance.SetWeaponInInventory(Items["Weapon"][0]);
        //기본 아이템들 등록 
        PlayerState.GetInstance.AddItemInInvevtory(Items["Item"][0], 5, false);
        PlayerState.GetInstance.AddItemInInvevtory(Items["Item"][1], 5, false);
    }

    public Sprite GetSprite(string name)
    {
        foreach (var icon in Icons)
        {
            if (icon.name.Equals(name))
                return icon;
        }

        return null;
    }
}
