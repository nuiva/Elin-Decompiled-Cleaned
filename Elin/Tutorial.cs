using System;

public class Tutorial : EClass
{
	public static bool debugSkip;

	public static void Play(string idStep)
	{
		bool flag = debugSkip || !EClass.core.config.game.tutorial;
		int id = EClass.player.flags.GetStoryRowID("_tutorial", idStep);
		EClass.debug.Log(idStep + "/" + id + "/" + flag + "/" + debugSkip + "/" + EClass.player.flags.playedStories.Contains(id));
		if (EClass.player.flags.playedStories.Contains(id))
		{
			return;
		}
		if (!flag)
		{
			EClass.core.actionsNextFrame.Add(delegate
			{
				EClass.player.flags.PlayStory("_tutorial", id);
				if ((bool)LayerDrama.Instance)
				{
					LayerDrama.Instance.SetOnKill(delegate
					{
						AfterPlay();
					});
				}
			});
		}
		else
		{
			EClass.player.flags.playedStories.Add(id);
			AfterPlay();
		}
		void AfterPlay()
		{
			if (idStep == "first")
			{
				EClass.pc.PickOrDrop(EClass.pc.pos, ThingGen.Create("book_tutorial"));
			}
			if (id < 950)
			{
				Msg.Say("tutorial_added");
				SE.WriteJournal();
			}
		}
	}

	public static void Remove(string idStep)
	{
		int storyRowID = EClass.player.flags.GetStoryRowID("_tutorial", idStep);
		EClass.player.flags.playedStories.Remove(storyRowID);
	}

	public static void Reserve(string idStep, Action onBeforePlay = null)
	{
		if (!EClass.player.flags.reservedTutorial.Contains(idStep))
		{
			int storyRowID = EClass.player.flags.GetStoryRowID("_tutorial", idStep);
			if (!EClass.player.flags.playedStories.Contains(storyRowID))
			{
				onBeforePlay?.Invoke();
				EClass.player.flags.reservedTutorial.Add(idStep);
			}
		}
	}

	public static void TryPlayReserve()
	{
		if (!EClass.ui.IsActive && EClass.player.flags.reservedTutorial.Count != 0 && !LayerDrama.Instance)
		{
			Play(EClass.player.flags.reservedTutorial[0]);
			EClass.player.flags.reservedTutorial.RemoveAt(0);
		}
	}
}
