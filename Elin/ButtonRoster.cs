using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonRoster : UIButton, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public Chara chara;

	public float iconPivot;

	public Image barMood;

	public UIText textName;

	public RectTransform rect;

	public Portrait portrait;

	private static Gradient gradient;

	public WidgetRoster roster => WidgetRoster.Instance;

	public void SetChara(Chara c)
	{
		chara = c;
		chara.SetImage(icon);
		bool flag = roster.extra.portrait && !chara.GetIdPortrait().IsEmpty();
		portrait.SetChara(c);
		portrait.SetActive(flag && !roster.extra.onlyName);
		icon.enabled = !flag && !roster.extra.onlyName;
		icon.rectTransform.anchoredPosition = new Vector2(0f, roster.extra.width / 2);
		Refresh();
	}

	public void Refresh()
	{
		if (gradient == null)
		{
			gradient = EClass.Colors.Dark.gradients["mood"];
		}
		float num = Mathf.Clamp((float)chara.hp / (float)chara.MaxHP, 0f, 1f);
		barMood.Rect().localScale = new Vector3(num, 1f, 1f);
		barMood.color = gradient.Evaluate(num);
		Color c = EClass.Colors.Dark.gradientHP.Evaluate((float)chara.hp / (float)chara.MaxHP);
		mainText.text = "".TagColor(c, chara.hp.ToString() ?? "");
		mainText.SetActive(roster.extra.showHP);
		if (roster.extra.showHP)
		{
			textName.SetText(chara.NameSimple);
		}
		else
		{
			textName.text = "".TagColor(c, chara.NameSimple);
		}
		textName.SetActive(roster.extra.onlyName);
		this.SetOnClick(delegate
		{
			bool flag = EClass.ui.IsActive;
			if (flag)
			{
				LayerInventory layer = EClass.ui.GetLayer<LayerInventory>();
				if ((bool)layer && layer.Inv.owner.IsPCFaction && layer.Inv.currency == CurrencyType.None)
				{
					flag = false;
				}
			}
			if (chara.IsPC || chara.IsDisabled || !EClass.pc.HasNoGoal || flag)
			{
				SE.Beep();
			}
			else
			{
				EClass.ui.CloseLayers();
				LayerInventory.CreateContainer(chara);
			}
		});
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if ((bool)WidgetMouseover.Instance)
		{
			WidgetMouseover.Instance.roster = chara;
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		if ((bool)WidgetMouseover.Instance)
		{
			WidgetMouseover.Instance.roster = null;
		}
	}
}
