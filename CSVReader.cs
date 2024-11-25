using UnityEngine;
using System;
using System.IO;
using System.Globalization;

public class CsvReader
{
    private readonly string _filePath;

    public CsvReader(string path)
    {
        _filePath = path;
    }

    public GazeDataSeries ReadCsv()
    {
        var gazeDataSeries = new GazeDataSeries();

        using (var reader = new StreamReader(_filePath))
        {
            // Kopfzeile überspringen
            reader.ReadLine();

            // Rest der Datei Zeile für Zeile lesen
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) continue;
                var values = line.Split(",");

                var dataPoint = ParseGazeData(values);

                gazeDataSeries.AddDataPoint(dataPoint);
            }
        }

        return gazeDataSeries;
    }

    private static GazeDataPoint ParseGazeData(
        string[] values)
    {
        var data = new GazeDataPoint
        {
            // Timestamp
            timestamp = DateTime.ParseExact(values[0],
                "yyyy-MM-ddTHH:mm:ss.fffZ",
                CultureInfo.InvariantCulture),

            // EyeGazePosition (X, Y, Z)
            eyeGazePosition = new Vector3(
                float.Parse(values[1],
                    CultureInfo.InvariantCulture),
                float.Parse(values[2],
                    CultureInfo.InvariantCulture),
                float.Parse(values[3],
                    CultureInfo.InvariantCulture)
            ),

            // GazeDirection (X, Y, Z)
            gazeDirection = new Vector3(
                float.Parse(values[4],
                    CultureInfo.InvariantCulture),
                float.Parse(values[5],
                    CultureInfo.InvariantCulture),
                float.Parse(values[6],
                    CultureInfo.InvariantCulture)
            ),

            // IsHit (true/false)
            isHit = bool.Parse(values[7]),

            // HitObjectName (string)
            hitObjectName = values[8],

            // HitDistance (float)
            hitDistance = float.Parse(values[9],
                CultureInfo.InvariantCulture),

            // HitPosition (X, Y, Z)
            hitPosition = new Vector3(
                float.Parse(values[10],
                    CultureInfo.InvariantCulture),
                float.Parse(values[11],
                    CultureInfo.InvariantCulture),
                float.Parse(values[12],
                    CultureInfo.InvariantCulture)
            ),

            // HasValidData (true/false)
            hasValidData = bool.Parse(values[13]),

            // LocalHitPosition (X, Y, Z)
            localHitPosition = new Vector3(
                float.Parse(values[14],
                    CultureInfo.InvariantCulture),
                float.Parse(values[15],
                    CultureInfo.InvariantCulture),
                float.Parse(values[16],
                    CultureInfo.InvariantCulture)
            ),

            // IsLocalTrackingEnabled (true/false)
            isLocalTrackingEnabled = bool.Parse(values[17]),

            // Velocity (float)
            velocity = float.Parse(values[18],
                CultureInfo.InvariantCulture),

            // Category (enum)
            category =
                (GazeCategory)Enum.Parse(
                    typeof(GazeCategory), values[19])
        };

        // GazeDataPoint Objekt zurückgeben
        return data;
    }
}