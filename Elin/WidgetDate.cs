using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WidgetDate : Widget
{
	public override object CreateExtra()
	{
		return new WidgetDate.Extra();
	}

	public WidgetDate.Extra extra
	{
		get
		{
			return base.config.extra as WidgetDate.Extra;
		}
	}

	public override void OnActivate()
	{
		WidgetDate.Instance = this;
		this._Refresh();
		base.InvokeRepeating("_Refresh", 0.1f, 0.1f);
	}

	public void _Refresh()
	{
		if (EMono.game.activeZone == null)
		{
			return;
		}
		string text = "";
		GameDate date = EMono.world.date;
		text += (this.extra.date ? EMono.world.date.GetText(Date.TextFormat.Widget) : "");
		if (this.extra.period)
		{
			text = text + " (" + EMono.world.date.periodOfDay.ToString().lang() + ") ";
		}
		text += (this.extra.realDate ? (" (" + DateTime.Now.ToString("H:mm") + ") ") : "");
		if (this.extra.loc)
		{
			text = text + " " + EMono._zone.NameWithLevel;
			if (EMono._zone.isClaimable && !EMono._zone.IsPCFaction)
			{
				text += "(";
				List<Element> list = EMono._zone.ListLandFeats();
				foreach (Element element in list)
				{
					if (element != list[0])
					{
						text += ", ";
					}
					text += element.Name;
				}
				text += ")";
			}
		}
		if (EMono._zone.idCurrentSubset != null)
		{
			text += ("(" + (EMono._zone.idExport + "_" + EMono._zone.idCurrentSubset).lang() + ")").TagColor(this.colorFestival);
		}
		if (EMono._zone.isPeace)
		{
			text += "zone_peace".lang().TagColor(this.colorFestival);
		}
		if (EMono.debug.showExtra)
		{
			text = string.Concat(new string[]
			{
				text,
				" (",
				EMono._zone.biome.name,
				"/Lv:",
				EMono._zone.lv.ToString(),
				"/Danger:",
				EMono._zone.DangerLv.ToString(),
				"/_danger:",
				EMono._zone._dangerLv.ToString(),
				"/elec:",
				EMono._zone.electricity.ToString(),
				")"
			});
			if (EMono._zone.IsInstance)
			{
				text += "Instance";
			}
		}
		if (this.extra.weather)
		{
			text = text + " - " + EMono.world.weather.GetName();
		}
		if (this.extra.room && EMono.pc.IsInActiveZone && EMono._zone.IsPCFaction)
		{
			Room room = EMono.pc.Cell.room;
			if (room != null)
			{
				text = text + " (" + room.Name + ")";
			}
			if (EMono.pc.pos.area != null)
			{
				text = text + " (" + EMono.pc.pos.area.Name + ")";
			}
		}
		text += EMono._zone.TextWidgetDate;
		foreach (ZoneEvent zoneEvent in EMono._zone.events.list)
		{
			text += zoneEvent.TextWidgetDate;
		}
		this.textTime.text = text;
		this.rectClock.SetActive(this.extra.clock);
		if (this.extra.clock)
		{
			this.imageHour.transform.localEulerAngles = new Vector3(0f, 0f, (float)(-(float)date.hour * 30 + 90));
		}
		float num = base.transform.position.x / (float)Screen.width;
		bool flag = base.config.pivot == RectPosition.BottomRIGHT || base.config.pivot == RectPosition.TopRIGHT || base.config.pivot == RectPosition.Right;
		this.Rect().SetPivot((float)(flag ? 1 : 0), 0f);
		this.rectClock.SetAnchor((float)(flag ? 1 : 0), 1f, (float)(flag ? 1 : 0), 1f);
		this.rectClock.anchoredPosition = new Vector2((float)(flag ? -60 : 60), -75f);
		this.textTime.RebuildLayout(false);
	}

	public override void OnChangePivot()
	{
		this._Refresh();
	}

	public static void Refresh()
	{
		if (WidgetDate.Instance)
		{
			WidgetDate.Instance._Refresh();
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uicontextMenu = m.AddChild("setting");
		uicontextMenu.AddToggle("showPeriodOfDay", this.extra.period, delegate(bool a)
		{
			this.extra.period = a;
			this._Refresh();
		});
		uicontextMenu.AddToggle("showRealDate", this.extra.realDate, delegate(bool a)
		{
			this.extra.realDate = a;
			this._Refresh();
		});
		uicontextMenu.AddToggle("showRoom", this.extra.room, delegate(bool a)
		{
			this.extra.room = a;
			this._Refresh();
		});
		uicontextMenu.AddToggle("showClock", this.extra.clock, delegate(bool a)
		{
			this.extra.clock = a;
			this._Refresh();
		});
		uicontextMenu.AddToggle("showLoc", this.extra.loc, delegate(bool a)
		{
			this.extra.loc = a;
			this._Refresh();
		});
		uicontextMenu.AddToggle("showWeather", this.extra.weather, delegate(bool a)
		{
			this.extra.weather = a;
			this._Refresh();
		});
		base.SetBaseContextMenu(m);
	}

	public static WidgetDate Instance;

	public Text textTime;

	public Text textLevel;

	public RectTransform rectClock;

	public Image imageHour;

	public Color colorFestival;

	public class Extra
	{
		public bool date;

		public bool realDate;

		public bool period;

		public bool room;

		public bool clock;

		public bool loc;

		public bool weather;
	}
}
