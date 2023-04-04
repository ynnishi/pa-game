using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Utility
{
    public static double[] Vector3toDoubleArray(Vector3 value)
    {
        return new double[3] { (double)value.x, (double)value.y, (double)value.z };
    }

    public static Vector3 DoubleArrayListToVector3(ArrayList value)
    {
        if (value.Count != 3)
        {
            Debug.LogError("Invalid Data");
            return Vector3.zero;
        }
        else
        {
            float vf0 = (float)Convert.ToDouble(value[0]);
            float vf1 = (float)Convert.ToDouble(value[1]);
            float vf2 = (float)Convert.ToDouble(value[2]);

            return new Vector3(vf0, vf1, vf2);
        }
    }

    public static string GenerateRandomAlphanumeric(int length = 44, bool removeMistakableChar = true)
    {
        string guid = Guid.NewGuid().ToString("N");

        string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(guid));

        str = str.Substring(0, length);

        if (removeMistakableChar)
        {
            //大文字化//
            str = str.ToUpper();

            //紛らわしい数字を置換する//
            StringBuilder sb = new StringBuilder(str);

            sb.Replace("0", "2");
            sb.Replace("O", "Z");
            sb.Replace("8", "4");
            sb.Replace("S", "Y");
            sb.Replace("1", "X"); //lと間違えやすい//
            sb.Replace("9", "6"); //qと間違えやすい//

            str = sb.ToString();
        }

        return str;
    }

    public static DateTime UtcToLocal(DateTime utcTime)
    {
        //デバイスのタイムゾーン設定からオフセットを取り出す。//
        //Japan Tokyoで offset = -9:00。DateTime.Nowの時刻を使っているわけではない。//
        TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now);
        return DateTime.SpecifyKind(utcTime + offset, DateTimeKind.Local);
    }

    public static T TryParse<T>(string value)
    {
        try
        {
            return (T)Enum.Parse(typeof(T), value);
        }
        catch
        {
            return default(T);
        }
    }

    public static List<T> ToList<T>(this ArrayList arrayList)
    {
        List<T> list = new List<T>(arrayList.Count);
        foreach (T instance in arrayList)
        {
            list.Add(instance);
        }
        return list;
    }

    public static T GetRandom<T>(this List<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

}