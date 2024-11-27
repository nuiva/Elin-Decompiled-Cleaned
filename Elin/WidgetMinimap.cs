using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WidgetMinimap : Widget, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IDragHandler, IChangeResolution, IInitializePotentialDragHandler
{
	public override object CreateExtra()
	{
		return new WidgetMinimap.Extra();
	}

	public override Type SetSiblingAfter
	{
		get
		{
			return typeof(WidgetBottomBar);
		}
	}

	public WidgetMinimap.Extra extra
	{
		get
		{
			return base.config.extra as WidgetMinimap.Extra;
		}
	}

	public int Size
	{
		get
		{
			return this.preview.Size;
		}
	}

	public override bool AlwaysBottom
	{
		get
		{
			return true;
		}
	}

	public override void OnActivate()
	{
		WidgetMinimap.Instance = this;
		this.preview.limitBounds = this.extra.limitBounds;
		this.preview.monoColor = this.extra.monoColor;
		this.preview.SetMap(EMono._map);
		base.InvokeRepeating("RefreshMarkers", 0f, this.interval);
		this.RefreshStyle();
		this.buttonMaptool.SetItem(new HotItemContext
		{
			id = "mapTool"
		});
	}

	public static void UpdateMap(List<Cell> newPoints)
	{
		if (WidgetMinimap.Instance && WidgetMinimap.Instance.preview.map.zone.IsActiveZone)
		{
			WidgetMinimap.Instance.preview.UpdateMap(newPoints);
		}
		newPoints.Clear();
	}

	public static void UpdateMap(int x, int z)
	{
		if (WidgetMinimap.Instance && WidgetMinimap.Instance.preview.map.zone.IsActiveZone)
		{
			WidgetMinimap.Instance.preview.UpdateMap(x, z);
		}
	}

	public static void UpdateMap()
	{
		if (WidgetMinimap.Instance && WidgetMinimap.Instance.preview.map.zone.IsActiveZone)
		{
			WidgetMinimap.Instance.preview.SetMap(EMono._map);
		}
	}

	public void OnMoveZone()
	{
		this.preview.SetMap(EMono._map);
		this.RefreshStyle();
	}

	public void Reload()
	{
		this.preview.SetMap(this.preview.map);
	}

	public void OnPointerDown(PointerEventData e)
	{
		if (!EMono.pc.HasNoGoal || EMono.game.activeZone.IsRegion || EMono.ui.IsActive)
		{
			SE.BeepSmall();
			return;
		}
		Vector2 size = this.rectMap.rect.size;
		Vector2 vector;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectMap, e.position, e.pressEventCamera, out vector);
		if (vector.x < 0f)
		{
			vector.x = 0f;
		}
		else if (vector.x >= size.x)
		{
			vector.x = size.x;
		}
		if (vector.y < 0f)
		{
			vector.y = 0f;
		}
		else if (vector.y >= size.y)
		{
			vector.y = size.y;
		}
		this.pos.Set((int)((float)this.Size * vector.x / size.x), (int)((float)this.Size * vector.y / size.y));
		this.pos.Clamp(false);
		if (this.preview.limitBounds)
		{
			this.pos.x += EMono._map.bounds.x - this.preview.offsetX;
			this.pos.z += EMono._map.bounds.z - this.preview.offsetZ;
		}
		if (this.pos.cell.outOfBounds || !this.pos.cell.isSeen)
		{
			SE.BeepSmall();
			return;
		}
		EMono.pc.SetAIImmediate(new AI_Goto(this.pos, 0, false, false));
	}

	public void OnPointerUp(PointerEventData e)
	{
	}

	public void OnInitializePotentialDrag(PointerEventData ped)
	{
		ped.useDragThreshold = false;
	}

	public void OnDrag(PointerEventData e)
	{
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		UIContextMenu uicontextMenu = m.AddChild("setting");
		uicontextMenu.AddSlider("size", (float a) => this.extra.scale.ToString() ?? "", (float)this.extra.scale, delegate(float a)
		{
			this.extra.scale = (int)a;
			this.RefreshStyle();
		}, 5f, 20f, true, true, false);
		uicontextMenu.AddSlider("width", (float a) => (100 + this.extra.width).ToString() + "%", (float)this.extra.width, delegate(float a)
		{
			this.extra.width = (int)a;
			this.RefreshStyle();
		}, -25f, 25f, true, true, false);
		uicontextMenu.AddSlider("alpha", (float a) => this.extra.alpha.ToString() ?? "", (float)this.extra.alpha, delegate(float a)
		{
			this.extra.alpha = (int)a;
			this.RefreshStyle();
		}, 0f, 255f, true, true, false);
		uicontextMenu.AddToggle("rotation", this.extra.rotate, delegate(bool a)
		{
			this.extra.rotate = a;
			this.RefreshStyle();
		});
		uicontextMenu.AddToggle("monoColor", this.extra.monoColor, delegate(bool a)
		{
			WidgetMinimap.Extra extra = this.extra;
			this.preview.monoColor = a;
			extra.monoColor = a;
			this.preview.SetMap(EMono._map);
			this.RefreshStyle();
		});
		base.SetBaseContextMenu(m);
	}

	public void RefreshStyle()
	{
		this.rectAll.localEulerAngles = ((this.extra.rotate && !EMono._zone.IsRegion) ? new Vector3(60f, 0f, -45f) : Vector3.zero);
		this.preview.matMap.SetColor("_Color", new Color(1f, 1f, 1f, (float)this.extra.alpha / 255f));
		this.Rect().sizeDelta = new Vector2(0.1f * this.baseSize.x * (float)(10 + this.extra.scale) * (float)(100 + this.extra.width) * 0.01f, 0.1f * this.baseSize.y * (float)(10 + this.extra.scale));
	}

	public void OnUpdate()
	{
		this.point.Set(EMono.screen.position);
		if (this.preview.limitBounds)
		{
			this.point.x -= EMono._map.bounds.x - this.preview.offsetX;
			this.point.z -= EMono._map.bounds.z - this.preview.offsetZ;
		}
		float x = (float)(this.point.x / this.Size) - 0.5f;
		float y = (float)(this.point.z / this.Size) - 0.5f;
		this.transBound.localScale = new Vector3(Mathf.Min((float)EMono.screen.width / (float)this.Size * 1.5f, 1f), Mathf.Min((float)EMono.screen.height / (float)this.Size / 2f, 1f), 1f);
		this.transBound.localPosition = new Vector3(x, y, 50f);
		this.transCam.localScale = new Vector3(1f / EMono.core.uiScale, 1f / EMono.core.uiScale, 1f);
	}

	public void RefreshMarkers()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		this.psAlly.Clear();
		this.EmitParticle(EMono.pc, this.psAlly, this.colorAlly);
		if (EMono._zone.instance != null)
		{
			foreach (Chara chara in EMono._map.charas)
			{
				if (chara.IsHostile())
				{
					this.EmitParticle(chara, this.psEnemy, this.colorEnemy);
				}
			}
		}
	}

	public void EmitParticle(Card c, ParticleSystem ps, Color col)
	{
		int num = c.pos.x;
		int num2 = c.pos.z;
		float z = c.IsPCFactionOrMinion ? 9f : (10f + 0.01f * (float)(c.pos.z * 200 + c.pos.x));
		if (this.preview.limitBounds)
		{
			num -= EMono._map.bounds.x - this.preview.offsetX;
			num2 -= EMono._map.bounds.z - this.preview.offsetZ;
		}
		float x = (float)num / (float)this.Size - 0.5f;
		float y = (float)num2 / (float)this.Size - 0.5f;
		this.count++;
		ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams
		{
			position = new Vector3(x, y, z),
			startSize = this.psSize,
			startLifetime = this.intervalPS,
			startColor = col
		};
		this.psAlly.Emit(emitParams, 1);
	}

	public static WidgetMinimap Instance;

	public float interval;

	public float intervalPS;

	public float psSize;

	public RectTransform rectAll;

	public RectTransform rectMap;

	public Vector2 baseSize;

	private Point pos = new Point();

	public UIButton buttonMaptool;

	public UIMapPreview preview;

	public Color colorAlly;

	public Color colorEnemy;

	public Color colorGuest;

	public ParticleSystem psAlly;

	public ParticleSystem psEnemy;

	public Transform transBound;

	public Vector2 posPs;

	public Transform transCam;

	private Point point = new Point();

	private int count;

	public class Extra
	{
		public bool rotate;

		public bool monoColor;

		public bool limitBounds;

		public int alpha;

		public int scale;

		public int width;
	}
}
