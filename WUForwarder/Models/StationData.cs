using System;
namespace WUForwarder.Models
{
    public class StationData
    {
        public float TemperatureC { get; }
        public float TemperatureF => convertToF(TemperatureC);
        public float DewPointC { get; }
        public float DewPointF => convertToF(DewPointC);
        public float Humidity { get; }
        public float PressureInch { get; }
        public float Voltage { get; }
        public float WindMph { get; }
        public float WindMph10mAvg { get; }
        public int WindDirection { get; }
        public float GustMph { get; }
        public int GustDirection { get; }


        public StationData(string data)
        {
            var parts = data.Split(';');
            WindMph = float.Parse(parts[0]);
            WindMph10mAvg = float.Parse(parts[1]);
            GustMph = float.Parse(parts[2]);
            GustDirection = int.Parse(parts[3]);
            WindDirection = int.Parse(parts[4]);
            PressureInch = float.Parse(parts[5]) * 0.02953f;
            Humidity = float.Parse(parts[6]);
            TemperatureC = float.Parse(parts[7]);
            Voltage = float.Parse(parts[8]);
            DewPointC = TemperatureC - ((100 - Humidity) / 5);
        }

        private static float convertToF(float value)
        {
            return (value * (9f / 5)) + 32;
        }
    }
}