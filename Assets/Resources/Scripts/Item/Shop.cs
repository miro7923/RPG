using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;

public class Shop : MonoBehaviour
{
    [SerializeField] private Text m_ItemInfoTxt; //상점 판매 아이템 정보 텍스트

    private List<BaseItemInfo> m_CurItems = new List<BaseItemInfo>(); //현재 선택된 무기 종류만 가지고 있을 배열

    [SerializeField] private Image[] m_ScorllContents; //아이템 스크롤 뷰의 컨텐츠를 가지고 있을 배열
    [SerializeField] private Image[] m_UpDownButtons; //아이템 스크롤 뷰의 아래위 화살표를 가지고 있을 배열 

    [SerializeField] private Inventory m_Inventory; //플레이어 인벤토리 

    [SerializeField] private GameObject m_Popup_ShortOfCash; //소지금 모자랄 때 출력할 팝업 오브젝트 

    [SerializeField] private Text m_PriceTxt; //아이템 가격 txt 오브젝트 

    [SerializeField] private ScrollRect m_Scroll; //아이템 스크롤뷰 오브젝트 

    [SerializeField] private GameObject m_Popup_CannotSell; //판매 불가능한 아이템용 예외처리 팝업 오브젝트 
    [SerializeField] private GameObject m_Popup_PleaseChoose; //아이템이 선택되지 않았는데 판매버튼을 눌렀을 때 출력할 팝업 오브젝트 

    [SerializeField] private GameObject m_Popup_ConfirmToAction; //구매 or 판매를 할 지 최종 확인할 때 출력할 팝업 오브젝트 
    [SerializeField] private Text m_ConfirmPopupTxt; //↑ 팝업의 텍스트 오브젝트(구매/판매에 따라 텍스트만 다르게 출력)
    [SerializeField] private GameObject m_Popup_Quantity; //구매 or 판매 수량 확인할 팝업 오브젝트 
    [SerializeField] private Text m_QuantityPopupTxt; //↑ 팝업의 텍스트 오브젝트(구매/판매에 따라 텍스트만 다르게 출력)
    [SerializeField] private GameObject m_Keypad; //구매 or 수량 입력받을 때 사용할 숫자패드 오브젝트 
    [SerializeField] private Text m_QuantityTxt; //↑ 통해서 입력받은 수량을 출력할 텍스트 오브젝트 (최대 999 제한)
    [SerializeField] private GameObject m_Popup_OutOfQuantity; //소지한 수량보다 많은 수량을 입력했을 때 출력할 팝업 오브젝트 
    private StringBuilder m_sbItemQuantity = new StringBuilder(); //입력받은 수량을 텍스트용으로 합쳐줄 stringBuilder  
    private int m_iItemQuantity = 1; //아이템 구매/판매 수량 (기본값은 1개)

    private int m_SlotIndex = 0; //판매용 아이템을 보여줄 슬롯 인덱스(0번부터 시작)

    private bool m_isBuy = true; //현재 판매모드인지 구매모드인지 확인할 변수(위에서 선언한 팝업 오브젝트들의 텍스트만 바꿔 출력할 때 사용)

    private void Start()
    {
        Init();

        GameManager.GetInstance.SetShop(this);
    }

    public void Init()
    {
        DeactivateSlots();

        GetItemsBySort("Sword");

        m_Inventory.LoadItemList();
    }

    private void DeactivateSlots()
    {
        foreach (var content in m_ScorllContents)
            content.gameObject.SetActive(false);

        foreach (var button in m_UpDownButtons)
            button.gameObject.SetActive(false);

        //스크롤 맨 위로 이동 
        m_Scroll.verticalNormalizedPosition = 1;

        //설명 및 가격 텍스트 초기화 
        m_ItemInfoTxt.text = "";
        m_PriceTxt.text = "";
    }

