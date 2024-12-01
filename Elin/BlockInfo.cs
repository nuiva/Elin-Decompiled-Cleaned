using System;

public class BlockInfo : BaseInspectPos
{
	public static BlockInfo Temp = new BlockInfo();

	public static BlockInfo TempList = new BlockInfo();

	public SourceBlock.Row source => pos.sourceBlock;

	public override bool CanInspect => _CanInspect(pos);

	public override string InspectName => source.GetName();

	public static BlockInfo GetTemp(Point _pos)
	{
		Temp.pos.Set(_pos);
		return Temp;
	}

	public static BlockInfo GetTempList(Point _pos)
	{
		if (!TempList.pos.Equals(_pos))
		{
			TempList = new BlockInfo();
		}
		TempList.pos.Set(_pos);
		return TempList;
	}

	public static bool _CanInspect(Point pos)
	{
		return pos.HasNonWallBlock;
	}

	public override void WriteNote(UINote n, Action<UINote> onWriteNote = null, IInspect.NoteMode mode = IInspect.NoteMode.Default, Recipe recipe = null)
	{
		n.Clear();
		UIItem uIItem = n.AddHeaderCard(pos.cell.GetBlockName());
		source.SetImage(uIItem.image2, null, source.GetColorInt(pos.matBlock));
		n.AddText("isMadeOf".lang(pos.cell.matBlock.GetText()));
		n.Build();
	}
}
