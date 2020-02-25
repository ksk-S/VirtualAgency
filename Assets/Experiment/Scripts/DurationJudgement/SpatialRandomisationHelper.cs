using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Leap;

[System.Serializable]
public class SpatialRandomisationHelper {

    System.Random rnd = new System.Random(12);

    public bool isRandomised = false;

    Vector3[] originalPos;

    Quaternion[] RandomAngle;

    Vector3[] randomIniPos;

    public bool isRandomAngle = true;
    public bool isRandomShift = true;

    public void StartSpaceRandom(Vector3[] _spherePositions)
    {
        isRandomised = true;
        Debug.Log("Randomisation start");

        originalPos = (Vector3[])_spherePositions.Clone();

        // random angle
        RandomAngle = new Quaternion[originalPos.Length];
        for (int i = 0; i < originalPos.Length; i++)
        {
            float[] quad = new float[4];
            quad[0] = (float)rnd.NextDouble() * Mathf.PI;
            quad[1] = (float)rnd.NextDouble() * Mathf.PI;
            quad[2] = (float)rnd.NextDouble() * Mathf.PI;
            quad[3] = (float)rnd.NextDouble() * Mathf.PI;

            RandomAngle[i] = Quaternion.Normalize(new Quaternion(quad[0], quad[1], quad[2], quad[3]));
        }

        // random shift
        randomIniPos = (Vector3[])originalPos.Clone();

        ShuffleArray(randomIniPos);
        for (int i = 0; i < originalPos.Length; i++)
        {
            randomIniPos[i] = originalPos[i] - randomIniPos[i];
        }
    }

    public void StopSpaceRandom()
    {
        isRandomised = false;

        //Debug.Log("Randomisation stop");
    }

    public Vector3 ConvertCoordinate(int id, Vector3 current_pos)
    {
        if (isRandomised)
        {
            Vector3 vec = current_pos;

            if (isRandomAngle)
            {
                vec = RotateAroundPivot(vec, originalPos[id], RandomAngle[id]);
            }
            if (isRandomShift)
            {
                vec = vec - randomIniPos[id];
            }
            return vec;
        }
        else
        {
            return current_pos;
        }
    }

    public Vector3 RotateAroundPivot(Vector3 Point, Vector3 Pivot, Quaternion Angle)
    {
        return Angle * (Point - Pivot) + Pivot;
    }

    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void ShuffleArray(Vector3[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            Vector3 value = array[k];
            array[k] = array[n];
            array[n] = value;
        }
    }
}
