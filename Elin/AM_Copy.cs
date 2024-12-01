using System.IO;
using SFB;
using UnityEngine;

public class AM_Copy : AM_BaseTileSelect
{
	public enum Mode
	{
		Copy,
		Place,
		Create
	}

	public string overwritePath;

	public PartialMap partialMap;

	public DirectoryInfo dir;

	public DirectoryInfo dirUser;

	public override BaseTileSelector.SelectType selectType
	{
		get
		{
			if (partialMap != null)
			{
				return BaseTileSelector.SelectType.Single;
			}
			return BaseTileSelector.SelectType.Multiple;
		}
	}

	public override bool IsBuildMode => true;

	public override BuildMenu.Mode buildMenuMode => BuildMenu.Mode.PartialMap;

	public override bool UseSubMenu => true;

	public override bool SubMenuAsGroup => false;

	public virtual Mode mode => Mode.Copy;

	public PartialMapMenu menu => PartialMapMenu.Instance;

	public override int CostMoney
	{
		get
		{
			if (partialMap == null)
			{
				return 0;
			}
			return partialMap.value;
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
		if (partialMap == null)
		{
			foreach (Thing thing in point.cell.Things)
			{
				if (!thing.trait.CanCopyInBlueprint)
				{
					return HitResult.Warning;
				}
			}
		}
		return HitResult.Valid;
	}

	public override void OnSelectStart(Point point)
	{
		RefreshMenu(show: false);
	}

	public override void OnSelectEnd(bool cancel)
	{
		if (cancel)
		{
			RefreshMenu(show: true);
		}
	}

	public override void OnAfterProcessTiles(Point start, Point end)
	{
		if (partialMap != null)
		{
			EClass.Sound.Play("build_area");
			partialMap.editMode = false;
			partialMap.procedural = true;
			if (MapPiece.IsEditor)
			{
				if (Input.GetKey(KeyCode.LeftShift))
				{
					partialMap.procedural = true;
				}
				else
				{
					partialMap.editMode = true;
				}
			}
			partialMap.Apply(end, PartialMap.ApplyMode.Apply);
			return;
		}
		partialMap = new PartialMap
		{
			allowRotate = Application.isEditor
		};
		int x = Mathf.Min(start.x, end.x);
		int z = Mathf.Min(start.z, end.z);
		int w = Mathf.Abs(start.x - end.x) + 1;
		int h = Mathf.Abs(start.z - end.z) + 1;
		partialMap.Save(x, z, w, h);
		if (!overwritePath.IsEmpty())
		{
			File.Copy(PartialMap.PathTemp, overwritePath, overwrite: true);
			SE.Play("camera");
			overwritePath = null;
			partialMap = null;
			RefreshMenu(show: true);
			return;
		}
		partialMap = PartialMap.Load();
		partialMap.localOffsetX = ((end.x > start.x) ? (start.x - end.x) : 0);
		partialMap.localOffsetZ = ((end.z > start.z) ? (start.z - end.z) : 0);
		RefreshMenu(show: false);
		if (mode == Mode.Create)
		{
			UIScreenshot.Create().Activate(partialMap, dir, OnSave);
			Clear();
		}
		else
		{
			SE.Click();
		}
	}

	public virtual void OnSave(PartialMap _partial)
	{
		if (MapPiece.IsEditor)
		{
			MapPiece.initialized = false;
			menu.RefreshCategory();
		}
		menu.DestorySprites();
		menu.Refresh();
	}

	public override void OnRenderTile(Point point, HitResult result, int dir)
	{
		if (!UIScreenshot.Instance)
		{
			if (partialMap != null)
			{
				partialMap.Apply(point, PartialMap.ApplyMode.Render);
			}
			else
			{
				base.OnRenderTile(point, result, dir);
			}
		}
	}

	public override void OnUpdateInput()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			PartialMap.ExportDialog();
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			ImportDialog();
		}
	}

	public override void RotateUnderMouse()
	{
		if (partialMap != null && partialMap.allowRotate)
		{
			partialMap.Rotate();
		}
		else
		{
			base.RotateUnderMouse();
		}
	}

	public void Clear()
	{
		partialMap.ClearMarkedCells();
		partialMap = null;
		RefreshMenu(show: true);
	}

	public override void OnCancel()
	{
		if (!overwritePath.IsEmpty())
		{
			overwritePath = null;
			SE.Play("actionMode");
			RefreshMenu(show: true);
		}
		else if (mode == Mode.Place)
		{
			Deactivate();
		}
		else if (!UIScreenshot.Instance)
		{
			if (partialMap != null)
			{
				Clear();
				SE.Play("actionMode");
			}
			else
			{
				Deactivate();
			}
		}
	}

	public void Import(string path)
	{
		partialMap = PartialMap.Load(path);
		RefreshMenu(show: false);
	}

	public void ImportDialog(string dir = null)
	{
		EClass.core.WaitForEndOfFrame(delegate
		{
			string[] array = StandaloneFileBrowser.OpenFilePanel("Import Map Piece", dir ?? CorePath.MapPieceSaveUser, "mp", multiselect: false);
			if (array.Length != 0)
			{
				Import(array[0]);
			}
		});
	}

	public void RefreshMenu(bool show)
	{
		if (!menu)
		{
			return;
		}
		menu.SetVisible(show);
		BuildMenu.Instance.terrainMenu.SetActive(!show);
		menu.buttonSave.SetActive(partialMap != null && partialMap.path.IsEmpty());
		menu.buttonDelete.SetActive(partialMap != null && !partialMap.path.IsEmpty());
		menu.buttonEdit.SetActive(partialMap != null && !partialMap.path.IsEmpty());
		menu.buttonSave.SetOnClick(delegate
		{
			UIScreenshot.Create().Activate(partialMap, dir, delegate(PartialMap a)
			{
				OnSave(a);
				Clear();
			});
		});
		menu.buttonDelete.SetOnClick(delegate
		{
			Dialog.YesNo("dialog_deleteMapPiece", delegate
			{
				SE.Trash();
				PartialMap.Delete(partialMap.path);
				menu.DestorySprites();
				menu.Refresh();
				Clear();
			});
		});
		menu.buttonEdit.SetOnClick(delegate
		{
			UIScreenshot.Create().Activate(partialMap, dir, OnSave, isUpdate: true);
			Clear();
		});
	}

	public override void OnActivate()
	{
		dir = (dirUser = new DirectoryInfo(CorePath.MapPieceSaveUser));
		if (mode == Mode.Copy)
		{
			PartialMapMenu.Activate();
		}
		RefreshMenu(show: true);
	}

	public override void OnDeactivate()
	{
		if (partialMap != null)
		{
			partialMap.ClearMarkedCells();
			partialMap = null;
		}
		if ((bool)menu)
		{
			menu.Deactivate();
		}
		if (Application.isEditor)
		{
			MapPiece.initialized = false;
		}
	}
}
