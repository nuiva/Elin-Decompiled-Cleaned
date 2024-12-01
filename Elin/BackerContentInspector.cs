using System;

public class BackerContentInspector : EMono
{
	[Serializable]
	public class Content
	{
		public int id;

		public virtual string name => id.ToString() ?? "";

		public virtual void OnValidate()
		{
		}

		public virtual void Apply()
		{
		}

		public virtual void Remove()
		{
		}
	}

	public class ContentObj : Content
	{
		public Point p;

		public override string name => p.cell.GetObjName();

		public override void Apply()
		{
			EMono._map.ApplyBackerObj(p, id);
		}

		public override void Remove()
		{
			EMono._map.backerObjs.Remove(p.index);
		}
	}

	public class ContentCard : Content
	{
		public Card c;

		public override string name => c.id + "/" + c.Name;

		public override void Apply()
		{
			c.ApplyBacker(id);
		}

		public override void Remove()
		{
			c.RemoveBacker();
		}
	}

	public static BackerContentInspector Instance;

	public Content content;

	private void Awake()
	{
		Instance = this;
	}

	public void Apply()
	{
		content.Apply();
	}

	public void Remove()
	{
		content.Remove();
	}
}
