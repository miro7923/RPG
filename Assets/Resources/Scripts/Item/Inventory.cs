using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class Inventory : MonoBehaviour
{
    [SerializeField] private Text m_PlayerGoldTxt; //플레이어 소지금

    [SerializeField] private List<Image> m_Slots;
    [SerializeField] private Sprite m_DefaultSlotImg;
    [SerializeField] private GameObject m_CountTextPrefab; //아이템 갯수 표시할 텍스트 프리펩 
    private int m_SlotIndex = 0; //몇 번째 슬롯에 이미지를 표시할 지 판단할 인덱스 값
    public int SelectedItemIndex { get; private set; } = -1; //몇 번째 슬롯에 있는 아이템이 선택되었는지 확인할 인덱스 값

    [SerializeField] private Text m_InventoryTxt; //플레이어 소지품 정보를 보여줄 텍스트

    [SerializeField] private Button m_ChangeButton; //무기 교체 버튼

    private void ShowItem(KeyValuePair<BaseItemInfo, int> item)
    {
        m_Slots[m_SlotIndex].sprite = ItemManager.GetInstance.GetSprite(item.Key.EngName);
        m_Slots[m_SlotIndex].color = new Color(1, 1, 1);

        if (0 < m_SlotIndex)
        {
            Text text = m_Slots[m_SlotIndex].GetComponentInChildren<Text>();
            if (!item.Key.Type.Equals("Weapon"))
            {
                int count = item.Value;
                if (text)
                    text.text = count.ToString();
                else
                {
                    text = Instantiate(m_CountTextPrefab, m_Slots[m_SlotIndex].transform).GetComponent<Text>();
                    text.text = count.ToString();
                }
                text.rectTransform.localPosition = Vector3.zero;
                text.gameObject.SetActive(true);
            }
            else
            {
                if (text)
                    text.gameObject.SetActive(false);
            }
        }

        m_SlotIndex++;
    }

    public void LoadItemList()
    {
        //슬롯 이미지 기본값으로 초기화 후 처음부터 가지고 있는 아이템 이미지 출력 
        for (int i = 0; m_Slots.Count > i; i++)
        {
            m_Slots[i].sprite = m_DefaultSlotImg;
            m_Slots[i].color = new Color(156 / 255f, 156 / 255f, 86 / 255f);

            if (1 <= i)
            {
                Text text = m_Slots[i].GetComponentInChildren<Text>();
                if (text)
                    text.gameObject.SetActive(false);
            }
        }

        m_SlotIndex = 0;

        for (int i = 0; PlayerState.GetInstance.PlayerItems.Count > i; i++)
            ShowItem(PlayerState.GetInstance.PlayerItems[i]);

        //플레이어 소지금 출력 
        m_PlayerGoldTxt.text = PlayerState.GetInstance.Gold.ToString();
    }

    public void ShowMyItemInfo(int index)
    {
        SelectedItemIndex = index;

        if (m_SlotIndex > SelectedItemIndex)
        {
            var item = PlayerState.GetInstance.PlayerItems[index];

            WeaponData weapon = null;
            HealthPack healthPack = null;

            if (item.Key.Type.Equals("Weapon"))
            {
                weapon = ItemManager.GetInstance.Items["Weapon"].Find((obj) => obj.Equals(item.Key)) as WeaponData;

                if (0 < SelectedItemIndex)
                    m_ChangeButton.gameObject.SetActive(true); //change 버튼 활성화
                else
                    m_ChangeButton.gameObject.SetActive(false);
            }
            else if (item.Key.Type.Equals("Item"))
            {
                healthPack = ItemManager.GetInstance.Items["Item"].Find((obj) => obj.Equals(item.Key)) as HealthPack;
                m_ChangeButton.gameObject.SetActive(false);
            }

            if (null != weapon || null != healthPack)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("이름 : ");

                if (null != weapon)
                {
                    sb.Append(weapon.KorName);
                    sb.Append("\n");

                    sb.Append("공격력 : ");
                    sb.Append(weapon.Power);
                    sb.Append("\n");

                    sb.Append("상점 판매 가격 : ");
                    sb.Append(weapon.Price * 0.3f);
                }
                else
                {
                    sb.Append(healthPack.KorName);
                    sb.Append("\n");

                    if (healthPack.Sort.Equals(ITEM_SORT.Hp))
                        sb.Append("회복량 : 전체 체력의 ");
                    else
                        sb.Append("회복량 : 전체 마력의 ");

                    sb.Append(healthPack.RestoreRate * 100);
                    sb.Append("%\n");

                    sb.Append("상점 판매 가격 : ");
                    sb.Append(healthPack.Price * 0.3f);
                }
                sb.Append("\n");

                m_InventoryTxt.text = sb.ToString();
            }
        }
    }

    public void ChangeWeapon()
    {
        //플레이어가 소지한 아이템 내에서 무기를 바꾼 후
        PlayerState.GetInstance.SwapWeapon(SelectedItemIndex);
        //바뀐 리스트를 토대로 다시 출력 
        LoadItemList();
    }
}
