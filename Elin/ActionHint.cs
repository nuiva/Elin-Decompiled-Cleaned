using System;
using UnityEngine;
using UnityEngine.UI;

public class ActionHint : EMono
{
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

	public ActionMode mode => EMono.scene.actionMode;

	private BaseTileSelector ts => EMono.screen.tileSelector;

	private void Awake()
	{
		moldDynamic = layoutDynamic.CreateMold<UIButton>();
		oriPos = this.Rect().anchoredPosition;
	}

	public void Refresh()
	{
		if (mode == null)
		{
			return;
		}
		bool flag = EMono.core.IsGameStarted && mode.ShowActionHint;
		base.gameObject.SetActive(flag);
		if (!flag)
		{
			return;
		}
		extra.SetActive(enable: true);
		bool digRamp = mode == ActionMode.Dig && ActionMode.Dig.mode == TaskDig.Mode.Ramp;
		bool mineRamp = mode == ActionMode.Mine && ActionMode.Mine.mode == TaskMine.Mode.Ramp;
		groupRadio.SetActive(digRamp || mineRamp);
		if (digRamp || mineRamp)
		{
			groupRadio.Init(0, delegate(int a)
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
					ActionMode.Dig.OnCreateMold();
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
					ActionMode.Mine.OnCreateMold();
				}
			});
			groupRadio.ToggleInteractable(enable: true);
		}
		dynamic = false;
		layoutDynamic.DestroyChildren();
		groupToggle.SetActive(dynamic);
		UpdateText();
	}

	public void AddToggle(string lang, bool on, Action<bool> action)
	{
		UIButton uIButton = Util.Instantiate(moldDynamic, layoutDynamic);
		uIButton.mainText.SetText(lang.lang());
		uIButton.SetToggle(on, action);
		dynamic = true;
	}

	public void UpdateText()
	{
		string hintText = mode.GetHintText();
		base.gameObject.SetActive(EMono.scene.actionMode.ShowActionHint && !hintText.IsEmpty());
		SetText(hintText);
		buttonPick.SetActive(mode == ActionMode.Inspect && ActionMode.Inspect.CanPutAway());
		buttonPick.SetOnClick(delegate
		{
			ActionMode.Inspect.TryPutAway();
		});
	}

	public void Show(string lang, bool icon = true)
	{
		string text = lang.lang();
		bool anime = text != lastText || !base.gameObject.activeSelf;
		lastText = text;
		iconNerun.SetActive(enable: false);
		base.gameObject.SetActive(value: true);
		buttonPick.SetActive(enable: false);
		extra.SetActive(enable: false);
		SetText(text, anime);
	}

	public void SetText(string s, bool anime = false)
	{
		this.Rect().anchoredPosition = oriPos;
		textTitle.SetText(s);
		this.RebuildLayout();
		if (anime)
		{
			animePop.Play(base.transform);
		}
	}
}
