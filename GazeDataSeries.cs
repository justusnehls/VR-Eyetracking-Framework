using UnityEngine;
using System.Collections.Generic;
using System;

public class GazeDataSeries
{
    private readonly List<GazeDataPoint> _dataPoints = new();

    // Neuen Datenpunkt hinzufügen
    public void AddDataPoint(GazeDataPoint data)
    {
        _dataPoints.Add(data);
    }

    // Um spezifischen Datenpunkt abzugreifen
    public GazeDataPoint GetDataPoint(int index)
    {
        if (index >= 0 && index < _dataPoints.Count)
        {
            return _dataPoints[index];
        }

        throw new IndexOutOfRangeException(
            "Index is out of range");
    }

    // Anzahl der Datenpunkte bekommen
    public int GetCount()
    {
        return _dataPoints.Count;
    }

    // Dispersion für ein spezifisches Fenster berechnen
    public float CalculateGazeDispersion(int startIndex,
        int endIndex)
    {
        float minX = float.MaxValue, maxX = float.MinValue;
        float minY = float.MaxValue, maxY = float.MinValue;
        float minZ = float.MaxValue, maxZ = float.MinValue;

        // Berechne Min- und Max-Werte für valide Punkte
        for (int j = startIndex; j <= endIndex; j++)
        {
            var point = _dataPoints[j];

            // Überspringe ungültige Punkte
            if (!point.hasValidData || !point.isHit) continue;

            minX = Mathf.Min(minX, point.hitPosition.x);
            maxX = Mathf.Max(maxX, point.hitPosition.x);
            minY = Mathf.Min(minY, point.hitPosition.y);
            maxY = Mathf.Max(maxY, point.hitPosition.y);
            minZ = Mathf.Min(minZ, point.hitPosition.z);
            maxZ = Mathf.Max(maxZ, point.hitPosition.z);
        }

        return (maxX - minX) + (maxY - minY) + (maxZ - minZ);
    }


    // Geschwindigkeiten für valide Datenpunkte berechnen
    public void CalculateGazeVelocities()
    {
        for (var i = 1; i < _dataPoints.Count; i++)
        {
            var current = _dataPoints[i];
            var previous = _dataPoints[i - 1];

            // Überspringen, ein Punkt nicht valide ist
            if (!current.hasValidData || !current.isHit ||
                !previous.hasValidData || !previous.isHit)
            {
                current.velocity =
                    float.NaN; // NaN setzen
                continue;
            }

            // Zeitdifferenz Punkt berechnen
            var timeDifference =
                (float)
                (current.timestamp - previous.timestamp)
                .TotalSeconds;

            // Geschwindigkeit berechnen
            if (timeDifference > 0)
            {
                current.velocity =
                    Vector3.Distance(previous.hitPosition,
                        current.hitPosition) / timeDifference;
            }
            else
            {
                current.velocity =
                    float.NaN; // NaN setzen
            }
        }
    }


    // Fensterdauer berechnen
    public float GetWindowDuration(int startIndex,
        int endIndex)
    {
        if (startIndex < 0 || endIndex >= _dataPoints.Count ||
            startIndex > endIndex)
        {
            throw new IndexOutOfRangeException(
                "Invalid range specified for window " +
                "duration calculation.");
        }

        var startTime = _dataPoints[startIndex].timestamp;
        var endTime = _dataPoints[endIndex].timestamp;

        // Zeit in Millisekunden berechnen
        return (float)(endTime - startTime).TotalMilliseconds;
    }
}