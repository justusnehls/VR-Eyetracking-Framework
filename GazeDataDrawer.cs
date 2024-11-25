using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GazeDataDrawer : MonoBehaviour
{
    private GazeDataSeries _gazeDataSeries;

    private List<GameObject>
        _instantiatedHitPoints =
            new(); // Instantiierte Punkte tracken

    private void OnEnable()
    {
        var filePath = Path.Combine(
            Application.persistentDataPath,
            "EyeTrackingData_" +
            GazeDataRecorder.instance.classificationMethod +
            ".csv");

        var reader = new CsvReader(filePath);
        _gazeDataSeries = reader.ReadCsv();
        GazeDataClassifier.ClassifyUsingVelocity(
            _gazeDataSeries, 1);

        DrawHitPointsInScene();
    }

    // Punkte in Szene anzeigen
    private void DrawHitPointsInScene()
    {
        for (var i = 0; i < _gazeDataSeries.GetCount(); i++)
        {
            var dataPoint = _gazeDataSeries.GetDataPoint(i);

            if (dataPoint.isHit)
            {
                // Kugel für Punkt erstellen
                var sphere =
                    GameObject.CreatePrimitive(PrimitiveType
                        .Sphere);
                sphere.transform.position =
                    dataPoint.hitPosition;
                sphere.transform.localScale =
                    Vector3.one * 0.01f;

                // Farbe nach GazeCategory auswählen
                Color color;
                switch (dataPoint.category)
                {
                    case GazeCategory.Fixation:
                        color = Color.red;
                        break;
                    case GazeCategory.Saccade:
                        color = Color.blue;
                        break;
                    default:
                        color = Color.gray;
                        break;
                }

                sphere.GetComponent<Renderer>().material
                    .color = color;

                _instantiatedHitPoints
                    .Add(sphere); // Kugel tracken
            }
        }
    }


    // Instantiierte Objekte aus Szene wieder entfernen
    private void RemoveHitPointsFromScene()
    {
        foreach (var hitPoint in 
                 _instantiatedHitPoints)
        {
            if (hitPoint != null)
            {
                Destroy(hitPoint);
            }
        }

        _instantiatedHitPoints.Clear(); // Liste leeren
    }

    private void OnDisable()
    {
        RemoveHitPointsFromScene();
    }
}