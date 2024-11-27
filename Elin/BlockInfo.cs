using System;

public class BlockInfo : BaseInspectPos
{
	public static BlockInfo GetTemp(Point _pos)
	{
		BlockInfo.Temp.pos.Set(_pos);
		return BlockInfo.Temp;
	}

	public static BlockInfo GetTempList(Point _pos)
	{
		if (!BlockInfo.TempList.pos.Equals(_pos))
		{
			BlockInfo.TempList = new BlockInfo();
		}
		BlockInfo.TempList.pos.Set(_pos);
		return BlockInfo.TempList;
	}

	public SourceBlock.Row source
	{
		get
		{
			return this.pos.sourceBlock;
		}
	}

	public static bool _CanInspect(Point pos)
	{
		return pos.HasNonWallBlock;
	}

	public override bool CanInspect
	{
		get
		{
			return BlockInfo._CanInspect(this.pos);
		}
	}

	public override string InspectName
	{
		get
		{
			return this.source.GetName();
		}
	}

	public override void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		UIItem uiitem = n.AddHeaderCard(this.pos.cell.GetBlockName(), null);
		this.source.SetImage(uiitem.image2, null, this.source.GetColorInt(this.pos.matBlock), true, 0, 0);
		n.AddText("isMadeOf".lang(this.pos.cell.matBlock.GetText("name", false), null, null, null, null), FontColor.DontChange);
		n.Build();
	}

	public static BlockInfo Temp = new BlockInfo();

	public static BlockInfo TempList = new BlockInfo();
}
