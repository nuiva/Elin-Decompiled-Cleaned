using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class UI : ELayer
{
	public SkinManager skins
	{
		get
		{
			return ELayer.core.skins;
		}
	}

	public bool IsActive
	{
		get
		{
			return this.layers.Count > 0 || this.contextMenu.isActive;
		}
	}

	public bool AllowInventoryInteractions
	{
		get
		{
			return !this.contextMenu.isActive && (this.layers.Count == 0 || base.TopLayer.option.allowInventoryInteraction);
		}
	}

	public bool BlockActions
	{
		get
		{
			return (this.wasActive && (!base.TopLayer || !base.TopLayer.option.passive)) || EInput.isInputFieldActive;
		}
	}

	public bool BlockMouseOverUpdate
	{
		get
		{
			return this.wasActive;
		}
	}

	public bool BlockInput
	{
		get
		{
			return this.IsDragging || base.IsBlockWidgetClick() || LayerAbility.hotElement != null || EInput.isInputFieldActive || this.contextMenu.isActive;
		}
	}

	public bool IsPauseGame
	{
		get
		{
			return this.layers.Count > 0 && base.TopLayer.option.pauseGame;
		}
	}

	public bool IsInventoryOpen
	{
		get
		{
			return this.layerFloat.GetLayer<LayerInventory>(false);
		}
	}

	public bool IsAbilityOpen
	{
		get
		{
			return this.layerFloat.GetLayer<LayerAbility>(false);
		}
	}

	public override bool blockWidgetClick
	{
		get
		{
			return false;
		}
	}

	public override RectTransform rectLayers
	{
		get
		{
			return this._rectLayers;
		}
	}

	public bool IsDragging
	{
		get
		{
			return this.currentDrag != null;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		base.InvokeRepeating("CheckWindowOrder", 1f, 0.5f);
	}

	public void OnCoreStart()
	{
		this.mouseInfo.SetActive(false);
	}

	public void OnActivateZone()
	{
		this.widgets.OnActivateZone();
		ELayer.player.queues.SetOwner(ELayer.pc);
		if (this.hud.hangCorner && this.hud.hangCorner.isActiveAndEnabled)
		{
			this.hud.hangCorner.Refresh();
		}
	}

	public void ShowFloats()
	{
		this.layerFloat.SetActive(true);
	}

	public void HideFloats()
	{
		this.layerFloat.SetActive(false);
	}

	public void OnKillGame()
	{
		this.widgets.OnKillGame();
		this.ShowBalloon(true);
		this.layerFloat.RemoveLayers(true);
		this.currentDrag = null;
	}

	public void OnClickAction(string _mode)
	{
		base.RemoveLayers(false);
		((ActionMode)typeof(ActionMode).GetField(_mode, BindingFlags.Static | BindingFlags.Public).GetValue(null)).Activate(true, false);
	}

	public void RefreshActiveState()
	{
		this.isPointerOverUI = false;
		foreach (GameObject gameObject in InputModuleEX.GetPointerEventData(-1).hovered)
		{
			if (gameObject && gameObject.layer == 5)
			{
				this.isPointerOverUI = true;
				break;
			}
		}
		this.wasActive = this.IsActive;
		if (this.wasActive)
		{
			CursorSystem.SetCursor(null, 0);
		}
	}

	public override void OnChangeLayer()
	{
		this.RefreshActiveState();
		CursorSystem.SetCursor(null, 0);
		this.hud.HideMouseInfo();
		if (ELayer.core.IsGameStarted)
		{
			ELayer.scene.actionMode.OnUpdateCursor();
		}
		CursorSystem.Instance.Draw();
		Layer topLayer = base.TopLayer;
		if (topLayer != null && topLayer.option.dontShowHint)
		{
			return;
		}
		if (this.layers.Count == 0)
		{
			this.hud.hint.Refresh();
			return;
		}
		if (base.TopLayer.option.hideFloatUI)
		{
			this.HideFloats();
		}
		if (base.TopLayer.option.hideWidgets)
		{
			this.widgets.Hide();
		}
		ELayer elayer = base.TopLayer as ELayer;
		if (elayer == null)
		{
			return;
		}
		elayer.TryShowHint(null);
	}

	public void FlashCover(float durationOut = 1f, float duration = 1f, float durationIn = 1f, Action onFadeOut = null, Action onComplete = null, Color color = default(Color))
	{
		this.ShowCover(durationOut, 1f, null, color);
		TweenUtil.Tween(durationOut + duration, null, delegate()
		{
			if (onFadeOut != null)
			{
				onFadeOut();
			}
			this.HideCover(durationIn, onComplete);
		});
	}

	public void ShowCover(float duration = 0f, float dest = 1f, Action onComplete = null, Color color = default(Color))
	{
		TweenUtil.KillTween(ref this.tweenCover, false);
		float a = this.hud.imageCover.color.a;
		this.hud.imageCover.color = ((color == default(Color)) ? Color.black : color).SetAlpha(a);
		this.hud.imageCover.SetActive(true);
		this.hidingCover = false;
		this.tweenCover = this.hud.imageCover.DOFade(dest, duration).OnComplete(delegate
		{
			Action onComplete2 = onComplete;
			if (onComplete2 == null)
			{
				return;
			}
			onComplete2();
		});
	}

	public void HideCover(float duration = 0f, Action onComplete = null)
	{
		if (this.hidingCover)
		{
			return;
		}
		this.hidingCover = true;
		TweenUtil.KillTween(ref this.tweenCover, false);
		this.tweenCover = this.hud.imageCover.DOFade(0f, duration).OnComplete(delegate
		{
			this.hud.imageCover.SetActive(false);
			Action onComplete2 = onComplete;
			if (onComplete2 != null)
			{
				onComplete2();
			}
			this.hidingCover = false;
		});
	}

	public bool IsCovered()
	{
		return !this.hidingCover;
	}

	public void ShowBalloon(bool enable)
	{
		ELayer.ui.rectDynamic.SetActive(enable);
		WidgetSystemIndicator.Refresh();
	}

	public void Show(float duration = 1f)
	{
		ELayer.scene.elomapActor.selector.srHighlight.SetActive(true);
		this.cg.DOKill(false);
		if (duration == 0f)
		{
			this.cg.alpha = 1f;
			return;
		}
		this.cg.DOFade(1f, duration);
	}

	public void Hide(float duration = 1f)
	{
		ELayer.scene.elomapActor.selector.srHighlight.SetActive(false);
		if (duration == 0f)
		{
			this.cg.alpha = 0f;
			return;
		}
		this.cg.DOFade(0f, duration);
	}

	public void OnUpdate()
	{
		if (this.hud.imageDrag.gameObject.activeSelf)
		{
			this.hud.imageDrag.transform.position = EInput.mpos + (ELayer.game.UseGrid ? this.hud.imageDragFix2 : this.hud.imageDragFix2);
			Util.ClampToScreen(this.hud.imageDrag.Rect(), this.hud.marginImageDrag);
		}
		if (ELayer.config.ui.blur && this.layers.Count > 0 && base.IsUseBlur())
		{
			UI.blurSize += Time.unscaledDeltaTime * this.blurSpeed;
			if (UI.blurSize > ELayer.config.ui.blurSize)
			{
				UI.blurSize = ELayer.config.ui.blurSize;
			}
			if (!this.blur.activeSelf)
			{
				this.blur.SetActive(true);
			}
			int siblingIndex = this.blur.transform.GetSiblingIndex();
			int num = 0;
			for (int i = this.layers.Count - 1; i >= 0; i--)
			{
				if (this.layers[i].IsUseBlur())
				{
					num = this.layers[i].transform.GetSiblingIndex() - 1;
					break;
				}
			}
			if (siblingIndex != num)
			{
				this.blur.transform.SetSiblingIndex(num);
			}
		}
		else
		{
			UI.blurSize -= Time.unscaledDeltaTime * this.blurSpeed;
			if (UI.blurSize < 0f)
			{
				UI.blurSize = 0f;
			}
			if (this.blur.activeSelf && UI.blurSize == 0f)
			{
				this.blur.SetActive(false);
			}
		}
		this.matBlur.SetFloat("_Size", UI.blurSize);
		if (!EInput.isShiftDown)
		{
			LayerInventory.highlightInv = null;
		}
		this.ShowMouseHint();
	}

	public void ShowMouseHint()
	{
		if (this.durationHideMouseHint > 0f)
		{
			this.durationHideMouseHint -= Core.delta;
			this.hud.textMouseHintLeft.SetActive(false);
			this.hud.textMouseHintRight.SetActive(false);
			return;
		}
		IMouseHint mouseHint = UIButton.currentHighlight as IMouseHint;
		bool flag = mouseHint != null && !this.IsDragging && this.isPointerOverUI && UIButton.currentHighlight && InputModuleEX.IsPointerOver(UIButton.currentHighlight) && LayerAbility.hotElement == null;
		bool flag2 = flag;
		if (flag2)
		{
			if (mouseHint.ShowMouseHintLeft())
			{
				this.hud.textMouseHintLeft.text = mouseHint.GetTextMouseHintLeft();
				if (this.hud.textMouseHintLeft.text == "")
				{
					flag2 = false;
				}
			}
			else
			{
				flag2 = false;
			}
		}
		if (flag)
		{
			if (mouseHint.ShowMouseHintRight())
			{
				this.hud.textMouseHintRight.text = mouseHint.GetTextMouseHintRight();
				if (this.hud.textMouseHintRight.text == "")
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
		}
		this.hud.textMouseHintLeft.SetActive(flag2);
		this.hud.textMouseHintRight.SetActive(flag);
		if (flag || flag2)
		{
			this.hud.textMouseHintLeft.transform.position = UIButton.currentHighlight.transform.position + this.hud.textMouseHintFixLeft;
			this.hud.textMouseHintRight.transform.position = UIButton.currentHighlight.transform.position + this.hud.textMouseHintFix;
			Util.ClampToScreen(this.hud.textMouseHintRight.Rect(), 10);
		}
	}

	public void HideMouseHint(float duration = 0.2f)
	{
		this.durationHideMouseHint = duration;
		this.hud.textMouseHintLeft.SetActive(false);
		this.hud.textMouseHintRight.SetActive(false);
	}

	public void CheckWindowOrder()
	{
		if (this.layers.Count > 0)
		{
			return;
		}
		foreach (GameObject gameObject in InputModuleEX.GetPointerEventData(-1).hovered)
		{
			if (gameObject)
			{
				Widget component = gameObject.GetComponent<Widget>();
				if (component && !component.AlwaysBottom && this.widgets.transform.GetChild(this.widgets.transform.childCount - 1) != gameObject.transform)
				{
					gameObject.transform.SetAsLastSibling();
					break;
				}
			}
		}
		if (WidgetFeed.Instance)
		{
			WidgetFeed.Instance.transform.SetAsLastSibling();
		}
	}

	public void ShowSceneSelector()
	{
		this.canvasScaler.scaleFactor = 1f;
		Shader.SetGlobalFloat("_UIBrightness", 1f);
		Shader.SetGlobalFloat("_UIContrast", 1f);
		this.layoutLang.SetActive(true);
		List<CoreDebug.StartScene> list = Util.EnumToList<CoreDebug.StartScene>();
		Button t = this.layoutLang.CreateMold(null);
		foreach (CoreDebug.StartScene l in list)
		{
			Button button = Util.Instantiate<Button>(t, this.layoutLang);
			CoreDebug.StartScene _l = l;
			button.GetComponentInChildren<Text>().text = l.ToString();
			button.onClick.AddListener(delegate()
			{
				this.layoutLang.SetActive(false);
				ELayer.debug.startScene = _l;
				ELayer.core.Init();
			});
		}
		this.layoutLang.RebuildLayout(false);
	}

	public void ShowLang()
	{
		this.canvasScaler.scaleFactor = 1f;
		Shader.SetGlobalFloat("_UIBrightness", 1f);
		Shader.SetGlobalFloat("_UIContrast", 1f);
		this.layoutLang.SetActive(true);
		Button t = this.layoutLang.CreateMold(null);
		foreach (LangSetting langSetting in MOD.langs.Values)
		{
			Button button = Util.Instantiate<Button>(t, this.layoutLang);
			LangSetting _l = langSetting;
			button.GetComponentInChildren<Text>().text = langSetting.name + " (" + langSetting.name_en + ")";
			button.onClick.AddListener(delegate()
			{
				this.layoutLang.SetActive(false);
				ELayer.core.langCode = _l.id;
				if (ELayer.debug.showSceneSelector || (Input.GetKey(KeyCode.LeftShift) && ELayer.debug.enable))
				{
					this.ShowSceneSelector();
					return;
				}
				ELayer.core.Init();
			});
		}
		this.layoutLang.RebuildLayout(false);
	}

	public void SetLight(bool enable)
	{
	}

	public UIContextMenu CreateContextMenu(string cid = "ContextMenu")
	{
		return this.contextMenu.Create(cid, true);
	}

	public UIContextMenu CreateContextMenuInteraction()
	{
		return this.contextMenu.Create("ContextInteraction", true);
	}

	public void Say(string text, Sprite sprite = null)
	{
		this.popSystem.PopText(text.lang(), sprite, "PopAchievement", default(Color), default(Vector3), 0f);
		Debug.Log(text);
	}

	public void FreezeScreen(float duration)
	{
		if (this.texFreeze)
		{
			UnityEngine.Object.Destroy(this.texFreeze);
		}
		this.texFreeze = ScreenCapture.CaptureScreenshotAsTexture();
		this.imageFreeze.SetActive(true);
		this.imageFreeze.texture = this.texFreeze;
		if (duration != 0f)
		{
			TweenUtil.Tween(duration, null, delegate()
			{
				this.UnfreezeScreen();
			});
		}
	}

	public void UnfreezeScreen()
	{
		if (this.texFreeze)
		{
			UnityEngine.Object.DestroyImmediate(this.texFreeze);
		}
		this.imageFreeze.SetActive(false);
	}

	public void ToggleAbility(bool delay = false)
	{
		if (!ELayer.game.altAbility)
		{
			if (!ELayer.ui.RemoveLayer<LayerAbility>())
			{
				LayerAbility layerAbility = ELayer.ui.AddLayer<LayerAbility>();
				if (delay)
				{
					layerAbility.windows[0].SetRect(ELayer.core.refs.rects.center, false);
					layerAbility.Delay(0.05f);
				}
			}
			return;
		}
		if (ELayer.ui.layerFloat.GetLayer<LayerAbility>(false))
		{
			ELayer.ui.layerFloat.RemoveLayer<LayerAbility>();
			ELayer.player.pref.layerAbility = false;
			SE.Play("pop_ability_deactivate");
			return;
		}
		ELayer.ui.layerFloat.AddLayer<LayerAbility>("LayerAbility/LayerAbilityFloat");
		ELayer.player.pref.layerAbility = true;
	}

	public void ToggleInventory(bool delay = false)
	{
		if (!this.IsInventoryOpen)
		{
			this.OpenFloatInv(false);
			return;
		}
		if (InvOwner.HasTrader)
		{
			SE.Beep();
			return;
		}
		List<Card> list = new List<Card>();
		foreach (LayerInventory layerInventory in LayerInventory.listInv)
		{
			if (layerInventory.IsPlayerContainer(false))
			{
				list.Add(layerInventory.invs[0].owner.Container);
			}
		}
		LayerInventory.listInv.ForeachReverse(delegate(LayerInventory l)
		{
			if (l.IsPlayerContainer(true))
			{
				ELayer.ui.layerFloat.RemoveLayer(l);
			}
		});
		ELayer.ui.widgets.DeactivateWidget("Equip");
		foreach (Card card in list)
		{
			card.c_windowSaveData.open = true;
		}
		SE.Play("pop_inventory_deactivate");
	}

	public void OpenFloatInv(bool ignoreSound = false)
	{
		TooltipManager.Instance.disableTimer = 0.1f;
		if (ignoreSound)
		{
			SoundManager.ignoreSounds = true;
		}
		ELayer.ui.layerFloat.AddLayer(LayerInventory.CreatePCBackpack(false));
		SoundManager.ignoreSounds = true;
		ELayer.ui.widgets.Activate("Equip");
		foreach (Thing thing in ELayer.pc.things.List((Thing a) => a.trait.IsContainer, false))
		{
			Window.SaveData c_windowSaveData = thing.c_windowSaveData;
			if (!(thing.trait is TraitToolBelt) && c_windowSaveData != null && c_windowSaveData.open)
			{
				LayerInventory.CreateContainer(thing);
			}
		}
		SoundManager.ignoreSounds = false;
		TooltipManager.Instance.disableTimer = 0f;
	}

	public void ToggleFeedback()
	{
		string text = "";
		if (Application.isEditor)
		{
			text = "Debug";
		}
		else
		{
			try
			{
				if (SteamAPI.IsSteamRunning())
				{
					text = SteamFriends.GetPersonaName();
					if (text.IsEmpty())
					{
						text = "NULL";
					}
				}
			}
			catch
			{
			}
		}
		if (text.IsEmpty())
		{
			Dialog.Ok("dialog_needToLogOn", null);
			return;
		}
		string text2 = "";
		try
		{
			if (!ELayer.config.rewardCode.IsEmpty())
			{
				text2 = "backer";
				text2 = ElinEncoder.GetID(ELayer.config.rewardCode);
			}
		}
		catch (Exception message)
		{
			text2 = "";
			Debug.Log(message);
		}
		if (!Application.isEditor && text2.IsEmpty())
		{
			string text3 = "public";
			try
			{
				if (!SteamApps.GetCurrentBetaName(out text3, 128) || !(text3 == "nightly"))
				{
					Dialog.Ok("dialog_feedbackTooMany", null);
					return;
				}
				Debug.Log(text3);
			}
			catch
			{
				Dialog.Ok("dialog_feedbackTooMany", null);
				return;
			}
		}
		if (ELayer.core.IsGameStarted && !ELayer.ui.GetLayer<LayerFeedback>(false) && ELayer.pc.HasCondition<ConHallucination>())
		{
			SE.Play("wow");
			Msg.Say("bug_hal");
			return;
		}
		if (!Application.isEditor && (ELayer.debug.enable || (ELayer.core.IsGameStarted && ELayer.player.flags.debugEnabled)))
		{
			Dialog.Ok("dialog_debugFeedback", null);
			return;
		}
		string userName = "Unknown";
		string[] array = Application.persistentDataPath.Split('/', StringSplitOptions.None);
		if (array.Length > 2)
		{
			userName = array[2];
		}
		LayerFeedback.userName = userName;
		LayerFeedback.playedHours = ELayer.config.maxPlayedHours;
		LayerFeedback.backerId = text2;
		LayerFeedback.steamName = text;
		ELayer.ui.ToggleLayer<LayerFeedback>(null);
		SE.Tab();
	}

	public void StartDrag(DragItem item)
	{
		Debug.Log(EInput.leftMouse.down);
		this.dragDuration = 0f;
		if (this.currentDrag != null)
		{
			this.EndDrag(true);
			if (this.currentDrag != null)
			{
				return;
			}
		}
		this.currentDrag = item;
		item.OnStartDrag();
		this.OnDrag();
		ELayer.core.actionsNextFrame.Add(delegate
		{
			TooltipManager.Instance.HideTooltips(true);
		});
	}

	public void OnDrag()
	{
		this.dragDuration += Core.delta;
		this.currentDrag.OnDrag(false, false);
	}

	public void OnDragSpecial()
	{
		if (this.currentDrag == null)
		{
			return;
		}
		if (!this.currentDrag.OnDragSpecial())
		{
			this.EndDrag(true);
		}
	}

	public void EndDrag(bool canceled = false)
	{
		if (this.currentDrag == null)
		{
			return;
		}
		bool flag = this.currentDrag.OnDrag(true, canceled);
		EInput.Consume(false, 1);
		EInput.dragHack = 0f;
		if (flag)
		{
			ELayer.ui.RemoveLayer<LayerRegisterHotbar>();
			this.currentDrag.OnEndDrag();
			this.currentDrag = null;
			if (this.nextDrag != null)
			{
				this.StartDrag(this.nextDrag);
				this.nextDrag = null;
			}
			else
			{
				this.hud.SetDragImage(null, null, null);
			}
			UIButton.TryShowTip(null, true, true);
			return;
		}
		SE.BeepSmall();
	}

	private static float blurSize;

	public Canvas canvas;

	public HUD hud;

	public ExtraHint extraHint;

	public InputModuleEX inputModule;

	public UIContextMenuManager contextMenu;

	public WidgetManager widgets;

	public UIMouseInfo mouseInfo;

	public RectTransform rectDynamic;

	public RectTransform rectDynamicEssential;

	public RectTransform _rectLayers;

	public ReflexConsole console;

	public GameObject blur;

	public Material matBlur;

	public LayoutGroup layoutLang;

	public CanvasScaler canvasScaler;

	public Image light;

	public CanvasGroup cg;

	public int minWidth;

	public Layer layerFloat;

	public UIAutoTurn autoTurn;

	public PopManager popGame;

	public PopManager popSystem;

	public float blurSpeed;

	public Texture2D texFreeze;

	public RawImage imageFreeze;

	public DragItem currentDrag;

	public DragItem nextDrag;

	private bool hidingCover;

	[NonSerialized]
	public float lightContrast;

	[NonSerialized]
	public float dragDuration;

	private Tween tweenCover;

	[NonSerialized]
	public bool wasActive;

	[NonSerialized]
	public bool isPointerOverUI;

	private float durationHideMouseHint;
}
