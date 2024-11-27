using System;

public class ObjInfo : BaseInspectPos
{
	public static ObjInfo GetTemp(Point _pos)
	{
		if (!ObjInfo.Temp.pos.Equals(_pos))
		{
			ObjInfo.TempDate = new VirtualDate(0);
		}
		ObjInfo.Temp.pos.Set(_pos);
		return ObjInfo.Temp;
	}

	public static ObjInfo GetTempList(Point _pos)
	{
		if (!ObjInfo.TempList.pos.Equals(_pos))
		{
			ObjInfo.TempList = new ObjInfo();
		}
		ObjInfo.TempList.pos.Set(_pos);
		return ObjInfo.TempList;
	}

	public SourceObj.Row source
	{
		get
		{
			return this.pos.sourceObj;
		}
	}

	public static bool _CanInspect(Point pos)
	{
		return pos.HasObj && (!pos.cell.HasFullBlock || pos.sourceObj.tileType.IsBlockMount);
	}

	public override bool CanInspect
	{
		get
		{
			return ObjInfo._CanInspect(this.pos);
		}
	}

	public override string InspectName
	{
		get
		{
			string text = this.pos.cell.GetObjName() + (EClass.debug.showExtra ? (" " + this.pos.cell.matObj.alias) : "");
			if (this.pos.growth != null)
			{
				if (this.pos.growth.CanHarvest())
				{
					text = "growth_harvest".lang(text, null, null, null, null);
				}
				else if (this.pos.growth.IsWithered())
				{
					text = "growth_withered".lang(text, null, null, null, null);
				}
				else if (this.pos.growth.IsMature)
				{
					text = "growth_mature".lang(text, null, null, null, null);
				}
				if (this.pos.growth.NeedSunlight && !this.pos.growth.CanGrow(ObjInfo.TempDate))
				{
					text = "growth_nosun".lang(text, null, null, null, null);
					if (this.pos.cell.HasRoof)
					{
						text = "growth_roof".lang(text, null, null, null, null);
					}
				}
			}
			return text;
		}
	}

	public override void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		UIItem uiitem = n.AddHeaderCard(this.source.GetName(), null);
		this.source.SetImage(uiitem.image2, null, this.source.GetColorInt(this.pos.cell.matObj), true, 0, 0);
		n.AddText("isMadeOf".lang(this.source.GetText("name", false), null, null, null, null), FontColor.DontChange);
		n.Build();
	}

	public static ObjInfo Temp = new ObjInfo();

	public static ObjInfo TempList = new ObjInfo();

	public static VirtualDate TempDate = new VirtualDate(0);
}
