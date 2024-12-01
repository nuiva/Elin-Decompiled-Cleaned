using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LayerFaith : ELayer
{
	[Serializable]
	public class Slot
	{
		public float scale;

		public float alpha;

		public Vector2 position;

		public Ease ease;
	}

	public Vector2 startPos;

	public float time;

	public float bgAlpha;

	public float startDelay;

	public Slot slotVoid;

	public List<Slot> slots;

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

	public void Activate(Religion r, Action<Religion> _onWorship)
	{
		onWorship = _onWorship;
		if (r == null)
		{
			foreach (Religion item in ELayer.game.religions.list)
			{
				if (!item.IsMinorGod)
				{
					AddReligion(item);
				}
			}
			isBranchFaith = true;
		}
		else
		{
			AddReligion(r);
		}
		TweenUtil.Tween(startDelay, RefreshSlots);
		SE.Play("zoomIn");
	}

	private void Update()
	{
		if (EInput.wheel != 0)
		{
			Move((EInput.wheel > 0) ? 1 : (-1));
		}
	}

	public void Move(int a)
	{
		index += a;
		if (index >= items.Count)
		{
			index = 0;
		}
		else if (index < 0)
		{
			index = items.Count - 1;
		}
		RefreshSlots();
		SE.Play("zoomOut");
	}

	public void RefreshSlots()
	{
		orders.Clear();
		int num = index;
		for (int i = 0; i < items.Count; i++)
		{
			if (num >= items.Count)
			{
				num = 0;
			}
			orders.Add(items[num]);
			num++;
		}
		for (int j = 0; j < items.Count; j++)
		{
			Slot slot = ((j >= slots.Count) ? slotVoid : slots[j]);
			UIItem item = orders[j];
			bool flag = j == 2;
			if (items.Count == 1)
			{
				flag = true;
				slot = slots[2];
			}
			item.button1.icon.raycastTarget = !flag;
			if (j == 1)
			{
				item.button1.SetOnClick(delegate
				{
					Move(-1);
				});
			}
			if (j == 3)
			{
				item.button1.SetOnClick(delegate
				{
					Move(1);
				});
			}
			item.button2.SetOnClick(delegate
			{
				Religion religion = item.refObj as Religion;
				if (religion.IsAvailable || !isBranchFaith)
				{
					Close();
					onWorship(religion);
				}
				else
				{
					SE.Beep();
				}
			});
			item.button3.SetOnClick(delegate
			{
				Close();
			});
			item.button2.GetComponent<CanvasGroup>().DOFade(flag ? 1f : 0f, time).SetEase(slot.ease);
			item.button3.GetComponent<CanvasGroup>().DOFade(flag ? 1f : 0f, time).SetEase(slot.ease);
			item.text1.DOFade(flag ? 1f : 0f, time).SetEase(slot.ease);
			item.text3.DOFade(flag ? 1f : 0f, time).SetEase(slot.ease);
			item.image2.DOFade(flag ? bgAlpha : 0f, time).SetEase(slot.ease);
			item.Rect().DOAnchorPos(slot.position, time).SetEase(slot.ease);
			item.Rect().DOScale(slot.scale, time).SetEase(slot.ease);
			item.GetComponent<CanvasGroup>().DOFade(slot.alpha, time).SetEase(slot.ease);
		}
	}

	public void AddReligion(Religion r)
	{
		UIItem uIItem = Util.Instantiate(moldItem, layout);
		uIItem.text1.SetText(r.source.GetDetail());
		uIItem.text2.SetText(r.Name);
		uIItem.button1.icon.sprite = Resources.Load<Sprite>("Media/Graphics/Image/Faction/" + r.source.id);
		if (isBranchFaith && !r.IsAvailable)
		{
			uIItem.button2.mainText.SetText("faithUnavailable".lang());
		}
		uIItem.refObj = r;
		items.Add(uIItem);
		uIItem.Rect().anchoredPosition = startPos;
	}
}
