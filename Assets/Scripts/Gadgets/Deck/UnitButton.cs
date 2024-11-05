using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    [HideInInspector] public OnePlayer player;
    public int unitId;
    public bool selected;
    public int cost = 100;
    [HideInInspector] public bool isDeckButton = true;

    public RectTransform selectedDisplay;

    

    public Text costText;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            selectedDisplay.localScale = 
                Vector3.Lerp(selectedDisplay.localScale, Vector3.one, Time.unscaledDeltaTime*10);
        }
        else
        {
            selectedDisplay.localScale =
                Vector3.Lerp(selectedDisplay.localScale, Vector3.zero, Time.unscaledDeltaTime * 10);
        }
    }

    public void Clicked()
    {
        if (isDeckButton)
        {
            if (!selected)
            {
                if (player.GetSelectedCount() < player.deckCount)
                {
                    selected = true;
                    player.selectedCount += 1;
                }
                else
                {
                    NoticePoper.PopNotice(2);
                }
            }
            else
            {
                selected = false;
                player.selectedCount -= 1;
            }
        }
        else
        {
            player.SelectedToSet(this);
        }

        GetComponent<SoundEffect>().PlayAudio();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable)
        {
            player.unitinfocardId = unitId;
            player.unitinfocardSprite = GetComponent<Image>().sprite;
            player.unitinfocardUpdated = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!player.unitinfocardUpdated) player.unitinfocardId = -1;//在这一帧没有被其它按钮更新过则关闭信息卡
    }
}
