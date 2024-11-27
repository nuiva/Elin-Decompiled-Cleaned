using System;
using System.Collections.Generic;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;
using UnityEngine.UI;

public class TraitMapBoard : TraitBoard
{
	public override bool IsHomeItem
	{
		get
		{
			return true;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass.debug.enable && !EClass._zone.IsPCFaction)
		{
			return;
		}
		p.TrySetAct("actChangeHomeIcon", delegate()
		{
			UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
			GridLayoutGroup parent = uicontextMenu.AddGridLayout();
			HashSet<int> hashSet = new HashSet<int>();
			foreach (Spatial spatial in EClass.game.spatials.map.Values)
			{
				if (spatial.icon > 0)
				{
					hashSet.Add(spatial.icon);
				}
			}
			foreach (int num in hashSet)
			{
				UIButton uibutton = Util.Instantiate<UIButton>("UI/Element/Button/ButtonContainerIcon", parent);
				int _i = num;
				uibutton.icon.sprite = TilemapUtils.GetOrCreateTileSprite(EClass.scene.elomap.actor.tileset, num, 0f);
				uibutton.icon.Rect().localScale = new Vector3(2f, 2f, 1f);
				uibutton.SetOnClick(delegate
				{
					SE.Click();
					EClass._zone.icon = _i;
					EClass.ui.contextMenu.currentMenu.Hide();
					EClass.scene.elomap.SetZone(EClass._zone.x, EClass._zone.y, EClass._zone, true);
				});
			}
			uicontextMenu.Show();
			return false;
		}, this.owner, null, 1, false, true, false);
		p.TrySetAct("actChangeBlockHeight", delegate()
		{
			UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
			uicontextMenu.AddSlider("adjustment", (float a) => a.ToString() ?? "", EClass._map.config.blockHeight * 10f, delegate(float b)
			{
				EClass._map.config.blockHeight = b * 0.1f;
			}, 0f, 40f, true, false, false);
			uicontextMenu.Show();
			return false;
		}, this.owner, null, 1, false, true, false);
		p.TrySetAct("actChangeSkyBlockHeight", delegate()
		{
			UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
			uicontextMenu.AddSlider("adjustment", (float a) => a.ToString() ?? "", (float)EClass._map.config.skyBlockHeight, delegate(float b)
			{
				EClass._map.config.skyBlockHeight = (int)b;
			}, 1f, 20f, true, false, false);
			uicontextMenu.Show();
			return false;
		}, this.owner, null, 1, false, true, false);
		p.TrySetAct("actChangeMapBG", delegate()
		{
			LayerList layerList = EClass.ui.AddLayer<LayerList>().SetSize(400f, -1f);
			List<MapBG> list = Util.EnumToList<MapBG>();
			Action<int> <>9__13;
			for (int i = 0; i < list.Count; i++)
			{
				LayerList layerList2 = layerList;
				string lang = list[i].ToString();
				Action<int> action;
				if ((action = <>9__13) == null)
				{
					action = (<>9__13 = delegate(int a)
					{
						EClass._map.config.bg = list[a];
						EClass.scene.RefreshBG();
					});
				}
				layerList2.Add(lang, action);
			}
			layerList.Show(true);
			return false;
		}, this.owner, null, 1, false, true, false);
		p.TrySetAct("actChangeShadowStrength", delegate()
		{
			UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
			uicontextMenu.AddSlider("adjustment", (float a) => a.ToString() + "%", EClass._map.config.shadowStrength * 100f, delegate(float b)
			{
				EClass._map.config.shadowStrength = b * 0.01f;
				EClass.screen.RefreshAll();
			}, 0f, 400f, true, false, false);
			uicontextMenu.Show();
			return false;
		}, this.owner, null, 1, false, true, false);
		p.TrySetAct("actChangeFogDensity", delegate()
		{
			LayerList layerList = EClass.ui.AddLayer<LayerList>().SetSize(400f, -1f);
			List<FogType> list = Util.EnumToList<FogType>();
			Action<int> <>9__16;
			for (int i = 0; i < list.Count; i++)
			{
				LayerList layerList2 = layerList;
				string lang = list[i].ToString();
				Action<int> action;
				if ((action = <>9__16) == null)
				{
					action = (<>9__16 = delegate(int a)
					{
						EClass._map.config.fog = list[a];
						EClass.screen.RefreshAll();
					});
				}
				layerList2.Add(lang, action);
			}
			layerList.Show(true);
			return false;
		}, this.owner, null, 1, false, true, false);
		p.TrySetAct("actChangeSkyColor", delegate()
		{
			EClass.ui.AddLayer<LayerColorPicker>().SetColor(EClass._map.config.colorScreen.Get(), new Color(0f, 0f, 0f, 0f), delegate(PickerState state, Color _c)
			{
				EClass._map.config.colorScreen.Set(_c);
				EClass.screen.RefreshGrading();
			});
			return false;
		}, this.owner, null, 1, false, true, false);
		p.TrySetAct("actChangeSeaColor", delegate()
		{
			EClass.ui.AddLayer<LayerColorPicker>().SetColor(EClass._map.config.colorSea.Get(), new Color(0f, 0f, 0f, 0f), delegate(PickerState state, Color _c)
			{
				EClass._map.config.colorSea.Set(_c);
				EClass.screen.RefreshGrading();
			});
			return false;
		}, this.owner, null, 1, false, true, false);
	}
}
