using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.TrustMetric
{
	public class ResourceAllocationPruner : MatrixPruner
	{
		protected override void Prune(Matrix<float> trustMatrix, int user, int n)
		{
			List<KeyValuePair<int, float>> raIndexes = new List<KeyValuePair<int, float>>();		
			foreach (int neighbour in trustMatrix[user])
			{
				float resourceAllocation = ResourceAllocationIndex(trustMatrix, user, neighbour);
				if (resourceAllocation != 0)
					raIndexes.Add(new KeyValuePair<int, float>(neighbour, resourceAllocation));
			}

			raIndexes.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

			for (int i = n; i < raIndexes.Count; i++)
				trustMatrix.remove(user, raIndexes[i].Key);
		}

		private float ResourceAllocationIndex(Matrix<float> trustMatrix, int x, int y)
		{
			float ra = 0;

			foreach (int neighbour in trustMatrix[x])
			{
				if (trustMatrix.HasRow(y) && trustMatrix[y].Contains(neighbour) && trustMatrix.HasRow(neighbour))
					ra += (float)1 / trustMatrix[neighbour].Count();
			}

			return ra;
		}
	}
}
