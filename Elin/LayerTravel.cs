using System.Collections.Generic;
using CreativeSpore.SuperTilemapEditor;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LayerTravel : ELayer
{
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

	public override bool blockWidgetClick => false;

	public override void OnInit()
	{
		if (useZoom)
		{
			ELayer.screen.SetZoom(ELayer.screen.TargetZoom);
			DOTween.To(() => ELayer.scene.screenElona.zoom, delegate(float x)
			{
				ELayer.scene.screenElona.zoom = x;
			}, endZoom, timeZoom).SetEase(easeZoom);
		}
		if (useBG)
		{
			bg.DOFade(fadeAlpha, fadeTime).SetEase(easeFade);
			ELayer.ui.layerFloat.SetActive(enable: false);
		}
	}

	public override void OnKill()
	{
		if (useBG)
		{
			ELayer.ui.layerFloat.SetActive(enable: true);
		}
		if (useZoom)
		{
			ELayer.screen.SetZoom(ELayer.screen.TargetZoom);
		}
	}

	public override void OnSwitchContent(Window window)
	{
		Refresh();
	}

	public int GetTravelFood(Zone z)
	{
		if (z.tempDist == 0)
		{
			return 0;
		}
		return 1 + GetTravelHours(z) / 24;
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
		return GetTravelHours(z);
	}

	public void Refresh()
	{
		currentZone = ELayer.scene.elomap.GetZone(ELayer.pc.pos);
		if (currentZone != null && !currentZone.isKnown)
		{
			currentZone = null;
		}
		list.callbacks = new UIList.Callback<Zone, UIButton>
		{
			onInstantiate = delegate(Zone a, UIButton b)
			{
				string text = "travelZone".lang(a.Name, a.tempDist.ToString() ?? "");
				if (a == ELayer.pc.homeZone)
				{
					text = "travelHome".lang() + text;
				}
				if (a.isRandomSite)
				{
					text += (" (" + "days1".lang() + (ELayer.world.date.GetRemainingHours(a.dateExpire) / 24 + 1) + "days2".lang() + ")").TagSize(13);
				}
				b.mainText.SetText(text);
				b.icon.sprite = TilemapUtils.GetOrCreateTileSprite(ELayer.scene.elomap.actor.tileset, a.icon) ?? b.icon.sprite;
				b.icon.SetNativeSize();
				b.refObj = a;
				int travelFood = GetTravelFood(a);
				int currency = ELayer.pc.GetCurrency("ration");
				b.subText.SetText(travelFood.ToString() ?? "", (currency >= travelFood) ? FontColor.Good : FontColor.Bad);
				b.subText2.SetText(Date.GetText2(GetTravelHours(a)));
			},
			onClick = delegate(Zone a, UIButton b)
			{
				if (a == currentZone)
				{
					Close();
					ELayer.player.EnterLocalZone();
				}
				else if (ELayer.pc.TryPay(GetTravelFood(a), "ration"))
				{
					if (ELayer.pc.burden.GetPhase() >= 3)
					{
						Msg.Say("errorOverweightTravel");
						Close();
					}
					else
					{
						ELayer.player.distanceTravel = a.tempDist;
						ELayer.player.dateTravel = ELayer.world.date.GetRaw();
						foreach (UIList.ButtonPair button in list.buttons)
						{
							UIButton uIButton = button.component as UIButton;
							if ((bool)uIButton && uIButton != b)
							{
								uIButton.GetComponent<CanvasGroup>().alpha = 0.5f;
							}
						}
						float time = 0f;
						int h = 0;
						int maxHours = GetTravelHours(a);
						ELayer.ui.AddLayer<LayerCover>().SetCondition(delegate(float delta)
						{
							time += delta;
							if (time >= 0.035f)
							{
								time = 0f;
								h++;
								ELayer.world.date.AdvanceHour();
								if (h >= maxHours)
								{
									Close();
									ELayer.pc.MoveImmediate(a.RegionPos);
									ELayer.player.lastZonePos = null;
									ZoneTransition.EnterState state = ((a.RegionEnterState == ZoneTransition.EnterState.Dir) ? ZoneTransition.DirToState(ELayer.pc.GetCurrentDir()) : a.RegionEnterState);
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
					}
				}
			},
			onList = delegate
			{
				List<Zone> obj = ELayer.game.activeZone.Region.ListTravelZones();
				obj.Sort((Zone a, Zone b) => GetSortVal(a) - GetSortVal(b));
				foreach (Zone item in obj)
				{
					if (windows[0].idTab != 1)
					{
						list.Add(item);
					}
				}
			}
		};
		list.List();
	}
}
