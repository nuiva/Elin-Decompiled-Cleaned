using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PartialMapMenu : EMono
{
	public AM_Copy am
	{
		get
		{
			return ActionMode.Copy;
		}
	}

	public DirectoryInfo dir
	{
		get
		{
			return this.am.dir;
		}
	}

	public bool IsEditor
	{
		get
		{
			return MapPiece.IsEditor;
		}
	}

	public PartialMap partial
	{
		get
		{
			return this.am.partialMap;
		}
	}

	public static void Activate()
	{
		if (!PartialMapMenu.Instance)
		{
			PartialMapMenu.Instance = Util.Instantiate<PartialMapMenu>("UI/BuildMenu/PartialMapMenu", EMono.ui);
			PartialMapMenu.Instance.Init();
		}
		PartialMapMenu.Instance.Refresh();
		MapPiece.CacheMap.Clear();
	}

	public void Init()
	{
		this.rootDir = new DirectoryInfo(CorePath.MapPieceSaveUser);
		this.goCat.SetActive(this.IsEditor);
		this.RefreshOptions();
		if (this.IsEditor)
		{
			this.RefreshCategory(null);
			if (PartialMapMenu.lastDir != null)
			{
				this.listCat.Select<DirectoryInfo>((DirectoryInfo a) => a.FullName == PartialMapMenu.lastDir.FullName, false);
				this.am.dir = PartialMapMenu.lastDir;
			}
			if (PartialMapMenu.lastDirSub != null)
			{
				this.RefreshCategory(PartialMapMenu.lastDir);
				this.listCatSub.Select<DirectoryInfo>((DirectoryInfo a) => a.FullName == PartialMapMenu.lastDirSub.FullName, false);
				this.am.dir = PartialMapMenu.lastDirSub;
			}
		}
	}

	public void Deactivate()
	{
		this.DestorySprites();
		UnityEngine.Object.DestroyImmediate(PartialMapMenu.Instance.gameObject);
	}

	public void SetVisible(bool visible)
	{
		this.RefreshOptions();
		this.goMain.SetActive(visible);
	}

	public void DestorySprites()
	{
		foreach (Sprite sprite in this.dictSprite.Values)
		{
			UnityEngine.Object.Destroy(sprite.texture);
			UnityEngine.Object.Destroy(sprite);
		}
		this.dictSprite.Clear();
	}

	public void RefreshOptions()
	{
		this.goOption.SetActive(this.IsEditor && this.partial != null);
		if (this.partial == null)
		{
			return;
		}
		this.toggleRotate.SetToggle(this.partial.allowRotate, delegate(bool on)
		{
			this.partial.allowRotate = on;
			this.partial.Update();
		});
		this.toggleIgnoreBlock.SetToggle(this.partial.ignoreBlock, delegate(bool on)
		{
			this.partial.ignoreBlock = on;
			this.partial.Update();
		});
	}

	public void Refresh()
	{
		this.list.callbacks = new UIList.Callback<MapMetaData, ButtonGrid>
		{
			onClick = delegate(MapMetaData a, ButtonGrid b)
			{
				SE.Click();
				this.am.Import(a.path);
				this.am.partialMap.name = a.name;
			},
			onRedraw = delegate(MapMetaData a, ButtonGrid b, int i)
			{
				string path = a.path.GetFullFileNameWithoutExtension() + ".txt";
				if (File.Exists(path))
				{
					string text = File.ReadAllText(path);
					if (!text.IsEmpty())
					{
						a.name = text;
					}
				}
				if (a.name.IsEmpty())
				{
					a.name = "unknown";
				}
				b.mainText.SetText(this.IsEditor ? new FileInfo(a.path).Name : a.name);
				b.subText.text = Lang._currency(a.partial.value, false, 14);
				Sprite sprite = this.dictSprite.TryGetValue(a.path, null);
				if (!sprite)
				{
					string path2 = a.path.GetFullFileNameWithoutExtension() + ".jpg";
					if (File.Exists(path2))
					{
						Texture2D texture2D = new Texture2D(1, 1);
						byte[] source = File.ReadAllBytes(path2);
						texture2D.LoadImage(source.ToArray<byte>());
						sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
						b.icon.sprite = sprite;
						this.dictSprite[a.path] = sprite;
					}
				}
				if (sprite)
				{
					b.icon.sprite = sprite;
				}
				UIButton componentInDirectChildren = b.GetComponentInDirectChildren<UIButton>();
				componentInDirectChildren.SetActive(this.IsEditor);
				componentInDirectChildren.SetOnClick(delegate
				{
					SE.Click();
					this.am.overwritePath = a.path;
					this.am.RefreshMenu(false);
				});
			},
			onList = delegate(UIList.SortMode m)
			{
				foreach (FileInfo fileInfo in this.dir.GetFiles().Concat(MOD.listPartialMaps))
				{
					if (fileInfo.Name.EndsWith(".mp"))
					{
						MapMetaData metaData = Map.GetMetaData(fileInfo.FullName);
						if (metaData != null && metaData.partial != null)
						{
							this.list.Add(metaData);
						}
					}
				}
			}
		};
		this.list.List();
	}

	public void RefreshCategory(DirectoryInfo dir = null)
	{
		UIList _listCat = this.listCat;
		bool isMain = true;
		if (dir == null)
		{
			dir = this.rootDir;
		}
		if (dir.FullName != this.rootDir.FullName)
		{
			_listCat = this.listCatSub;
			isMain = false;
		}
		_listCat.Clear();
		_listCat.callbacks = new UIList.Callback<DirectoryInfo, UIButton>
		{
			onClick = delegate(DirectoryInfo a, UIButton b)
			{
				SE.Click();
				_listCat.Select(a, false);
				this.am.dir = a;
				if (isMain)
				{
					PartialMapMenu.lastDirSub = a;
					PartialMapMenu.lastDir = a;
					this.RefreshCategory(a);
				}
				else
				{
					PartialMapMenu.lastDirSub = a;
				}
				this.Refresh();
			},
			onInstantiate = delegate(DirectoryInfo a, UIButton b)
			{
				int num = 0;
				foreach (FileInfo fileInfo in a.GetFiles("*.mp", SearchOption.AllDirectories))
				{
					num++;
				}
				b.mainText.text = ((a == dir && !isMain) ? "(Root)" : (a.Name + "(" + num.ToString() + ")"));
			},
			onList = delegate(UIList.SortMode m)
			{
				_listCat.Add(isMain ? this.rootDir : dir);
				foreach (DirectoryInfo o in isMain ? new DirectoryInfo(CorePath.MapPieceSave).GetDirectories() : dir.GetDirectories())
				{
					_listCat.Add(o);
				}
			}
		};
		_listCat.List(false);
		this.listCatSub.SetActive(this.listCatSub.items.Count > 1);
	}

	public void OpenFolder()
	{
		Util.ShowExplorer(CorePath.MapPieceSave + "Test", false);
	}

	public static PartialMapMenu Instance;

	public static DirectoryInfo lastDir;

	public static DirectoryInfo lastDirSub;

	public GameObject goMain;

	public GameObject goCat;

	public GameObject goOption;

	public UIList listCat;

	public UIList listCatSub;

	public UIDynamicList list;

	public CanvasGroup cg;

	public UIButton buttonSave;

	public UIButton buttonEdit;

	public UIButton buttonDelete;

	public UIButton toggleRotate;

	public UIButton toggleIgnoreBlock;

	public Dictionary<string, Sprite> dictSprite = new Dictionary<string, Sprite>();

	public DirectoryInfo rootDir;
}
