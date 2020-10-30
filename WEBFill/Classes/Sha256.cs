using System;
using System.IO;
using System.Security.Cryptography;

namespace WEBFill.Classes
{
    public static class Sha256
    {
        /// <summary>
        /// Возвращает хэш-значение для загружаемого файла .mp3
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetHash(string fileName)
        {
            using (FileStream stream = File.OpenRead(fileName))
            {
                var sha256 = new SHA256Managed();
                byte[] hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}