    public void GetItemsBySort(string sort)
    {
        //아이템 출력용 슬롯 전체 비활성화 후 필요한 만큼만 활성화 처리 
        DeactivateSlots();

        m_CurItems.Clear();

        if (sort.Equals("Sword") || sort.Equals("Hammer") || sort.Equals("Wand"))
        {
            ITEM_SORT eSort = ITEM_SORT.None;
            switch (sort)
            {
                case "Sword":
                    eSort = ITEM_SORT.Sword;
                    break;
                case "Hammer":
                    eSort = ITEM_SORT.Hammer;
                    break;
                case "Wand":
                    eSort = ITEM_SORT.Wand;
                    break;
            }

            //분류별로 출력을 하기 위해서 전체 무기 리스트에서 매개변수로 주어지는 종류만 가져옴 
            foreach (var item in ItemManager.GetInstance.Items["Weapon"])
            {
                if (item.Sort.Equals(eSort))
                    m_CurItems.Add(item);
            }
        }
        else if (sort.Equals("Item"))
        {
            foreach (var item in ItemManager.GetInstance.Items["Item"])
                m_CurItems.Add(item);
        }

        //무기 갯수만큼 컨텐츠 활성화 처리를 여기서 하기
        //종류별로 아이콘 이미지를 찾아와서 넣어준다. 
        for (int i = 0; m_CurItems.Count > i; i++)
        {
            m_ScorllContents[i].gameObject.SetActive(true);
            m_ScorllContents[i].color = new Color(255, 255, 255, 255);
            m_ScorllContents[i].sprite = ItemManager.GetInstance.GetSprite(m_CurItems[i].EngName);
        }

        //선택된 아이템 갯수가 3개보다 많으면 스크롤 가능한 화살표 활성화
        if (3 < m_CurItems.Count)
        {
            foreach (var button in m_UpDownButtons)
                button.gameObject.SetActive(true);
        }
    }

    public void SetSlotIndex(int index)
    {
        //각 슬롯별로 번호를 부여해서 해당 슬롯번호와 일치하는 인덱스에 있는 무기 정보를 보여준다. 
        m_SlotIndex = index;
    }

    public void ShowItemInfo()
    {
        //아이템 하나에 대한 정보를 보여주기 위한 함수
        //컨텐츠의 Button 컴포넌트와 연결해서 인덱스 번호를 가져옴
        //슬롯별로 설정된 인덱스의 아이템 정보를 보여준다.

        StringBuilder sb = new StringBuilder();
        sb.Append("이름 : ");
        sb.Append(m_CurItems[m_SlotIndex].KorName);
        sb.Append("\n");

        if (m_CurItems[m_SlotIndex].Type.Equals("Weapon"))
        {
            var weapon = m_CurItems[m_SlotIndex] as WeaponData;
            sb.Append("공격력 : ");
            sb.Append(weapon.Power);
            sb.Append("\n");

            sb.Append("특수 스킬 : ");
            sb.Append(weapon.SkillName);
            sb.Append("\n");
        }
        else if (m_CurItems[m_SlotIndex].Type.Equals("Item"))
        {
            var item = m_CurItems[m_SlotIndex] as HealthPack;

            if (item.Sort.Equals(ITEM_SORT.Hp))
                sb.Append("회복량 : 전체 체력의 ");
            else
                sb.Append("회복량 : 전체 마력의 ");

            sb.Append(item.RestoreRate * 100);
            sb.Append("%\n");

        }

        m_ItemInfoTxt.text = sb.ToString();

        m_PriceTxt.text = m_CurItems[m_SlotIndex].Price.ToString();
    }

    public void BuyOrSell()
    {
        switch (m_isBuy)
        {
            case true:
                BuyItem();
                break;
            case false:
                SellItem();
                break;
        }
    }

    public void GetQuantity(int num)
    {
        if (3 > m_sbItemQuantity.Length)
        {
            if (0 == m_sbItemQuantity.Length && 0 == num)
                return;

            m_sbItemQuantity.Append(num);
            m_QuantityTxt.text = m_sbItemQuantity.ToString();
        }
    }

    public void UndoQuantity()
    {
        if (0 < m_sbItemQuantity.Length)
        {
            m_sbItemQuantity.Remove(m_sbItemQuantity.Length - 1, 1);
            m_QuantityTxt.text = m_sbItemQuantity.ToString();
        }
    }

    public void ConfirmQuantity()
    {
        m_iItemQuantity = int.Parse(m_QuantityTxt.text);
    }

    public void PopupUI(bool isBuy)
    {
        m_isBuy = isBuy; //팝업 오브젝트를 하나로 돌려쓰기 위해서 현재 구매모드인지 판매모드인지 확인할 변수 

        //구매/판매 하려는 아이템 종류에 따라서 다른 UI 출력
        switch (m_isBuy)
        {
            case true:
                m_QuantityPopupTxt.text = "몇 개 구매 하시겠어요?";
                m_ConfirmPopupTxt.text = "정말 구매 하시겠어요?";

                if (m_CurItems[0].Type.Equals("Item"))
                {
                    //물약 종류 구매시엔 수량 물어보는 ui 출력 
                    m_Keypad.SetActive(true);
                    m_Popup_Quantity.SetActive(true);
                }
                else
                {
                    //무기 종류 구매시엔 1개 구매가 기본값
                    //무기 하나마다 인벤토리 한 칸 차지
                    m_Popup_ConfirmToAction.SetActive(true);
                }
                break;
            case false:
                m_QuantityPopupTxt.text = "몇 개 판매 하시겠어요?";
                m_ConfirmPopupTxt.text = "정말 판매 하시겠어요?";

                if (PlayerState.GetInstance.PlayerItems[m_Inventory.SelectedItemIndex].Key.Type.Equals("Item"))
                {
                    //물약 종류 판매시엔 수량 물어보는 ui 출력 
                    m_Keypad.SetActive(true);
                    m_Popup_Quantity.SetActive(true);
                }
                else
                {
                    //무기 종류 판매시엔 1개 판매가 기본값
                    m_Popup_ConfirmToAction.SetActive(true);
                }
                break;
        }
    }

