using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class LayerFaith : ELayer
{
	public void Activate(Religion r, Action<Religion> _onWorship)
	{
		this.onWorship = _onWorship;
		if (r == null)
		{
			foreach (Religion religion in ELayer.game.religions.list)
			{
				if (!religion.IsMinorGod)
				{
					this.AddReligion(religion);
				}
			}
			this.isBranchFaith = true;
		}
		else
		{
			this.AddReligion(r);
		}
		TweenUtil.Tween(this.startDelay, new Action(this.RefreshSlots), null);
		SE.Play("zoomIn");
	}

	private void Update()
	{
		if (EInput.wheel != 0)
		{
			this.Move((EInput.wheel > 0) ? 1 : -1);
		}
	}

	public void Move(int a)
	{
		this.index += a;
		if (this.index >= this.items.Count)
		{
			this.index = 0;
		}
		else if (this.index < 0)
		{
			this.index = this.items.Count - 1;
		}
		this.RefreshSlots();
		SE.Play("zoomOut");
	}

	public void RefreshSlots()
	{
		this.orders.Clear();
		int num = this.index;
		for (int i = 0; i < this.items.Count; i++)
		{
			if (num >= this.items.Count)
			{
				num = 0;
			}
			this.orders.Add(this.items[num]);
			num++;
		}
		for (int j = 0; j < this.items.Count; j++)
		{
			LayerFaith.Slot slot = (j >= this.slots.Count) ? this.slotVoid : this.slots[j];
			UIItem item = this.orders[j];
			bool flag = j == 2;
			if (this.items.Count == 1)
			{
				flag = true;
				slot = this.slots[2];
			}
			item.button1.icon.raycastTarget = !flag;
			if (j == 1)
			{
				item.button1.SetOnClick(delegate
				{
					this.Move(-1);
				});
			}
			if (j == 3)
			{
				item.button1.SetOnClick(delegate
				{
					this.Move(1);
				});
			}
			item.button2.SetOnClick(delegate
			{
				Religion religion = item.refObj as Religion;
				if (religion.IsAvailable || !this.isBranchFaith)
				{
					this.Close();
					this.onWorship(religion);
					return;
				}
				SE.Beep();
			});
			item.button3.SetOnClick(delegate
			{
				this.Close();
			});
			item.button2.GetComponent<CanvasGroup>().DOFade(flag ? 1f : 0f, this.time).SetEase(slot.ease);
			item.button3.GetComponent<CanvasGroup>().DOFade(flag ? 1f : 0f, this.time).SetEase(slot.ease);
			item.text1.DOFade(flag ? 1f : 0f, this.time).SetEase(slot.ease);
			item.text3.DOFade(flag ? 1f : 0f, this.time).SetEase(slot.ease);
			item.image2.DOFade(flag ? this.bgAlpha : 0f, this.time).SetEase(slot.ease);
			item.Rect().DOAnchorPos(slot.position, this.time, false).SetEase(slot.ease);
			item.Rect().DOScale(slot.scale, this.time).SetEase(slot.ease);
			item.GetComponent<CanvasGroup>().DOFade(slot.alpha, this.time).SetEase(slot.ease);
		}
	}

	public void AddReligion(Religion r)
	{
		UIItem uiitem = Util.Instantiate<UIItem>(this.moldItem, this.layout);
		uiitem.text1.SetText(r.source.GetDetail());
		uiitem.text2.SetText(r.Name);
		uiitem.button1.icon.sprite = Resources.Load<Sprite>("Media/Graphics/Image/Faction/" + r.source.id);
		if (this.isBranchFaith && !r.IsAvailable)
		{
			uiitem.button2.mainText.SetText("faithUnavailable".lang());
		}
		uiitem.refObj = r;
		this.items.Add(uiitem);
		uiitem.Rect().anchoredPosition = this.startPos;
	}

	public Vector2 startPos;

	public float time;

	public float bgAlpha;

	public float startDelay;

	public LayerFaith.Slot slotVoid;

	public List<LayerFaith.Slot> slots;

	public UIItem moldItem;

	public LayoutGroup layout;

	public Action<Religion> onWorship;

	[NonSerialized]
	public bool isBranchFaith;

	[NonSerialized]
	public int index;

	[NonSerialized]
	public List<UIItem> items = new List<UIItem>();

	[NonSerialized]
	public List<UIItem> orders = new List<UIItem>();

	[Serializable]
	public class Slot
	{
		public float scale;

		public float alpha;

		public Vector2 position;

		public Ease ease;
	}
}
