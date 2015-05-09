using System;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace MvcWorkoutApplication.Helpers
{
    public class RSAHelper
    {
        //private RsaPrivateCrtKeyParameters privKey;
        //private RsaKeyParameters pubKey;

        public RSAHelper(RsaPrivateCrtKeyParameters privKey, RsaKeyParameters pubKey)
        {
            this.PrivKey = privKey;
            this.PubKey = pubKey;
        }

        public RsaPrivateCrtKeyParameters PrivKey
        {
            get;
            set;
        }

        public RsaKeyParameters PubKey
        {
            get;
            set;
        }

        public static AsymmetricCipherKeyPair MakeKeyPair()
        {
            RsaKeyPairGenerator rsaGen = new RsaKeyPairGenerator();

            rsaGen.Init(new KeyGenerationParameters(new SecureRandom(), 1024));
            AsymmetricCipherKeyPair keyPair = rsaGen.GenerateKeyPair();

            return keyPair;
        }

        public byte[] encrypt(byte[] data)
        {
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());
            engine.Init(true, PubKey);
            //Console.WriteLine("RSA encryption algorithm: {0}", engine.AlgorithmName);

            return engine.ProcessBlock(data, 0, data.Length);
        }

        public byte[] decrypt(byte[] data)
        {
            IAsymmetricBlockCipher engine = new Pkcs1Encoding(new RsaEngine());
            engine.Init(false, PrivKey);

            return engine.ProcessBlock(data, 0, data.Length);
        }

        public byte[] sign(byte[] data)
        {
            ISigner sig = SignerUtilities.GetSigner("SHA1withRSA");
            sig.Init(true, PrivKey);

            sig.BlockUpdate(data, 0, data.Length);

            return sig.GenerateSignature();
        }

        public Boolean verify(byte[] data, byte[] signature)
        {
            ISigner sig = SignerUtilities.GetSigner("SHA1withRSA");
            sig.Init(false, PubKey);

            sig.BlockUpdate(data, 0, data.Length);

            return sig.VerifySignature(signature);
        }

        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        private string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
    }
}