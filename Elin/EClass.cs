using System;
using Newtonsoft.Json;

[JsonObject(MemberSerialization.OptIn)]
public class EClass
{
	public static Game game
	{
		get
		{
			return EClass.core.game;
		}
	}

	public static bool AdvMode
	{
		get
		{
			return ActionMode.IsAdv;
		}
	}

	public static Player player
	{
		get
		{
			return EClass.core.game.player;
		}
	}

	public static Chara pc
	{
		get
		{
			return EClass.core.game.player.chara;
		}
	}

	public static UI ui
	{
		get
		{
			return EClass.core.ui;
		}
	}

	public static Map _map
	{
		get
		{
			return EClass.core.game.activeZone.map;
		}
	}

	public static Zone _zone
	{
		get
		{
			return EClass.core.game.activeZone;
		}
	}

	public static FactionBranch Branch
	{
		get
		{
			return EClass.core.game.activeZone.branch;
		}
	}

	public static FactionBranch BranchOrHomeBranch
	{
		get
		{
			return EClass.Branch ?? EClass.pc.homeBranch;
		}
	}

	public static Faction Home
	{
		get
		{
			return EClass.core.game.factions.Home;
		}
	}

	public static Faction Wilds
	{
		get
		{
			return EClass.core.game.factions.Wilds;
		}
	}

	public static Scene scene
	{
		get
		{
			return EClass.core.scene;
		}
	}

	public static BaseGameScreen screen
	{
		get
		{
			return EClass.core.screen;
		}
	}

	public static GameSetting setting
	{
		get
		{
			return EClass.core.gameSetting;
		}
	}

	public static GameData gamedata
	{
		get
		{
			return EClass.core.gamedata;
		}
	}

	public static ColorProfile Colors
	{
		get
		{
			return EClass.core.Colors;
		}
	}

	public static World world
	{
		get
		{
			return EClass.core.game.world;
		}
	}

	public static SourceManager sources
	{
		get
		{
			return EClass.core.sources;
		}
	}

	public static SourceManager editorSources
	{
		get
		{
			return Core.SetCurrent(null).sources;
		}
	}

	public static SoundManager Sound
	{
		get
		{
			return SoundManager.current;
		}
	}

	public static CoreDebug debug
	{
		get
		{
			return EClass.core.debug;
		}
	}

	public static int rnd(int a)
	{
		return Rand.rnd(a);
	}

	public static int curve(int a, int start, int step, int rate = 75)
	{
		if (a <= start)
		{
			return a;
		}
		for (int i = 0; i < 10; i++)
		{
			int num = start + i * step;
			if (a <= num)
			{
				return a;
			}
			a = num + (a - num) * rate / 100;
		}
		return a;
	}

	public static int rndHalf(int a)
	{
		return a / 2 + Rand.rnd(a / 2);
	}

	public static float rndf(float a)
	{
		return Rand.rndf(a);
	}

	public static int rndSqrt(int a)
	{
		return Rand.rndSqrt(a);
	}

	public static void Wait(float a, Card c)
	{
		Game.Wait(a, c);
	}

	public static void Wait(float a, Point p)
	{
		Game.Wait(a, p);
	}

	public static int Bigger(int a, int b)
	{
		if (a <= b)
		{
			return b;
		}
		return a;
	}

	public static int Smaller(int a, int b)
	{
		if (a >= b)
		{
			return b;
		}
		return a;
	}

	public static Core core;
}
