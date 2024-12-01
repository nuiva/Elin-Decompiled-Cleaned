using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LayerHoard : ELayer
{
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

	public Hoard hoard => ELayer.player.hoard;

	public override void OnAfterInit()
	{
		ELayer.sources.collectibles.Init();
		Msg.TalkHomeMemeber("layerHoard");
		ActionMode.NoMap.Activate();
		actor = Util.Instantiate(actor);
		if (hoard.items.Count == 0)
		{
			OnClickClear();
		}
		else
		{
			Reset();
		}
		InvokeRepeating("UpdateMousePos", 0f, 0.1f);
		if (!ELayer.debug.debugHoard)
		{
			UIButton[] array = debugButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(enable: false);
			}
		}
		layoutMenu.RebuildLayout(recursive: true);
		SoundManager.disableSpread = true;
		ELayer.Sound.zone.enabled = false;
		ELayer.scene.cam.enabled = false;
	}

	public void Reset()
	{
		actor.Activate();
		textMode.text = "[" + ("h_" + hoard.mode).lang() + "]";
	}

	public override void OnKill()
	{
		actor.Clear();
		Object.DestroyImmediate(actor.gameObject);
		ActionMode.DefaultMode.Activate();
		ELayer.ui.cg.alpha = 1f;
		ELayer.Sound.maxSounds = ELayer.Sound._maxSounds;
		SoundManager.disableSpread = false;
		ELayer.Sound.zone.enabled = true;
		ELayer.scene.cam.enabled = true;
	}

	private void Update()
	{
		if (textScore.gameObject.activeSelf && hoard.score != lastScore)
		{
			textScore.text = hoard.score + " / " + hoard.hiScore;
			lastScore = hoard.score;
		}
		if (Input.GetMouseButtonDown(2))
		{
			Reset();
		}
		if (ELayer.debug.enable)
		{
			if (Input.GetKeyDown(KeyCode.H))
			{
				hoard.hentai = true;
				hoard.Add("lulu", 2);
				hoard.Add("tentacle", 1);
				hoard.Add("tentacle2", 1);
				hoard.Add("tentacle3", 1);
				hoard.Add("tentacle4", 2);
				Msg.SayPic("UN_farris", "変態だー！");
				Reset();
			}
			if (Input.GetKeyDown(KeyCode.M))
			{
				try
				{
					CollectibleActor collectibleActor = actor.actors.FirstOrDefault((CollectibleActor a) => a.item.id == "mani");
					if ((bool)collectibleActor && collectibleActor.gameObject.activeInHierarchy)
					{
						Msg.Nerun("あ・・・");
						RigidExplode rigidExplode = collectibleActor.gameObject.AddComponent<RigidExplode>();
						rigidExplode.particle = actor.psExplode;
						rigidExplode.radius *= 10f;
						rigidExplode.force *= 10f;
						rigidExplode.chance = 1f;
						rigidExplode.intervalMin = (rigidExplode.intervalMax = 2f);
						rigidExplode.rb = collectibleActor.GetComponent<Rigidbody2D>();
						actor.updates.Add(rigidExplode);
					}
				}
				catch
				{
				}
			}
		}
		UIButton componentOf = InputModuleEX.GetComponentOf<UIButton>();
		if (ELayer.ui.GetTopLayer() != this || ((bool)componentOf && componentOf.transform.IsChildOf(base.transform)) || hasMouseMoved)
		{
			idleTimer = 0f;
			v = 1;
		}
		else
		{
			idleTimer += Time.deltaTime;
			if (idleTimer > 1f)
			{
				v = -1;
			}
		}
		(cg ?? ELayer.ui.cg).alpha += (float)v * Time.deltaTime * fadeSpeed;
		ELayer.Sound.maxSounds = hoard.maxSounds;
	}

	private void UpdateMousePos()
	{
		hasMouseMoved = (Input.mousePosition - lastMousePos).magnitude > 15f;
		lastMousePos = Input.mousePosition;
	}

	public void AddRandom()
	{
		foreach (SourceCollectible.Row row in ELayer.sources.collectibles.rows)
		{
			if (!row.tag.Contains("noSpawn"))
			{
				int num = ((row.num == 0) ? 10 : row.num);
				hoard.Add(row.id, ELayer.rnd(ELayer.rnd(ELayer.rnd(ELayer.rnd(num + 1) + 1) + 1) + 1));
			}
		}
		Reset();
	}

	public void Generate()
	{
		hoard.Clear();
		foreach (SourceCollectible.Row row in ELayer.sources.collectibles.rows)
		{
			if (!row.tag.Contains("noSpawn"))
			{
				hoard.Add(row.id);
			}
		}
		hoard.mode = Hoard.Mode.lux;
		Reset();
	}

	public void OnClickClear()
	{
		hoard.Clear();
		hoard.Add("gold", 1);
		Reset();
	}

	public void OnClickNextMode()
	{
		hoard.mode = hoard.mode.NextEnum();
		Msg.Nerun("h_filter".lang(("h_" + hoard.mode).lang()));
		Reset();
	}

	public void OnClickConfig()
	{
		UIContextMenu uIContextMenu = ELayer.ui.contextMenu.Create("ContextHoard");
		int max = actor.bgs.Length - 1;
		uIContextMenu.AddSlider("room", (float n) => n + " / " + max, hoard.bg, delegate(float v)
		{
			hoard.bg = (int)v;
			actor.RefreshBG();
		}, 0f, max, isInt: true, hideOther: false);
		uIContextMenu.AddSlider("reflection", (float n) => n + " %", hoard.reflection, delegate(float v)
		{
			hoard.reflection = (int)v;
			actor.RefreshBG();
		}, 0f, 300f, isInt: true, hideOther: false);
		uIContextMenu.AddSlider("maxSounds", (float n) => n.ToString() ?? "", hoard.maxSounds, delegate(float v)
		{
			hoard.maxSounds = (int)v;
		}, 0f, 64f, isInt: true, hideOther: false);
		uIContextMenu.AddSlider("volume", (float n) => n.ToString() ?? "", hoard.volume, delegate(float v)
		{
			hoard.volume = (int)v;
		}, 0f, 200f, isInt: true, hideOther: false);
		uIContextMenu.AddToggle("voice", hoard.voice, delegate(bool on)
		{
			hoard.voice = on;
		});
		uIContextMenu.AddToggle("shadow", hoard.shadow, delegate(bool on)
		{
			hoard.shadow = on;
		});
		uIContextMenu.AddToggle("pixelPerfect", hoard.pixelPerfect, delegate(bool on)
		{
			hoard.pixelPerfect = on;
			actor.RefreshZoom();
		});
		uIContextMenu.Show();
	}
}
