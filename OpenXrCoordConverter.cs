using UnityEngine;

public class OpenXrCoordConverter
{
    // Vector3 von OpenXR zu Unity konvertieren (Position)
    public static Vector3 ConvertToUnityPosition(
        Vector3 openXrPosition)
    {
        return new Vector3(openXrPosition.x,
            openXrPosition.y,
            -openXrPosition.z);
    }

    // Quaternion von OpenXR zu Unity konvertieren
    private static Quaternion ConvertToUnityRotation(
        Quaternion openXrRotation)
    {
        return new Quaternion(-openXrRotation.x,
            openXrRotation.y, -openXrRotation.z,
            openXrRotation.w);
    }

    // OpenXR Quaternion zu Unity Richtungsvektor
    public static Vector3 ConvertRotationToUnityDirection(
        Quaternion originRotation, Quaternion openXrRotation)
    {
        return originRotation *
               ConvertToUnityRotation(openXrRotation) *
               Vector3.back;
    }
}