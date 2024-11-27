using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemGeneral : UIItem, IPrefImage
{
	public void SetChara(Chara c)
	{
		this.card = c;
		c.SetImage(this.button1.icon);
		string text = c.Name;
		FactionBranch branch = EClass.Branch;
		int? num = (branch != null) ? new int?(branch.uidMaid) : null;
		int uid = c.uid;
		if (num.GetValueOrDefault() == uid & num != null)
		{
			text += ("(" + "maid".lang() + ")").TagSize(12);
		}
		FontColor c2 = FontColor.ButtonGeneral;
		if (c.isDead)
		{
			c2 = FontColor.Bad;
		}
		else if (c.IsPCParty)
		{
			c2 = FontColor.Good;
		}
		else if (c.hp < c.MaxHP / 2)
		{
			c2 = FontColor.Warning;
		}
		this.button1.mainText.SetText(text, c2);
		RectTransform rectTransform = this.button1.icon.rectTransform;
	}

	public RenderRow GetRenderRow()
	{
		Card card = this.card;
		if (card == null)
		{
			return null;
		}
		return card.sourceRenderCard;
	}

	public void OnRefreshPref()
	{
		if (this.card != null && this.card.isChara)
		{
			this.SetChara(this.card.Chara);
		}
	}

	public void Clear()
	{
		if (this.count > 0)
		{
			foreach (UIButton uibutton in base.transform.GetComponentsInDirectChildren(true))
			{
				if (uibutton != this.button1)
				{
					UnityEngine.Object.DestroyImmediate(uibutton.gameObject);
				}
			}
			this.count = 0;
		}
	}

	public UIButton AddSubButton(Sprite sprite, Action action, string lang = null, Action<UITooltip> onTooltip = null)
	{
		UIButton uibutton = Util.Instantiate<UIButton>("UI/Element/Button/SubButton", base.transform);
		uibutton.Rect().anchoredPosition = new Vector2((float)(this.count * -40 - 20 - 10), 0f);
		uibutton.icon.sprite = sprite;
		uibutton.onClick.AddListener(delegate()
		{
			action();
		});
		if (!lang.IsEmpty())
		{
			uibutton.tooltip.enable = true;
			uibutton.tooltip.lang = lang;
		}
		if (onTooltip != null)
		{
			uibutton.tooltip.id = "note";
			uibutton.tooltip.onShowTooltip = onTooltip;
			uibutton.tooltip.enable = true;
		}
		uibutton.highlightTarget = this.button1;
		this.count++;
		return uibutton;
	}

	public void SetMainText(string lang, Sprite sprite = null, bool disableMask = true)
	{
		this.button1.mainText.SetText(lang.lang());
		if (sprite)
		{
			this.button1.icon.sprite = sprite;
			this.button1.icon.SetNativeSize();
			if (disableMask)
			{
				this.DisableMask();
				return;
			}
		}
		else
		{
			this.DisableIcon();
		}
	}

	public UIButton SetSubText(string lang, int x, FontColor c = FontColor.Default, TextAnchor align = TextAnchor.MiddleLeft)
	{
		this.button1.subText.SetActive(true);
		this.button1.subText.SetText(lang.lang(), c);
		this.button1.subText.alignment = align;
		this.button1.mainText.rectTransform.sizeDelta = new Vector2((float)(x - this.paddingSubText), 20f);
		this.button1.subText.rectTransform.anchoredPosition = new Vector2((float)x, 0f);
		return this.button1;
	}

	public UIButton SetSubText2(string lang, FontColor c = FontColor.Default, TextAnchor align = TextAnchor.MiddleRight)
	{
		this.button1.subText2.SetActive(true);
		this.button1.subText2.SetText(lang.lang(), c);
		this.button1.subText2.alignment = align;
		return this.button1;
	}

	public T AddPrefab<T>(string id) where T : Component
	{
		return Util.Instantiate<T>("UI/Element/Item/Extra/" + id, base.transform);
	}

	public void SetSound(SoundData data = null)
	{
		this.button1.soundClick = (data ?? SE.DataClick);
	}

	public void DisableIcon()
	{
		this.button1.icon.transform.parent.SetActive(false);
		if (this.button1.keyText)
		{
			return;
		}
		this.button1.mainText.rectTransform.anchoredPosition = new Vector2(20f, 0f);
	}

	public void DisableMask()
	{
		this.image2.enabled = false;
	}

	public void Build()
	{
		RectTransform rectTransform = this.button1.Rect();
		if (this.count > 0)
		{
			rectTransform.sizeDelta = new Vector2((float)(this.count * -40 - 10 - 3), 0f);
		}
	}

	private const int IconSize = 40;

	private const int IconPadding = 10;

	private const int ButtonPaddingWhenIcon = 3;

	public LayoutGroup layout;

	public int paddingSubText = 50;

	public Card card;

	private int count;
}
