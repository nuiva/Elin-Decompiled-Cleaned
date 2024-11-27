using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFB;
using UnityEngine;

public class LayerEditHouse : ELayer
{
	public TraitHouseBoard.Data data
	{
		get
		{
			return this.board.data;
		}
	}

	public override void OnInit()
	{
		this.windows[0].AddBottomButton("save", delegate
		{
			ELayer.core.WaitForEndOfFrame(delegate
			{
				string text = StandaloneFileBrowser.SaveFilePanel("Save Template", CorePath.LotTemplate, "new house", "tpl");
				if (!string.IsNullOrEmpty(text))
				{
					IO.SaveFile(text, this.board.data, false, null);
					this.RefreshTemplates();
				}
			});
		}, false);
	}

	public void SetBoard(TraitHouseBoard b)
	{
		if (BuildMenu.Instance)
		{
			BuildMenu.Instance.SetActive(false);
		}
		ELayer.screen.tileMap.usingHouseBoard = true;
		this.board = b2;
		this.ddRoofStyle.SetList<RoofStyle>(this.data.idRoofStyle, ELayer.screen.tileMap.roofStyles, (RoofStyle a, int b) => a.GetName(b).lang(), delegate(int a, RoofStyle b)
		{
			this.data.idRoofStyle = a;
			this.ApplyData();
			this.RefreshBlockList();
		}, true);
		this.toggleReverse.SetToggle(this.data.reverse, delegate(bool on)
		{
			this.data.reverse = on;
			this.ApplyData();
		});
		this.toggleAltRoof.SetToggle(this.data.altRoof, delegate(bool on)
		{
			this.data.altRoof = on;
			this.ApplyData();
		});
		List<int> blocks = new List<int>();
		blocks.Add(0);
		foreach (SourceObj.Row row in ELayer.sources.objs.rows)
		{
			if (row.tileType == TileType.Roof && !row.tag.Contains("hidden"))
			{
				blocks.Add(row.id);
			}
		}
		int num = blocks.IndexOf(this.data.idRoofTile);
		if (num == -1)
		{
			num = 0;
		}
		this.sliderRoof.SetList<int>(num, blocks, delegate(int a, int b)
		{
			this.data.idRoofTile = b;
			this.ApplyData();
		}, (int a) => blocks.IndexOf(a).ToString() ?? "");
		List<int> ramps = new List<int>();
		foreach (SourceBlock.Row row2 in ELayer.sources.blocks.rows)
		{
			if (row2.tileType == TileType.Stairs || row2.tileType == TileType.Slope)
			{
				ramps.Add(row2.id);
			}
		}
		int num2 = ramps.IndexOf(this.data.idRamp);
		if (num2 == -1)
		{
			num2 = 0;
		}
		this.sliderRamp.SetList<int>(num2, ramps, delegate(int a, int b)
		{
			this.data.idRamp = b;
			this.ApplyData();
		}, (int a) => ramps.IndexOf(a).ToString() ?? "");
		this.sliderWallHeight.SetSlider((float)this.data.height, delegate(float a)
		{
			this.data.height = (int)a;
			this.ApplyData();
			return a.ToString() ?? "";
		}, 1, 20, true);
		this.sliderHeightFix.SetSlider((float)this.data.heightFix, delegate(float a)
		{
			this.data.heightFix = (int)a;
			this.ApplyData();
			return a.ToString() ?? "";
		}, 0, 60, true);
		this.sliderDecoFix.SetSlider((float)this.data.decoFix, delegate(float a)
		{
			this.data.decoFix = (int)a;
			this.ApplyData();
			return a.ToString() ?? "";
		}, -20, 20, true);
		this.sliderDecoFix2.SetSlider((float)this.data.decoFix2, delegate(float a)
		{
			this.data.decoFix2 = (int)a;
			this.ApplyData();
			return a.ToString() ?? "";
		}, -20, 400, true);
		this.sliderDeco.SetSlider((float)this.data.idDeco, delegate(float a)
		{
			this.data.idDeco = (int)a;
			this.ApplyData();
			return a.ToString() ?? "";
		}, 0, this.maxWallDeco, true);
		this.sliderDeco2.SetSlider((float)this.data.idDeco2, delegate(float a)
		{
			this.data.idDeco2 = (int)a;
			this.ApplyData();
			return a.ToString() ?? "";
		}, 0, this.maxWallDeco, true);
		if (this.first)
		{
			this.RefreshTemplates();
		}
		this.RefreshBlockList();
		this.first = false;
	}

