using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonElement : UIButton
{
	public void SetElement(Element _e, ElementContainer _owner, ButtonElement.Mode _mode = ButtonElement.Mode.Skill)
	{
		this.e = _e;
		this.owner = _owner;
		this.mode = _mode;
		this.Refresh();
	}

	public void Refresh()
	{
		string str = "";
		if (WindowChara.Instance && WindowChara.Instance.chara == EClass.pc && EClass.player.trackedElements.Contains(this.e.id))
		{
			str = "*";
		}
		switch (this.mode)
		{
		case ButtonElement.Mode.Attribute:
			this.mainText.SetText(str + this.e.Name, FontColor.Default);
			this.e.SetTextValue(this.subText);
			goto IL_517;
		case ButtonElement.Mode.Tech:
		{
			this.mainText.SetText(str + this.e.Name + " " + this.e.DisplayValue.ToString(), FontColor.Default);
			int techUpgradeCost = EClass.Branch.GetTechUpgradeCost(this.e);
			int currency = EClass.pc.GetCurrency("money2");
			if (EClass.Branch.elements.ValueWithoutLink(this.e.id) == 0 || this.e.source.cost[0] == 0 || techUpgradeCost == 0)
			{
				this.subText.SetText("-");
				goto IL_517;
			}
			this.subText.SetText(techUpgradeCost.ToString() ?? "", (currency >= techUpgradeCost) ? FontColor.Good : FontColor.Bad);
			goto IL_517;
		}
		case ButtonElement.Mode.Feat:
		case ButtonElement.Mode.FeatPurchase:
		case ButtonElement.Mode.FeatMini:
			if (this.mode == ButtonElement.Mode.FeatPurchase)
			{
				this.mainText.SetText(this.e.FullName, FontColor.Default);
				this.subText.text = this.e.GetDetail().SplitNewline()[0].StripLastPun();
				this.subText2.text = "".TagColor((EClass.pc.feat >= this.e.CostLearn) ? EClass.Colors.Skin.textGood : EClass.Colors.Skin.textBad, this.e.CostLearn.ToString() ?? "");
			}
			else
			{
				string str2 = (this.mode == ButtonElement.Mode.FeatMini) ? this.e.FullName : this.e.source.GetText("textPhase", false).SplitNewline().TryGet(this.e.Value - 1, -1).StripLastPun();
				FontColor c = this.e.HasTag("neg") ? FontColor.Bad : FontColor.ButtonSelectable;
				if (this.e.source.category == "ether")
				{
					c = FontColor.Ether;
				}
				this.mainText.SetText(str + str2, c);
				this.subText.text = ((this.mode == ButtonElement.Mode.FeatMini) ? "" : (this.e as Feat).GetHint(this.owner));
			}
			if (this.imagePotential)
			{
				this.imagePotential.SetActive(false);
				goto IL_517;
			}
			goto IL_517;
		case ButtonElement.Mode.Policy:
			this.mainText.SetText(str + this.e.Name, FontColor.Default);
			this.e.SetTextValue(this.subText);
			Mathf.Clamp((float)this.e.Potential / 300f, 0.1f, 1f);
			if (this.imagePotential)
			{
				this.imagePotential.SetActive(false);
				goto IL_517;
			}
			goto IL_517;
		case ButtonElement.Mode.OnlyValue:
			this.e.SetTextValue(this.subText);
			this.imagePotential.SetActive(false);
			goto IL_517;
		case ButtonElement.Mode.LandFeat:
		{
			string text = str + this.e.Name;
			if (this.e.HasTag("network") && EClass.Branch != null && EClass.Branch.HasNetwork)
			{
				text = "feat_network".lang(text, null, null, null, null);
			}
			this.mainText.SetText(text + " " + this.e.Value.ToString(), FontColor.Default);
			this.subText.horizontalOverflow = HorizontalWrapMode.Overflow;
			this.subText.text = this.e.GetDetail().SplitNewline()[0].StripLastPun();
			if (this.imagePotential)
			{
				this.imagePotential.SetActive(false);
				goto IL_517;
			}
			goto IL_517;
		}
		}
		this.mainText.SetText(str + this.e.Name, (this.e.owner != null && this.e.IsFactionElement(this.e.owner.Card as Chara)) ? FontColor.Myth : FontColor.Default);
		this.e.SetTextValue(this.subText);
		bool flag = this.e.source.category == "skill";
		if (this.imagePotential)
		{
			this.imagePotential.SetActive(flag && this.imagePotential.enabled);
		}
		IL_517:
		if (this.imagePotential)
		{
			int num = (this.e.Potential - 80) / 20;
			this.imagePotential.enabled = (this.e.Potential != 80);
			this.imagePotential.sprite = EClass.core.refs.spritesPotential[Mathf.Clamp(Mathf.Abs(num), 0, EClass.core.refs.spritesPotential.Count - 1)];
			this.imagePotential.color = ((num >= 0) ? Color.white : new Color(1f, 0.7f, 0.7f));
		}
		this.e.SetImage(this.icon);
		if (this.mode == ButtonElement.Mode.FeatPurchase && this.e.Value > 1)
		{
			Element refEle = Element.Create(this.e.id, this.e.Value - 1);
			base.SetTooltip("note", delegate(UITooltip tt)
			{
				this.e.WriteNoteWithRef(tt.note, this.owner, null, refEle);
			}, true);
			return;
		}
		base.SetTooltip("note", delegate(UITooltip tt)
		{
			this.e.WriteNote(tt.note, this.owner, null);
		}, true);
	}

	public void SetGrid(Element e, Chara c)
	{
		bool flag = false;
		IList<BodySlot> list = c.body.slots.Copy<BodySlot>();
		list.Sort((BodySlot a, BodySlot b) => c.body.GetSortVal(b) - c.body.GetSortVal(a));
		string text;
		foreach (BodySlot bodySlot in list)
		{
			if (bodySlot.elementId != 44)
			{
				UIItem uiitem = Util.Instantiate<UIItem>(this.moldItemResist, this.layout);
				Thing thing = bodySlot.thing;
				if (thing == null)
				{
					uiitem.text1.SetActive(false);
					uiitem.image1.color = Color.white.SetAlpha(0.5f);
				}
				else if (!thing.IsIdentified)
				{
					flag = true;
					uiitem.text1.SetText("?", FontColor.Passive);
					uiitem.image1.color = Color.white.SetAlpha(0.5f);
				}
				else
				{
					Element element = bodySlot.thing.elements.GetElement(e.id);
					int num = 0;
					if (element != null)
					{
						num = element.Value;
						if (!element.CanLink(thing.elements) || element.IsGlobalElement)
						{
							num = 0;
						}
					}
					if (num == 0)
					{
						uiitem.text1.SetActive(false);
						uiitem.image1.color = Color.white.SetAlpha(0.5f);
					}
					text = (e.IsFlag ? "✓" : (Mathf.Abs(num).ToString() ?? ""));
					uiitem.text1.SetText(text, (num >= 0) ? FontColor.Good : FontColor.Bad);
				}
			}
		}
		text = "";
		int value = e.Value;
		int num2 = value;
		if (e is Resistance)
		{
			num2 = Element.GetResistLv(value);
			if (num2 > 0)
			{
				text = Lang.GetList("resist")[Mathf.Min(num2, 5)];
			}
			else
			{
				text = Lang.GetList("resistNeg")[-num2];
			}
		}
		text = text + " (" + (flag ? "?" : (e.IsFlag ? "✓" : (value.ToString() ?? ""))) + ")";
		Sprite icon = e.GetIcon("");
		if (icon != null && this.icon)
		{
			this.icon.sprite = icon;
		}
		this.mainText.SetText(e.ShortName, e.IsFactionElement(c) ? FontColor.Myth : FontColor.Default);
		this.subText.SetText(text, (num2 == 0) ? FontColor.Default : ((num2 > 0) ? FontColor.Good : FontColor.Bad));
		base.SetTooltip("note", delegate(UITooltip tt)
		{
			e.WriteNote(tt.note, c.elements, null);
		}, true);
	}

	public Image imagePotential;

	public GameObject goExp;

	public Element e;

	public ElementContainer owner;

	public UIButton buttonUpgrade;

	public UIItem moldItemResist;

	public LayoutGroup layout;

	public ButtonElement.Mode mode;

	public enum Mode
	{
		Skill,
		Attribute,
		Tech,
		Feat,
		Enchant,
		Policy,
		OnlyValue,
		FeatPurchase,
		LandFeat,
		FeatMini
	}
}
