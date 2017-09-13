using Jayrock.Json.Conversion;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZZX.Util
{
    public class RSAUtil
    {
        public static string Encrypt(string data, string publickey, string charset)
        {
            byte[] dataInBytes = Encoding.GetEncoding(charset).GetBytes(data);
            byte[] btEncryptedSecret = Encrypt(dataInBytes, publickey);
            return Convert.ToBase64String(btEncryptedSecret);
        }

       
        public static string Decrypt(string data, string privateKey, String charset)
        {
            byte[] dataInBytes = Convert.FromBase64String(data);
            byte[] de = Decrypt(dataInBytes, privateKey);
            return Encoding.GetEncoding(charset).GetString(de);
        }

        public static string Sign(string data, string privateKey, string charset)
        {
            privateKey = ParseRSAPrivateKey(privateKey);
            RSACryptoServiceProvider rsaCsp = new RSACryptoServiceProvider();
            rsaCsp.FromXmlString(privateKey);

            byte[] dataBytes = null;
            if (string.IsNullOrEmpty(charset))
            {
                dataBytes = Encoding.GetEncoding(charset).GetBytes(data);
            }
            else
            {
                dataBytes = Encoding.GetEncoding(charset).GetBytes(data);
            }
            //这里签名用了RSA 应该用SHA256
            SHA256CryptoServiceProvider sha2 = new SHA256CryptoServiceProvider();
            //SHA1CryptoServiceProvider sha2 = new SHA1CryptoServiceProvider();
            byte[] signatureBytes = rsaCsp.SignData(dataBytes, "SHA256");
           
            
            return Convert.ToBase64String(signatureBytes);
        }

        
        public static bool Verify(string content, string signedString, string publicKey, string charset)
        {
            publicKey = ParseRSAPublicKey(publicKey);
            RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider();
            rsaPub.PersistKeyInCsp = false;
            rsaPub.FromXmlString(publicKey);

            byte[] data = Encoding.GetEncoding(charset).GetBytes(content);
            byte[] signedData = Convert.FromBase64String(signedString);

            SHA256CryptoServiceProvider sha2 = new SHA256CryptoServiceProvider(); //SHA256CryptoServiceProvider
            //SHA1CryptoServiceProvider sha2 = new SHA1CryptoServiceProvider();
            bool result = rsaPub.VerifyData(data, "SHA256", signedData);
            return result;
        }

        private static byte[] Encrypt(byte[] dataInBytes, string publickey)
        {
            publickey = ParseRSAPublicKey(publickey);
            RSACryptoServiceProvider rsaSender = new RSACryptoServiceProvider();
            rsaSender.FromXmlString(publickey);


            int keySize = 0;
            int blockSize = 0;
            int lastblockSize = 0;
            int counter = 0;
            int iterations = 0;
            int index = 0;

            byte[] btPlaintextToken;
            byte[] btEncryptedToken;
            byte[] btEncryptedSecret;

            keySize = rsaSender.KeySize / 8;
            blockSize = keySize - 11;

            if ((dataInBytes.Length % blockSize) != 0)
            {
                iterations = dataInBytes.Length / blockSize + 1;
            }
            else
            {
                iterations = dataInBytes.Length / blockSize;
            }

            btPlaintextToken = new byte[blockSize];			//See REMARK 2 below.
            btEncryptedSecret = new byte[iterations * keySize];	//See REMARK 2 below.
            for (index = 0, counter = 0; counter < iterations; counter++, index += blockSize)
            {
                if (counter == (iterations - 1)) //last block
                {
                    //Size of last block to be encrypted.
                    lastblockSize = dataInBytes.Length % blockSize;
                    if (lastblockSize == 0)
                    {
                        lastblockSize = blockSize;
                    }

                    //REMARK 1: Caution! The last block of buffer may be of different size!
                    //		    Don't copy into a block which is larger than what's actually there in the last block. 
                    //		    - you won't get exception, but last block will not be decrypted properly.
                    btPlaintextToken = new byte[lastblockSize];
                    Array.Copy(dataInBytes, index, btPlaintextToken, 0, lastblockSize);
                }
                else
                {
                    Array.Copy(dataInBytes, index, btPlaintextToken, 0, blockSize);
                }

                //REMARK 2: Okay, here's the TRICK! (Again, you won't find this on MSDN!)
                //(a) Size of blocks going in: blockSize (btPlaintextToken going in)
                //(b) Size of blocks going out: keySize (btEncryptedToken coming out)
                //If you don't do this carefully, you get cryptographic exception: "Bad Data"
                btEncryptedToken = rsaSender.Encrypt(btPlaintextToken, false); //set fOAEP to true only if you have the high encryption pack.
                Array.Copy(btEncryptedToken, 0, btEncryptedSecret, counter * keySize, keySize);
            }

            return btEncryptedSecret;
        }

        public static byte[] Decrypt(byte[] dataInBytes, string privateKey)
        {
            privateKey = ParseRSAPrivateKey(privateKey);
            int keySize = 0;
            int blockSize = 0;
            int counter = 0;
            int iterations = 0;
            int index = 0;
            int byteCount = 0;

            byte[] btPlaintextToken;
            byte[] btEncryptedToken;
            byte[] btDecryptedSecret;

            RSACryptoServiceProvider rsaReceiver = new RSACryptoServiceProvider();
            rsaReceiver.FromXmlString(privateKey);
            keySize = rsaReceiver.KeySize / 8;
            blockSize = keySize - 11;

            //REMARK 3: This should always be true: "btEncryptedSecret.Length % keySize == 0"
            if ((dataInBytes.Length % keySize) != 0)
            {
                //Error condition.
                return null;
            }

            iterations = dataInBytes.Length / keySize;

            btEncryptedToken = new byte[keySize];	//See REMARK 4 below.
            Queue tokenQueue = new Queue();
            for (index = 0, counter = 0; counter < iterations; index += blockSize, counter++)
            {

                Array.Copy(dataInBytes, counter * keySize, btEncryptedToken, 0, keySize);

                //REMARK 4: Okay, here's another trick:
                //(a) Encrypted blocks going in: btEncryptedToken / size: keySize
                //(b) Decrypted plain text coming out: btPlaintextToken / size: blockSize
                btPlaintextToken = rsaReceiver.Decrypt(btEncryptedToken, false);
                tokenQueue.Enqueue(btPlaintextToken);
            }

            byteCount = 0;
            foreach (byte[] PlaintextToken in tokenQueue)
            {
                //REMARK 5: Size of decrypted buffer depends on size of last block.
                byteCount += PlaintextToken.Length;
            }

            counter = 0;
            btDecryptedSecret = new byte[byteCount]; //REMARK 5 (see foreach loop above)
            foreach (byte[] PlaintextToken in tokenQueue)
            {
                if (counter == (iterations - 1))
                {
                    Array.Copy(PlaintextToken, 0, btDecryptedSecret, btDecryptedSecret.Length - PlaintextToken.Length, PlaintextToken.Length);
                }
                else
                {
                    Array.Copy(PlaintextToken, 0, btDecryptedSecret, counter * blockSize, blockSize);
                }

                counter++;
            }

            return btDecryptedSecret;
        }

        public static string ParseRSAPrivateKey(string privateKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        public static string ParseRSAPublicKey(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        public static string ParseBizResponse(string fullResponse, string privateKey, string charset)
        {
            IDictionary json = JsonConvert.Import(fullResponse) as IDictionary;
            if (json != null)
            {
                string encryptedNode = json[ZZXConstants.ENCRYPTED].ToString();

                bool encrypted = false;
                if (encryptedNode != null)
                {
                    encrypted = bool.Parse(encryptedNode);
                }

                string bizResponse = null;
                foreach (object key in json.Keys)
                {
                    string keyStr = key as string;
                    if (keyStr.EndsWith(ZZXConstants.RESPONSE_SUFFIX))
                    {
                        bizResponse = json[key] as string;
                        if (encrypted)
                        {
                            bizResponse = Decrypt(bizResponse, privateKey, charset);
                        }
                        break;
                    }
                }

                return bizResponse;
            }

            return null;
        }

        public static void VerifySign(string fullResponse, string decryptedBizRsp, string publicKey, string charset)
        {
            IDictionary json = JsonConvert.Import(fullResponse) as IDictionary;
            if (json != null)
            {
                string bizResponseSign = json[ZZXConstants.BIZ_RESPONSE_SIGN] as string;
                if (bizResponseSign != null)
                {
                    bool verifyResult = Verify(decryptedBizRsp, bizResponseSign, publicKey, charset);
                    if (verifyResult == false)
                    {
                        throw new ZZXException("check sign failed: " + json.ToString());
                    }

                }
            }
        }
    }


    /// <summary>
    /// RSA签名工具类。
    /// </summary>
    public class RSAUtil2
    {

        /// <summary>
        /// java公钥转C#所需公钥
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static string RSAPublicKeyJava2DotNet(string publicKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
                Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
                Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        }

        public static string RSAEncryptMore(string xmlPublicKey, string m_strEncryptString)
        {
            if (string.IsNullOrEmpty(m_strEncryptString))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(xmlPublicKey))
            {
                throw new ArgumentException("Invalid Public Key");
            }

            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                var inputBytes = Encoding.UTF8.GetBytes(m_strEncryptString);//有含义的字符串转化为字节流
                rsaProvider.FromXmlString(xmlPublicKey);//载入公钥
                int bufferSize = (rsaProvider.KeySize / 8) - 11;//单块最大长度
                var buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(inputBytes),
                     outputStream = new MemoryStream())
                {
                    while (true)
                    { //分段加密
                        int readSize = inputStream.Read(buffer, 0, bufferSize);
                        if (readSize <= 0)
                        {
                            break;
                        }

                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var encryptedBytes = rsaProvider.Encrypt(temp, false);
                        outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    }
                    return Convert.ToBase64String(outputStream.ToArray());//转化为字节流方便传输
                }
            }
        }





        #region  加密  
        /// <summary>  
        /// RSA加密  
        /// </summary>  
        /// <param name="publicKeyJava"></param>  
        /// <param name="data"></param>  
        /// <returns></returns>  
        public static string EncryptJava(string publicKeyJava, string data, string encoding = "UTF-8")
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            //byte[] cipherbytes;
            rsa.FromPublicKeyJavaString(publicKeyJava);

            //☆☆☆☆.NET 4.6以后特有☆☆☆☆  
            //HashAlgorithmName hashName = new System.Security.Cryptography.HashAlgorithmName(hashAlgorithm);  
            //RSAEncryptionPadding padding = RSAEncryptionPadding.OaepSHA512;//RSAEncryptionPadding.CreateOaep(hashName);//.NET 4.6以后特有                 
            //cipherbytes = rsa.Encrypt(Encoding.GetEncoding(encoding).GetBytes(data), padding);  
            //☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆  

            //☆☆☆☆.NET 4.6以前请用此段代码☆☆☆☆  
            //cipherbytes = rsa.Encrypt(Encoding.GetEncoding(encoding).GetBytes(data), false);

            using (var rsaProvider = new RSACryptoServiceProvider())
            {
                var inputBytes = Encoding.UTF8.GetBytes(data);//有含义的字符串转化为字节流                              
                int bufferSize = (rsa.KeySize / 8) - 11;//单块最大长度
                var buffer = new byte[bufferSize];
                using (MemoryStream inputStream = new MemoryStream(inputBytes),
                     outputStream = new MemoryStream())
                {
                    while (true)
                    { //分段加密
                        int readSize = inputStream.Read(buffer, 0, bufferSize);
                        if (readSize <= 0)
                        {
                            break;
                        }

                        var temp = new byte[readSize];
                        Array.Copy(buffer, 0, temp, 0, readSize);
                        var encryptedBytes = rsaProvider.Encrypt(temp, false);
                        outputStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    }
                    return Convert.ToBase64String(outputStream.ToArray());//转化为字节流方便传输
                }
            }
        }
        /// <summary>  
        /// RSA加密  
        /// </summary>  
        /// <param name="publicKeyCSharp"></param>  
        /// <param name="data"></param>  
        /// <returns></returns>  
        public static string EncryptCSharp(string publicKeyCSharp, string data, string encoding = "UTF-8")
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.FromXmlString(publicKeyCSharp);

            //☆☆☆☆.NET 4.6以后特有☆☆☆☆  
            //HashAlgorithmName hashName = new System.Security.Cryptography.HashAlgorithmName(hashAlgorithm);  
            //RSAEncryptionPadding padding = RSAEncryptionPadding.OaepSHA512;//RSAEncryptionPadding.CreateOaep(hashName);//.NET 4.6以后特有                 
            //cipherbytes = rsa.Encrypt(Encoding.GetEncoding(encoding).GetBytes(data), padding);  
            //☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆  

            //☆☆☆☆.NET 4.6以前请用此段代码☆☆☆☆  
            cipherbytes = rsa.Encrypt(Encoding.GetEncoding(encoding).GetBytes(data), false);

            return Convert.ToBase64String(cipherbytes);
        }

        /// <summary>  
        /// RSA加密PEM秘钥  
        /// </summary>  
        /// <param name="publicKeyPEM"></param>  
        /// <param name="data"></param>  
        /// <returns></returns>  
        public static string EncryptPEM(string publicKeyPEM, string data, string encoding = "UTF-8")
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            byte[] cipherbytes;
            rsa.LoadPublicKeyPEM(publicKeyPEM);

            //☆☆☆☆.NET 4.6以后特有☆☆☆☆  
            //HashAlgorithmName hashName = new System.Security.Cryptography.HashAlgorithmName(hashAlgorithm);  
            //RSAEncryptionPadding padding = RSAEncryptionPadding.OaepSHA512;//RSAEncryptionPadding.CreateOaep(hashName);//.NET 4.6以后特有                 
            //cipherbytes = rsa.Encrypt(Encoding.GetEncoding(encoding).GetBytes(data), padding);  
            //☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆☆  

            //☆☆☆☆.NET 4.6以前请用此段代码☆☆☆☆  
            cipherbytes = rsa.Encrypt(Encoding.GetEncoding(encoding).GetBytes(data), false);

            return Convert.ToBase64String(cipherbytes);
        }
        #endregion

        /// <summary>
        /// 解密公钥
        /// </summary>
        /// <param name="s"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <summary>
        /// 解密公钥
        /// </summary>
        /// <param name="s"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string DecryptByPublicKey(string s, string key)
        {
            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            //非对称加密算法，加解密用  
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());
            //解密  
            try
            {
                engine.Init(false, GetPublicKeyParameter(key));
                byte[] byteData = Convert.FromBase64String(s);
                var ResultData = engine.ProcessBlock(byteData, 0, byteData.Length);
                return System.Text.Encoding.UTF8.GetString(ResultData);
            }
            catch (Exception ex)
            {
                return ex.Message;

            }
        }

        //#region 私有属性
        ////private RSAParameters RSAKeyInfo;
        //private static RSACryptoServiceProvider RSA = null;
        ////私钥
        //private const string NET_PRIVATE_KEY = @"<RSAKeyValue><Modulus>ndSLc+4nW6DJbZKjs+UrQynUjxca1IPOIyfcZxPB7lpEQFUJWwpN+hDabWdVeFldNhaNSFg1UlQz4N2wPR030ui62ayyD66yEm0KCvAUOfw0fVhiEf/5CmoLSz+co6fAYvCf5GymwB0fjziiIorNvmZiAJyBNrm4JLbbvsoNDIU=</Modulus><Exponent>AQAB</Exponent><P>zS4nps270U327EPDQjcCQVQXSnOQILtJyiH8V0QoImQpT6a1dhFwLfe/bl/3L7nBr3PLk9nkPMtUdwXnZ6lrcQ==</P><Q>xOwSJfUODzVETrMc2D2947krqcR+XYubvPIsiDyeYqqMFQMYA+ONZKoExn3o1tb1ORvunTApH2d/f5qq6aJgVQ==</Q><DP>vwHio+QOnrDn19bVZUT0coCoFgUy/WWdMfElis/GVQ3Nb3sQntNpDUIAEe6AnQtehclUkVVcpkPbY9o5LEWJ4Q==</DP><DQ>JB0zOtjVSj63l0NL7/Bqyb+k3U6W6ir3VdCIEDglx+yFIjleByCNRr/Tfl+K+xOTB3Uy7ortj7/YZxuDarOHvQ==</DQ><InverseQ>Ueugp68z1cKJXLXSFz/LRJNd+uh4vVOBt6ndBtmJ+H4gI0JgBoL8QmR5X1iiD7v9LD+5cJng5k4uriil6cAeFw==</InverseQ><D>InIDqV59inrR2y8YuSc3xOW5NS1mtqC5eWS2rmxac8mRgbTNYOgj0oKhGSVnOufN9wL+/J37rSchV18qmnvo9bABSEMYNlTkViTgmAWdU3sIXa8EmFVS6sf6Ba+SBTYQLv8PyzxWXU3aXFdLGvU/WIY2QRYtIIL/mHsLrw3/p0E=</D></RSAKeyValue>";
        ////公钥参数
        //private const string PUB_KEY_MODULES = @"1lpnLvumD8/NedJ7s4WS8UO9OORbXVTgJXmfa72bI4A1L1l6Np91BETQ+yB8Fq6iGWw5OR8OB2UbRBcopb2etepDqWd7kmCtbVT36kTW+E8dWdaVjbI2BCXEGaXuzPPdGOlp52OaawYR5zyG0MiCvJ4jE7RDJax4Cl24ZqPUs4U=";
        ////公钥参数
        //private const string PUB_KEY_EXP = @"AQAB";
        //public RSAUtil()
        //{
        //}
        //#endregion
        //#region 私有方法
        ////取大头的数据
        //private static ushort readBEUInt16(BinaryReader br)
        //{
        //    byte[] b = br.ReadBytes(2);
        //    byte[] rb = new byte[2];
        //    rb[0] = b[1];
        //    rb[1] = b[0];
        //    return BitConverter.ToUInt16(rb, 0);
        //}
        //private static bool equalBytes(byte[] first, byte[] second)
        //{
        //    try
        //    {
        //        if (first.Length != second.Length)
        //        {
        //            return false;
        //        }
        //        for (int i = 0; i < first.Length; i++)
        //        {
        //            if (first[i] != second[i])
        //            {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return false;
        //    }
        //}
        //private static int getHead(BinaryReader br, byte elementFlag)
        //{
        //    try
        //    {
        //        int count = 0;
        //        byte bt = 0;
        //        bt = br.ReadByte();
        //        if (elementFlag != 0x00 && bt != elementFlag)
        //        {
        //            throw (new Exception("pem format err,element head : " + bt + " != " + elementFlag));
        //        }
        //        count = getElementLen(br);
        //        return count;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return -1;
        //    }
        //}
        //private static int getElementLen(BinaryReader br)
        //{
        //    try
        //    {
        //        ushort count = 0;
        //        byte bt = 0;
        //        bt = br.ReadByte();
        //        if (bt == 0x81)
        //        {
        //            count = br.ReadByte();
        //        }
        //        else if (bt == 0x82)
        //        {
        //            count = readBEUInt16(br); ;
        //        }
        //        else
        //        {
        //            count = bt;
        //        }
        //        return (int)count;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return -1;
        //    }
        //}
        //private static byte[] loadBytesFromPemFile(String fileName)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    using (StreamReader sr = new StreamReader(fileName))
        //    {
        //        String line;
        //        do
        //        {
        //            line = sr.ReadLine();
        //        } while (line != null && (line.Length == 0 || line.Substring(0, 1) != "-"));
        //        do
        //        {
        //            line = sr.ReadLine();
        //        } while (line != null && (line.Length == 0 || line.Substring(0, 1) == "-"));
        //        while (line != null && (line.Length == 0 || line.Substring(0, 1) != "-"))
        //        {
        //            sb.Append(line);
        //            line = sr.ReadLine();
        //        }
        //    }
        //    //Response.Write("base64:" + sb.ToString() + "<br>\n");
        //    return Convert.FromBase64String(sb.ToString());
        //}
        //private static byte[] stripLeftZeros(byte[] a)
        //{
        //    int lastZero = -1;
        //    for (int i = 0; i < a.Length; i++)
        //    {
        //        if (a[i] == 0)
        //        {
        //            lastZero = i;
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    lastZero++;
        //    byte[] result = new byte[a.Length - lastZero];
        //    Array.Copy(a, lastZero, result, 0, result.Length);
        //    return result;
        //}
        //private static byte[] getElement(BinaryReader br, byte elementFlag)
        //{
        //    try
        //    {
        //        int count = 0;
        //        byte bt = 0;
        //        bt = br.ReadByte();
        //        if (elementFlag != 0x00 && bt != elementFlag)
        //        {
        //            throw (new Exception("pem format err,element head : " + bt + " != " + elementFlag));
        //        }
        //        count = getElementLen(br);
        //        byte[] value = stripLeftZeros(br.ReadBytes(count));
        //        return value;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return null;
        //    }
        //}
        //#endregion
        //#region 公有方法
        ///// <summary>
        ///// 通过私key文件 获取RSAParameters
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <returns></returns>
        //public static RSAParameters getPrivateKeyFromPem(String fileName)
        //{
        //    byte[] keyBytes = loadBytesFromPemFile(fileName);
        //    RSAParameters para = new RSAParameters();
        //    BinaryReader br = new BinaryReader(new MemoryStream(keyBytes));
        //    byte bt = 0;
        //    ushort twoBytes = 0;
        //    twoBytes = readBEUInt16(br);
        //    if (twoBytes == 0x3081)
        //    {
        //        br.ReadByte();
        //    }
        //    else if (twoBytes == 0x3082)
        //    {
        //        br.ReadInt16();
        //    }
        //    else
        //    {
        //        throw (new Exception("pem format err,head 1: " + twoBytes + " != 0x3081 or 0x3082," + 0x3082));
        //    }
        //    twoBytes = readBEUInt16(br);
        //    bt = br.ReadByte();
        //    if (twoBytes != 0x0201 || bt != 0x00)
        //    {
        //        throw (new Exception("pem format err,head 2: " + twoBytes + " != 0x0201 or " + bt + " != 0x00"));
        //    }
        //    para.Modulus = getElement(br, 0x02);
        //    para.Exponent = getElement(br, 0x02);
        //    para.D = getElement(br, 0x02);
        //    para.P = getElement(br, 0x02);
        //    para.Q = getElement(br, 0x02);
        //    para.DP = getElement(br, 0x02);
        //    para.DQ = getElement(br, 0x02);
        //    para.InverseQ = getElement(br, 0x02);
        //    if (para.Equals(""))
        //    {
        //        throw (new Exception("pem format err,para=null!"));
        //    }
        //    return para;
        //}
        ///// <summary>
        ///// 通过公key文件 获取RSAParameters
        ///// </summary>
        ///// <param name="fileName"></param>
        ///// <returns></returns>
        //public static RSAParameters getPublicKeyFromPem(String fileName)
        //{
        //    byte[] keyBytes = loadBytesFromPemFile(fileName);
        //    RSAParameters para = new RSAParameters();
        //    BinaryReader br = new BinaryReader(new MemoryStream(keyBytes));
        //    byte bt = 0;
        //    ushort twoBytes = 0;
        //    //两个30开头的Sequence
        //    getHead(br, 0x30);
        //    getHead(br, 0x30);
        //    //{ 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 }
        //    byte[] correctOid = { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
        //    byte[] oid = getElement(br, 0x06);
        //    if (!equalBytes(correctOid, oid))
        //    {
        //        throw (new Exception("pem format err,oid err"));
        //    }
        //    bt = br.ReadByte();
        //    //05 00
        //    if (bt == 0x05)
        //    {
        //        br.ReadByte();
        //    }
        //    else
        //    {
        //        //已经获取了一个字节，只能调用两个函数组合，不能用getElement
        //        int len = getElementLen(br);
        //        br.ReadBytes(len);
        //    }
        //    //03开头的BitString，03+len+00
        //    getHead(br, 0x03);
        //    br.ReadByte();
        //    //30开头的Sequence
        //    getHead(br, 0x30);
        //    para.Modulus = getElement(br, 0x02);
        //    para.Exponent = getElement(br, 0x02);
        //    if (para.Equals(""))
        //    {
        //        throw (new Exception("pem format err,para=null!"));
        //    }
        //    return para;
        //}
        //public static bool verifySignature(byte[] signature, string signedData, RSAParameters pubPara)
        //{
        //    try
        //    {
        //        RSA = new RSACryptoServiceProvider();
        //        RSAParameters RSAParams = RSA.ExportParameters(false);
        //        RSACryptoServiceProvider RSA2 = new RSACryptoServiceProvider();
        //        //RSA2.ImportParameters(priPara)
        //        RSA2.ImportParameters(pubPara);
        //        byte[] hash = Encoding.UTF8.GetBytes(signedData);
        //        if (RSA2.VerifyData(hash, "SHA1", signature))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        return false;
        //    }
        //}
        ///// <summary>
        ///// 验证签名数据
        ///// </summary>
        ///// <param name="signature">秘钥</param>
        ///// <param name="signedData">明文</param>
        ///// <param name="pubFileName">公钥文件</param>
        ///// <returns></returns>
        //public static bool verifySignature(string signature, string signedData, string pubFileName)
        //{
        //    RSAParameters pubPara;
        //    pubPara = getPublicKeyFromPem(pubFileName);
        //    byte[] sign = Convert.FromBase64String(signature);
        //    //Convert.FromBase64String(signature);
        //    return verifySignature(sign, signedData, pubPara);
        //}
        ///// <summary>
        ///// 数据签名
        ///// </summary>
        ///// <param name="dataToBeSigned">需要加密的字符串</param>
        ///// <param name="priFileName">私钥文件</param>
        ///// <returns></returns>
        //public static string signData(string dataToBeSigned, string priFileName)
        //{
        //    RSAParameters priPara;
        //    priPara = getPrivateKeyFromPem(priFileName);
        //    RSA = new RSACryptoServiceProvider();
        //    //RSA.FromXmlString(NET_PRIVATE_KEY);
        //    RSAParameters RSAParams = RSA.ExportParameters(false);
        //    RSACryptoServiceProvider RSA2 = new RSACryptoServiceProvider();
        //    RSA2.ImportParameters(priPara);
        //    byte[] data = Encoding.UTF8.GetBytes(dataToBeSigned);
        //    byte[] endata = RSA2.SignData(data, "SHA1");
        //    return Convert.ToBase64String(endata);
        //}
        ///// <summary>
        ///// 数据加密
        ///// </summary>
        ///// <param name="dataSigned"></param>
        ///// <param name="pubFileName"></param>
        ///// <returns></returns>
        //public static string RSAEncrypt(string dataSign, string publicFileName)
        //{
        //    RSAParameters priPara;
        //    string hyxfmes = "";
        //    priPara = getPublicKeyFromPem(publicFileName);
        //    try
        //    {
        //        RSA = new RSACryptoServiceProvider();
        //        RSAParameters RSAParams = RSA.ExportParameters(false);
        //        RSACryptoServiceProvider RSA2 = new RSACryptoServiceProvider();
        //        RSA2.ImportParameters(priPara);
        //        byte[] hash = Encoding.UTF8.GetBytes(dataSign);
        //        byte[] de = RSA2.Encrypt(hash, false);
        //        hyxfmes = Convert.ToBase64String(de, Base64FormattingOptions.None);
        //        return hyxfmes;
        //    }
        //    catch (Exception e)
        //    {
        //        return "数据加密失败！";
        //    }
        //}
        ///// <summary>
        ///// 数据解密
        ///// </summary>
        ///// <param name="dataSigned"></param>
        ///// <param name="pubFileName"></param>
        ///// <returns></returns>
        //public static string RSADecrypt(string dataSigned, string privateFileName)
        //{
        //    RSAParameters pubPara;
        //    pubPara = getPrivateKeyFromPem(privateFileName);
        //    try
        //    {
        //        RSA = new RSACryptoServiceProvider();
        //        RSAParameters RSAParams = RSA.ExportParameters(false);
        //        RSACryptoServiceProvider RSA2 = new RSACryptoServiceProvider();
        //        RSA2.ImportParameters(pubPara);
        //        byte[] hash = Convert.FromBase64String(dataSigned);
        //        byte[] de = RSA2.Decrypt(hash, false);
        //        return Encoding.UTF8.GetString(de);
        //    }
        //    catch (Exception e)
        //    {
        //        return e.ToString();
        //    }
        //}
        //#endregion
        /// <summary>
        /// 获取公钥
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static AsymmetricKeyParameter GetPublicKeyParameter(string s)
        {
            s = s.Replace("\r", "").Replace("\n", "").Replace(" ", "");
            byte[] publicInfoByte = Convert.FromBase64String(s);
            Asn1Object pubKeyObj = Asn1Object.FromByteArray(publicInfoByte);//这里也可以从流中读取，从本地导入
            AsymmetricKeyParameter pubKey = PublicKeyFactory.CreateKey(publicInfoByte);
            return pubKey;
        }


    }

    public static class RSAExtensions
    {
        /// <summary>
        ///  把java的私钥转换成.net的xml格式
        /// </summary>
        /// <param name="rsa"></param>
        /// <param name="privateJavaKey"></param>
        /// <returns></returns>
        public static string ConvertToXmlPrivateKey(this RSA rsa, string privateJavaKey)
        {
            RsaPrivateCrtKeyParameters privateKeyParam = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateJavaKey));
            string xmlPrivateKey = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                         Convert.ToBase64String(privateKeyParam.Modulus.ToByteArrayUnsigned()),
                         Convert.ToBase64String(privateKeyParam.PublicExponent.ToByteArrayUnsigned()),
                         Convert.ToBase64String(privateKeyParam.P.ToByteArrayUnsigned()),
                         Convert.ToBase64String(privateKeyParam.Q.ToByteArrayUnsigned()),
                         Convert.ToBase64String(privateKeyParam.DP.ToByteArrayUnsigned()),
                         Convert.ToBase64String(privateKeyParam.DQ.ToByteArrayUnsigned()),
                         Convert.ToBase64String(privateKeyParam.QInv.ToByteArrayUnsigned()),
                         Convert.ToBase64String(privateKeyParam.Exponent.ToByteArrayUnsigned()));
            return xmlPrivateKey;
        }
        /// <summary>
        /// RSA加载JAVA  PrivateKey
        /// </summary>
        /// <param name="privateJavaKey">java提供的第三方私钥</param>
        /// <returns></returns>
        public static void FromPrivateKeyJavaString(this RSA rsa, string privateJavaKey)
        {
            string xmlPrivateKey = rsa.ConvertToXmlPrivateKey(privateJavaKey);
            rsa.FromXmlString(xmlPrivateKey);
        }

        /// <summary>
        /// 把java的公钥转换成.net的xml格式
        /// </summary>
        /// <param name="publicJavaKey">java提供的第三方公钥</param>
        /// <returns></returns>
        public static string ConvertToXmlPublicJavaKey(this RSA rsa, string publicJavaKey)
        {
            RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicJavaKey));
            string xmlpublicKey = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
              Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
              Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
            return xmlpublicKey;
        }

        /// <summary>
        /// 把java的私钥转换成.net的xml格式
        /// </summary>
        /// <param name="publicJavaKey">java提供的第三方公钥</param>
        /// <returns></returns>
        public static void FromPublicKeyJavaString(this RSA rsa, string publicJavaKey)
        {
            string xmlpublicKey = rsa.ConvertToXmlPublicJavaKey(publicJavaKey);
            rsa.FromXmlString(xmlpublicKey);
        }
        ///// <summary>
        ///// RSA公钥格式转换，java->.net
        ///// </summary>
        ///// <param name="publicKey">java生成的公钥</param>
        ///// <returns></returns>
        //private static string ConvertJavaPublicKeyToDotNet(this RSA rsa,string publicKey)
        //{           
        //    RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
        //    return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
        //        Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()),
        //        Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
        //}

        /// <summary>Extension method for initializing a RSACryptoServiceProvider from PEM data string.</summary>

        #region Methods

        /// <summary>Extension method which initializes an RSACryptoServiceProvider from a DER public key blob.</summary>
        public static void LoadPublicKeyDER(this RSACryptoServiceProvider provider, byte[] DERData)
        {
            byte[] RSAData = GetRSAFromDER(DERData);
            byte[] publicKeyBlob = GetPublicKeyBlobFromRSA(RSAData);
            provider.ImportCspBlob(publicKeyBlob);
        }

        /// <summary>Extension method which initializes an RSACryptoServiceProvider from a DER private key blob.</summary>
        public static void LoadPrivateKeyDER(this RSACryptoServiceProvider provider, byte[] DERData)
        {
            byte[] privateKeyBlob = GetPrivateKeyDER(DERData);
            provider.ImportCspBlob(privateKeyBlob);
        }

        /// <summary>Extension method which initializes an RSACryptoServiceProvider from a PEM public key string.</summary>
        public static void LoadPublicKeyPEM(this RSACryptoServiceProvider provider, string sPEM)
        {
            byte[] DERData = GetDERFromPEM(sPEM);
            LoadPublicKeyDER(provider, DERData);
        }

        /// <summary>Extension method which initializes an RSACryptoServiceProvider from a PEM private key string.</summary>
        public static void LoadPrivateKeyPEM(this RSACryptoServiceProvider provider, string sPEM)
        {
            byte[] DERData = GetDERFromPEM(sPEM);
            LoadPrivateKeyDER(provider, DERData);
        }

        /// <summary>Returns a public key blob from an RSA public key.</summary>
        internal static byte[] GetPublicKeyBlobFromRSA(byte[] RSAData)
        {
            byte[] data = null;
            UInt32 dwCertPublicKeyBlobSize = 0;
            if (CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING,
                new IntPtr((int)CRYPT_OUTPUT_TYPES.RSA_CSP_PUBLICKEYBLOB), RSAData, (UInt32)RSAData.Length, CRYPT_DECODE_FLAGS.NONE,
                data, ref dwCertPublicKeyBlobSize))
            {
                data = new byte[dwCertPublicKeyBlobSize];
                if (!CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING,
                    new IntPtr((int)CRYPT_OUTPUT_TYPES.RSA_CSP_PUBLICKEYBLOB), RSAData, (UInt32)RSAData.Length, CRYPT_DECODE_FLAGS.NONE,
                    data, ref dwCertPublicKeyBlobSize))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return data;
        }

        /// <summary>Converts DER binary format to a CAPI CRYPT_PRIVATE_KEY_INFO structure.</summary>
        internal static byte[] GetPrivateKeyDER(byte[] DERData)
        {
            byte[] data = null;
            UInt32 dwRSAPrivateKeyBlobSize = 0;
            IntPtr pRSAPrivateKeyBlob = IntPtr.Zero;
            if (CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING, new IntPtr((int)CRYPT_OUTPUT_TYPES.PKCS_RSA_PRIVATE_KEY),
                DERData, (UInt32)DERData.Length, CRYPT_DECODE_FLAGS.NONE, data, ref dwRSAPrivateKeyBlobSize))
            {
                data = new byte[dwRSAPrivateKeyBlobSize];
                if (!CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING, new IntPtr((int)CRYPT_OUTPUT_TYPES.PKCS_RSA_PRIVATE_KEY),
                    DERData, (UInt32)DERData.Length, CRYPT_DECODE_FLAGS.NONE, data, ref dwRSAPrivateKeyBlobSize))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return data;
        }

        /// <summary>Converts DER binary format to a CAPI CERT_PUBLIC_KEY_INFO structure containing an RSA key.</summary>
        internal static byte[] GetRSAFromDER(byte[] DERData)
        {
            byte[] data = null;
            byte[] publicKey = null;
            CERT_PUBLIC_KEY_INFO info;
            UInt32 dwCertPublicKeyInfoSize = 0;
            IntPtr pCertPublicKeyInfo = IntPtr.Zero;
            if (CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING, new IntPtr((int)CRYPT_OUTPUT_TYPES.X509_PUBLIC_KEY_INFO),
                DERData, (UInt32)DERData.Length, CRYPT_DECODE_FLAGS.NONE, data, ref dwCertPublicKeyInfoSize))
            {
                data = new byte[dwCertPublicKeyInfoSize];
                if (CryptDecodeObject(CRYPT_ENCODING_FLAGS.X509_ASN_ENCODING | CRYPT_ENCODING_FLAGS.PKCS_7_ASN_ENCODING, new IntPtr((int)CRYPT_OUTPUT_TYPES.X509_PUBLIC_KEY_INFO),
                    DERData, (UInt32)DERData.Length, CRYPT_DECODE_FLAGS.NONE, data, ref dwCertPublicKeyInfoSize))
                {
                    GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
                    try
                    {
                        info = (CERT_PUBLIC_KEY_INFO)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(CERT_PUBLIC_KEY_INFO));
                        publicKey = new byte[info.PublicKey.cbData];
                        Marshal.Copy(info.PublicKey.pbData, publicKey, 0, publicKey.Length);
                    }
                    finally
                    {
                        handle.Free();
                    }
                }
                else
                    throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            else
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return publicKey;
        }

        /// <summary>Extracts the binary data from a PEM file.</summary>
        internal static byte[] GetDERFromPEM(string sPEM)
        {
            UInt32 dwSkip, dwFlags;
            UInt32 dwBinarySize = 0;

            if (!CryptStringToBinary(sPEM, (UInt32)sPEM.Length, CRYPT_STRING_FLAGS.CRYPT_STRING_BASE64HEADER, null, ref dwBinarySize, out dwSkip, out dwFlags))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            byte[] decodedData = new byte[dwBinarySize];
            if (!CryptStringToBinary(sPEM, (UInt32)sPEM.Length, CRYPT_STRING_FLAGS.CRYPT_STRING_BASE64HEADER, decodedData, ref dwBinarySize, out dwSkip, out dwFlags))
                throw new Win32Exception(Marshal.GetLastWin32Error());
            return decodedData;
        }

        #endregion Methods

        #region P/Invoke Constants

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_ACQUIRE_CONTEXT_FLAGS : uint
        {
            CRYPT_NEWKEYSET = 0x8,
            CRYPT_DELETEKEYSET = 0x10,
            CRYPT_MACHINE_KEYSET = 0x20,
            CRYPT_SILENT = 0x40,
            CRYPT_DEFAULT_CONTAINER_OPTIONAL = 0x80,
            CRYPT_VERIFYCONTEXT = 0xF0000000
        }

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_PROVIDER_TYPE : uint
        {
            PROV_RSA_FULL = 1
        }

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_DECODE_FLAGS : uint
        {
            NONE = 0,
            CRYPT_DECODE_ALLOC_FLAG = 0x8000
        }

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_ENCODING_FLAGS : uint
        {
            PKCS_7_ASN_ENCODING = 0x00010000,
            X509_ASN_ENCODING = 0x00000001,
        }

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_OUTPUT_TYPES : int
        {
            X509_PUBLIC_KEY_INFO = 8,
            RSA_CSP_PUBLICKEYBLOB = 19,
            PKCS_RSA_PRIVATE_KEY = 43,
            PKCS_PRIVATE_KEY_INFO = 44
        }

        /// <summary>Enumeration derived from Crypto API.</summary>
        internal enum CRYPT_STRING_FLAGS : uint
        {
            CRYPT_STRING_BASE64HEADER = 0,
            CRYPT_STRING_BASE64 = 1,
            CRYPT_STRING_BINARY = 2,
            CRYPT_STRING_BASE64REQUESTHEADER = 3,
            CRYPT_STRING_HEX = 4,
            CRYPT_STRING_HEXASCII = 5,
            CRYPT_STRING_BASE64_ANY = 6,
            CRYPT_STRING_ANY = 7,
            CRYPT_STRING_HEX_ANY = 8,
            CRYPT_STRING_BASE64X509CRLHEADER = 9,
            CRYPT_STRING_HEXADDR = 10,
            CRYPT_STRING_HEXASCIIADDR = 11,
            CRYPT_STRING_HEXRAW = 12,
            CRYPT_STRING_NOCRLF = 0x40000000,
            CRYPT_STRING_NOCR = 0x80000000
        }

        #endregion P/Invoke Constants

        #region P/Invoke Structures

        /// <summary>Structure from Crypto API.</summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CRYPT_OBJID_BLOB
        {
            internal UInt32 cbData;
            internal IntPtr pbData;
        }

        /// <summary>Structure from Crypto API.</summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct CRYPT_ALGORITHM_IDENTIFIER
        {
            internal IntPtr pszObjId;
            internal CRYPT_OBJID_BLOB Parameters;
        }

        /// <summary>Structure from Crypto API.</summary>
        [StructLayout(LayoutKind.Sequential)]
        struct CRYPT_BIT_BLOB
        {
            internal UInt32 cbData;
            internal IntPtr pbData;
            internal UInt32 cUnusedBits;
        }

        /// <summary>Structure from Crypto API.</summary>
        [StructLayout(LayoutKind.Sequential)]
        struct CERT_PUBLIC_KEY_INFO
        {
            internal CRYPT_ALGORITHM_IDENTIFIER Algorithm;
            internal CRYPT_BIT_BLOB PublicKey;
        }

        #endregion P/Invoke Structures

        #region P/Invoke Functions

        /// <summary>Function for Crypto API.</summary>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptDestroyKey(IntPtr hKey);

        /// <summary>Function for Crypto API.</summary>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptImportKey(IntPtr hProv, byte[] pbKeyData, UInt32 dwDataLen, IntPtr hPubKey, UInt32 dwFlags, ref IntPtr hKey);

        /// <summary>Function for Crypto API.</summary>
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptReleaseContext(IntPtr hProv, Int32 dwFlags);

        /// <summary>Function for Crypto API.</summary>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptAcquireContext(ref IntPtr hProv, string pszContainer, string pszProvider, CRYPT_PROVIDER_TYPE dwProvType, CRYPT_ACQUIRE_CONTEXT_FLAGS dwFlags);

        /// <summary>Function from Crypto API.</summary>
        [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptStringToBinary(string sPEM, UInt32 sPEMLength, CRYPT_STRING_FLAGS dwFlags, [Out] byte[] pbBinary, ref UInt32 pcbBinary, out UInt32 pdwSkip, out UInt32 pdwFlags);

        /// <summary>Function from Crypto API.</summary>
        [DllImport("crypt32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptDecodeObjectEx(CRYPT_ENCODING_FLAGS dwCertEncodingType, IntPtr lpszStructType, byte[] pbEncoded, UInt32 cbEncoded, CRYPT_DECODE_FLAGS dwFlags, IntPtr pDecodePara, ref byte[] pvStructInfo, ref UInt32 pcbStructInfo);

        /// <summary>Function from Crypto API.</summary>
        [DllImport("crypt32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CryptDecodeObject(CRYPT_ENCODING_FLAGS dwCertEncodingType, IntPtr lpszStructType, byte[] pbEncoded, UInt32 cbEncoded, CRYPT_DECODE_FLAGS flags, [In, Out] byte[] pvStructInfo, ref UInt32 cbStructInfo);

        #endregion P/Invoke Functions
    }
}
