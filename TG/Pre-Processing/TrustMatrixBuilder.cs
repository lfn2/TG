using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TG;
using TG.DataBuild;
using TG.Recommendation;

namespace TG.Pre_Processing
{
	public class TrustMatrixBuilder
	{
		public static Matrix<float> BuildEstimatedTrustMatrix(Matrix<float> trustMatrix, int neighbourhoodDistance)
		{
			Matrix<float> estimatedTrustMatrix = new Matrix<float>();

			foreach (int sourceUser in trustMatrix.Rows)
			{
				HashSet<int> queueItems = new HashSet<int>();
				Queue<int> distanceQueue = new Queue<int>();
				Queue<int> userQueue = new Queue<int>();

				foreach (int trustedUser in trustMatrix[sourceUser])
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

					estimatedTrustMatrix[sourceUser, user] = (float)(neighbourhoodDistance - distance + 1) / neighbourhoodDistance;

					if (trustMatrix.Rows.Contains(user))
						foreach (int trustedUser in trustMatrix[user])
							if (!queueItems.Contains(trustedUser) && distance < neighbourhoodDistance)
							{
								userQueue.Enqueue(trustedUser);
								distanceQueue.Enqueue(distance + 1);
								queueItems.Add(trustedUser);
							}
				}

			}

			return estimatedTrustMatrix;
		}
	}
}
