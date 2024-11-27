using System;
using System.Collections.Generic;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class EloMapActor : EMono
{
	public bool IsActive
	{
		get
		{
			return base.gameObject.activeSelf;
		}
	}

	private void Awake()
	{
		this.SetActive(false);
	}

	public void Initialize(EloMap _elomap)
	{
		this.elomap = _elomap;
		if (!this.transMap)
		{
			this.transMap = Util.Instantiate<Transform>(this.elomap.idMap, null);
			this.transLight = UnityEngine.Object.Instantiate<Transform>(this.moldLight);
			this.lights.Clear();
			this.transMap.SetActive(false);
			this.transLight.SetActive(false);
			EMono.scene.screenElona.SetUnitSize();
		}
		if (!this.elomap.initialized)
		{
			this.elomap.Init(this);
		}
		STETilemap fogmap = this.elomap.fogmap;
		this.transMap.position = new Vector3((float)(-(float)fogmap.MinGridX) * EMono.scene.screenElona.tileAlign.x + EMono.scene.screenElona.actorPos.x, (float)(-(float)fogmap.MinGridY) * EMono.scene.screenElona.tileAlign.y + EMono.scene.screenElona.actorPos.y, EMono.scene.screenElona.actorPos.z);
	}

	public void OnActivate()
	{
		this.transLight.SetActive(true);
		foreach (EloMapLight eloMapLight in this.lights)
		{
			eloMapLight.sr.transform.position = TilemapUtils.GetGridWorldPos(this.elomap.fogmap, eloMapLight.gx, eloMapLight.gy);
		}
		this.OnChangeHour();
		this.elomap.objmap.UpdateMesh();
	}

	public void OnDeactivate()
	{
		if (this.transMap)
		{
			this.transMap.SetActive(false);
		}
		if (this.transLight)
		{
			this.transLight.SetActive(false);
		}
	}

	public void OnChangeHour()
	{
		Color white = Color.white;
		white.a = EMono.scene.profile.light.orbitAlphaCurve.Evaluate(EMono.scene.timeRatio);
		white.a *= white.a;
		foreach (EloMapLight eloMapLight in this.lights)
		{
			eloMapLight.sr.color = white;
		}
	}

	public EloMap GetEloMap()
	{
		this.Initialize(EMono.world.region.elomap);
		return this.elomap;
	}

	public void OnKillGame()
	{
		this.SetActive(false);
		if (this.transMap)
		{
			this.transLight.SetActive(false);
			this.transMap.SetActive(false);
		}
		if (this.transMap)
		{
			UnityEngine.Object.DestroyImmediate(this.transMap.gameObject);
			UnityEngine.Object.DestroyImmediate(this.transLight.gameObject);
			this.lights.Clear();
		}
	}

	public EloMap elomap;

	public Transform transMap;

	public Transform transLight;

	public Transform moldLight;

	public Tileset tileset;

	public EloMapTileSelector selector;

	public List<EloMapLight> lights = new List<EloMapLight>();
}