    public void BuyItem()
    {
        //button 클릭하면 호출
        //현재 선택된 인덱스의 무기 이름을 넘겨준다.
        //player 소지금 줄이고 gold txt 업데이트 

        //가진 돈이 충분하면 아이템을 살 수 있고 아니라면 예외 팝업 출력 
        if (m_CurItems[m_SlotIndex].Price * m_iItemQuantity > PlayerState.GetInstance.Gold)
        {
            m_Popup_ShortOfCash.SetActive(true);
            return;
        }

        PlayerState.GetInstance.BuyItem(m_CurItems[m_SlotIndex].Price * m_iItemQuantity);

        if (m_CurItems[0].Type.Equals("Item"))
            PlayerState.GetInstance.AddItemInInvevtory(m_CurItems[m_SlotIndex], m_iItemQuantity, true);
        else
            PlayerState.GetInstance.AddItemInInvevtory(m_CurItems[m_SlotIndex], m_iItemQuantity, false);

        m_Inventory.LoadItemList();

        //구매가 끝나면 기본 구매 갯수 1로 초기화 및 각종 팝업 닫기
        InitPopups();
    }

    private void InitPopups()
    {
        m_iItemQuantity = 1;
        m_QuantityTxt.text = "";
        m_sbItemQuantity.Clear();
        m_Popup_ConfirmToAction.SetActive(false);
        m_Popup_Quantity.SetActive(false);
        m_Keypad.SetActive(false);
    }

    public void SellItem()
    {
        //button 클릭하면 호출
        //플레이어가 가진 아이템 배열에서 해당 아이템을 삭제하고 그만큼 돈 증가
        //판매 금액은 원래 가격의 30%
        //판매 후 아이템 재배치

        if (0 < m_Inventory.SelectedItemIndex)
        {
            var selectedItem = PlayerState.GetInstance.PlayerItems[m_Inventory.SelectedItemIndex];
            WeaponData weapon = null;
            HealthPack healthPack = null;

            if (selectedItem.Key.Type.Equals("Weapon"))
                weapon = ItemManager.GetInstance.Items["Weapon"].Find((obj) => obj.Equals(selectedItem.Key)) as WeaponData;
            else
                healthPack = ItemManager.GetInstance.Items["Item"].Find((obj) => obj.Equals(selectedItem.Key)) as HealthPack;

            float sellingPrice = 0;
            if (null != weapon)
            {
                //무기는 바로 판매 
                PlayerState.GetInstance.RemoveItemInInventory(m_Inventory.SelectedItemIndex);

                sellingPrice = weapon.Price * 0.3f;
            }
            else
            {
                //물약은 수량 확인 후 판매
                if (m_iItemQuantity > selectedItem.Value)
                {
                    //내가 가진 수량보다 많이 판매하려고 하면 예외 팝업 출력
                    m_Popup_OutOfQuantity.SetActive(true);
                    return;
                }

                PlayerState.GetInstance.RemoveItemInInventory(m_Inventory.SelectedItemIndex, m_iItemQuantity);

                sellingPrice = healthPack.Price * 0.3f * m_iItemQuantity;
            }

            PlayerState.GetInstance.SellItem(sellingPrice);
            m_Inventory.LoadItemList();
        }
        else if (0 == m_Inventory.SelectedItemIndex)
        {
            //0번째엔 항상 현재 착용중인 무기 저장 
            //현재 착용중인 무기는 판매할 수 없어요 출력
            m_Popup_CannotSell.SetActive(true);
        }
        else
        {
            //m_Inventory.SelectedItemIndex의 초기값은 -1 => 아무 아이템을 선택하지 않은 경우 
            //아이템을 선택해 주세요 출력
            m_Popup_PleaseChoose.SetActive(true);
        }

        InitPopups();
    }

    public void LoadPlayScene()
    {
        GameManager.GetInstance.LoadPlayScene();
    }
}
