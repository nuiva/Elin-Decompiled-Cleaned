using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonRoster : UIButton, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public WidgetRoster roster
	{
		get
		{
			return WidgetRoster.Instance;
		}
	}

	public void SetChara(Chara c)
	{
		this.chara = c;
		this.chara.SetImage(this.icon);
		bool flag = this.roster.extra.portrait && !this.chara.GetIdPortrait().IsEmpty();
		this.portrait.SetChara(c, null);
		this.portrait.SetActive(flag && !this.roster.extra.onlyName);
		this.icon.enabled = (!flag && !this.roster.extra.onlyName);
		this.icon.rectTransform.anchoredPosition = new Vector2(0f, (float)(this.roster.extra.width / 2));
		this.Refresh();
	}

	public void Refresh()
	{
		if (ButtonRoster.gradient == null)
		{
			ButtonRoster.gradient = EClass.Colors.Dark.gradients["mood"];
		}
		float num = Mathf.Clamp((float)this.chara.hp / (float)this.chara.MaxHP, 0f, 1f);
		this.barMood.Rect().localScale = new Vector3(num, 1f, 1f);
		this.barMood.color = ButtonRoster.gradient.Evaluate(num);
		Color c = EClass.Colors.Dark.gradientHP.Evaluate((float)this.chara.hp / (float)this.chara.MaxHP);
		this.mainText.text = "".TagColor(c, this.chara.hp.ToString() ?? "");
		this.mainText.SetActive(this.roster.extra.showHP);
		if (this.roster.extra.showHP)
		{
			this.textName.SetText(this.chara.NameSimple);
		}
		else
		{
			this.textName.text = "".TagColor(c, this.chara.NameSimple);
		}
		this.textName.SetActive(this.roster.extra.onlyName);
		this.SetOnClick(delegate
		{
			bool flag = EClass.ui.IsActive;
			if (flag)
			{
				LayerInventory layer = EClass.ui.GetLayer<LayerInventory>(false);
				if (layer && layer.Inv.owner.IsPCFaction && layer.Inv.currency == CurrencyType.None)
				{
					flag = false;
				}
			}
			if (this.chara.IsPC || this.chara.IsDisabled || !EClass.pc.HasNoGoal || flag)
			{
				SE.Beep();
				return;
			}
			EClass.ui.CloseLayers();
			LayerInventory.CreateContainer(this.chara);
		});
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if (WidgetMouseover.Instance)
		{
			WidgetMouseover.Instance.roster = this.chara;
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		if (WidgetMouseover.Instance)
		{
			WidgetMouseover.Instance.roster = null;
		}
	}

	public Chara chara;

	public float iconPivot;

	public Image barMood;

	public UIText textName;

	public RectTransform rect;

	public Portrait portrait;

	private static Gradient gradient;
}
