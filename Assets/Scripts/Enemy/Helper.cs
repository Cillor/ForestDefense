using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml.Serialization;

public static class Helper
{
    public static float Round(float n, float dp)
    {
        float stage1 = n * Mathf.Pow(10, dp);
        float stage2 = Mathf.Round(stage1);
        return stage2 / Mathf.Pow(10, dp);
    }

    public static string WattsNomenclature(float wattsConvert)
    {
        float watts;
        float aux = wattsConvert / 1000f;

        if (aux >= 1)
            watts = Round(aux, 2);
        else
            watts = wattsConvert;


        if (aux < 1f)
            return watts + " W";
        else if (aux < 1000)
            return watts + " kW";
        else if (aux < 1000000)
            return watts + " MW";
        else if (aux < 1000000000)
            return watts + " GW";
        else
            return watts + " ??";
    }

    public static string Serialize<T>(this T toSerialize)
    {
        XmlSerializer xml = new XmlSerializer(typeof(T));
        StringWriter writer = new StringWriter();
        xml.Serialize(writer, toSerialize);
        return writer.ToString();
    }

    public static T Deserialize<T>(this string toDeserialize)
    {
        XmlSerializer xml = new XmlSerializer(typeof(T));
        StringReader reader = new StringReader(toDeserialize);
        return (T)xml.Deserialize(reader);
    }
}
