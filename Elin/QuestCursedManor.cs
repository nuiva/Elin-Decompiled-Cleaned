public class QuestCursedManor : QuestSequence
{
	public override string TitlePrefix => "☆";

	public override bool HasDLC => Steam.HasDLC(ID_DLC.CursedManor);

	public override bool CanAutoAdvance => false;
}
