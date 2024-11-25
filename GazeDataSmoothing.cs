using System.Collections.Generic;
using UnityEngine;

public static class GazeDataSmoothing
{
    // Glättet Daten (HitPosition) mit Sliding Window
    public static void SmoothGazeData(
        GazeDataSeries dataSeries, int windowSize)
    {
        if (windowSize < 1)
        {
            Debug.LogError(
                "Window size must be at least 1.");
            return;
        }

        var smoothedPositions = new List<Vector3>();

        for (int i = 0; i < dataSeries.GetCount(); i++)
        {
            Vector3 sum = Vector3.zero;
            int count = 0;

            // Punkte im Fenster sammeln
            for (int j = Mathf.Max(0, i - windowSize + 1);
                 j <= i;
                 j++)
            {
                var point = dataSeries.GetDataPoint(j);
                if (point.hasValidData &&
                    point
                        .isHit)
                {
                    sum += point.hitPosition;


                    count++;
                }
            }

            if (count > 0)
            {
                // Durchschnittswert bilden
                smoothedPositions.Add(sum / count);
            }
            else
            {
                // Originalwert beibehalten,
                // wenn keine validen Punkte dabei sind
                smoothedPositions.Add(dataSeries
                    .GetDataPoint(i).hitPosition);
            }
        }

        // HitPosition Feld in GazeDataSeris aktualisieren
        for (int i = 0; i < dataSeries.GetCount(); i++)
        {
            var point = dataSeries.GetDataPoint(i);
            if (point.hasValidData &&
                point.isHit) // Valide Punkte aktualisieren
            {
                point.hitPosition = smoothedPositions[i];
            }
        }
    }
}