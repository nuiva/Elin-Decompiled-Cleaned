using System;

public class QuestSequence : Quest
{
	public SourceQuest.Row originalSource
	{
		get
		{
			return EClass.sources.quests.map[this.id];
		}
	}

	public override string idSource
	{
		get
		{
			return this.id + ((this.phase == 0) ? "" : (this.phase.ToString() ?? ""));
		}
	}

	public override string GetTitle()
	{
		string text = base.source.GetText("name", true);
		if (!text.IsEmpty())
		{
			return text;
		}
		for (int i = this.phase; i > 0; i--)
		{
			text = EClass.sources.quests.map[this.id].GetText("name", true);
			if (!text.IsEmpty())
			{
				return text;
			}
		}
		return this.originalSource.GetText("name", false);
	}

	public override void OnCompleteTask()
	{
		base.NextPhase();
	}
}
