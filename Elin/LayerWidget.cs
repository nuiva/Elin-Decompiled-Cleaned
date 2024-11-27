using System;
using System.Linq;

public class LayerWidget : ELayer
{
	public override void OnInit()
	{
		ELayer.ui.HideFloats();
		WidgetManager widgets = ELayer.ui.widgets;
		LayerWidget.Instance = this;
		BaseList baseList = this.list;
		BaseList baseList2 = this.list2;
		UIList.Callback<Widget.Config, ItemWidget> callback = new UIList.Callback<Widget.Config, ItemWidget>();
		callback.onClick = delegate(Widget.Config a, ItemWidget b)
		{
		};
		callback.onInstantiate = delegate(Widget.Config a, ItemWidget b)
		{
			b.buttonActivate.mainText.text = Lang.Get("Widget" + a.id);
			b.buttonActivate.subText.SetActive(a.IsSystem);
			b.buttonActivate.onClick.AddListener(delegate()
			{
				if (a.IsSealed)
				{
					SE.Beep();
					return;
				}
				if (a.id == "QuestTracker")
				{
					if (ELayer.ui.widgets.GetWidget("QuestTracker") != null)
					{
						ELayer.player.questTracker = false;
					}
					else
					{
						ELayer.player.questTracker = true;
					}
					WidgetHotbar.RefreshButtons();
				}
				Widget widget2 = widgets.Toggle(a);
				if (widget2 != null)
				{
					widget2.SoundActivate();
				}
				this.Refresh();
			});
			b.buttonLock.onClick.AddListener(delegate()
			{
				widgets.ToggleLock(a);
				this.Refresh();
			});
			b.config = a;
		};
		callback.onRefresh = new Action(this.Refresh);
		UIList.ICallback callbacks = callback;
		baseList2.callbacks = callback;
		baseList.callbacks = callbacks;
		foreach (Widget.Meta meta in ELayer.setting.ui.widgetMetas)
		{
			Widget.Config config = widgets.configs[meta.id];
			if (config.IsInRightMode() && !meta.debugOnly)
			{
				if (config.id.Contains("Hotbar"))
				{
					this.list2.Add(config);
				}
				else
				{
					this.list.Add(config);
				}
			}
		}
		this.list.Refresh(false);
		this.list2.Refresh(false);
		foreach (Widget widget in widgets.list)
		{
			widget.OnManagerActivate();
		}
		this.windows[0].AddBottomSpace(20);
		Action <>9__7;
		this.windows[0].AddBottomButton("resetWidget", delegate
		{
			string langDetail = "dialogResetWidget";
			Action actionYes;
			if ((actionYes = <>9__7) == null)
			{
				actionYes = (<>9__7 = delegate()
				{
					this.Close();
					ELayer.ui.widgets.Load(ELayer.player.useSubWidgetTheme, null);
					ELayer.ui.widgets.Reset(false);
					ELayer.ui.AddLayer<LayerWidget>();
				});
			}
			Dialog.YesNo(langDetail, actionYes, null, "yes", "no");
		}, false);
		Action <>9__8;
		this.windows[0].AddBottomButton("loadTheme", delegate
		{
			WidgetManager widgets = widgets;
			Action onLoad;
			if ((onLoad = <>9__8) == null)
			{
				onLoad = (<>9__8 = delegate()
				{
					this.Close();
					ELayer.ui.AddLayer<LayerWidget>();
				});
			}
			widgets.DialogLoad(onLoad);
		}, false);
		this.windows[0].AddBottomButton("saveTheme", delegate
		{
			widgets.DialogSave(delegate
			{
				Dialog.Ok("dialogSaveTheme", null);
			});
		}, false);
	}

	public override void OnUpdateInput()
	{
		if (EInput.leftMouse.clicked)
		{
			if (EInput.leftMouse.dragging)
			{
				return;
			}
			Widget componentOf = InputModuleEX.GetComponentOf<Widget>();
			if (componentOf)
			{
				componentOf.ShowContextMenu();
			}
		}
	}

	public void Refresh()
	{
		foreach (UIList.ButtonPair buttonPair in this.list.buttons.Concat(this.list2.buttons))
		{
			ItemWidget itemWidget = buttonPair.component as ItemWidget;
			itemWidget.imageLock.SetActive(itemWidget.config.locked);
			itemWidget.imageActive.SetActive(itemWidget.config.state == Widget.State.Active);
			itemWidget.buttonLock.mainText.text = Lang.Get(itemWidget.config.locked ? "unlockWidget" : "lockWidget");
		}
	}

	public override void OnKill()
	{
		foreach (Widget widget in ELayer.ui.widgets.list)
		{
			widget.OnManagerDeactivate();
		}
		ELayer.ui.ShowFloats();
	}

	public static LayerWidget Instance;

	public UIList list;

	public UIList list2;
}
