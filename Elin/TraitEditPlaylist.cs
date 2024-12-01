public class TraitEditPlaylist : TraitItem
{
	public override bool OnUse(Chara c)
	{
		if (!EClass._zone.source.idPlaylist.IsEmpty() && !EClass.debug)
		{
			SE.Beep();
			Msg.Say("cantEditPlaylist");
			return false;
		}
		EClass.ui.AddLayer<LayerEditPlaylist>().Activate();
		return false;
	}
}
