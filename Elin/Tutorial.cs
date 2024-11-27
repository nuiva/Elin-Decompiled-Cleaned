using System;

public class Tutorial : EClass
{
	public static void Play(string idStep)
	{
		Tutorial.<>c__DisplayClass1_0 CS$<>8__locals1 = new Tutorial.<>c__DisplayClass1_0();
		CS$<>8__locals1.idStep = idStep;
		bool flag = Tutorial.debugSkip || !EClass.core.config.game.tutorial;
		CS$<>8__locals1.id = EClass.player.flags.GetStoryRowID("_tutorial", CS$<>8__locals1.idStep);
		EClass.debug.Log(string.Concat(new string[]
		{
			CS$<>8__locals1.idStep,
			"/",
			CS$<>8__locals1.id.ToString(),
			"/",
			flag.ToString(),
			"/",
			Tutorial.debugSkip.ToString(),
			"/",
			EClass.player.flags.playedStories.Contains(CS$<>8__locals1.id).ToString()
		}));
		if (EClass.player.flags.playedStories.Contains(CS$<>8__locals1.id))
		{
			return;
		}
		if (!flag)
		{
			EClass.core.actionsNextFrame.Add(delegate
			{
				EClass.player.flags.PlayStory("_tutorial", CS$<>8__locals1.id, false);
				if (LayerDrama.Instance)
				{
					Layer instance = LayerDrama.Instance;
					Action onKill;
					if ((onKill = CS$<>8__locals1.<>9__2) == null)
					{
						onKill = (CS$<>8__locals1.<>9__2 = delegate()
						{
							base.<Play>g__AfterPlay|0();
						});
					}
					instance.SetOnKill(onKill);
				}
			});
			return;
		}
		EClass.player.flags.playedStories.Add(CS$<>8__locals1.id);
		CS$<>8__locals1.<Play>g__AfterPlay|0();
	}

	public static void Remove(string idStep)
	{
		int storyRowID = EClass.player.flags.GetStoryRowID("_tutorial", idStep);
		EClass.player.flags.playedStories.Remove(storyRowID);
	}

	public static void Reserve(string idStep, Action onBeforePlay = null)
	{
		if (EClass.player.flags.reservedTutorial.Contains(idStep))
		{
			return;
		}
		int storyRowID = EClass.player.flags.GetStoryRowID("_tutorial", idStep);
		if (EClass.player.flags.playedStories.Contains(storyRowID))
		{
			return;
		}
		if (onBeforePlay != null)
		{
			onBeforePlay();
		}
		EClass.player.flags.reservedTutorial.Add(idStep);
	}

	public static void TryPlayReserve()
	{
		if (EClass.ui.IsActive)
		{
			return;
		}
		if (EClass.player.flags.reservedTutorial.Count == 0)
		{
			return;
		}
		if (LayerDrama.Instance)
		{
			return;
		}
		Tutorial.Play(EClass.player.flags.reservedTutorial[0]);
		EClass.player.flags.reservedTutorial.RemoveAt(0);
	}

	public static bool debugSkip;
}
