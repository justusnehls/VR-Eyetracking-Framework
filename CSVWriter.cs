using System.Globalization;
using System.IO;

public class CsvWriter
{
    private readonly string _filePath;

    public CsvWriter(string path)
    {
        // Dateipfad
        _filePath = path;
    }

    public void WriteCsv(GazeDataSeries gazeDataSeries)
    {
        // CSV Datei öffnen und überschreiben
        using var writer =
            new StreamWriter(_filePath, false);

        // Kopfzeile
        writer.WriteLine(
            "Timestamp,EyeGazePosX,EyeGazePosY," +
            "EyeGazePosZ,GazeDirX,GazeDirY,GazeDirZ," +
            "IsHit,HitObjectName,HitDistance,HitPosX," +
            "HitPosY,HitPosZ,HasValidData,LocalHitPosX," +
            "LocalHitPosY,LocalHitPosZ," +
            "IsLocalTrackingEnabled,Velocity," +
            "Category");

        // Über GazeDataSeries iterieren
        for (var i = 0; i < gazeDataSeries.GetCount(); i++)
        {
            var dataPoint = gazeDataSeries.GetDataPoint(i);
            writer.WriteLine(FormatGazeData(dataPoint));
        }
    }

    // GazeDataPoint Objekt als String für CSV formatieren
    private static string FormatGazeData(GazeDataPoint data)
    {
        return string.Format(
            "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}," +
            "{11},{12},{13},{14},{15},{16},{17},{18},{19}",
            data.timestamp.ToString(
                "yyyy-MM-ddTHH:mm:ss.fffZ",
                CultureInfo.InvariantCulture),
            data.eyeGazePosition.x.ToString("F4",
                CultureInfo.InvariantCulture),
            data.eyeGazePosition.y.ToString("F4",
                CultureInfo.InvariantCulture),
            data.eyeGazePosition.z.ToString("F4",
                CultureInfo.InvariantCulture),
            data.gazeDirection.x.ToString("F4",
                CultureInfo.InvariantCulture),
            data.gazeDirection.y.ToString("F4",
                CultureInfo.InvariantCulture),
            data.gazeDirection.z.ToString("F4",
                CultureInfo.InvariantCulture),
            data.isHit,
            string.IsNullOrEmpty(data.hitObjectName)
                ? "None"
                : data.hitObjectName,
            data.hitDistance.ToString("F4",
                CultureInfo.InvariantCulture),
            data.hitPosition.x.ToString("F4",
                CultureInfo.InvariantCulture),
            data.hitPosition.y.ToString("F4",
                CultureInfo.InvariantCulture),
            data.hitPosition.z.ToString("F4",
                CultureInfo.InvariantCulture),
            data.hasValidData,
            data.localHitPosition.x.ToString("F4",
                CultureInfo.InvariantCulture),
            data.localHitPosition.y.ToString("F4",
                CultureInfo.InvariantCulture),
            data.localHitPosition.z.ToString("F4",
                CultureInfo.InvariantCulture),
            data.isLocalTrackingEnabled,
            data.velocity.ToString("F4",
                CultureInfo.InvariantCulture),
            data.category.ToString()
        );
    }
}