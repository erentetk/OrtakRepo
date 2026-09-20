namespace Sirket.Ortak
{
    /// <summary>
    /// Butun projelerin ortak kullandigi temel aritmetik islemler.
    /// Senin gercek durumunda burasi IDeviceMapper, RuntimeSettingStore
    /// gibi paylasilan siniflarin yeri olacak.
    /// </summary>
    public static class Hesaplama
    {
        public static double Topla(double a, double b) => a + b;

        public static double Cikar(double a, double b) => a - b;

        /// <summary>(1.1.0 ile eklendi)</summary>
        public static double Carp(double a, double b) => a * b;

         /// <summary>(1.2.0 ile eklendi)</summary>
        public static double Bol(double a, double b) => a / b;

    }
}
