using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LayerHoard : ELayer
{
	public Hoard hoard
	{
		get
		{
			return ELayer.player.hoard;
		}
	}

	public override void OnAfterInit()
	{
		ELayer.sources.collectibles.Init();
		Msg.TalkHomeMemeber("layerHoard");
		ActionMode.NoMap.Activate(true, false);
		this.actor = Util.Instantiate<HoardActor>(this.actor, null);
		if (this.hoard.items.Count == 0)
		{
			this.OnClickClear();
		}
		else
		{
			this.Reset();
		}
		base.InvokeRepeating("UpdateMousePos", 0f, 0.1f);
		if (!ELayer.debug.debugHoard)
		{
			UIButton[] array = this.debugButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(false);
			}
		}
		this.layoutMenu.RebuildLayout(true);
		SoundManager.disableSpread = true;
		ELayer.Sound.zone.enabled = false;
		ELayer.scene.cam.enabled = false;
	}

	public void Reset()
	{
		this.actor.Activate();
		this.textMode.text = "[" + ("h_" + this.hoard.mode.ToString()).lang() + "]";
	}

	public override void OnKill()
	{
		this.actor.Clear();
		UnityEngine.Object.DestroyImmediate(this.actor.gameObject);
		ActionMode.DefaultMode.Activate(true, false);
		ELayer.ui.cg.alpha = 1f;
		ELayer.Sound.maxSounds = ELayer.Sound._maxSounds;
		SoundManager.disableSpread = false;
		ELayer.Sound.zone.enabled = true;
		ELayer.scene.cam.enabled = true;
	}

	private void Update()
	{
		if (this.textScore.gameObject.activeSelf && this.hoard.score != this.lastScore)
		{
			this.textScore.text = this.hoard.score.ToString() + " / " + this.hoard.hiScore.ToString();
			this.lastScore = this.hoard.score;
		}
		if (Input.GetMouseButtonDown(2))
		{
			this.Reset();
		}
		if (ELayer.debug.enable)
		{
			if (Input.GetKeyDown(KeyCode.H))
			{
				this.hoard.hentai = true;
				this.hoard.Add("lulu", 2, false);
				this.hoard.Add("tentacle", 1, false);
				this.hoard.Add("tentacle2", 1, false);
				this.hoard.Add("tentacle3", 1, false);
				this.hoard.Add("tentacle4", 2, false);
				Msg.SayPic("UN_farris", "変態だー！", null);
				this.Reset();
			}
			if (Input.GetKeyDown(KeyCode.M))
			{
				try
				{
					CollectibleActor collectibleActor = this.actor.actors.FirstOrDefault((CollectibleActor a) => a.item.id == "mani");
					if (collectibleActor && collectibleActor.gameObject.activeInHierarchy)
					{
						Msg.Nerun("あ・・・", "UN_nerun");
						RigidExplode rigidExplode = collectibleActor.gameObject.AddComponent<RigidExplode>();
						rigidExplode.particle = this.actor.psExplode;
						rigidExplode.radius *= 10f;
						rigidExplode.force *= 10f;
						rigidExplode.chance = 1f;
						rigidExplode.intervalMin = (rigidExplode.intervalMax = 2f);
						rigidExplode.rb = collectibleActor.GetComponent<Rigidbody2D>();
						this.actor.updates.Add(rigidExplode);
					}
				}
				catch
				{
				}
			}
		}
		UIButton componentOf = InputModuleEX.GetComponentOf<UIButton>();
		if (ELayer.ui.GetTopLayer() != this || (componentOf && componentOf.transform.IsChildOf(base.transform)) || this.hasMouseMoved)
		{
			this.idleTimer = 0f;
			this.v = 1;
		}
		else
		{
			this.idleTimer += Time.deltaTime;
			if (this.idleTimer > 1f)
			{
				this.v = -1;
			}
		}
		(this.cg ?? ELayer.ui.cg).alpha += (float)this.v * Time.deltaTime * this.fadeSpeed;
		ELayer.Sound.maxSounds = this.hoard.maxSounds;
	}

	private void UpdateMousePos()
	{
		this.hasMouseMoved = ((Input.mousePosition - this.lastMousePos).magnitude > 15f);
		this.lastMousePos = Input.mousePosition;
	}

	public void AddRandom()
	{
		foreach (SourceCollectible.Row row in ELayer.sources.collectibles.rows)
		{
			if (!row.tag.Contains("noSpawn"))
			{
				int num = (row.num == 0) ? 10 : row.num;
				this.hoard.Add(row.id, ELayer.rnd(ELayer.rnd(ELayer.rnd(ELayer.rnd(num + 1) + 1) + 1) + 1), false);
			}
		}
		this.Reset();
	}

	public void Generate()
	{
		this.hoard.Clear();
		foreach (SourceCollectible.Row row in ELayer.sources.collectibles.rows)
		{
			if (!row.tag.Contains("noSpawn"))
			{
				this.hoard.Add(row.id);
			}
		}
		this.hoard.mode = Hoard.Mode.lux;
		this.Reset();
	}

	public void OnClickClear()
	{
		this.hoard.Clear();
		this.hoard.Add("gold", 1, false);
		this.Reset();
	}

	public void OnClickNextMode()
	{
		this.hoard.mode = this.hoard.mode.NextEnum<Hoard.Mode>();
		Msg.Nerun("h_filter".lang(("h_" + this.hoard.mode.ToString()).lang(), null, null, null, null), "UN_nerun");
		this.Reset();
	}

	public void OnClickConfig()
	{
		UIContextMenu uicontextMenu = ELayer.ui.contextMenu.Create("ContextHoard", true);
		int max = this.actor.bgs.Length - 1;
		uicontextMenu.AddSlider("room", (float n) => n.ToString() + " / " + max.ToString(), (float)this.hoard.bg, delegate(float v)
		{
			this.hoard.bg = (int)v;
			this.actor.RefreshBG();
		}, 0f, (float)max, true, false, false);
		uicontextMenu.AddSlider("reflection", (float n) => n.ToString() + " %", (float)this.hoard.reflection, delegate(float v)
		{
			this.hoard.reflection = (int)v;
			this.actor.RefreshBG();
		}, 0f, 300f, true, false, false);
		uicontextMenu.AddSlider("maxSounds", (float n) => n.ToString() ?? "", (float)this.hoard.maxSounds, delegate(float v)
		{
			this.hoard.maxSounds = (int)v;
		}, 0f, 64f, true, false, false);
		uicontextMenu.AddSlider("volume", (float n) => n.ToString() ?? "", (float)this.hoard.volume, delegate(float v)
		{
			this.hoard.volume = (int)v;
		}, 0f, 200f, true, false, false);
		uicontextMenu.AddToggle("voice", this.hoard.voice, delegate(bool on)
		{
			this.hoard.voice = on;
		});
		uicontextMenu.AddToggle("shadow", this.hoard.shadow, delegate(bool on)
		{
			this.hoard.shadow = on;
		});
		uicontextMenu.AddToggle("pixelPerfect", this.hoard.pixelPerfect, delegate(bool on)
		{
			this.hoard.pixelPerfect = on;
			this.actor.RefreshZoom();
		});
		uicontextMenu.Show();
	}

	public HoardActor actor;

	public bool autoList;

	public int autoNum;

	public float idleTimer;

	public float fadeSpeed;

	public CanvasGroup cg;

	public UIText textMode;

	public UIText textScore;

	public UIText textTime;

	public UIButton[] debugButtons;

	public LayoutGroup layoutMenu;

	private int lastScore;

	public int v;

	private Vector3 lastMousePos;

	private bool hasMouseMoved;
}
