using System;
using System.Collections.Generic;
using CreativeSpore.SuperTilemapEditor;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class LayerTravel : ELayer
{
	public override bool blockWidgetClick
	{
		get
		{
			return false;
		}
	}

	public override void OnInit()
	{
		if (this.useZoom)
		{
			ELayer.screen.SetZoom(ELayer.screen.TargetZoom);
			DOTween.To(() => ELayer.scene.screenElona.zoom, delegate(float x)
			{
				ELayer.scene.screenElona.zoom = x;
			}, this.endZoom, this.timeZoom).SetEase(this.easeZoom);
		}
		if (this.useBG)
		{
			this.bg.DOFade(this.fadeAlpha, this.fadeTime).SetEase(this.easeFade);
			ELayer.ui.layerFloat.SetActive(false);
		}
	}

	public override void OnKill()
	{
		if (this.useBG)
		{
			ELayer.ui.layerFloat.SetActive(true);
		}
		if (this.useZoom)
		{
			ELayer.screen.SetZoom(ELayer.screen.TargetZoom);
		}
	}

	public override void OnSwitchContent(Window window)
	{
		this.Refresh();
	}

	public int GetTravelFood(Zone z)
	{
		if (z.tempDist == 0)
		{
			return 0;
		}
		return 1 + this.GetTravelHours(z) / 24;
	}

	public int GetTravelHours(Zone z)
	{
		return z.tempDist * 3 * 100 / (100 + ELayer.pc.Evalue(407) * 2);
	}

	public int GetSortVal(Zone z)
	{
		if (ELayer.pc.homeZone == z)
		{
			return -1;
		}
		return this.GetTravelHours(z);
	}

	public void Refresh()
	{
		this.currentZone = ELayer.scene.elomap.GetZone(ELayer.pc.pos);
		if (this.currentZone != null && !this.currentZone.isKnown)
		{
			this.currentZone = null;
		}
		this.list.callbacks = new UIList.Callback<Zone, UIButton>
		{
			onInstantiate = delegate(Zone a, UIButton b)
			{
				string text = "travelZone".lang(a.Name, a.tempDist.ToString() ?? "", null, null, null);
				if (a == ELayer.pc.homeZone)
				{
					text = "travelHome".lang() + text;
				}
				if (a.isRandomSite)
				{
					text += string.Concat(new string[]
					{
						" (",
						"days1".lang(),
						(ELayer.world.date.GetRemainingHours(a.dateExpire) / 24 + 1).ToString(),
						"days2".lang(),
						")"
					}).TagSize(13);
				}
				b.mainText.SetText(text);
				b.icon.sprite = (TilemapUtils.GetOrCreateTileSprite(ELayer.scene.elomap.actor.tileset, a.icon, 0f) ?? b.icon.sprite);
				b.icon.SetNativeSize();
				b.refObj = a;
				int travelFood = this.GetTravelFood(a);
				int currency = ELayer.pc.GetCurrency("ration");
				b.subText.SetText(travelFood.ToString() ?? "", (currency >= travelFood) ? FontColor.Good : FontColor.Bad);
				b.subText2.SetText(Date.GetText2(this.GetTravelHours(a)));
			},
			onClick = delegate(Zone a, UIButton b)
			{
				if (a == this.currentZone)
				{
					this.Close();
					ELayer.player.EnterLocalZone(false, null);
					return;
				}
				if (!ELayer.pc.TryPay(this.GetTravelFood(a), "ration"))
				{
					return;
				}
				if (ELayer.pc.burden.GetPhase() >= 3)
				{
					Msg.Say("errorOverweightTravel");
					this.Close();
					return;
				}
				ELayer.player.distanceTravel = a.tempDist;
				ELayer.player.dateTravel = ELayer.world.date.GetRaw(0);
				foreach (UIList.ButtonPair buttonPair in this.list.buttons)
				{
					UIButton uibutton = buttonPair.component as UIButton;
					if (uibutton && uibutton != b)
					{
						uibutton.GetComponent<CanvasGroup>().alpha = 0.5f;
					}
				}
				float time = 0f;
				int h = 0;
				int maxHours = this.GetTravelHours(a);
				ELayer.ui.AddLayer<LayerCover>().SetCondition(delegate(float delta)
				{
					time += delta;
					if (time >= 0.035f)
					{
						time = 0f;
						int h = h;
						h++;
						ELayer.world.date.AdvanceHour();
						if (h >= maxHours)
						{
							this.Close();
							ELayer.pc.MoveImmediate(a.RegionPos, true, true);
							ELayer.player.lastZonePos = null;
							ZoneTransition.EnterState state = (a.RegionEnterState == ZoneTransition.EnterState.Dir) ? ZoneTransition.DirToState(ELayer.pc.GetCurrentDir()) : a.RegionEnterState;
							if (a is Zone_Lumiest)
							{
								state = ZoneTransition.EnterState.Right;
							}
							ELayer.pc.MoveZone(a, state);
							return true;
						}
					}
					return false;
				});
			},
			onList = delegate(UIList.SortMode m)
			{
				List<Zone> list = ELayer.game.activeZone.Region.ListTravelZones(100);
				list.Sort((Zone a, Zone b) => this.GetSortVal(a) - this.GetSortVal(b));
				foreach (Zone o in list)
				{
					if (this.windows[0].idTab != 1)
					{
						this.list.Add(o);
					}
				}
			}
		};
		this.list.List(false);
	}

	public UIList list;

	public Zone currentZone;

	public Image bg;

	public float fadeAlpha;

	public float fadeTime;

	public Ease easeFade;

	public Ease easeZoom;

	public float startZoom;

	public float endZoom;

	public float timeZoom;

	public bool highlightZone;

	public bool useBG;

	public bool useZoom;

	private Point posHighlight = new Point();
}
