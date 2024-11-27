using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class WidgetMouseover : Widget
{
	public override void OnActivate()
	{
		WidgetMouseover.Instance = this;
		this.Hide(true);
	}

	public void Refresh()
	{
		this.timer += Core.delta;
		PointTarget mouseTarget = EMono.scene.mouseTarget;
		if (!mouseTarget.hasTargetChanged && this.timer < 0.1f)
		{
			return;
		}
		this.timer = 0f;
		bool flag = EInput.isShiftDown && mouseTarget.pos.Equals(EMono.pc.pos) && (EMono.pc.ride != null || EMono.pc.parasite != null);
		if (((!flag && mouseTarget.target == null) || (ActionMode.IsAdv && Input.GetMouseButton(0))) && this.roster == null)
		{
			this.Hide(false);
			return;
		}
		string text = "";
		Card card = mouseTarget.card;
		if (this.roster != null && this.roster.ExistsOnMap)
		{
			flag = false;
			card = this.roster;
		}
		if (flag)
		{
			if (EMono.pc.ride != null)
			{
				text += EMono.pc.ride.GetHoverText();
				text += EMono.pc.ride.GetHoverText2();
			}
			if (EMono.pc.parasite != null)
			{
				if (EMono.pc.ride != null)
				{
					text += Environment.NewLine;
				}
				text += EMono.pc.parasite.GetHoverText();
				text += EMono.pc.parasite.GetHoverText2();
			}
		}
		else if (card != null)
		{
			text = card.GetHoverText();
			if (this.roster == null)
			{
				int count = EMono.scene.mouseTarget.cards.Count;
				if (count > 1)
				{
					text += "otherCards".lang((count - 1).ToString() ?? "", null, null, null, null);
				}
			}
			text += card.GetHoverText2();
			if (this.roster == null && mouseTarget.target != card)
			{
				text = text + Environment.NewLine + mouseTarget.target.InspectName;
			}
		}
		else
		{
			text = mouseTarget.target.InspectName;
		}
		this.Show(text);
	}

	public void Show(string s)
	{
		this.layout.SetActive(true);
		this.cg.alpha = 1f;
		TweenUtil.KillTween(ref this.tween, false);
		this.textName.text = s;
		RectPosition anchor = this.Rect().GetAnchor();
		if (anchor <= RectPosition.TopRIGHT)
		{
			if (anchor != RectPosition.TopLEFT)
			{
				if (anchor != RectPosition.TopRIGHT)
				{
					goto IL_76;
				}
				goto IL_68;
			}
		}
		else if (anchor != RectPosition.BottomLEFT)
		{
			if (anchor != RectPosition.BottomRIGHT)
			{
				goto IL_76;
			}
			goto IL_68;
		}
		this.layout.childAlignment = TextAnchor.MiddleLeft;
		goto IL_82;
		IL_68:
		this.layout.childAlignment = TextAnchor.MiddleRight;
		goto IL_82;
		IL_76:
		this.layout.childAlignment = TextAnchor.MiddleCenter;
		IL_82:
		this.layout.RebuildLayout(false);
	}

	public void Hide(bool immediate = false)
	{
		this.imageBG.raycastTarget = false;
		if (immediate)
		{
			this.layout.SetActive(false);
			return;
		}
		TweenUtil.KillTween(ref this.tween, false);
		this.tween = this.cg.DOFade(0f, 0.3f).OnComplete(delegate
		{
			if (this.layout)
			{
				this.layout.SetActive(false);
			}
		}).SetEase(Ease.InQuad);
	}

	public override void OnManagerActivate()
	{
		base.OnManagerActivate();
		this.imageBG.raycastTarget = true;
		this.Show("[" + Lang.Get("WidgetMouseover") + "]");
	}

	public override void OnManagerDeactivate()
	{
		base.OnManagerDeactivate();
		this.Hide(true);
	}

	public static WidgetMouseover Instance;

	public Chara roster;

	public UIText textName;

	public LayoutGroup layout;

	public CanvasGroup cg;

	public Tween tween;

	private float timer;
}
