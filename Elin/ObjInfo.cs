using System;

public class ObjInfo : BaseInspectPos
{
	public static ObjInfo Temp = new ObjInfo();

	public static ObjInfo TempList = new ObjInfo();

	public static VirtualDate TempDate = new VirtualDate();

	public SourceObj.Row source => pos.sourceObj;

	public override bool CanInspect => _CanInspect(pos);

	public override string InspectName
	{
		get
		{
			string text = pos.cell.GetObjName() + (EClass.debug.showExtra ? (" " + pos.cell.matObj.alias) : "");
			if (pos.growth != null)
			{
				if (pos.growth.CanHarvest())
				{
					text = "growth_harvest".lang(text);
				}
				else if (pos.growth.IsWithered())
				{
					text = "growth_withered".lang(text);
				}
				else if (pos.growth.IsMature)
				{
					text = "growth_mature".lang(text);
				}
				if ((EClass._zone.IsPCFaction || EClass._zone is Zone_Tent) && pos.growth.NeedSunlight && !pos.growth.CanGrow(TempDate))
				{
					text = "growth_nosun".lang(text);
					if (pos.cell.HasRoof)
					{
						text = "growth_roof".lang(text);
					}
				}
			}
			return text;
		}
	}

	public static ObjInfo GetTemp(Point _pos)
	{
		if (!Temp.pos.Equals(_pos))
		{
			TempDate = new VirtualDate();
		}
		Temp.pos.Set(_pos);
		return Temp;
	}

	public static ObjInfo GetTempList(Point _pos)
	{
		if (!TempList.pos.Equals(_pos))
		{
			TempList = new ObjInfo();
		}
		TempList.pos.Set(_pos);
		return TempList;
	}

	public static bool _CanInspect(Point pos)
	{
		if (pos.HasObj)
		{
			if (pos.cell.HasFullBlock)
			{
				return pos.sourceObj.tileType.IsBlockMount;
			}
			return true;
		}
		return false;
	}

	public override void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		UIItem uIItem = n.AddHeaderCard(source.GetName());
		source.SetImage(uIItem.image2, null, source.GetColorInt(pos.cell.matObj));
		n.AddText("isMadeOf".lang(source.GetText()));
		n.Build();
	}
}
