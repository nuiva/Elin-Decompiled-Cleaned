using System.Linq;

public class LayerWidget : ELayer
{
	public static LayerWidget Instance;

	public UIList list;

	public UIList list2;

	public override void OnInit()
	{
		ELayer.ui.HideFloats();
		WidgetManager widgets = ELayer.ui.widgets;
		Instance = this;
		UIList uIList = list;
		UIList uIList2 = list2;
		UIList.Callback<Widget.Config, ItemWidget> obj = new UIList.Callback<Widget.Config, ItemWidget>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(Widget.Config a, ItemWidget b)
			{
				b.buttonActivate.mainText.text = Lang.Get("Widget" + a.id);
				b.buttonActivate.subText.SetActive(a.IsSystem);
				b.buttonActivate.onClick.AddListener(delegate
				{
					if (a.IsSealed)
					{
						SE.Beep();
					}
					else
					{
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
						widgets.Toggle(a)?.SoundActivate();
						Refresh();
					}
				});
				b.buttonLock.onClick.AddListener(delegate
				{
					widgets.ToggleLock(a);
					Refresh();
				});
				b.config = a;
			},
			onRefresh = Refresh
		};
		UIList.ICallback callbacks = obj;
		uIList2.callbacks = obj;
		uIList.callbacks = callbacks;
		foreach (Widget.Meta widgetMeta in ELayer.setting.ui.widgetMetas)
		{
			Widget.Config config = widgets.configs[widgetMeta.id];
			if (config.IsInRightMode() && !widgetMeta.debugOnly)
			{
				if (config.id.Contains("Hotbar"))
				{
					list2.Add(config);
				}
				else
				{
					list.Add(config);
				}
			}
		}
		list.Refresh();
		list2.Refresh();
		foreach (Widget item in widgets.list)
		{
			item.OnManagerActivate();
		}
		windows[0].AddBottomSpace();
		windows[0].AddBottomButton("resetWidget", delegate
		{
			Dialog.YesNo("dialogResetWidget", delegate
			{
				Close();
				ELayer.ui.widgets.Load(ELayer.player.useSubWidgetTheme);
				ELayer.ui.widgets.Reset(toggleTheme: false);
				ELayer.ui.AddLayer<LayerWidget>();
			});
		});
		windows[0].AddBottomButton("loadTheme", delegate
		{
			widgets.DialogLoad(delegate
			{
				Close();
				ELayer.ui.AddLayer<LayerWidget>();
			});
		});
		windows[0].AddBottomButton("saveTheme", delegate
		{
			widgets.DialogSave(delegate
			{
				Dialog.Ok("dialogSaveTheme");
			});
		});
	}

	public override void OnUpdateInput()
	{
		if (EInput.leftMouse.clicked && !EInput.leftMouse.dragging)
		{
			Widget componentOf = InputModuleEX.GetComponentOf<Widget>();
			if ((bool)componentOf)
			{
				componentOf.ShowContextMenu();
			}
		}
	}

	public void Refresh()
	{
		foreach (UIList.ButtonPair item in list.buttons.Concat(list2.buttons))
		{
			ItemWidget itemWidget = item.component as ItemWidget;
			itemWidget.imageLock.SetActive(itemWidget.config.locked);
			itemWidget.imageActive.SetActive(itemWidget.config.state == Widget.State.Active);
			itemWidget.buttonLock.mainText.text = Lang.Get(itemWidget.config.locked ? "unlockWidget" : "lockWidget");
		}
	}

	public override void OnKill()
	{
		foreach (Widget item in ELayer.ui.widgets.list)
		{
			item.OnManagerDeactivate();
		}
		ELayer.ui.ShowFloats();
	}
}
