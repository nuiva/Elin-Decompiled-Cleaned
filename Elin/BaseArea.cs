using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class BaseArea : EClass, IInspect
{
	public virtual string Name
	{
		get
		{
			if (!this.data.name.IsEmpty())
			{
				return this.data.name;
			}
			return this.type.source.GetName();
		}
	}

	public bool IsPrivate
	{
		get
		{
			return this.data.accessType == BaseArea.AccessType.Private;
		}
	}

	public SourceArea.Row source
	{
		get
		{
			return this.type.source;
		}
	}

	public Point GetRandomFreePos()
	{
		for (int i = 0; i < 100; i++)
		{
			Point point = this.points.RandomItem<Point>();
			if (!point.IsBlocked)
			{
				return point;
			}
		}
		return null;
	}

	public Thing GetEmptySeat()
	{
		foreach (Point point in this.points)
		{
			foreach (Thing thing in point.Things)
			{
				if (thing.IsInstalled && !thing.pos.HasChara && thing.trait is TraitChair)
				{
					return thing;
				}
			}
		}
		return null;
	}

	public virtual Point GetRandomPoint(bool walkable = true, bool allowChara = true)
	{
		if (walkable)
		{
			for (int i = 0; i < 100; i++)
			{
				foreach (Point point in this.points)
				{
					if (!point.IsBlocked && (allowChara || !point.HasChara))
					{
						return point;
					}
				}
			}
		}
		return this.points.RandomItem<Point>();
	}

	public void ChangeType(string _id)
	{
		this.type = ClassCache.Create<AreaType>("AreaType" + _id, "Elin");
		this.type.id = _id;
		this.type.owner = this;
		if (this.plate != null)
		{
			this.plate.areaData.type = this.type;
		}
	}

	public void SetRandomName(int seed = -1)
	{
		this.data.name = this.GetRandomName(seed);
	}

	public string GetRandomName(int seed = -1)
	{
		if (seed != -1)
		{
			Rand.SetSeed(seed);
		}
		FactionBranch branch = EClass.Branch;
		string combinedName = WordGen.GetCombinedName((branch != null) ? branch.GetRandomName() : null, this.ListRoomNames().RandomItem<string>(), true);
		if (seed != -1)
		{
			Rand.SetSeed(-1);
		}
		return combinedName;
	}

	public HashSet<string> ListRoomNames()
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (string item in Lang.GetList("rooms"))
		{
			hashSet.Add(item);
		}
		foreach (Point point in this.points)
		{
			foreach (Thing thing in point.Things)
			{
				if (thing.IsInstalled && !thing.source.roomName.IsEmpty())
				{
					foreach (string item2 in thing.source.GetTextArray("roomName"))
					{
						hashSet.Add(item2);
					}
				}
			}
		}
		return hashSet;
	}

	public int GetSortVal(UIList.SortMode m)
	{
		return this.source._index;
	}

	public List<BaseArea.Interaction> ListInteractions()
	{
		return new List<BaseArea.Interaction>
		{
			new BaseArea.Interaction
			{
				text = "accessType".lang(("access_" + this.data.accessType.ToString()).lang(), null, null, null, null),
				action = delegate()
				{
					UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
					using (List<BaseArea.AccessType>.Enumerator enumerator = Util.EnumToList<BaseArea.AccessType>().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							BaseArea.AccessType t = enumerator.Current;
							uicontextMenu.AddButton(((this.data.accessType == t) ? "context_checker".lang() : "") + ("access_" + t.ToString()).lang(), delegate()
							{
								this.data.accessType = t;
								if (this.plate != null)
								{
									this.plate.areaData.accessType = this.data.accessType;
								}
								SE.ClickOk();
							}, true);
						}
					}
					CursorSystem.ignoreCount = 5;
					uicontextMenu.Show();
				}
			},
			new BaseArea.Interaction
			{
				text = "changeName",
				action = delegate()
				{
					List<string> list = new List<string>();
					EClass.ui.AddLayer<LayerList>().SetStringList(delegate
					{
						list.Clear();
						for (int i = 0; i < 10; i++)
						{
							list.Add(this.GetRandomName(-1));
						}
						return list;
					}, delegate(int a, string b)
					{
						this.data.name = list[a];
						if (this.plate != null)
						{
							this.plate.areaData.name = list[a];
						}
					}, true).SetSize(450f, -1f).EnableReroll();
				}
			},
			new BaseArea.Interaction
			{
				text = "toggleShowWallItem".lang() + "(" + (this.data.showWallItem ? "on" : "off").lang() + ")",
				action = delegate()
				{
					this.data.showWallItem = !this.data.showWallItem;
					SE.ClickOk();
				}
			},
			new BaseArea.Interaction
			{
				text = "toggleAtrium".lang() + "(" + (this.data.atrium ? "on" : "off").lang() + ")",
				action = delegate()
				{
					this.data.atrium = !this.data.atrium;
					if (this.plate != null)
					{
						this.plate.areaData.atrium = this.data.atrium;
					}
					SE.ClickOk();
				}
			},
			new BaseArea.Interaction
			{
				text = "limitRoomHeight",
				action = delegate()
				{
					List<string> list = new List<string>();
					EClass.ui.AddLayer<LayerList>().SetStringList(delegate
					{
						list.Clear();
						for (int i = 1; i < 10; i++)
						{
							list.Add(i.ToString() ?? "");
						}
						return list;
					}, delegate(int a, string b)
					{
						this.data.maxHeight = a + 1;
						if (this.plate != null)
						{
							this.plate.areaData.maxHeight = a + 1;
						}
						EClass._map.rooms.RefreshAll();
					}, true);
				}
			},
			new BaseArea.Interaction
			{
				text = "changeGroup",
				action = delegate()
				{
					List<string> list = new List<string>();
					EClass.ui.AddLayer<LayerList>().SetStringList(delegate
					{
						list.Clear();
						for (int i = 0; i < 5; i++)
						{
							list.Add(i.ToString() ?? "");
						}
						return list;
					}, delegate(int a, string b)
					{
						this.data.group = a;
						if (this.plate != null)
						{
							this.plate.areaData.group = a;
						}
						EClass._map.rooms.RefreshAll();
					}, true);
				}
			}
		};
	}

	public void OnInspect()
	{
	}

	public bool CanInspect
	{
		get
		{
			return true;
		}
	}

	public string InspectName
	{
		get
		{
			return this.Name;
		}
	}

	public Point InspectPoint
	{
		get
		{
			return Point.Invalid;
		}
	}

	public void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		n.AddHeaderCard(this.Name, null);
		n.Build();
	}

	public Vector3 InspectPosition
	{
		get
		{
			return Vector3.zero;
		}
	}

	[JsonProperty]
	public int uid;

	[JsonProperty]
	public PointList points = new PointList();

	[JsonProperty]
	public AreaData data = new AreaData();

	[JsonProperty]
	public AreaType type = new AreaTypeRoom();

	public TraitRoomPlate plate;

	public enum AccessType
	{
		Public,
		Resident,
		Private
	}

	public class Interaction
	{
		public string text;

		public Action action;
	}
}
