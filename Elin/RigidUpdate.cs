using System;
using UnityEngine;

public class RigidUpdate : EMono
{
	public CollectibleActor actor
	{
		get
		{
			CollectibleActor result;
			if ((result = this._actor) == null)
			{
				result = (this._actor = base.GetComponent<CollectibleActor>());
			}
			return result;
		}
	}

	public virtual void OnFixedUpdate()
	{
	}

	public static float delta;

	public static float leftX;

	public static float rightX;

	[NonSerialized]
	public Rigidbody2D rb;

	[NonSerialized]
	public bool active = true;

	[NonSerialized]
	public CollectibleActor _actor;
}