	private void RefreshBlockList()
	{
		bool flag = ELayer.screen.tileMap.roofStyles[this.data.idRoofStyle].type != RoofStyle.Type.FlatFloor;
		List<int> blocks = new List<int>();
		if (flag)
		{
			using (List<SourceBlock.Row>.Enumerator enumerator = ELayer.sources.blocks.rows.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					SourceBlock.Row row = enumerator.Current;
					if (row.tileType == TileType.Block)
					{
						blocks.Add(row.id);
					}
				}
				goto IL_E2;
			}
		}
		foreach (SourceFloor.Row row2 in ELayer.sources.floors.rows)
		{
			blocks.Add(row2.id);
		}
		IL_E2:
		int num = blocks.IndexOf(this.data.idBlock);
		if (num == -1)
		{
			num = 0;
		}
		this.sliderBlock.SetList<int>(num, blocks, delegate(int a, int b)
		{
			this.data.idBlock = b;
			this.ApplyData();
		}, (int a) => blocks.IndexOf(a).ToString() ?? "");
		this.buttonColorRoof.icon.color = IntColor.FromInt(this.data.colRoof);
		Action<PickerState, Color> <>9__6;
		this.buttonColorRoof.SetOnClick(delegate
		{
			LayerColorPicker layerColorPicker = ELayer.ui.AddLayer<LayerColorPicker>();
			Color startColor = IntColor.FromInt(this.data.colRoof);
			Color resetColor = IntColor.FromInt(this.data.colRoof);
			Action<PickerState, Color> onChangeColor;
			if ((onChangeColor = <>9__6) == null)
			{
				onChangeColor = (<>9__6 = delegate(PickerState state, Color c)
				{
					this.data.colRoof = IntColor.ToInt(c);
					this.buttonColorRoof.icon.color = c;
					this.board.ApplyData();
				});
			}
			layerColorPicker.SetColor(startColor, resetColor, onChangeColor);
		});
		this.buttonColorBlock.icon.color = IntColor.FromInt(this.data.colBlock);
		Action<PickerState, Color> <>9__7;
		this.buttonColorBlock.SetOnClick(delegate
		{
			LayerColorPicker layerColorPicker = ELayer.ui.AddLayer<LayerColorPicker>();
			Color startColor = IntColor.FromInt(this.data.colBlock);
			Color resetColor = IntColor.FromInt(this.data.colBlock);
			Action<PickerState, Color> onChangeColor;
			if ((onChangeColor = <>9__7) == null)
			{
				onChangeColor = (<>9__7 = delegate(PickerState state, Color c)
				{
					this.data.colBlock = IntColor.ToInt(c);
					this.buttonColorBlock.icon.color = c;
					this.board.ApplyData();
				});
			}
			layerColorPicker.SetColor(startColor, resetColor, onChangeColor);
		});
		this.buttonColorDeco.icon.color = IntColor.FromInt(this.data.colDeco);
		Action<PickerState, Color> <>9__8;
		this.buttonColorDeco.SetOnClick(delegate
		{
			LayerColorPicker layerColorPicker = ELayer.ui.AddLayer<LayerColorPicker>();
			Color startColor = IntColor.FromInt(this.data.colDeco);
			Color resetColor = IntColor.FromInt(this.data.colDeco);
			Action<PickerState, Color> onChangeColor;
			if ((onChangeColor = <>9__8) == null)
			{
				onChangeColor = (<>9__8 = delegate(PickerState state, Color c)
				{
					this.data.colDeco = IntColor.ToInt(c);
					this.buttonColorDeco.icon.color = c;
					this.board.ApplyData();
				});
			}
			layerColorPicker.SetColor(startColor, resetColor, onChangeColor);
		});
		this.buttonColorDeco2.icon.color = IntColor.FromInt(this.data.colDeco2);
		Action<PickerState, Color> <>9__9;
		this.buttonColorDeco2.SetOnClick(delegate
		{
			LayerColorPicker layerColorPicker = ELayer.ui.AddLayer<LayerColorPicker>();
			Color startColor = IntColor.FromInt(this.data.colDeco2);
			Color resetColor = IntColor.FromInt(this.data.colDeco2);
			Action<PickerState, Color> onChangeColor;
			if ((onChangeColor = <>9__9) == null)
			{
				onChangeColor = (<>9__9 = delegate(PickerState state, Color c)
				{
					this.data.colDeco2 = IntColor.ToInt(c);
					this.buttonColorDeco2.icon.color = c;
					this.board.ApplyData();
				});
			}
			layerColorPicker.SetColor(startColor, resetColor, onChangeColor);
		});
	}

	public void RefreshTemplates()
	{
		FileInfo[] list = (from a in new DirectoryInfo(CorePath.LotTemplate).GetFiles()
		where a.Name.EndsWith(".tpl")
		select a).ToArray<FileInfo>();
		this.ddTemplate.SetList<FileInfo>(0, list, (FileInfo a, int b) => a.Name, delegate(int a, FileInfo b)
		{
			int idBGM = this.board.data.idBGM;
			this.board.data = IO.LoadFile<TraitHouseBoard.Data>(b.FullName, false, null);
			this.board.data.idBGM = idBGM;
			this.ApplyData();
			this.SetBoard(this.board);
		}, true);
	}

	public void ApplyData()
	{
		this.board.ApplyData();
		this.toggleAltRoof.SetActive(this.board.data.idRoofTile != 0);
	}

	public override void OnKill()
	{
		ELayer.screen.tileMap.usingHouseBoard = false;
		if (BuildMenu.Instance)
		{
			BuildMenu.Instance.SetActive(true);
		}
	}

	public TraitHouseBoard board;

	public UISlider sliderBlock;

	public UISlider sliderRoof;

	public UISlider sliderRamp;

	public UISlider sliderWallHeight;

	public UISlider sliderHeightFix;

	public UISlider sliderDeco;

	public UISlider sliderDeco2;

	public UISlider sliderDecoFix;

	public UISlider sliderDecoFix2;

	public UIDropdown ddRoofStyle;

	public UIDropdown ddTemplate;

	public UIButton toggleReverse;

	public UIButton toggleAltRoof;

	public UIButton buttonColorRoof;

	public UIButton buttonColorBlock;

	public UIButton buttonColorDeco;

	public UIButton buttonColorDeco2;

	public UIButton toggleAtrium;

	public int maxWallDeco;

	private bool first = true;
}
