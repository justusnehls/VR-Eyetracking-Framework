public static class GazeDataClassifier
{
    // Dispersionsbasiert
    public static void ClassifyUsingDispersion(
        GazeDataSeries dataSeries, float dispersionThreshold,
        float windowDurationMs)
    {
        int windowStart = 0;

        for (int i = 0; i < dataSeries.GetCount(); i++)
        {
            // Berechne die Dauer des Fensters (Zeit)
            float duration =
                dataSeries.GetWindowDuration(windowStart, i);

            // Solange Dauer kleiner als Mindestdauer ist,
            // Fenster erweitern
            if (duration < windowDurationMs)
            {
                continue;
            }

            // Dispersion für aktuelles Fenster ermitteln
            float dispersion =
                dataSeries.CalculateGazeDispersion(
                    windowStart, i);

            // Prüfen, ob Dispersion Schwellenwert erfüllt
            if (dispersion <= dispersionThreshold)
            {
                for (int j = windowStart; j <= i; j++)
                {
                    dataSeries.GetDataPoint(j).category =
                        GazeCategory.Fixation;
                }

                // Fenster auf nächsten Punkt
                // nach aktuellem Fenster verschieben
                windowStart = i + 1;
            }
            else
            {
                for (int j = windowStart; j <= i; j++)
                {
                    if (dataSeries.GetDataPoint(j)
                            .category ==
                        GazeCategory.Uncategorized)
                    {
                        dataSeries.GetDataPoint(j).category =
                            GazeCategory.Saccade;
                    }
                }

                // Fenster um 1 verschieben
                windowStart++;
            }
        }
    }

    // Geschwindigkeitsbasierte Klassifikation
    public static void ClassifyUsingVelocity(
        GazeDataSeries dataSeries, float velocityThreshold)
    {
        // Berechnung der Geschwindigkeiten
        dataSeries.CalculateGazeVelocities();

        for (int i = 0; i < dataSeries.GetCount(); i++)
        {
            var current = dataSeries.
                GetDataPoint(i);

            // Überspringen, wenn Geschwindigkeit ungültig
            if (float.IsNaN(current.velocity))
            {
                current.category =
                    GazeCategory
                        .Uncategorized;
                continue;
            }

            // Geschwindigkeit mit Schwellenwert vergleichen
            if (current.velocity > velocityThreshold)
            {
                current.category =
                    GazeCategory
                        .Saccade;
            }
            else if (current.isHit && current.hasValidData)
            {
                current.category =
                    GazeCategory
                        .Fixation;
            }
            else
            {
                current.category =
                    GazeCategory
                        .Uncategorized;
            }
        }
    }
}