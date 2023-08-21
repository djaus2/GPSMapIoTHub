using System;

namespace GPSMap.Data
{
    /// <summary>
    /// Was found that GPS coords are out by a scale of about 1.7 and shifted by about 0..25 of a degree.
    /// This is a fudge fix for that.
    /// Two data points are required with the known corect GPS coords for each.
    /// The actual GPS value is also required.
    /// - The app records 10 and averages them.
    /// </summary>
    public static class ScaleGPS
    {
        private const int lat = 0;
        private const int lon = 1;

        public static double[] mGradient = new double[] { 1.0, 1.0 };
        private static double[] _gpsPoint1 = new double[2];
        private static double[] _gpsPoint2 = new double[2];
        private static double[] _mapPoint1 = new double[2];
        private static double[] _mapPoint2 = new double[2];

        private static double[] gpsPoint1 { get => _gpsPoint1; set => _gpsPoint1 = value; }
        private static double[] gpsPoint2 { get => _gpsPoint2; set => _gpsPoint2 = value; }
        private static double[] mapPoint1 { get => _mapPoint1; set => _mapPoint1 = value; }
        private static double[] mapPoint2 { get => _mapPoint2; set => _mapPoint2 = value; }

        public static void SetMap1(double[] mapPoint)
        {
            mapPoint1[lat] = mapPoint[lat];
            mapPoint1[lon] = mapPoint[lon];
        }

        public static void SetMap2(double[] mapPoint)
        {
            mapPoint2[lat] = mapPoint[lat];
            mapPoint2[lon] = mapPoint[lon];
        }

        public static void SetGPSDataPoin1( double[] gpsPoint)
        {
            gpsPoint1[lat] = gpsPoint[lat];
            gpsPoint1[lon] = gpsPoint[lon];
        }

        public static void SetGPSDataPoin2(double[] gpsPoint)
        {
            gpsPoint2[lat] = gpsPoint[lat];
            gpsPoint2[lon] = gpsPoint[lon];
        }


        public static void Calibrate ()
        {
            mGradient[lat] = (_mapPoint2[lat] - _mapPoint1[lat]) / (_gpsPoint2[lat] - _gpsPoint1[lat]);
            mGradient[lon] = (_mapPoint2[lon] - _mapPoint1[lon]) / (_gpsPoint2[lon] - _gpsPoint1[lon]);
        }

        private static double fixLat(double latR)
        {
            return _mapPoint1[lat] + mGradient[lat] * (latR - _gpsPoint1[lat]);
        }

        private  static double fixLong(double longR)
        {
            return _mapPoint1[lon] + mGradient[lon] * (longR - _gpsPoint1[lon]);
        }

        public static double[] FixGPS(double[] point)
        {
            double latitude = fixLat(point[lat]);
            double longitude = fixLong(point[lon]);
            return new double[] { latitude, longitude };
        }
    }

}
