using UnityEngine;
using System;

public class GazeDataPoint
{
    // Standardfelder
    public DateTime timestamp { get; set; } // UTC Timestamp

    public Vector3
        eyeGazePosition
    {
        get;
        set;
    } // Ursprungsposition von Gaze Ray

    public Vector3
        gazeDirection { get; set; } // Richtung von Gaze Ray

    public bool
        isHit
    {
        get;
        set;
    } // Ob Objekt (Collider) getroffen wurde

    public string
        hitObjectName
    {
        get;
        set;
    } // Name des getroffenen Objekts

    public float
        hitDistance
    {
        get;
        set;
    } // Distanz zu getroffenem Objekt

    public Vector3
        hitPosition
    {
        get;
        set;
    } // Punkt, wo Gaze Ray auf Objekt trifft

    public bool
        hasValidData
    {
        get;
        set;
    } // Ob Input Actions funktionieren

    // Felder für bewegte Objekte
    public Vector3
        localHitPosition
    {
        get;
        set;
    } // Hit Position in lokalen Koordinaten

    public bool
        isLocalTrackingEnabled
    {
        get;
        set;
    } // Ob lokale Koordinaten getrackt werden sollen

    // Felder die später berechnet werden
    public float
        velocity
    {
        get;
        set;
    } // Geschwindigkeit zwischen diesem und vorherigem Punkt

    public GazeCategory
        category { get; set; } // Augenbewegungsart

    public override string ToString()
    {
        return
            $"Timestamp: {timestamp}," +
            $"EyeGazePosition: {eyeGazePosition}," +
            $"GazeDirection: {gazeDirection}, " +
            $"IsHit: {isHit}," +
            $"HitObjectName: {hitObjectName}," +
            $"HitDistance: {hitDistance}," +
            $"HitPosition: {hitPosition}, " +
            $"HasValidData: {hasValidData}," +
            $"LocalHitPosition: {localHitPosition}," +
            $"IsLocalTrackingEnabled: " +
            $"{isLocalTrackingEnabled}, " +
            $"Velocity: {velocity}, Category: {category}";
    }
}