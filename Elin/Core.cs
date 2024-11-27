using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using ReflexCLI;
using Steamworks;
using UnityEngine;

public class Core : BaseCore
{
	public bool IsGameStarted
	{
		get
		{
			return this.game != null && this.game.activeZone != null;
		}
	}

	public override float uiScale
	{
		get
		{
			return this.ui.canvasScaler.scaleFactor;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
		IO.CreateDirectory(CorePath.Temp);
		Core.SetCurrent(null);
		Lang.langCode = "";
		if (Application.isEditor && this.debug.langCode != Lang.LangCode.None)
		{
			this.forceLangCode = this.debug.langCode.ToString();
		}
		foreach (string text in Application.isEditor ? this.debug.command.Split(',', StringSplitOptions.None) : Environment.GetCommandLineArgs())
		{
			text = text.Replace("-", "").ToUpper();
			Debug.Log("Commandline args:" + text);
			if (text.StartsWith("LANG_"))
			{
				this.forceLangCode = text.Replace("LANG_", "");
			}
			else if (!(text == "DEV"))
			{
				if (!(text == "UPDATE_LANG"))
				{
					if (text == "NOMOD")
					{
						ModManager.disableMod = true;
					}
				}
				else
				{
					Lang.runUpdate = true;
				}
			}
			else
			{
				this.releaseMode = ReleaseMode.Debug;
			}
		}
		NewsList.dict = null;
		InvOwner.Trader = (InvOwner.Main = null);
		SpriteSheet.dict.Clear();
		SpriteSheet.loadedPath.Clear();
		SpriteReplacerAnimation.dict.Clear();
		LayerDrama.haltPlaylist = false;
		LayerDrama.keepBGM = false;
		BaseModManager.isInitialized = false;
		Net.isUploading = false;
		MapPiece.initialized = false;
		ActPlan.warning = false;
		Game.isPaused = false;
		if (Application.isEditor && BaseCore.resetRuntime)
		{
			this.ResetRuntime();
		}
		Window.dictData.Clear();
		LayerInventory.listInv.Clear();
		this.debug = Resources.Load<CoreDebug>("_Debug " + this.releaseMode.ToString());
		this.debug.enable = (this.releaseMode > ReleaseMode.Public);
		if (this.useUImat)
		{
			Canvas.GetDefaultCanvasMaterial().CopyPropertiesFromMaterial(this.matUI);
			Canvas.GetDefaultCanvasMaterial().shader = this.matUI.shader;
		}
		if (!this.ui)
		{
			this.ui = Util.Instantiate<UI>("UI/UI", null);
		}
		if (!this.scene)
		{
			this.scene = Util.Instantiate<Scene>("Scene/Scene", null);
		}
		this.scene.screenNoMap.Activate();
		this.ui.ShowCover(0f, 1f, null, default(Color));
		this.canvas = this.ui.canvas;
		this.ui.OnCoreStart();
		BaseCore.BlockInput = (() => this.ui.IsDragging);
		CommandRegistry.assemblies.Add(typeof(Core).Assembly);
		SoundData.EditorPlaySpatial = delegate(SoundData a)
		{
			this.game.player.chara.PlaySound(a.name, 1f, true);
		};
		SoundManager.funcCanPlayBGM = (() => !LayerDrama.haltPlaylist && !LayerDrama.keepBGM);
		FileDragAndDrop.onDrop = new Action<List<string>>(this.textures.OnDropFile);
		MOD.langs.Clear();
		MOD.OnAddPcc = new Action<DirectoryInfo>(this.pccs.Add);
		MOD.listTalk = new TalkDataList();
		MOD.tones = new ToneDataList();
		MOD.listMaps.Clear();
		MOD.listPartialMaps.Clear();
		Portrait.modPortraitBGFs = new ModItemList<Sprite>(0);
		Portrait.modPortraitBGs = new ModItemList<Sprite>(0);
		Portrait.modPortraits = new ModItemList<Sprite>(0);
		Portrait.modOverlays = new ModItemList<Sprite>(0);
		Portrait.modFull = new ModItemList<Sprite>(0);
		Portrait.dictList = new Dictionary<string, List<ModItem<Sprite>>>();
		TileType.Init();
		this.screen.tileMap.rendererObjDummy.Init();
		Debug.Log("Core Awake");
		Debug.Log(this.steam.steamworks.settings.applicationId);
	}

	private void Start()
	{
		this.config = CoreConfig.TryLoadConfig();
		string str = "Loading Config:";
		string path = CoreConfig.path;
		string str2 = " - ";
		CoreConfig coreConfig = this.config;
		Debug.Log(str + path + str2 + ((coreConfig != null) ? coreConfig.ToString() : null));
		if (this.config != null && this.config.other.disableMods)
		{
			ModManager.disableMod = true;
		}
		try
		{
			this.steam.Init();
		}
		catch
		{
		}
		this.StartCase();
	}

	public void StartCase()
	{
		Debug.Log("StartCase:" + ModManager.IsInitialized.ToString());
		if (!ModManager.IsInitialized)
		{
			this.mods.Init(CorePath.rootMod, "_Elona");
			base.StartCoroutine(this.mods.RefreshMods(delegate
			{
				MOD.actorSources.Initialize();
				this.StartCase();
			}, !ModManager.disableMod && !BaseCore.IsOffline && !this.debug.skipModSync && (this.config == null || this.config.other.syncMods)));
			return;
		}
		Debug.Log("Plugins:" + ModManager.ListPluginObject.Count.ToString());
		foreach (object obj in ModManager.ListPluginObject)
		{
			Component component = obj as Component;
			if (component)
			{
				GameObject gameObject = component.gameObject;
				gameObject.SendMessage("OnStartCore");
				foreach (Component component2 in gameObject.GetComponentsInChildren<Component>())
				{
					Debug.Log(component2.GetType().Assembly.GetName().Name);
					ClassCache.assemblies.Add(component2.GetType().Assembly.GetName().Name);
				}
				break;
			}
			Debug.Log("no go");
		}
		this.sources.Init();
		if (!this.forceLangCode.IsEmpty())
		{
			this.langCode = this.forceLangCode;
		}
		else if (this.config == null)
		{
			Debug.Log("Config doesn't exist.");
			try
			{
				if (this.debug.enable || !SteamAPI.IsSteamRunning())
				{
					BookList.dict = null;
					this.ui.ShowLang();
					return;
				}
				string currentGameLanguage = SteamApps.GetCurrentGameLanguage();
				if (!(currentGameLanguage == "chinese"))
				{
					if (!(currentGameLanguage == "japanese"))
					{
						BookList.dict = null;
						this.ui.ShowLang();
						return;
					}
					this.langCode = "JP";
				}
				else
				{
					this.langCode = "CN";
				}
			}
			catch (Exception message)
			{
				Debug.Log(message);
			}
		}
		if (this.debug.showSceneSelector || (Input.GetKey(KeyCode.LeftShift) && this.debug.enable))
		{
			this.ui.ShowSceneSelector();
			return;
		}
		this.Init();
	}

	public void Update()
	{
		this.frame++;
		SoundManager.requestCount = 0;
		InputModuleEX.UpdateEventData();
		EInput.uiMousePosition = Input.mousePosition / this.ui.canvasScaler.scaleFactor;
		if (!this.initialized)
		{
			return;
		}
		this.ui.OnUpdate();
		this.debug.UpdateAlways();
		PoolManager.ignorePool = this.debug.ignorePool;
		if (this.screen.tileMap && !this.screen.tileMap.passBlock.mat.mainTexture)
		{
			this.textures.RefreshTextures();
		}
		if (this.useUImat)
		{
			Canvas.GetDefaultCanvasMaterial().CopyPropertiesFromMaterial(this.matUI);
		}
		Core.avgDelta += (Time.smoothDeltaTime - Core.delta) * 0.1f;
		Core.delta = Time.smoothDeltaTime;
		if (Core.delta > 0.1f)
		{
			Core.delta = 0.1f;
		}
		EInput.delta = (ButtonState.delta = Core.delta);
		bool? flag = this.lastFullScreen;
		bool fullScreen = Screen.fullScreen;
		if (!(flag.GetValueOrDefault() == fullScreen & flag != null))
		{
			int width = Display.main.systemWidth;
			int height = Display.main.systemHeight;
			if (this.config != null && this.config.graphic.fixedResolution)
			{
				width = this.config.graphic.w;
				height = this.config.graphic.h;
			}
			if (Screen.fullScreen)
			{
				Screen.SetResolution(width, height, true);
			}
			this.lastFullScreen = new bool?(Screen.fullScreen);
			this.nextResolutionUpdate = 0f;
		}
		if (this.nextResolutionUpdate <= 0f)
		{
			if (this.config != null && this.config.graphic.fixedResolution)
			{
				int w = this.config.graphic.w;
				int h = this.config.graphic.h;
				if (Screen.width != w || Screen.height != h)
				{
					Screen.SetResolution(w, h, Screen.fullScreen);
				}
			}
			if (Screen.width != this.lastScreenWidth || Screen.height != this.lastScreenHeight)
			{
				this.OnChangeResolution();
			}
			this.nextResolutionUpdate = 0.1f;
		}
		else
		{
			this.nextResolutionUpdate -= Core.delta;
		}
		if (this.IsGameStarted)
		{
			this.game.OnUpdate();
			Core.gameDeltaNoPause = Core.delta * this.scene.actionMode.gameSpeed;
			Core.gameDelta = (FlockChild.delta = (this.ui.IsPauseGame ? 0f : Core.gameDeltaNoPause));
		}
		else
		{
			Core.gameDelta = 0f;
		}
		this.scene.OnUpdate();
		if (this.actionsNextFrame.Count > 0)
		{
			for (int i = this.actionsNextFrame.Count - 1; i >= 0; i--)
			{
				this.actionsNextFrame[i]();
				this.actionsNextFrame.RemoveAt(i);
			}
		}
	}

	private void LateUpdate()
	{
		if (!this.initialized)
		{
			return;
		}
		if (UIButton.actionTooltip != null)
		{
			UIButton.actionTooltip();
			UIButton.actionTooltip = null;
		}
		if (this.actionsLateUpdate.Count > 0)
		{
			for (int i = this.actionsLateUpdate.Count - 1; i >= 0; i--)
			{
				this.actionsLateUpdate[i]();
				this.actionsLateUpdate.RemoveAt(i);
			}
		}
		this.scene.OnLateUpdate();
	}

	public void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			AudioListener.volume = 1f;
		}
		else if (this.config.other.muteBackground)
		{
			AudioListener.volume = 0f;
		}
		if (focus && Core.Instance)
		{
			if (this.actionsNextFrame.Count == 0)
			{
				this.actionsNextFrame.Add(delegate
				{
					this.textures.RefreshTextures();
					this.actionsNextFrame.Add(delegate
					{
						if (this.IsGameStarted)
						{
							if (Application.isEditor && WidgetMinimap.Instance)
							{
								WidgetMinimap.Instance.Reload();
							}
							foreach (CardRow cardRow in this.sources.cards.rows)
							{
								if (cardRow.replacer.hasChacked && cardRow.replacer.data != null)
								{
									cardRow.replacer.data.GetSprite(false);
								}
							}
						}
					});
				});
			}
			if (this.IsGameStarted && this.debug.enable)
			{
				foreach (PCC.Part part in this.pccs.allParts.Values)
				{
					foreach (ModItem<Texture2D> modItem in part.modTextures.Values)
					{
						modItem.ClearCache();
					}
				}
				CharaActorPCC[] array = UnityEngine.Object.FindObjectsOfType<CharaActorPCC>();
				for (int i = 0; i < array.Length; i++)
				{
					array[i].provider.Rebuild(PCCState.Normal);
				}
			}
			EInput.Consume(false, 1);
			EInput.dragHack = 0f;
		}
	}

	public void OnApplicationQuit()
	{
		try
		{
			foreach (Widget widget in this.ui.widgets.GetComponentsInChildren<Widget>(true))
			{
				if (widget.gameObject != null)
				{
					UnityEngine.Object.Destroy(widget.gameObject);
				}
			}
			IO.PrintLog();
		}
		catch (Exception ex)
		{
			string str = "Exception:";
			Exception ex2 = ex;
			Debug.Log(str + ((ex2 != null) ? ex2.ToString() : null));
		}
		try
		{
			if (SteamAPI.IsSteamRunning())
			{
				SteamAPI.Shutdown();
			}
		}
		catch (Exception ex3)
		{
			string str2 = "Exception:";
			Exception ex4 = ex3;
			Debug.Log(str2 + ((ex4 != null) ? ex4.ToString() : null));
		}
		try
		{
			IO.DeleteDirectory(CorePath.Temp);
			GameIO.DeleteEmptyGameFolders();
		}
		catch (Exception ex5)
		{
			string str3 = "Exception:";
			Exception ex6 = ex5;
			Debug.Log(str3 + ((ex6 != null) ? ex6.ToString() : null));
		}
	}

	public void ResetRuntime()
	{
		BaseCore.resetRuntime = false;
		BiomeProfile.forceInitialize = true;
		WidgetHotbar.registering = false;
		WidgetHotbar.registeringItem = null;
		WordGen.initialized = false;
		RecipeManager.rebuild = true;
		BookList.dict = null;
		SpawnList.allList.Clear();
	}

	public void Init()
	{
		Debug.Log("Initializing:" + this.langCode + "/" + this.forceLangCode);
		base.StartCoroutine("OnEndOfFrame");
		base.InvokeRepeating("Update100ms", 0.1f, 0.1f);
		UnityEngine.Object.DestroyImmediate(this.ui.layoutLang.gameObject);
		CoreConfig.Init();
		this.SetLang(this.config.lang, false);
		this.refs.Init();
		this.debug.Init();
		SpriteSheet.Add("Media/Graphics/Icon/icons_48");
		Cal.Init();
		this.Colors.Init();
		this.gameSetting.Init();
		this.mods.InitLang();
		if (!Lang.isBuiltin)
		{
			this.sources.ImportSourceTexts();
		}
		PCCManager.current.Init();
		SpriteVariationManager.current.Init();
		this.scene.InitPass();
		this.textures.Init();
		this.textures.RefreshTextures();
		SoundManager.current.Reset();
		if (this.debug.validatePref)
		{
			SourcePrefInspector.Instance.ValidatePrefs();
		}
		if (this.debug.startScene == CoreDebug.StartScene.Title)
		{
			this.scene.Init(Scene.Mode.Title);
		}
		else
		{
			this.debug.QuickStart();
		}
		this.ui.HideCover(2f, null);
		this.initialized = true;
	}

	public void Quit()
	{
		Application.Quit();
		Debug.Log("Quit");
	}

	public override void ConsumeInput()
	{
		EInput.Consume(false, 1);
	}

	public static Core SetCurrent(Core _current = null)
	{
		if (Core.Instance)
		{
			return Core.Instance;
		}
		Core.Instance = (_current ?? UnityEngine.Object.FindObjectOfType<Core>());
		BaseCore.Instance = Core.Instance;
		if (Core.Instance)
		{
			Core.Instance.SetReferences();
		}
		return Core.Instance;
	}

	public static Core GetCurrent()
	{
		return Core.Instance ?? Core.SetCurrent(null);
	}

	public void SetReferences()
	{
		CorePath.Init();
		SourceData.dataPath = CorePath.packageCore + "/Data/Source/";
		EMono.core = this;
		EClass.core = this;
		Core.Instance = this;
		SkinManager._Instance = this.skins;
		PathManager.Instance = this.pathManager;
		EffectManager.Instance = this.effects;
		this.sources.InitLang();
	}

	public void SetReleaseMode(ReleaseMode m)
	{
		if (this.releaseMode == m)
		{
			return;
		}
		this.releaseMode = m;
		this.debug = Resources.Load<CoreDebug>("_Debug " + this.releaseMode.ToString());
	}

	private IEnumerator OnEndOfFrame()
	{
		for (;;)
		{
			yield return new WaitForEndOfFrame();
			if (this.screen.guide)
			{
				this.screen.guide.OnEndOfFrame();
			}
		}
		yield break;
	}

	private void Update100ms()
	{
		if (!this.IsGameStarted)
		{
			return;
		}
		this.game.updater.Update100ms();
	}

	public void OnChangeResolution()
	{
		if (this.config != null)
		{
			this.config.OnChangeResolution();
		}
		this.screen.RefreshScreenSize();
		this.lastScreenWidth = Screen.width;
		this.lastScreenHeight = Screen.height;
		IChangeResolution[] componentsInChildren = this.ui.GetComponentsInChildren<IChangeResolution>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].OnChangeResolution();
		}
		this.scene.camSupport.OnChangeResolution();
	}

	public void Halt()
	{
	}

	public override void FreezeScreen(float duration)
	{
		this.ui.FreezeScreen(duration);
	}

	public override void UnfreezeScreen()
	{
		this.ui.UnfreezeScreen();
	}

	public override void RebuildBGMList()
	{
		this.refs.RebuildBGMList();
	}

	public override void StopEventSystem(Component c, Action action, float duration = 0.12f)
	{
		this.eventSystem.enabled = false;
		c.transform.DOScale(new Vector3(1f, 0f, 1f), duration).OnKill(delegate
		{
			this.eventSystem.enabled = true;
		}).OnComplete(delegate
		{
			action();
		}).SetEase(Ease.Linear);
	}

	public override void StopEventSystem(float duration = 0.2f)
	{
		this.eventSystem.enabled = false;
		TweenUtil.Tween(duration, null, delegate()
		{
			this.eventSystem.enabled = true;
		});
	}

	public void SetLang(string langCode, bool force = false)
	{
		if (Lang.langCode == langCode && this.sources && this.sources.langGeneral)
		{
			return;
		}
		Lang.Init(langCode);
		AliasGen.list = null;
		NameGen.list = null;
		WordGen.initialized = false;
		this.sources.OnChangeLang();
		if (this.game != null && !this.IsGameStarted)
		{
			this.game.Kill();
		}
		this.config.OnSetLang();
	}

	public static int[] ParseElements(string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return new int[0];
		}
		string[] array = str.Replace("\n", "").Split(',', StringSplitOptions.None);
		int[] array2 = new int[array.Length * 2];
		for (int i = 0; i < array.Length; i++)
		{
			string[] array3 = array[i].Split('/', StringSplitOptions.None);
			array2[i * 2] = Core.GetElement(array3[0]);
			array2[i * 2 + 1] = ((array3.Length == 1) ? 1 : int.Parse(array3[1]));
		}
		return array2;
	}

	public static int GetElement(string id)
	{
		Core.SetCurrent(null);
		if (Core.sourceElement == null)
		{
			Core.sourceElement = UnityEngine.Object.FindObjectOfType<SourceManager>().GetComponent<SourceManager>().elements;
		}
		if (!Core.sourceElement.initialized)
		{
			Core.sourceElement.Init();
		}
		SourceElement.Row row;
		if (!Core.sourceElement.alias.TryGetValue(id ?? "_void", out row))
		{
			Debug.LogError("exception:" + id);
			row = Core.sourceElement.rows[0];
		}
		return row.id;
	}

	public void ApplySkins()
	{
		IUISkin[] componentsInChildren = this.ui.GetComponentsInChildren<IUISkin>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ApplySkin();
		}
	}

	public new static Core Instance;

	public static float delta;

	public static float avgDelta;

	public static float gameDelta;

	public static float gameDeltaNoPause;

	public static bool spiked;

	private static SourceElement sourceElement;

	public CoreDebug debug;

	public SourceManager sources;

	public PathManager pathManager;

	public EffectManager effects;

	public ModManager mods = new ModManager();

	public PCCManager pccs;

	public SkinManager skins;

	public TextureManager textures = new TextureManager();

	public GameSetting gameSetting;

	public GameData gamedata;

	public CoreRef refs;

	public ColorProfile Colors;

	public Material matUI;

	public Steam steam;

	public bool useUImat;

	public bool trial;

	public UI ui;

	public Scene scene;

	public BaseGameScreen screen;

	[NonSerialized]
	public CoreConfig config;

	public Game game;

	[NonSerialized]
	public bool initialized;

	private bool? lastFullScreen;

	private float nextResolutionUpdate;

	private float skinTimer;
}
