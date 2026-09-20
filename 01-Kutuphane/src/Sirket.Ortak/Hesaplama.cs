namespace Sirket.Ortak
{
    /// <summary>
    /// Butun projelerin ortak kullandigi temel aritmetik islemler.
    /// Senin gercek durumunda burasi IDeviceMapper, RuntimeSettingStore
    /// gibi paylasilan siniflarin yeri olacak.
    /// </summary>
    public static class Hesaplama
    {   
        /// <summary>(1.0.0 ile eklendi)</summary>
        public static double Topla(double a, double b) => a + b;
        /// <summary>(1.0.0 ile eklendi)</summary>
        public static double Cikar(double a, double b) => a - b;

        /// <summary>(1.1.0 ile eklendi)</summary>
        public static double Carp(double a, double b) => a * b;

         /// <summary>(1.2.0 ile eklendi)</summary>
        public static double Bol(double a, double b) => a / b;

        /// <summary>(1.3.0 ile eklendi)</summary>
        public static double Yuzde(double sayi, double yuzde) => sayi * yuzde / 100;

        /// <summary>(1.4.0 ile eklendi)</summary>
        public static double Us(double taban, double kuvvet) => Math.Pow(taban, kuvvet);
    }
}
