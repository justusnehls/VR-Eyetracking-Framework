using UnityEngine;

public static class GazeRaycaster
{
    public static bool Raycast(Vector3 origin,
        Vector3 direction, float maxDistance,
        out RaycastHit hitInfo)
    {
        Ray ray = new Ray(origin, direction);

        // Raycast durchführen
        // return true wenn etwas getroffen wurde + hitInfo
        return Physics.Raycast(ray, out hitInfo, maxDistance);
    }
}