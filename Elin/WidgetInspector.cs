using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class WidgetInspector : Widget
{
	public class Extra
	{
		[JsonProperty]
		public bool moveToMouse;
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

	private static PointTarget mouseTarget => EMono.scene.mouseTarget;

	public Extra extra => base.config.extra as Extra;

	public override object CreateExtra()
	{
		return new Extra();
	}

	public static void OnClickPoint()
	{
	}

	public static void Show()
	{
	}

	public static void Hide()
	{
		if ((bool)Instance)
		{
			Instance.Close();
		}
	}

	public override void OnActivate()
	{
		Instance = this;
		moldButton = layoutButton.CreateMold<UIButton>();
		moldText = layoutLog.CreateMold<UIText>();
	}

	public override void OnDeactivate()
	{
		target = null;
	}

	public void OnUpdateInput()
	{
		if (!selected)
		{
			if (mouseTarget.hasValidTarget)
			{
				_Show();
			}
			else if (target != null)
			{
				target = null;
				base.gameObject.SetActive(value: false);
			}
		}
	}

	public void _Show()
	{
		base.gameObject.SetActive(value: true);
		if (extra.moveToMouse)
		{
			base.transform.position = Input.mousePosition + posFix;
			ClampToScreen();
		}
		EMono.Sound.Play("pop_inspector");
		SwitchPage(0);
	}

	public void SwitchPage(int _index)
	{
		index = _index;
		target = list[index];
		string text = "";
		layoutButton.DestroyChildren();
		layoutLog.DestroyChildren();
		texts.Clear();
		if (target is Chara)
		{
			Chara c = target as Chara;
			text = c.Name;
			c.SetImage(iconCard);
			if (c.IsHomeMember())
			{
				AddButton("detail", delegate
				{
					EMono.ui.AddLayer<LayerChara>().SetChara(c);
				});
			}
		}
		else if (target is Thing)
		{
			Thing t = target as Thing;
			text = t.Name;
			t.SetImage(iconCard);
			AddButton("detail", delegate
			{
				EMono.ui.AddLayer<LayerInfo>().Set(t);
			});
		}
		else
		{
			text = (target as Area).Name;
		}
		iconCard.rectTransform.pivot = new Vector2(1f, 0f);
		iconCard.SetActive(target is Card);
		iconArea.SetActive(target is Area);
		AddLog(target.ToString());
		textTitle.SetText(text);
		Refresh();
		this.RebuildLayout(recursive: true);
	}

	public void Refresh()
	{
		if (target is Chara)
		{
			_ = target;
		}
	}

	public void AddLog(string text, Color c = default(Color))
	{
		UIText uIText = Util.Instantiate(moldText, layoutLog);
		texts.Add(uIText);
		uIText.SetText(text);
		if (texts.Count > maxLog)
		{
			UnityEngine.Object.DestroyImmediate(texts[0].gameObject);
			texts.Remove(texts[0]);
		}
	}

	public void AddButton(string id = "test", Action action = null)
	{
		UIButton uIButton = Util.Instantiate(moldButton, layoutButton);
		uIButton.icon.sprite = SpriteSheet.Get("icon_" + id) ?? uIButton.icon.sprite;
		uIButton.mainText.SetText(id.lang());
		if (action != null)
		{
			uIButton.onClick.AddListener(delegate
			{
				action();
			});
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("setting").AddToggle("moveToMouse", extra.moveToMouse, delegate(bool a)
		{
			extra.moveToMouse = a;
		});
		m.AddChild("style").AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", base.config.skin.button, delegate(float a)
		{
			base.config.skin.button = (int)a;
			ApplySkin();
		}, 0f, base.config.skin.Skin.buttons.Count - 1, isInt: true);
		SetBaseContextMenu(m);
	}
}
