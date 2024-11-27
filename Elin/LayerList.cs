using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LayerList : ELayer
{
	protected override void Awake()
	{
		base.Awake();
		if (this.buttonReroll)
		{
			this.buttonReroll.SetActive(false);
		}
	}

	public override void OnInit()
	{
		if (this.highlightTarget)
		{
			this.highlightTarget.DoHighlightTransition(false);
		}
		EInput.WaitReleaseKey();
	}

	public override void OnUpdateInput()
	{
		foreach (UIList.ButtonPair buttonPair in this.list.buttons)
		{
			ItemGeneral itemGeneral = buttonPair.component as ItemGeneral;
			UIButton uibutton = itemGeneral ? itemGeneral.button1 : (buttonPair.component as UIButton);
			if (uibutton && uibutton.interactable && !EInput.waitReleaseAnyKey && uibutton.keyText && !uibutton.keyText.text.IsEmpty() && uibutton.keyText.text == Input.inputString)
			{
				SE.ClickOk();
				uibutton.onClick.Invoke();
				return;
			}
		}
		base.OnUpdateInput();
	}

	public LayerList SetMold(int index)
	{
		if (index != 0)
		{
			if (index == 1)
			{
				this.list.moldItem = this.moldItemDetail;
			}
		}
		else
		{
			this.list.moldItem = this.moldItemGeneral;
		}
		return this;
	}

	public LayerList SetSize(float w = 450f, float h = -1f)
	{
		Vector2 sizeDelta = this.windows[0].Rect().sizeDelta;
		this.windows[0].Rect().sizeDelta = new Vector2((w == -1f) ? sizeDelta.x : w, (h == -1f) ? sizeDelta.y : h);
		if (w != -1f)
		{
			this.autoX = false;
		}
		if (h != -1f)
		{
			this.autoY = false;
		}
		return this;
	}

	public LayerList SetPivot(float x, float y = -1f)
	{
		this.windows[0].Rect().pivot = new Vector2(x, y);
		return this;
	}

	public void RefreshSize()
	{
		RectTransform rectTransform = this.windows[0].Rect();
		float x = rectTransform.sizeDelta.x;
		float y = rectTransform.sizeDelta.y;
		this.RebuildLayout(true);
		Vector2 sizeDelta = this.scroll.content.Rect().sizeDelta;
		if (this.autoX)
		{
			x = Mathf.Clamp(sizeDelta.x, this.sizeMin.x, this.sizeMax.x) + this.paddings.x;
		}
		if (this.autoY)
		{
			y = Mathf.Clamp(sizeDelta.y, this.sizeMin.y, this.sizeMax.y) + this.paddings.y;
		}
		rectTransform.sizeDelta = new Vector2(x, y);
	}

	public LayerList SetList<TValue>(ICollection<TValue> items, Func<TValue, string> getString, Action<int, string> onSelect, bool autoClose = true)
	{
		List<string> strs = new List<string>();
		foreach (TValue arg in items)
		{
			strs.Add(getString(arg));
		}
		this.SetStringList(() => strs, onSelect, autoClose);
		return this;
	}

	public LayerList SetStringList(Func<ICollection<string>> getList, Action<int, string> onSelect, bool autoClose = true)
	{
		this.list.callbacks = new UIList.Callback<string, ItemGeneral>
		{
			onClick = delegate(string a, ItemGeneral b)
			{
				onSelect(this.list.items.IndexOf(a), a);
				if (autoClose)
				{
					this.Close();
				}
			},
			onInstantiate = delegate(string a, ItemGeneral item)
			{
				item.button1.mainText.text = a;
				item.DisableIcon();
				item.Build();
				if (this.noSound)
				{
					item.button1.soundClick = null;
				}
			},
			onList = delegate(UIList.SortMode m)
			{
				foreach (string o in getList())
				{
					this.list.Add(o);
				}
			}
		};
		this.list.List(false);
		this.RefreshSize();
		return this;
	}

	public LayerList SetList2<TValue>(ICollection<TValue> _list, Func<TValue, string> getText, Action<TValue, ItemGeneral> onClick, Action<TValue, ItemGeneral> onInstantiate, bool autoClose = true)
	{
		this.list.callbacks = new UIList.Callback<TValue, ItemGeneral>
		{
			onClick = delegate(TValue a, ItemGeneral b)
			{
				onClick(a, b);
				if (autoClose)
				{
					this.Close();
				}
			},
			onInstantiate = delegate(TValue a, ItemGeneral item)
			{
				item.button1.mainText.text = getText(a);
				item.DisableIcon();
				item.Build();
				if (this.noSound)
				{
					item.button1.soundClick = null;
				}
				if (onInstantiate != null)
				{
					onInstantiate(a, item);
				}
			},
			onList = delegate(UIList.SortMode m)
			{
				foreach (TValue tvalue in _list)
				{
					this.list.Add(tvalue);
				}
			}
		};
		this.list.List(false);
		this.RefreshSize();
		UIButton.TryShowTip(base.transform, true, true);
		return this;
	}

	public LayerList SetListCheck<TValue>(ICollection<TValue> _list, Func<TValue, string> getText, Action<TValue, ItemGeneral> onClick, Action<List<UIList.ButtonPair>> onValidate)
	{
		LayerList.<>c__DisplayClass24_0<TValue> CS$<>8__locals1 = new LayerList.<>c__DisplayClass24_0<TValue>();
		CS$<>8__locals1.onValidate = onValidate;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.getText = getText;
		CS$<>8__locals1.onClick = onClick;
		CS$<>8__locals1._list = _list;
		this.list.moldItem = this.moldItemCheck.transform;
		BaseList baseList = this.list;
		UIList.Callback<TValue, ItemGeneral> callback = new UIList.Callback<TValue, ItemGeneral>();
		callback.onClick = delegate(TValue a, ItemGeneral b)
		{
		};
		callback.onInstantiate = delegate(TValue a, ItemGeneral item)
		{
			item.button1.mainText.text = CS$<>8__locals1.getText(a);
			item.DisableIcon();
			item.Build();
			item.button1.SetOnClick(delegate
			{
				CS$<>8__locals1.onClick(a, item);
				CS$<>8__locals1.<SetListCheck>g__Validate|0();
			});
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (TValue tvalue in CS$<>8__locals1._list)
			{
				CS$<>8__locals1.<>4__this.list.Add(tvalue);
			}
		};
		baseList.callbacks = callback;
		this.list.List(false);
		CS$<>8__locals1.<SetListCheck>g__Validate|0();
		this.RefreshSize();
		UIButton.TryShowTip(base.transform, true, true);
		return this;
	}

	public LayerList EnableReroll()
	{
		this.buttonReroll.SetActive(true);
		this.buttonReroll.onClick.RemoveAllListeners();
		this.buttonReroll.onClick.AddListener(delegate()
		{
			this.list.List(false);
		});
		this.list.RebuildLayout(true);
		this.RefreshSize();
		return this;
	}

	public LayerList ManualList(Action<UIList, LayerList> onInit)
	{
		this.buttonReroll.SetActive(false);
		this.list.moldItem = this.moldItemGeneral;
		onInit(this.list, this);
		this.list.List(false);
		this.RefreshSize();
		return this;
	}

	public LayerList SetHeader(string lang)
	{
		this.windows[0].SetCaption(lang.lang());
		return this;
	}

	public LayerList SetNoSound()
	{
		this.noSound = true;
		return this;
	}

	public void SetHighlightTarget(UIButton _target)
	{
		this.highlightTarget = _target;
		this.highlightTarget.DoHighlightTransition(false);
	}

	private void Update()
	{
		if (this.highlightTarget)
		{
			this.highlightTarget.DoHighlightTransition(false);
		}
	}

	private void LateUpdate()
	{
		if (this.highlightTarget)
		{
			this.highlightTarget.DoHighlightTransition(false);
		}
	}

	public override void OnKill()
	{
		if (this.highlightTarget)
		{
			ELayer.core.WaitForEndOfFrame(delegate
			{
				if (this.highlightTarget)
				{
					if (!InputModuleEX.IsPointerOver(this.highlightTarget))
					{
						this.highlightTarget.DoNormalTransition(true);
						return;
					}
					this.highlightTarget.DoHighlightTransition(false);
				}
			});
		}
		TooltipManager.Instance.HideTooltips(true);
		TooltipManager.Instance.disableHide = null;
		EInput.WaitReleaseKey();
	}

	public void Add(string lang, Action<int> action)
	{
		if (!this.initialized)
		{
			this.initialized = true;
		}
		this.customItems.Add(new LayerList.CustomItem
		{
			lang = lang,
			action = action,
			id = this.customItems.Count
		});
	}

	public void Show(bool autoClose = true)
	{
		BaseList baseList = this.list;
		UIList.Callback<LayerList.CustomItem, ItemGeneral> callback = new UIList.Callback<LayerList.CustomItem, ItemGeneral>();
		callback.onClick = delegate(LayerList.CustomItem a, ItemGeneral b)
		{
			a.action(a.id);
			if (autoClose)
			{
				this.Close();
			}
		};
		callback.onInstantiate = delegate(LayerList.CustomItem a, ItemGeneral item)
		{
			item.button1.mainText.text = a.lang.lang();
			item.DisableIcon();
			item.Build();
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			foreach (LayerList.CustomItem o in this.customItems)
			{
				this.list.Add(o);
			}
		};
		baseList.callbacks = callback;
		this.buttonReroll.SetActive(false);
		this.list.List(false);
		this.RefreshSize();
		if (!autoClose)
		{
			this.windows[0].AddBottomButton("back", new UnityAction(this.Close), true);
		}
	}

	public UIList list;

	public UIButton buttonReroll;

	public UIButton highlightTarget;

	public UIItem moldItemCheck;

	public Transform moldItemGeneral;

	public Transform moldItemDetail;

	public bool useItem;

	public bool autoX;

	public bool autoY;

	public bool noSound;

	public UIScrollView scroll;

	public Vector2 sizeMin;

	public Vector2 sizeMax;

	public Vector2 paddings;

	private bool initialized;

	public List<LayerList.CustomItem> customItems = new List<LayerList.CustomItem>();

	public class CustomItem
	{
		public string lang;

		public Action<int> action;

		public int id;
	}
}
