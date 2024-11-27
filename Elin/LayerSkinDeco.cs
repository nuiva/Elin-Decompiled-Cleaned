using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerSkinDeco : ELayer
{
	public List<SkinDeco> decos
	{
		get
		{
			return this.widget.config.skin.decos;
		}
	}

	public SkinConfig cfg
	{
		get
		{
			return this.widget.config.skin;
		}
	}

	public override void OnInit()
	{
		base.OnInit();
	}

	public void SetWidget(Widget _widget)
	{
		this.widget = _widget;
		foreach (SkinDeco skinDeco in this.decos)
		{
			this.Activate(skinDeco.actor);
		}
		this.widget.transform.SetParent(this.widgetHolder, false);
	}

	public override void OnUpdateInput()
	{
		if (EInput.leftMouse.clicked)
		{
			if (EInput.leftMouse.dragging)
			{
				return;
			}
			SkinDecoActor selected = InputModuleEX.GetComponentOf<SkinDecoActor>();
			Vector3 clickPos = Input.mousePosition;
			UIContextMenu m = ELayer.ui.CreateContextMenu("ContextMenu");
			if (selected)
			{
				SkinDeco deco = selected.owner;
				Action<PickerState, Color> <>9__9;
				m.AddButton("editColor", delegate()
				{
					LayerColorPicker layerColorPicker = ELayer.ui.AddLayer<LayerColorPicker>();
					Color color = deco.color;
					Color white = Color.white;
					Action<PickerState, Color> onChangeColor;
					if ((onChangeColor = <>9__9) == null)
					{
						onChangeColor = (<>9__9 = delegate(PickerState state, Color _c)
						{
							deco.color = _c;
							selected.Refresh();
						});
					}
					layerColorPicker.SetColor(color, white, onChangeColor);
				}, true);
				m.AddButton("bringToTop", delegate()
				{
					this.decos.Remove(deco);
					this.decos.Add(deco);
					selected.transform.SetAsLastSibling();
				}, true);
				m.AddSlider("rotation", (float n) => (n * 45f).ToString() ?? "", (float)deco.rz, delegate(float a)
				{
					deco.rz = (int)a;
					selected.Refresh();
				}, 0f, 7f, true, true, false);
				m.AddSlider("size", (float n) => n.ToString() ?? "", (float)Mathf.Abs(deco.sx), delegate(float a)
				{
					deco.sx = (int)a * ((deco.sx > 0) ? 1 : -1);
					deco.sy = (int)a * ((deco.sy > 0) ? 1 : -1);
					selected.Refresh();
				}, 10f, 400f, true, true, false);
				m.AddToggle("shadow", deco.shadow, delegate(bool a)
				{
					deco.shadow = a;
					selected.Refresh();
				});
				m.AddToggle("reverse", deco.reverse, delegate(bool a)
				{
					deco.reverse = a;
					selected.Refresh();
				});
				m.AddButton("removeDeco", delegate()
				{
					this.widget.RemoveDeco(selected.owner);
				}, true);
			}
			else
			{
				UIContextMenu parent = m.AddChild("addDeco");
				UIList uilist = Util.Instantiate<UIList>("UI/Element/List/ListImageGrid", parent);
				BaseList baseList = uilist;
				UIList.Callback<Sprite, UIButton> callback = new UIList.Callback<Sprite, UIButton>();
				callback.onInstantiate = delegate(Sprite a, UIButton _b)
				{
					_b.icon.sprite = a;
				};
				callback.onClick = delegate(Sprite a, UIButton _b)
				{
					SkinDeco skinDeco = new SkinDeco
					{
						sx = 100,
						sy = 100,
						color = Color.white
					};
					skinDeco.id = int.Parse(a.name.Remove(0, 4));
					this.widget.AddDeco(skinDeco);
					SkinDecoActor actor = skinDeco.actor;
					this.Activate(actor);
					actor.transform.position = clickPos;
					EInput.Consume(false, 1);
					m.Hide();
				};
				baseList.callbacks = callback;
				foreach (Sprite o in Resources.LoadAll<Sprite>("Media/Graphics/Deco/"))
				{
					uilist.Add(o);
				}
				uilist.Refresh(false);
			}
			m.Show();
		}
		if (EInput.rightMouse.down)
		{
			UIContextMenu uicontextMenu = ELayer.ui.CreateContextMenu("ContextMenu");
			uicontextMenu.AddButton("quitEdit", delegate()
			{
				this.Close();
			}, true);
			uicontextMenu.Show();
		}
	}

	public void Activate(SkinDecoActor actor)
	{
		actor.image.raycastTarget = true;
		actor.transform.SetParent(base.transform, true);
		UIDragPanel uidragPanel = actor.gameObject.AddComponent<UIDragPanel>();
		uidragPanel.target = actor.Rect();
		uidragPanel.bound = actor.Rect();
		uidragPanel.clamp = false;
	}

	public override void OnKill()
	{
		this.widget.transform.SetParent(ELayer.ui.widgets.transform, false);
		foreach (SkinDeco skinDeco in this.decos)
		{
			SkinDecoActor actor = skinDeco.actor;
			actor.image.raycastTarget = false;
			UnityEngine.Object.DestroyImmediate(actor.gameObject.GetComponent<UIDragPanel>());
			actor.transform.SetParent(this.widget.transform, true);
			skinDeco.x = (int)actor.Rect().anchoredPosition.x;
			skinDeco.y = (int)actor.Rect().anchoredPosition.y;
		}
	}

	[NonSerialized]
	public Widget widget;

	public Transform widgetHolder;
}
