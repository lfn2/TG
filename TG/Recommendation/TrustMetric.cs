using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.Recommendation
{
	public class TrustMetric
	{
		public static Dictionary<int, Dictionary<int, double>> EstimateTrust(Dictionary<int, Dictionary<int, double>> trustMatrix, int maxDistance)
		{
			Dictionary<int, Dictionary<int, double>> estimateMatrix = new Dictionary<int, Dictionary<int, double>>();

			foreach (int sourceUser in trustMatrix.Keys)
			{
				Dictionary<int, int> queueItems = new Dictionary<int, int>();
				Queue<int> queue = new Queue<int>();

				foreach (int trustedUser in trustMatrix[sourceUser].Keys)
					if (!queueItems.ContainsKey(trustedUser))
					{
						queue.Enqueue(trustedUser);
						queueItems.Add(trustedUser, 1);
					}

				while (queue.Count != 0)
				{
					int user = queue.Dequeue();
					int distance = queueItems[user];

					if (!estimateMatrix.ContainsKey(sourceUser))
						estimateMatrix[sourceUser] = new Dictionary<int, double>();

					estimateMatrix[sourceUser][user] = (double) (maxDistance - distance + 1) / maxDistance;

					if (trustMatrix.ContainsKey(user))					
						foreach (int trustedUser in trustMatrix[user].Keys)
							if (!queueItems.ContainsKey(trustedUser) && distance < maxDistance)
							{
								queue.Enqueue(trustedUser);
								queueItems.Add(trustedUser, distance + 1);
							}	
				}

			}

			return estimateMatrix;
		}
	}
}
