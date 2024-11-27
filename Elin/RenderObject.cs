using System;
using System.Collections.Generic;
using UnityEngine;

public class RenderObject : EClass, IRenderer, ISyncScreen
{
	public long Sync
	{
		get
		{
			return this.sync;
		}
	}

	public virtual void OnEnterScreen()
	{
	}

	public virtual void OnLeaveScreen()
	{
	}

	public virtual void RenderToRenderCam(RenderParam p)
	{
	}

	public virtual void Draw(RenderParam p)
	{
	}

	public virtual void Draw(RenderParam p, ref Vector3 v, bool drawShadow)
	{
	}

	public static float gameDelta;

	public static float gameSpeed;

	public static float altitudeFix;

	public static GameSetting.RenderSetting.AnimeSetting animeSetting;

	public static GameSetting.RenderSetting renderSetting;

	public static RenderParam shared = new RenderParam();

	public static RenderParam currentParam;

	public static Vector3 tempV;

	public static bool enableAnime;

	public static List<ISyncScreen> syncList;

	public static long syncFrame;

	public bool usePass;

	public bool isSynced;

	public long sync;

	public RenderData data;
}
