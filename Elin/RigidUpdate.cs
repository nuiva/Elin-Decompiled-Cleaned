using System;
using UnityEngine;

public class RigidUpdate : EMono
{
	public static float delta;

	public static float leftX;

	public static float rightX;

	[NonSerialized]
	public Rigidbody2D rb;

	[NonSerialized]
	public bool active = true;

	[NonSerialized]
	public CollectibleActor _actor;

	public CollectibleActor actor => _actor ?? (_actor = GetComponent<CollectibleActor>());

	public virtual void OnFixedUpdate()
	{
	}
}
