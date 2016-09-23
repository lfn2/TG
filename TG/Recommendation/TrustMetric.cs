using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Recommendation
{
	public class TrustMetric
	{
		public static Dictionary<int, Dictionary<int, float>> EstimateTrust(Dictionary<int, Dictionary<int, float>> trustMatrix, int maxDistance)
		{
			Dictionary<int, Dictionary<int, float>> estimateMatrix = new Dictionary<int, Dictionary<int, float>>();

			foreach (int sourceUser in trustMatrix.Keys)
			{
				HashSet<int> queueItems = new HashSet<int>();
				Queue<int> distanceQueue = new Queue<int>();
				Queue<int> userQueue = new Queue<int>();

				foreach (int trustedUser in trustMatrix[sourceUser].Keys)
					if (!queueItems.Contains(trustedUser))
					{
						userQueue.Enqueue(trustedUser);
						distanceQueue.Enqueue(1);
						queueItems.Add(trustedUser);
					}

				while (userQueue.Count != 0)
				{
					int user = userQueue.Dequeue();
					int distance = distanceQueue.Dequeue();

					if (!estimateMatrix.ContainsKey(sourceUser))
						estimateMatrix[sourceUser] = new Dictionary<int, float>();

					estimateMatrix[sourceUser][user] = (float)(maxDistance - distance + 1) / maxDistance;

					if (trustMatrix.ContainsKey(user))
						foreach (int trustedUser in trustMatrix[user].Keys)
							if (!queueItems.Contains(trustedUser) && distance < maxDistance)
							{
								userQueue.Enqueue(trustedUser);
								distanceQueue.Enqueue(distance + 1);
								queueItems.Add(trustedUser);
							}
				}

			}

			return estimateMatrix;
		}

		public static SparseMatrix<float> EstimateTrust2(SparseMatrix<float> trustMatrix, int maxDistance)
		{
			SparseMatrix<float> estimateMatrix = new SparseMatrix<float>();

			foreach (int sourceUser in trustMatrix.GetRows().Keys)
			{
				HashSet<int> queueItems = new HashSet<int>();
				Queue<int> distanceQueue = new Queue<int>();
				Queue<int> userQueue = new Queue<int>();

				foreach (int trustedUser in trustMatrix.GetRows()[sourceUser].Keys)
					if (!queueItems.Contains(trustedUser))
					{
						userQueue.Enqueue(trustedUser);
						distanceQueue.Enqueue(1);
						queueItems.Add(trustedUser);
					}

				while (userQueue.Count != 0)
				{
					int user = userQueue.Dequeue();
					int distance = distanceQueue.Dequeue();

					if (estimateMatrix.GetAt(sourceUser, user) == default(float))
						estimateMatrix.SetAt(sourceUser, user, (float)(maxDistance - distance + 1) / maxDistance);

					if (trustMatrix.GetRows().ContainsKey(user))
						foreach (int trustedUser in trustMatrix.GetRows()[user].Keys)
							if (!queueItems.Contains(trustedUser) && distance < maxDistance)
							{
								userQueue.Enqueue(trustedUser);
								distanceQueue.Enqueue(distance + 1);
								queueItems.Add(trustedUser);
							}
				}
			}

			return estimateMatrix;
		}

	}
}
