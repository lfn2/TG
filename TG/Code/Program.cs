using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TG.TrustMetric;

namespace TG
{
	public class Program
	{
		static void Main(string[] args)
		{
			while (true)
			{
				Console.WriteLine("Choose your trust propagation metric");
				Console.WriteLine("1 - Linear Trust Propagation");
				Console.WriteLine("2 - Resource Allocation");

				int metricInput = Convert.ToInt32(Console.ReadLine());

				Console.WriteLine("Choose the propagation distance");

				int propagationDistance = Convert.ToInt32(Console.ReadLine());

				Console.WriteLine("Choose top N");
				int topN = Convert.ToInt32(Console.ReadLine());

				EMetric metric = (EMetric)metricInput;

				Matrix<float> originalTrustMatrix = new Matrix<float>(Resources.trust_data_file);

				Console.WriteLine("Building estimated trust matrix...");
				Matrix<float> estimatedTrustMatrix = GetEstimatedTrustMatrix(originalTrustMatrix, metric, propagationDistance, topN);
				Console.WriteLine("Estimated trust matrix built");

				Matrix<int> ratingsMatrix = new Matrix<int>(Resources.rating_data_file);

				Console.WriteLine("Running mean average error...");
				double mae = Experiment.MeanAverageError(ratingsMatrix, estimatedTrustMatrix);

				Console.WriteLine($"Mean Average Error: {mae}");
				Console.WriteLine();
			}
			
		}

		private static Matrix<float> GetEstimatedTrustMatrix(Matrix<float> originalTrustMatrix, EMetric metric, int propagationDistance, int topN)
		{
			Matrix<float> estimatedTrustMatrix;

			string matrixFile = GetMatrixFile(metric, propagationDistance, topN);

			if (File.Exists(matrixFile))
				estimatedTrustMatrix = new Matrix<float>(matrixFile);
			else
			{
				estimatedTrustMatrix = TrustMatrixBuilderFactory.GetMatrixBuilder(metric, originalTrustMatrix, topN).BuildMatrix(propagationDistance);
				estimatedTrustMatrix.WriteToFile(matrixFile);
			}

			return estimatedTrustMatrix;
		}
		
		private static string GetMatrixFile(EMetric metric, int propagationDistance, int topN)
		{
			string matrixFile = string.Empty;

			switch(metric)
			{
				case EMetric.LinearTrustPropagation:
					matrixFile = String.Format(Resources.linear_trust_matrix, propagationDistance);
					break;
				case EMetric.ResourceAllocation:
					matrixFile = String.Format(Resources.resource_allocation, propagationDistance, topN);
					break;
			}

			return matrixFile;
		}

	}
}
