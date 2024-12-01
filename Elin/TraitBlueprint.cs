using System.IO;

public class TraitBlueprint : TraitScroll
{
	public string path
	{
		get
		{
			return owner.GetStr(54);
		}
		set
		{
			owner.SetStr(54, value);
		}
	}

	public override bool CanStackTo(Thing to)
	{
		return path == to.GetStr(54);
	}

	public override void OnRead(Chara c)
	{
		if (!EClass.debug.godBuild && !EClass._zone.IsPCFaction)
		{
			Msg.Say("skillbook_invalidZone");
			return;
		}
		if (!path.IsEmpty() && !File.Exists(path))
		{
			Msg.SayNothingHappen();
			return;
		}
		ActionMode.Blueprint.Activate();
		ActionMode.Blueprint.SetBlueprint(this);
	}

	public override void SetName(ref string s)
	{
		if (!owner.c_idRefName.IsEmpty())
		{
			s = "_written".lang(owner.c_idRefName, s);
		}
	}
}
