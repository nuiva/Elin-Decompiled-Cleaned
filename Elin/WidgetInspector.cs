using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class WidgetInspector : Widget
{
	private static PointTarget mouseTarget
	{
		get
		{
			return EMono.scene.mouseTarget;
		}
	}

	public override object CreateExtra()
	{
		return new WidgetInspector.Extra();
	}

	public WidgetInspector.Extra extra
	{
		get
		{
			return base.config.extra as WidgetInspector.Extra;
		}
	}

	public static void OnClickPoint()
	{
	}

	public static void Show()
	{
	}

	public static void Hide()
	{
		if (WidgetInspector.Instance)
		{
			WidgetInspector.Instance.Close();
		}
	}

	public override void OnActivate()
	{
		WidgetInspector.Instance = this;
		this.moldButton = this.layoutButton.CreateMold(null);
		this.moldText = this.layoutLog.CreateMold(null);
	}

	public override void OnDeactivate()
	{
		WidgetInspector.target = null;
	}

	public void OnUpdateInput()
	{
		if (this.selected)
		{
			return;
		}
		if (WidgetInspector.mouseTarget.hasValidTarget)
		{
			this._Show();
			return;
		}
		if (WidgetInspector.target != null)
		{
			WidgetInspector.target = null;
			base.gameObject.SetActive(false);
		}
	}

	public void _Show()
	{
		base.gameObject.SetActive(true);
		if (this.extra.moveToMouse)
		{
			base.transform.position = Input.mousePosition + this.posFix;
			base.ClampToScreen();
		}
		EMono.Sound.Play("pop_inspector");
		this.SwitchPage(0);
	}

	public void SwitchPage(int _index)
	{
		this.index = _index;
		WidgetInspector.target = this.list[this.index];
		this.layoutButton.DestroyChildren(false, true);
		this.layoutLog.DestroyChildren(false, true);
		this.texts.Clear();
		string name;
		if (WidgetInspector.target is Chara)
		{
			Chara c = WidgetInspector.target as Chara;
			name = c.Name;
			c.SetImage(this.iconCard);
			if (c.IsHomeMember())
			{
				this.AddButton("detail", delegate
				{
					EMono.ui.AddLayer<LayerChara>().SetChara(c);
				});
			}
		}
		else if (WidgetInspector.target is Thing)
		{
			Thing t = WidgetInspector.target as Thing;
			name = t.Name;
			t.SetImage(this.iconCard);
			this.AddButton("detail", delegate
			{
				EMono.ui.AddLayer<LayerInfo>().Set(t, false);
			});
		}
		else
		{
			name = (WidgetInspector.target as Area).Name;
		}
		this.iconCard.rectTransform.pivot = new Vector2(1f, 0f);
		this.iconCard.SetActive(WidgetInspector.target is Card);
		this.iconArea.SetActive(WidgetInspector.target is Area);
		this.AddLog(WidgetInspector.target.ToString(), default(Color));
		this.textTitle.SetText(name);
		this.Refresh();
		this.RebuildLayout(true);
	}

	public void Refresh()
	{
		if (WidgetInspector.target is Chara)
		{
			object obj = WidgetInspector.target;
		}
	}

	public void AddLog(string text, Color c = default(Color))
	{
		UIText uitext = Util.Instantiate<UIText>(this.moldText, this.layoutLog);
		this.texts.Add(uitext);
		uitext.SetText(text);
		if (this.texts.Count > this.maxLog)
		{
			UnityEngine.Object.DestroyImmediate(this.texts[0].gameObject);
			this.texts.Remove(this.texts[0]);
		}
	}

	public void AddButton(string id = "test", Action action = null)
	{
		UIButton uibutton = Util.Instantiate<UIButton>(this.moldButton, this.layoutButton);
		uibutton.icon.sprite = (SpriteSheet.Get("icon_" + id) ?? uibutton.icon.sprite);
		uibutton.mainText.SetText(id.lang());
		if (action != null)
		{
			uibutton.onClick.AddListener(delegate()
			{
				action();
			});
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("setting").AddToggle("moveToMouse", this.extra.moveToMouse, delegate(bool a)
		{
			this.extra.moveToMouse = a;
		});
		m.AddChild("style").AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", (float)base.config.skin.button, delegate(float a)
		{
			base.config.skin.button = (int)a;
			this.ApplySkin();
		}, 0f, (float)(base.config.skin.Skin.buttons.Count - 1), true, true, false);
		base.SetBaseContextMenu(m);
	}

	public static WidgetInspector Instance;

	public static object target;

	public LayoutGroup layoutButton;

	public LayoutGroup layoutLog;

	public UIText textTitle;

	public Image iconCard;

	public Image iconArea;

	public int maxLog;

	public int index;

	public Vector3 posFix;

	public List<object> list;

	public Sprite spriteArea;

	public bool selected;

	private List<UIText> texts = new List<UIText>();

	private UIButton moldButton;

	private UIText moldText;

	public class Extra
	{
		[JsonProperty]
		public bool moveToMouse;
	}
}
