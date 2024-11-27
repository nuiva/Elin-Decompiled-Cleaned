using System;
using System.IO;

public class TraitBlueprint : TraitScroll
{
	public override bool CanStackTo(Thing to)
	{
		return this.path == to.GetStr(54, null);
	}

	public string path
	{
		get
		{
			return this.owner.GetStr(54, null);
		}
		set
		{
			this.owner.SetStr(54, value);
		}
	}

	public override void OnRead(Chara c)
	{
		if (!EClass.debug.godBuild && !EClass._zone.IsPCFaction)
		{
			Msg.Say("skillbook_invalidZone");
			return;
		}
		if (!this.path.IsEmpty() && !File.Exists(this.path))
		{
			Msg.SayNothingHappen();
			return;
		}
		ActionMode.Blueprint.Activate(true, false);
		ActionMode.Blueprint.SetBlueprint(this);
	}

	public override void SetName(ref string s)
	{
		if (!this.owner.c_idRefName.IsEmpty())
		{
			s = "_written".lang(this.owner.c_idRefName, s, null, null, null);
		}
	}
}
