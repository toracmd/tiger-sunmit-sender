﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace RenewEDSenderM.Support
{
    class Encryption
    {
        static UInt32[] table = { 
                             0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50a5, 0x60c6, 0x70e7, 0x8108, 0x9129, 0xa14a,
			0xb16b, 0xc18c, 0xd1ad, 0xe1ce, 0xf1ef, 0x1231, 0x0210, 0x3273, 0x2252, 0x52b5, 0x4294, 0x72f7, 0x62d6,
			0x9339, 0x8318, 0xb37b, 0xa35a, 0xd3bd, 0xc39c, 0xf3ff, 0xe3de, 0x2462, 0x3443, 0x0420, 0x1401, 0x64e6,
			0x74c7, 0x44a4, 0x5485, 0xa56a, 0xb54b, 0x8528, 0x9509, 0xe5ee, 0xf5cf, 0xc5ac, 0xd58d, 0x3653, 0x2672,
			0x1611, 0x0630, 0x76d7, 0x66f6, 0x5695, 0x46b4, 0xb75b, 0xa77a, 0x9719, 0x8738, 0xf7df, 0xe7fe, 0xd79d,
			0xc7bc, 0x48c4, 0x58e5, 0x6886, 0x78a7, 0x0840, 0x1861, 0x2802, 0x3823, 0xc9cc, 0xd9ed, 0xe98e, 0xf9af,
			0x8948, 0x9969, 0xa90a, 0xb92b, 0x5af5, 0x4ad4, 0x7ab7, 0x6a96, 0x1a71, 0x0a50, 0x3a33, 0x2a12, 0xdbfd,
			0xcbdc, 0xfbbf, 0xeb9e, 0x9b79, 0x8b58, 0xbb3b, 0xab1a, 0x6ca6, 0x7c87, 0x4ce4, 0x5cc5, 0x2c22, 0x3c03,
			0x0c60, 0x1c41, 0xedae, 0xfd8f, 0xcdec, 0xddcd, 0xad2a, 0xbd0b, 0x8d68, 0x9d49, 0x7e97, 0x6eb6, 0x5ed5,
			0x4ef4, 0x3e13, 0x2e32, 0x1e51, 0x0e70, 0xff9f, 0xefbe, 0xdfdd, 0xcffc, 0xbf1b, 0xaf3a, 0x9f59, 0x8f78,
			0x9188, 0x81a9, 0xb1ca, 0xa1eb, 0xd10c, 0xc12d, 0xf14e, 0xe16f, 0x1080, 0x00a1, 0x30c2, 0x20e3, 0x5004,
			0x4025, 0x7046, 0x6067, 0x83b9, 0x9398, 0xa3fb, 0xb3da, 0xc33d, 0xd31c, 0xe37f, 0xf35e, 0x02b1, 0x1290,
			0x22f3, 0x32d2, 0x4235, 0x5214, 0x6277, 0x7256, 0xb5ea, 0xa5cb, 0x95a8, 0x8589, 0xf56e, 0xe54f, 0xd52c,
			0xc50d, 0x34e2, 0x24c3, 0x14a0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405, 0xa7db, 0xb7fa, 0x8799, 0x97b8,
			0xe75f, 0xf77e, 0xc71d, 0xd73c, 0x26d3, 0x36f2, 0x0691, 0x16b0, 0x6657, 0x7676, 0x4615, 0x5634, 0xd94c,
			0xc96d, 0xf90e, 0xe92f, 0x99c8, 0x89e9, 0xb98a, 0xa9ab, 0x5844, 0x4865, 0x7806, 0x6827, 0x18c0, 0x08e1,
			0x3882, 0x28a3, 0xcb7d, 0xdb5c, 0xeb3f, 0xfb1e, 0x8bf9, 0x9bd8, 0xabbb, 0xbb9a, 0x4a75, 0x5a54, 0x6a37,
			0x7a16, 0x0af1, 0x1ad0, 0x2ab3, 0x3a92, 0xfd2e, 0xed0f, 0xdd6c, 0xcd4d, 0xbdaa, 0xad8b, 0x9de8, 0x8dc9,
			0x7c26, 0x6c07, 0x5c64, 0x4c45, 0x3ca2, 0x2c83, 0x1ce0, 0x0cc1, 0xef1f, 0xff3e, 0xcf5d, 0xdf7c, 0xaf9b,
			0xbfba, 0x8fd9, 0x9ff8, 0x6e17, 0x7e36, 0x4e55, 0x5e74, 0x2e93, 0x3eb2, 0x0ed1, 0x1ef0
                             };
        static byte[] AES_KEY = { };
        static byte[] AES_IV = { };
        static byte[] MD5_KEY = { };
        static string MD5_KEY_STR = "";
        static string encoding = "UTF-8";
        private static byte[] MD5_Encrypt(byte[] data)
        {
            // 1 普通md5，无密钥
            //MD5 md5 = new MD5CryptoServiceProvider();
            // 2 HMACMD5，有密钥
            System.Security.Cryptography.HMACMD5 md5 = new HMACMD5();
            md5.Key = Encoding.Default.GetBytes(MD5_KEY_STR);
            byte[] result = md5.ComputeHash(data);
            
            return result;
        }
        /// <summary>
        /// Hash an input string
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>the hash as a 32 character hexadecimal string</returns>
        public static string getMd5Hash(string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = MD5_Encrypt(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
        /// <summary>
        /// Verify a hash against a string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool verifyMd5Hash(string input, string hash)
        {
            // Hash the input.
            string hashOfInput = getMd5Hash(input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 加密AES：字符串加密为字节数组
        /// </summary>
        /// <param name="plainText">明文</param>
        /// <param name="Key">AES密钥</param>
        /// <param name="IV">AES初始向量</param>
        /// <returns>AES加密密文</returns>
        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;

        }
        public static void testcDes_aes()
        {
            int i;
            string s;
            s = DecryptStringFromBytes_Aes(DataPackage.GetBytes("AD4E729DCD81551ABC7C2983C86D3C6A58A862EAB7762371A0F6A3C83FBDC8EC79D54004F3249D83EE1D8BBE3C906270C99C44D6CF4C7B049B643A7DA312DCC0C5600F18FBB91ADF460802532B9FD2C766EC60477242D78F9CAA0092DA2F1B0492D92951E9EF1E68096B7EA76C41ABF30EB177D316196693D66C3C8AB2C767ED461BABF65A472C57DE9C972B10EDE61E916F38D678C48ED447928E77EF77798D71F71B29AA4A2542C866B4A623FDC6F8838B2D3AD851EC9091C029072A49BB9DDFCCF2831858E339FE73534A65432E5AC65F3E0DF735510BCD64F8B7C2B1F264", out i),
                Encoding.ASCII.GetBytes("0000000000123456"), Encoding.ASCII.GetBytes("0000000000123456"));
            s = DecryptStringFromBytes_Aes(DataPackage.GetBytes("AD4E729DCD81551ABC7C2983C86D3C6A58A862EAB7762371A0F6A3C83FBDC8ECB9D1BAFF2DB3F42557CAA390744DE28F3B9AB0A9B6440DEE139F4FAC3DFD275530E9FF04CB855447AB82D440167FADF0D238DB8E6BC8A06FD801570E6277632085CCA41809E7DEB74E9728283F88FE789532DF9595DC9DC752FB16FE613D4CA41C1A004EBD8D15BB2EE9E6C4252D7CC4C5B506B3B2CBBE017C4B0DA380E2757F67AA5DD5CE46E1091116BBD66D8420AA1EFD4009051F7BC483741DED1388CCAC9FA2AFAB46D73ED582D8F68BB96F0E3E36C6DB5EB47ED5B0E9ADD5B5F47C5B69E31610DD8F4E9C1FEB57C6E9A5EB967C88950536D936FB6D0541542394B6D82DB0976663320587C0A4FFE8EE918B784B", out i),
                Encoding.ASCII.GetBytes("0000000000123456"), Encoding.ASCII.GetBytes("0000000000123456"));

        }
        public static byte[] EncryptStringToBytes_Aes__(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");
            byte[] encrypted;
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.None;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            rijndaelCipher.Key = Key;
            rijndaelCipher.IV = IV;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();

            // Create the streams used for encryption.
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, transform, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {

                        //Write all data to the stream.
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
            return encrypted;
        }
        /// <summary>
        /// AES 128位
        /// 加密模式：CBC
        /// 填充模式采用：NoPadding
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        public static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.None;
            rijndaelCipher.KeySize = 128;
           
            //rijndaelCipher.BlockSize = 128;
            rijndaelCipher.Key = Key;
            rijndaelCipher.IV = IV;
            
            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            string plaintext = null;

            byte[] bplain = transform.TransformFinalBlock(cipherText, 0, cipherText.Length);
            plaintext = Encoding.UTF8.GetString(bplain);

            //using (MemoryStream msDecrypt = new MemoryStream(cipherText))
            //{
            //    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, transform, CryptoStreamMode.Read))
            //    {
            //        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
            //        {

            //            // Read the decrypted bytes from the decrypting stream and place them in a string.
            //            plaintext = srDecrypt.ReadToEnd();
            //        }
            //    }
            //}
            return plaintext;
        }
        /// <summary>
        /// 解密AES：字节数组解密为字符数组
        /// </summary>
        /// <param name="cipherText">密文, 密文为字节流</param>
        /// <param name="Key">AES密钥</param>
        /// <param name="IV">AES密钥向量</param>
        /// <returns>明文，明文为字符串</returns>
        public static string DecryptStringFromBytes_Aes__(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
         
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                
                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return plaintext;
        }
        /// <summary>
        /// Calculate the 16 bits length crc value of a buffer
        /// </summary>
        /// <param name="buffer">a byte array which to be calculated</param>
        /// <returns>The 16 bits length crc value</returns>
        public static  UInt16 CRC16(byte[] buffer)
        {
            UInt32 crc = 0;
            int l = buffer.Length;
            for (int i = 0; i < l; i++)
            {
                UInt32 by = (crc >> 8) & 0xff;
                crc = (crc & 0xffff) << 8;
                crc = (crc ^ table[(buffer[i] ^ by) & 0xff]) & 0xffff;
            }
            return (UInt16)crc;
        }
    }
}
