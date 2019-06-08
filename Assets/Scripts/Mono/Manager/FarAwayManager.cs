using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using My.Tools;

public class FarAwayManager : MonoSingleton<FarAwayManager>
{
	private struct Client {
		public int id;
		public Vector3 position;
		public float radius;

		public Client(int i, Vector3 p, float r) {
			this.id = i;
			this.position = p;
			this.radius = r;
		}
	}

	[SerializeField] private Vector3 basicFarAwayPosition = new Vector3(1000, 1000, 1000);
	[SerializeField, Range(1f, 100f)] private float basicRadius = 1f;

	private readonly Vector3 direction = Shared.vector3Right;
	private List<Client> clients = new List<Client>();




	public Vector3 GetFarAwayPosition(GameObject o) => this.GetFarAwayPosition(o, this.basicRadius);

	public Vector3 GetFarAwayPosition(GameObject o, float radius)
	{
		Vector3 pos;
		Client client;

		pos = this.basicFarAwayPosition;

		if(this.clients.Count > 0)
		{
			Client last = this.clients[this.clients.Count - 1];
			pos = last.position + this.direction * (last.radius + radius);
		}

		client = new Client(o.GetInstanceID(), pos, radius);

		this.clients.Add(client);
		this.clients.Sort(CompareClient);

		return pos;
	}

	public void ReleaseFarAwayPosition(GameObject o)
	{
		this.clients.RemoveAll(x => x.id == o.GetInstanceID());
		this.clients.Sort(CompareClient);
	}

	private static int CompareClient(Client x, Client y) => (x.position.x > y.position.x) ? 1 : -1;
}
