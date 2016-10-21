using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Code
{
	public class BFSQueue
	{
		HashSet<int> queueItems;
		Queue<int> distanceQueue;
		Queue<int> userQueue;
		int maxDistance;

		public BFSQueue(int maxDistance)
		{
			queueItems = new HashSet<int>();
			distanceQueue = new Queue<int>();
			userQueue = new Queue<int>();
			this.maxDistance = maxDistance;
		}

		public void Enqueue(BFSNode node)
		{
			if (!queueItems.Contains(node.value) && node.distance <= maxDistance)
			{
				userQueue.Enqueue(node.value);
				distanceQueue.Enqueue(node.distance);
				queueItems.Add(node.value);
			}
		}

		public BFSNode Deque()
		{
			return new BFSNode(userQueue.Dequeue(), distanceQueue.Dequeue());
		}

		public int Count()
		{
			return userQueue.Count();
		}

	}

	public class BFSNode
	{
		public int value;
		public int distance;

		public BFSNode(int value, int distance)
		{
			this.value = value;
			this.distance = distance;
		}
	}
}
