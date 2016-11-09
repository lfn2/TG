using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TG.TrustMetric
{
	public class ResourceAllocationIndexBuilder
	{

		public static Dictionary<int, HashSet<int>> BuildIndexes(Matrix<float> trustMatrix, int n)
		{
			object dictLock = new object();
			Dictionary<int, HashSet<int>> dict = new Dictionary<int, HashSet<int>>();

			Parallel.ForEach(trustMatrix.Rows,
							(user, loop) =>
							{
								List<KeyValuePair<int, float>> indexes = new List<KeyValuePair<int, float>>();
								foreach (int neighbour in trustMatrix[user])
								{
									float resourceAllocation = ResourceAllocationIndex(trustMatrix, user, neighbour);
									indexes.Add(new KeyValuePair<int, float>(neighbour, resourceAllocation));
								}

								if (indexes.Count > 0)
								{
									HashSet<int> set = new HashSet<int>();

									indexes.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

									for (int i = 0; i < n && i < indexes.Count; i++)
										set.Add(indexes[i].Key);

									lock (dictLock)
									{
										dict[user] = new HashSet<int>(set);
									}
								}
							});


			//foreach (int user in trustMatrix.Rows)
			//{
			//	List<KeyValuePair<int, float>> indexes = new List<KeyValuePair<int, float>>();
			//	foreach (int neighbour in trustMatrix[user])
			//	{
			//		float resourceAllocation = ResourceAllocationIndex(trustMatrix, user, neighbour);
			//		indexes.Add(new KeyValuePair<int, float>(neighbour, resourceAllocation));
			//	}

			//	if (indexes.Count > 0)
			//	{
			//		dict[user] = new HashSet<int>();

			//		indexes.Sort((pair1, pair2) => pair1.Value.CompareTo(pair2.Value));

			//		for (int i = 0; i < n && i < indexes.Count; i++)						
			//			dict[user].Add(indexes[i].Key);
			//	}
			//}

			return dict;
		}

		private static float ResourceAllocationIndex(Matrix<float> matrix, int x, int y)
		{
			float ra = 0;

			foreach (int neighbour in matrix[x])
			{
				if (matrix.HasRow(y) && matrix[y].Contains(neighbour) && matrix.HasRow(neighbour))
					ra += (float)1 / matrix[neighbour].Count();
			}

			return ra;
		}

	}
}
