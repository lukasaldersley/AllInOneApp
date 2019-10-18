using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace AllInOneApp
{
    class CryptoInterface
    {
        public static async Task<Boolean> IsInitial()
        {
            String x;
            try
            {
                x = await StorageInterface.ReadFromRoamingFolder("PWM/Passwords0");
            }
            catch(ArgumentOutOfRangeException EX)//Non-Unicode zeichen in dem verschlüsselten text können nicht richtig interpretiert werden, das heist aber, dass die Datei existiert und schon was drinsteht => reicht aus für diese überprüfung
            {
                x = EX.Message;//Eigentlicher inhalt der datei wird durch einen garantiert korrekten String ersetzt, falls die ursprüngliche nachricht nicht als String gelesen werden kann.
                EX.PrintStackTrace();
            }
            Debug.WriteLine("HAVE READ");
            bool X= (x == null || x.Equals("")) ? true : false;//falls entweder irgendwas in der datei steht oder die datei zeichen enthält die nicht gelesen werden können (d.h. die date nicht leer ist true sonst false;)
            Debug.WriteLine(X);
            return X;
        }

        public static async Task InitialSetup(CryptographicKey AesKey)
        {
            StoreRsaKey(GenerateRsaKey(), AesKey);
            await StorageInterface.WriteToRoamingFolder("PWM/Passwords0", "");
            await StorageInterface.WriteToRoamingFolder("PWM/INDEX", "1");
        }

        public static CryptographicKey GestAesKey(String Password)
        {
            return SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesCbcPkcs7).CreateSymmetricKey(CryptographicBuffer.ConvertStringToBinary(CreateFixedlengthString(128, Password), BinaryStringEncoding.Utf8));
        }

        public static async Task StorePasswords(String Passwords, CryptographicKey AesKey)
        {
            CryptographicKey k = await GetRasKey(AesKey);
            int a = int.Parse(await StorageInterface.ReadFromRoamingFolder("PWM/INDEX"));
            for (int i = 0; i < a; i++)
            {
                await StorageInterface.DeleteFromRoamingFolder("PWM/Passwords" + i);
            }


        String[] PWD = Passwords.DivideToLength(768);
            await StorageInterface.WriteToRoamingFolder("PWM/INDEX", PWD.Length.ToString());
            for (int i = 0; i < PWD.Length; i++)
            {
                IBuffer plain = CryptographicBuffer.ConvertStringToBinary(PWD[i], BinaryStringEncoding.Utf8);
                IBuffer cryptic = CryptographicEngine.Encrypt(k, plain, null);
                await StorageInterface.WriteBufferToRoamingFolder("PWM/Passwords" + i, cryptic);
            }
        }

        public static async Task<String> GetPasswords(CryptographicKey AesKey)
        {
            CryptographicKey k = await GetRasKey(AesKey);
            String R = "";
            try
            {
                int a = int.Parse(await StorageInterface.ReadFromRoamingFolder("PWM/INDEX"));
                for (int i = 0; i < a; i++)
                {
                    IBuffer crypitic = await StorageInterface.ReadBufferFromRoamingFolder("PWM/Passwords" + i);
                    IBuffer plain = DecryptRsa(k, crypitic);
                    String X = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, plain);
                    if (X == null)
                    {
                        X = "";
                    }
                    R += X;
                }
            }
            catch(Exception e)
            {
                e.PrintStackTrace();
                return "";
            }
            return R;
        }



        private static CryptographicKey GenerateRsaKey(uint bits = 8192)
        {
            return AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1).CreateKeyPair(bits);
        }

        private static async void StoreRsaKey(CryptographicKey key, CryptographicKey aesKey)
        {
            await StorageInterface.WriteBufferToRoamingFolder("PWM/RsaKey", EncryptAes(aesKey, key.Export()));
        }

        private static async Task<CryptographicKey> GetRasKey(CryptographicKey AesKey)
        {
            return AsymmetricKeyAlgorithmProvider.OpenAlgorithm(AsymmetricAlgorithmNames.RsaPkcs1).ImportKeyPair(DecryptAes(AesKey, await StorageInterface.ReadBufferFromRoamingFolder("PWM/RsaKey")));
        }



        public static IBuffer EncryptAes(CryptographicKey key, IBuffer data)
        {
            return Encrypt(key, data);
        }

        private static IBuffer EncryptRsa(CryptographicKey key, IBuffer data)
        {
            return Encrypt(key, data);
        }

        private static IBuffer Encrypt(CryptographicKey key, IBuffer data)
        {
            return CryptographicEngine.Encrypt(key, data, null);
        }

        private static IBuffer DecryptAes(CryptographicKey key, IBuffer data)
        {
            return Decrypt(key, data);
        }

        private static IBuffer DecryptRsa(CryptographicKey key, IBuffer data)
        {
            return Decrypt(key, data);
        }

        private static IBuffer Decrypt(CryptographicKey key, IBuffer data)
        {
            return CryptographicEngine.Decrypt(key, data, null);
        }

        private static String CreateFixedlengthString(int length, String str)
        {
            if (str.Length == 0)
            {
                return "";
            }
            char[] cArr = str.ToCharArray();
            if (str.Length < length)
            {
                int counter = 0;
                for (int i = str.Length; i < length; i++)
                {
                    if (counter == cArr.Length)
                    {
                        counter = 0;
                    }
                    str += cArr[counter];
                    counter++;
                }
            }
            else if (str.Length > length)
            {
                str = "";
                for (int i = 0; i < length; i++)
                {
                    str += cArr[i];
                }
            }
            return str;
        }

        private static String CreateRandomAlphanumericalStringOfFixedLength(int length)
        {
            Random random = new Random();
            String randomString = "";
            char[] availableChars = {
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
            };
            for (int i = 0; i < length; i++)
            {
                randomString += availableChars[((int)(random.NextDouble() * 61))];
            }
            return randomString;
        }

        public static IBuffer CreateTestfile()
        {
            return CryptographicBuffer.ConvertStringToBinary(CreateRandomAlphanumericalStringOfFixedLength(512), BinaryStringEncoding.Utf8);
        }
    }
}
