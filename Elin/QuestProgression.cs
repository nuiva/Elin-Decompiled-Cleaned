using System;

public class QuestProgression : QuestSequence
{
	public override string TitlePrefix
	{
		get
		{
			return "☆";
		}
	}

	public override string GetDetail(bool onJournal = false)
	{
		string text = base.source.GetDetail().Split('|', StringSplitOptions.None).TryGet(onJournal ? 1 : 0, -1);
		string text2 = this.GetTextProgress();
		if (!text2.IsEmpty())
		{
			text2 = "\n\n" + text2;
		}
		return GameLang.Convert(text) + text2;
	}
}
