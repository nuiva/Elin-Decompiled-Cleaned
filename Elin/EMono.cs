using System;
using UnityEngine;

public class EMono : MonoBehaviour
{
	public static Game game
	{
		get
		{
			return EMono.core.game;
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
			return EMono.core.game.player;
		}
	}

	public static Chara pc
	{
		get
		{
			return EMono.core.game.player.chara;
		}
	}

	public static UI ui
	{
		get
		{
			return EMono.core.ui;
		}
	}

	public static Map _map
	{
		get
		{
			return EMono.core.game.activeZone.map;
		}
	}

	public static Zone _zone
	{
		get
		{
			return EMono.core.game.activeZone;
		}
	}

	public static FactionBranch Branch
	{
		get
		{
			return EMono.core.game.activeZone.branch;
		}
	}

	public static FactionBranch BranchOrHomeBranch
	{
		get
		{
			return EMono.Branch ?? EMono.pc.homeBranch;
		}
	}

	public static Faction Home
	{
		get
		{
			return EMono.core.game.factions.Home;
		}
	}

	public static Scene scene
	{
		get
		{
			return EMono.core.scene;
		}
	}

	public static BaseGameScreen screen
	{
		get
		{
			return EMono.core.screen;
		}
	}

	public static GameSetting setting
	{
		get
		{
			return EMono.core.gameSetting;
		}
	}

	public static GameData gamedata
	{
		get
		{
			return EMono.core.gamedata;
		}
	}

	public static ColorProfile Colors
	{
		get
		{
			return EMono.core.Colors;
		}
	}

	public static World world
	{
		get
		{
			return EMono.core.game.world;
		}
	}

	public static SoundManager Sound
	{
		get
		{
			return SoundManager.current;
		}
	}

	public static SourceManager sources
	{
		get
		{
			return EMono.core.sources;
		}
	}

	public static SourceManager editorSources
	{
		get
		{
			return Core.SetCurrent(null).sources;
		}
	}

	public static CoreDebug debug
	{
		get
		{
			return EMono.core.debug;
		}
	}

	public static int rnd(int a)
	{
		return Rand.rnd(a);
	}

	public static Core core;
}
