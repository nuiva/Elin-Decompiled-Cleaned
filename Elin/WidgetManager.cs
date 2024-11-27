using System;
using System.Collections.Generic;
using System.Linq;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class WidgetManager : EMono
{
	public List<Widget.Meta> metas
	{
		get
		{
			return EMono.setting.ui.widgetMetas;
		}
	}

	public Dictionary<string, Widget.Config> configs
	{
		get
		{
			return EMono.player.widgets.dict;
		}
	}

	public void OnActivateZone()
	{
		if (!this.first)
		{
			return;
		}
		this.first = false;
		foreach (Widget.Config config in this.configs.Values)
		{
			if (config.state == Widget.State.Active)
			{
				this.ActivateWidget(config);
			}
		}
		bool flag = EMono.ui.layerFloat.GetLayer<LayerInventory>(false);
		if (this.GetWidget("Equip") == null)
		{
			if (flag)
			{
				this.Activate("Equip");
				return;
			}
		}
		else if (!flag)
		{
			this.DeactivateWidget("Equip");
		}
	}

	public void OnGameInstantiated()
	{
		if (Application.isEditor && EMono.debug.resetPlayerConfig && !EMono.player.isEditor)
		{
			EMono.player.useSubWidgetTheme = false;
			EMono.ui.widgets.Load(false, null);
		}
		this.first = true;
		if (EMono.player.useSubWidgetTheme)
		{
			if (EMono.player.subWidgets == null)
			{
				this.Load(true, null);
			}
		}
		else if (EMono.player.mainWidgets == null)
		{
			this.Load(false, null);
			if (Screen.width <= 1300 && !EMono.player.useSubWidgetTheme)
			{
				foreach (KeyValuePair<string, Widget.Config> keyValuePair in EMono.player.mainWidgets.dict)
				{
					if (keyValuePair.Key == "StatsBar")
					{
						keyValuePair.Value.state = Widget.State.Inactive;
					}
				}
			}
		}
		if (this.metaMap.Count == 0)
		{
			foreach (Widget.Meta meta in this.metas)
			{
				this.metaMap.Add(meta.id, meta);
			}
		}
		foreach (Widget.Meta meta2 in this.metas)
		{
			if (!this.configs.ContainsKey(meta2.id))
			{
				Widget.Config config = new Widget.Config
				{
					meta = meta2,
					valid = true,
					id = meta2.id,
					state = (meta2.enabled ? Widget.State.Active : Widget.State.Inactive),
					locked = meta2.locked
				};
				config.skin.SetID(0);
				this.configs.Add(meta2.id, config);
			}
			else
			{
				Widget.Config config2 = this.configs[meta2.id];
				config2.valid = true;
				config2.meta = meta2;
			}
		}
		foreach (Widget.Config config3 in this.configs.Values.ToList<Widget.Config>())
		{
			if (!config3.valid)
			{
				this.configs.Remove(config3.id);
			}
		}
	}

	public void OnKillGame()
	{
		this.KillWidgets();
	}

	public void OnChangeActionMode()
	{
		foreach (Widget widget in this.list)
		{
			widget.OnChangeActionMode();
		}
	}

	public void UpdateConfigs()
	{
		foreach (Widget widget in this.list)
		{
			widget.UpdateConfig();
		}
	}

	public void Activate(string id)
	{
		if (!this.GetWidget(id))
		{
			this.ActivateWidget(this.configs[id]);
		}
	}

	public Widget Toggle(string id)
	{
		Widget widget = this.GetWidget(id);
		if (widget)
		{
			this.DeactivateWidget(widget);
			return null;
		}
		return this.ActivateWidget(this.configs[id]);
	}

	public Widget Toggle(Widget.Config c)
	{
		Widget widget = this.GetWidget(c.id);
		if (widget)
		{
			this.DeactivateWidget(widget);
			return null;
		}
		return this.ActivateWidget(c);
	}

	public void ToggleLock(Widget.Config c)
	{
		c.locked = !c.locked;
		this.RefreshWidget(c);
	}

	public Widget ActivateWidget(Widget.Config c)
	{
		return this.ActivateWidget(c.id);
	}

	public Widget ActivateWidget(string id)
	{
		string text = "Widget" + id;
		Widget widget = Util.Instantiate<Widget>("UI/Widget/" + text, this) ?? Util.Instantiate<Widget>("UI/Widget/" + text + "/" + text, this);
		if (!widget)
		{
			Debug.LogError("Widget:" + id + " not found.");
			return null;
		}
		this.list.Add(widget);
		widget.gameObject.name = text;
		widget.Activate();
		this.RefreshWidget(widget);
		if (LayerWidget.Instance)
		{
			widget.OnManagerActivate();
		}
		if (widget.AlwaysBottom)
		{
			Type setSiblingAfter = widget.SetSiblingAfter;
			bool flag = false;
			if (setSiblingAfter != null)
			{
				foreach (Widget widget2 in this.list)
				{
					if (widget2.GetType() == setSiblingAfter)
					{
						widget.transform.SetSiblingIndex(widget2.transform.GetSiblingIndex() + 1);
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				widget.transform.SetAsFirstSibling();
			}
		}
		return widget;
	}

	public void RefreshWidget(Widget.Config c)
	{
		this.RefreshWidget(this.GetWidget(c.id));
	}

	public void RefreshWidget(Widget w)
	{
		if (!w)
		{
			return;
		}
		w.dragPanel.GetComponent<Graphic>().raycastTarget = !w.config.locked;
	}

	public Widget GetWidget(string id)
	{
		foreach (Widget widget in this.list)
		{
			if (widget.gameObject.name == "Widget" + id)
			{
				return widget;
			}
		}
		return null;
	}

	public void DeactivateWidget(string id)
	{
		this.DeactivateWidget(this.GetWidget(id));
	}

	public void DeactivateWidget(Widget w)
	{
		if (w == null)
		{
			return;
		}
		this.list.Remove(w);
		w.Deactivate();
		if (LayerWidget.Instance)
		{
			LayerWidget.Instance.Refresh();
		}
	}

	public void KillWidgets()
	{
		this.DestroyChildren(true, true);
		this.list.Clear();
		this.first = true;
	}

	public void Show()
	{
		foreach (Widget widget in this.list)
		{
			if (widget.IsInRightMode())
			{
				widget.gameObject.SetActive(true);
			}
		}
	}

	public void Hide()
	{
		foreach (Widget widget in this.list)
		{
			if (widget is WidgetCurrentTool || widget is WidgetQuestTracker || widget is WidgetTracker || widget is WidgetMemo || widget is WidgetEquip)
			{
				widget.gameObject.SetActive(false);
			}
		}
	}

	public void Reset(bool toggleTheme)
	{
		if (WidgetMainText.Instance.box.isShowingLog)
		{
			WidgetMainText.Instance._ToggleLog();
		}
		if (WidgetMainText.Instance)
		{
			(WidgetMainText.boxBk = WidgetMainText.Instance.box).transform.SetParent(base.transform.parent, false);
		}
		this.KillWidgets();
		if (toggleTheme)
		{
			EMono.player.useSubWidgetTheme = !EMono.player.useSubWidgetTheme;
		}
		this.OnGameInstantiated();
		this.OnActivateZone();
		this.OnChangeActionMode();
	}

	public void DialogSave(Action onSave = null)
	{
		EMono.core.WaitForEndOfFrame(delegate
		{
			string text = StandaloneFileBrowser.SaveFilePanel("Save Widget Theme", CorePath.WidgetSave, "new theme", "json");
			if (!string.IsNullOrEmpty(text))
			{
				if (!EMono.debug.enable && (text.Contains("Default.json") || text.Contains("Modern.json") || text.Contains("Classic.json")))
				{
					Dialog.Ok("dialogInvalidTheme", null);
					return;
				}
				this.Save(text);
				if (onSave != null)
				{
					onSave();
				}
			}
		});
	}

	public void DialogLoad(Action onLoad = null)
	{
		EMono.core.WaitForEndOfFrame(delegate
		{
			string[] array = StandaloneFileBrowser.OpenFilePanel("Load Widget Theme", CorePath.WidgetSave, "json", false);
			if (array.Length != 0)
			{
				this.Load(EMono.player.useSubWidgetTheme, array[0]);
				this.Reset(false);
				if (onLoad != null)
				{
					onLoad();
				}
			}
		});
	}

	public void Save(string path = null)
	{
		if (path == null)
		{
			path = CorePath.WidgetSave + (EMono.player.useSubWidgetTheme ? EMono.core.config.other.idSubWidgetTheme : EMono.core.config.other.idMainWidgetTheme) + ".json";
		}
		this.UpdateConfigs();
		IO.SaveFile(path, EMono.player.widgets, false, null);
	}

	public void Load(bool isSubTheme, string path = null)
	{
		if (path == null)
		{
			path = CorePath.WidgetSave + (isSubTheme ? EMono.core.config.other.idSubWidgetTheme : EMono.core.config.other.idMainWidgetTheme) + ".json";
		}
		WidgetManager.SaveData saveData = IO.LoadFile<WidgetManager.SaveData>(path, false, null);
		if (isSubTheme)
		{
			EMono.player.subWidgets = saveData;
		}
		else
		{
			EMono.player.mainWidgets = saveData;
		}
		this.currentPath = path;
	}

	public Dictionary<string, Widget.Meta> metaMap = new Dictionary<string, Widget.Meta>();

	public string currentPath;

	[NonSerialized]
	public List<Widget> list = new List<Widget>();

	private bool first;

	public class SaveData
	{
		public Dictionary<string, Widget.Config> dict;
	}
}
