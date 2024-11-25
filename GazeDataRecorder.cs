using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.IO;

public class GazeDataRecorder : MonoBehaviour
{
    // Maximaldistanz
    public float maxDistance = 100f;

    // Input Action für Augenposition
    public InputActionProperty eyeGazePositionAction;

    // Input Action für Augenrotation
    public InputActionProperty eyeGazeRotationAction;

    // InputAction ob Augen getrackt werden
    public InputActionProperty eyeGazeIsTrackedAction;

    // Transform des Ursprungs des Blickvektors (MainCamera)
    public Transform rayOriginTransform;

    // Layer mit Objekten deren relative hitPos getrackt wird
    public LayerMask exactTrackingLayer;

    // Klassifikationsmethode, die benutzt werden soll
    public ClassificationMethod classificationMethod;

    // Geschwindigkeitsschwellenwert für Klassifikation
    public float velocityTreshold = 0.2f;

    // Mindestdauer einer Fixation in Millisekunden
    public float minimumDuration = 150f;

    // Dispersionsschwellenwert für Klassifikation
    public float dispersionTreshold = 0.1f;

    // Sollen die Daten geglättet werden?
    public bool useSmoothing = true;

    // Fenstergröße für Glättung der Daten
    public int smoothingWindowSize = 15;

    private GazeDataSeries _gazeDataSeries;
    private GazeDataPoint _dataPoint;
    private CsvWriter _csvWriter;

    // Statische Singleton Instanz
    public static GazeDataRecorder instance
    {
        get;
        private set;
    }

    private void Awake()
    {
        // Sicherstellen, dass nur eine Instanz existiert
        if (instance != null && instance != this)
        {
            Debug.LogWarning(
                "Multiple GazeDataRecorder instances found," +
                " Destroying the duplicate.");
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnEnable()
    {
        // GazeDataSeries instantiieren
        _gazeDataSeries = new GazeDataSeries();
    }

    private void Update()
    {
        // Überprüfen, Input Actions verfügbar Werte ausgeben
        if (!eyeGazeIsTrackedAction.action.enabled ||
            eyeGazeIsTrackedAction.action
                .ReadValue<float>() == 0f)
        {
            // Sonst entsprechenden GazeDataPoint zurückgeben
            _dataPoint = new GazeDataPoint
            {
                timestamp = DateTime.UtcNow,
                hitObjectName = "No Object Hit",
                hasValidData = false
            };

            _gazeDataSeries.AddDataPoint(_dataPoint);
            return;
        }

        // Input Actions auslesen
        var eyeGazePosition = eyeGazePositionAction.action
            .ReadValue<Vector3>();
        var eyeGazeRotation = eyeGazeRotationAction.action
            .ReadValue<Quaternion>();

        // Position in Unity Koordinatensystem konvertieren
        eyeGazePosition =
            OpenXrCoordConverter.ConvertToUnityPosition(
                eyeGazePosition);

        // Augenposition in Weltkoordinaten konvertieren
        var worldEyeGazePosition =
            rayOriginTransform
                .TransformPoint(eyeGazePosition);

        // Rotation in Richtungsvektor umwandeln
        var eyeGazeDirection =
            OpenXrCoordConverter
                .ConvertRotationToUnityDirection(
                    rayOriginTransform.rotation,
                    eyeGazeRotation);

        // Variablen für Raycast
        var isHit = false;
        var hitObjectName = "None";
        var hitDistance = 0f;
        var hitPosition = Vector3.zero;
        Vector3 localHitPosition = Vector3.zero;
        var isLocalTrackingEnabled = false;

        // Raycast ausführen
        if (GazeRaycaster.Raycast(worldEyeGazePosition,
                eyeGazeDirection, maxDistance,
                out var hitInfo))
        {
            isHit = true;
            hitObjectName = hitInfo.collider.gameObject.name;
            hitDistance = hitInfo.distance;
            hitPosition = hitInfo.point;

            // Check ob relative Position getrackt werden soll
            if ((exactTrackingLayer.value &
                 (1 << hitInfo.collider.gameObject.layer)) !=
                0)
            {
                var localPosition =
                    hitInfo.collider.transform
                        .InverseTransformPoint(hitPosition);
                localHitPosition = new Vector3(
                    localPosition.x, localPosition.y,
                    localPosition.z);
                isLocalTrackingEnabled = true;
            }
        }

        // GazeDataPoint erstellen und mit Daten befüllen
        _dataPoint = new GazeDataPoint
        {
            timestamp = DateTime.UtcNow,
            eyeGazePosition = worldEyeGazePosition,
            gazeDirection = eyeGazeDirection,
            isHit = isHit,
            hitObjectName = hitObjectName,
            hitDistance = hitDistance,
            hitPosition = hitPosition,
            hasValidData = true,
            localHitPosition = localHitPosition,
            isLocalTrackingEnabled = isLocalTrackingEnabled
        };

        // GazeDataPoint zu GazeDataSeries hinzufügen
        _gazeDataSeries.AddDataPoint(_dataPoint);
    }

    private void OnDisable()
    {
        // Rohdaten schreiben
        var filePath = Path.Combine(
            Application.persistentDataPath,
            "EyeTrackingData_raw.csv");
        _csvWriter = new CsvWriter(filePath);
        _csvWriter.WriteCsv(_gazeDataSeries);

        // Gegebenenfalls Daten glätten
        if (useSmoothing)
        {
            GazeDataSmoothing.SmoothGazeData(_gazeDataSeries,
                smoothingWindowSize);
        }

        // Daten mit gewählter Methode klassifizieren
        switch (classificationMethod)
        {
            case ClassificationMethod.VelocityThreshold:
                GazeDataClassifier.ClassifyUsingVelocity(
                    _gazeDataSeries, velocityTreshold);
                break;

            case ClassificationMethod.DispersionThreshold:
                GazeDataClassifier.ClassifyUsingDispersion(
                    _gazeDataSeries, dispersionTreshold,
                    minimumDuration);
                break;

            default:
                Debug.LogWarning(
                    "Unknown classification " +
                    "method selected.");
                break;
        }

        // Klassifizierte Daten schreiben
        filePath = Path.Combine(
            Application.persistentDataPath,
            "EyeTrackingData_" + classificationMethod +
            ".csv");
        _csvWriter = new CsvWriter(filePath);
        _csvWriter.WriteCsv(_gazeDataSeries);
    }
}