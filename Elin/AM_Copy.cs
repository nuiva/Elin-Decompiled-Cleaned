using System;
using System.Collections.Generic;
using System.IO;
using SFB;
using UnityEngine;

public class AM_Copy : AM_BaseTileSelect
{
	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			if (this.partialMap != null)
			{
				return BaseTileSelector.SelectType.Single;
			}
			return BaseTileSelector.SelectType.Multiple;
		}
	}

	public override bool IsBuildMode
	{
		get
		{
			return true;
		}
	}

	public override BuildMenu.Mode buildMenuMode
	{
		get
		{
			return BuildMenu.Mode.PartialMap;
		}
	}

	public override bool UseSubMenu
	{
		get
		{
			return true;
		}
	}

	public override bool SubMenuAsGroup
	{
		get
		{
			return false;
		}
	}

	public virtual AM_Copy.Mode mode
	{
		get
		{
			return AM_Copy.Mode.Copy;
		}
	}

	public PartialMapMenu menu
	{
		get
		{
			return PartialMapMenu.Instance;
		}
	}

	public override string OnSetSubMenuButton(int a, UIButton b)
	{
		if (a != 0)
		{
			return null;
		}
		b.SetCheck(PartialMap.relative);
		return "copyRelative";
	}

	public override void OnClickSubMenu(int a)
	{
		PartialMap.relative = !PartialMap.relative;
	}

	public override HitResult HitTest(Point point, Point start)
	{
		if (!base.Summary.CanExecute())
		{
			return HitResult.Invalid;
		}
		if (this.partialMap == null)
		{
			using (List<Thing>.Enumerator enumerator = point.cell.Things.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.trait.CanCopyInBlueprint)
					{
						return HitResult.Warning;
					}
				}
			}
			return HitResult.Valid;
		}
		return HitResult.Valid;
	}

	public override void OnSelectStart(Point point)
	{
		this.RefreshMenu(false);
	}

	public override void OnSelectEnd(bool cancel)
	{
		if (cancel)
		{
			this.RefreshMenu(true);
		}
	}

	public override void OnAfterProcessTiles(Point start, Point end)
	{
		if (this.partialMap != null)
		{
			EClass.Sound.Play("build_area");
			this.partialMap.editMode = false;
			this.partialMap.procedural = true;
			if (MapPiece.IsEditor)
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					this.partialMap.procedural = true;
				}
				else
				{
					this.partialMap.editMode = true;
				}
			}
			this.partialMap.Apply(end, PartialMap.ApplyMode.Apply);
			return;
		}
		this.partialMap = new PartialMap
		{
			allowRotate = Application.isEditor
		};
		int x = Mathf.Min(start.x, end.x);
		int z = Mathf.Min(start.z, end.z);
		int w = Mathf.Abs(start.x - end.x) + 1;
		int h = Mathf.Abs(start.z - end.z) + 1;
		this.partialMap.Save(x, z, w, h);
		if (!this.overwritePath.IsEmpty())
		{
			File.Copy(PartialMap.PathTemp, this.overwritePath, true);
			SE.Play("camera");
			this.overwritePath = null;
			this.partialMap = null;
			this.RefreshMenu(true);
			return;
		}
		this.partialMap = PartialMap.Load(null);
		this.partialMap.localOffsetX = ((end.x > start.x) ? (start.x - end.x) : 0);
		this.partialMap.localOffsetZ = ((end.z > start.z) ? (start.z - end.z) : 0);
		this.RefreshMenu(false);
		if (this.mode == AM_Copy.Mode.Create)
		{
			UIScreenshot.Create().Activate(this.partialMap, this.dir, new Action<PartialMap>(this.OnSave), false);
			this.Clear();
			return;
		}
		SE.Click();
	}

	public virtual void OnSave(PartialMap _partial)
	{
		if (MapPiece.IsEditor)
		{
			MapPiece.initialized = false;
			this.menu.RefreshCategory(null);
		}
		this.menu.DestorySprites();
		this.menu.Refresh();
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (UIScreenshot.Instance)
		{
			return;
		}
		if (this.partialMap != null)
		{
			this.partialMap.Apply(point, PartialMap.ApplyMode.Render);
			return;
		}
		base.OnRenderTile(point, result, dir);
	}

	public override int CostMoney
	{
		get
		{
			if (this.partialMap == null)
			{
				return 0;
			}
			return this.partialMap.value;
		}
	}

	public override void OnUpdateInput()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			PartialMap.ExportDialog(null);
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			this.ImportDialog(null);
		}
	}

	public override void RotateUnderMouse()
	{
		if (this.partialMap != null && this.partialMap.allowRotate)
		{
			this.partialMap.Rotate();
			return;
		}
		base.RotateUnderMouse();
	}

	public void Clear()
	{
		this.partialMap.ClearMarkedCells();
		this.partialMap = null;
		this.RefreshMenu(true);
	}

	public override void OnCancel()
	{
		if (!this.overwritePath.IsEmpty())
		{
			this.overwritePath = null;
			SE.Play("actionMode");
			this.RefreshMenu(true);
			return;
		}
		if (this.mode == AM_Copy.Mode.Place)
		{
			base.Deactivate();
			return;
		}
		if (UIScreenshot.Instance)
		{
			return;
		}
		if (this.partialMap != null)
		{
			this.Clear();
			SE.Play("actionMode");
			return;
		}
		base.Deactivate();
	}

	public void Import(string path)
	{
		this.partialMap = PartialMap.Load(path);
		this.RefreshMenu(false);
	}

	public void ImportDialog(string dir = null)
	{
		EClass.core.WaitForEndOfFrame(delegate
		{
			string[] array = StandaloneFileBrowser.OpenFilePanel("Import Map Piece", dir ?? CorePath.MapPieceSaveUser, "mp", false);
			if (array.Length != 0)
			{
				this.Import(array[0]);
			}
		});
	}

	public void RefreshMenu(bool show)
	{
		if (this.menu)
		{
			this.menu.SetVisible(show);
			BuildMenu.Instance.terrainMenu.SetActive(!show);
			this.menu.buttonSave.SetActive(this.partialMap != null && this.partialMap.path.IsEmpty());
			this.menu.buttonDelete.SetActive(this.partialMap != null && !this.partialMap.path.IsEmpty());
			this.menu.buttonEdit.SetActive(this.partialMap != null && !this.partialMap.path.IsEmpty());
			this.menu.buttonSave.SetOnClick(delegate
			{
				UIScreenshot.Create().Activate(this.partialMap, this.dir, delegate(PartialMap a)
				{
					this.OnSave(a);
					this.Clear();
				}, false);
			});
			this.menu.buttonDelete.SetOnClick(delegate
			{
				Dialog.YesNo("dialog_deleteMapPiece", delegate
				{
					SE.Trash();
					PartialMap.Delete(this.partialMap.path);
					this.menu.DestorySprites();
					this.menu.Refresh();
					this.Clear();
				}, null, "yes", "no");
			});
			this.menu.buttonEdit.SetOnClick(delegate
			{
				UIScreenshot.Create().Activate(this.partialMap, this.dir, new Action<PartialMap>(this.OnSave), true);
				this.Clear();
			});
		}
	}

	public override void OnActivate()
	{
		this.dir = (this.dirUser = new DirectoryInfo(CorePath.MapPieceSaveUser));
		if (this.mode == AM_Copy.Mode.Copy)
		{
			PartialMapMenu.Activate();
		}
		this.RefreshMenu(true);
	}

	public override void OnDeactivate()
	{
		if (this.partialMap != null)
		{
			this.partialMap.ClearMarkedCells();
			this.partialMap = null;
		}
		if (this.menu)
		{
			this.menu.Deactivate();
		}
		if (Application.isEditor)
		{
			MapPiece.initialized = false;
		}
	}

	public string overwritePath;

	public PartialMap partialMap;

	public DirectoryInfo dir;

	public DirectoryInfo dirUser;

	public enum Mode
	{
		Copy,
		Place,
		Create
	}
}
