using System;
using UnityEngine;
using UnityEngine.UI;

public class ActionHint : EMono
{
	public ActionMode mode
	{
		get
		{
			return EMono.scene.actionMode;
		}
	}

	private BaseTileSelector ts
	{
		get
		{
			return EMono.screen.tileSelector;
		}
	}

	private void Awake()
	{
		this.moldDynamic = this.layoutDynamic.CreateMold(null);
		this.oriPos = this.Rect().anchoredPosition;
	}

	public void Refresh()
	{
		if (this.mode == null)
		{
			return;
		}
		bool flag = EMono.core.IsGameStarted && this.mode.ShowActionHint;
		base.gameObject.SetActive(flag);
		if (!flag)
		{
			return;
		}
		this.extra.SetActive(true);
		bool digRamp = this.mode == ActionMode.Dig && ActionMode.Dig.mode == TaskDig.Mode.Ramp;
		bool mineRamp = this.mode == ActionMode.Mine && ActionMode.Mine.mode == TaskMine.Mode.Ramp;
		this.groupRadio.SetActive(digRamp | mineRamp);
		if (digRamp | mineRamp)
		{
			this.groupRadio.Init(0, delegate(int a)
			{
				if (digRamp)
				{
					if (a == 0)
					{
						ActionMode.Dig.ramp = 3;
					}
					if (a == 1)
					{
						ActionMode.Dig.ramp = 4;
					}
					if (a == 2)
					{
						ActionMode.Dig.ramp = 5;
					}
					ActionMode.Dig.OnCreateMold(false);
				}
				if (mineRamp)
				{
					if (a == 0)
					{
						ActionMode.Mine.ramp = 3;
					}
					if (a == 1)
					{
						ActionMode.Mine.ramp = 4;
					}
					if (a == 2)
					{
						ActionMode.Mine.ramp = 5;
					}
					ActionMode.Mine.OnCreateMold(false);
				}
			}, false);
			this.groupRadio.ToggleInteractable(true);
		}
		this.dynamic = false;
		this.layoutDynamic.DestroyChildren(false, true);
		this.groupToggle.SetActive(this.dynamic);
		this.UpdateText();
	}

	public void AddToggle(string lang, bool on, Action<bool> action)
	{
		UIButton uibutton = Util.Instantiate<UIButton>(this.moldDynamic, this.layoutDynamic);
		uibutton.mainText.SetText(lang.lang());
		uibutton.SetToggle(on, action);
		this.dynamic = true;
	}

	public void UpdateText()
	{
		string hintText = this.mode.GetHintText();
		base.gameObject.SetActive(EMono.scene.actionMode.ShowActionHint && !hintText.IsEmpty());
		this.SetText(hintText, false);
		this.buttonPick.SetActive(this.mode == ActionMode.Inspect && ActionMode.Inspect.CanPutAway());
		this.buttonPick.SetOnClick(delegate
		{
			ActionMode.Inspect.TryPutAway();
		});
	}

	public void Show(string lang, bool icon = true)
	{
		string text = lang.lang();
		bool anime = text != this.lastText || !base.gameObject.activeSelf;
		this.lastText = text;
		this.iconNerun.SetActive(false);
		base.gameObject.SetActive(true);
		this.buttonPick.SetActive(false);
		this.extra.SetActive(false);
		this.SetText(text, anime);
	}

	public void SetText(string s, bool anime = false)
	{
		this.Rect().anchoredPosition = this.oriPos;
		this.textTitle.SetText(s);
		this.RebuildLayout(false);
		if (anime)
		{
			this.animePop.Play(base.transform, null, -1f, 0f);
		}
	}

	public Transform extra;

	public LayoutGroup layoutDynamic;

	public UIText textTitle;

	public UIText textDetail;

	public UIButton buttonPick;

	public UIButton buttonSavePartialMap;

	public UIButton buttonDeletePartialMap;

	public UISelectableGroup groupRadio;

	public UISelectableGroup groupToggle;

	public Anime animePop;

	public Image iconNerun;

	public CanvasGroup cg;

	[NonSerialized]
	public bool dynamic;

	private UIButton moldDynamic;

	private Vector3 oriPos;

	private string lastText;
}
