using System;
using UnityEngine;

public class ModMinigame<T> : MiniGame where T : Component
{
	public void Load()
	{
		Debug.Log(this.path);
		if (!this.game)
		{
			this.asset = AssetBundle.LoadFromFile(this.path + "/Asset_" + this.id + "/asset");
			UnityEngine.Object @object = this.asset.LoadAsset(this.id);
			Debug.Log(@object);
			this.go = (UnityEngine.Object.Instantiate(@object) as GameObject);
			Debug.Log(this.go);
			this.game = this.go.GetComponentInChildren<T>();
		}
		base.SetAudioMixer(this.go);
		Debug.Log(this.game);
	}

	public void Kill()
	{
		UnityEngine.Object.Destroy(this.go);
		this.game = default(T);
		if (this.asset)
		{
			this.asset.Unload(true);
		}
	}

	public T game;
}
