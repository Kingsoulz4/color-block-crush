using System;
using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public static class QT
{

	const string KEY_64 = "mySecretKey1234567890engineer123gsedgsdg";
	const string IV_64 = "santhosh";

	public static string EnCrypt(string strContent, string strKey)
	{
		if (string.IsNullOrEmpty(strContent)) return string.Empty;
		if (strKey.Length > 8) strKey = strKey.Substring(0, 8); else strKey = KEY_64;
		byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(strKey);
		byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);
		DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
		int i = cryptoProvider.KeySize;
		MemoryStream ms = new MemoryStream();
		CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
		StreamWriter sw = new StreamWriter(cst);
		sw.Write(strContent);
		sw.Flush();
		cst.FlushFinalBlock();
		sw.Flush();
		return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
	}
	public static string DeCrypt(string strContent, string strKey)
	{
		if (string.IsNullOrEmpty(strContent)) return string.Empty;
		if (strKey.Length > 8) strKey = strKey.Substring(0, 8); else strKey = KEY_64;
		byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(strKey);
		byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);
		byte[] byEnc;
		try
		{
			byEnc = Convert.FromBase64String(strContent);
		}
		catch
		{
			return null;
		}
		DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
		MemoryStream ms = new MemoryStream(byEnc);
		CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
		StreamReader sr = new StreamReader(cst);
		return sr.ReadToEnd();
	}

    private static byte[] _cachedKey;
    private static string _lastPassword;


    public static readonly byte[] FixedSalt = Encoding.UTF8.GetBytes("MyFixedSalt1234");
    public const int KdfIterations = 30_000;   

    public static string EncryptAndCompress(string jsonString, string password)
    {
        byte[] raw = Encoding.UTF8.GetBytes(jsonString);
        
        byte[] compressed;
        using (var ms = new MemoryStream())
        {
            using (var brotli = new BrotliStream(ms, CompressionLevel.Fastest, leaveOpen: true))
            {
                brotli.Write(raw, 0, raw.Length);
            }
            compressed = ms.ToArray();
        }

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        
        using var kdf = new Rfc2898DeriveBytes(password, FixedSalt, KdfIterations, HashAlgorithmName.SHA256);
        aes.Key = kdf.GetBytes(32);
        aes.GenerateIV();
        byte[] iv = aes.IV;
        
        using var encryptor = aes.CreateEncryptor();
        byte[] cipher = encryptor.TransformFinalBlock(compressed, 0, compressed.Length);
        
        byte[] payload = new byte[iv.Length + cipher.Length];
        Buffer.BlockCopy(iv, 0, payload, 0, iv.Length);
        Buffer.BlockCopy(cipher, 0, payload, iv.Length, cipher.Length);
        return Convert.ToBase64String(payload);
    }

    private static byte[] CompressBrotli(byte[] data)
    {
        using var output = new MemoryStream();
        using (var brotli = new BrotliStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            brotli.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    public static string DecryptAndDecompress(string base64Input, string password)
    {
        byte[] data = Convert.FromBase64String(base64Input);
        byte[] iv = new byte[16];
        Array.Copy(data, 0, iv, 0, iv.Length);
        byte[] cipher = new byte[data.Length - iv.Length];
        Array.Copy(data, iv.Length, cipher, 0, cipher.Length);

        using var kdf = new Rfc2898DeriveBytes(password, FixedSalt, KdfIterations, HashAlgorithmName.SHA256);
        byte[] key = kdf.GetBytes(32);

        using var aes = Aes.Create();
        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Key = key;
        aes.IV = iv;

        byte[] compressed = aes.CreateDecryptor().TransformFinalBlock(cipher, 0, cipher.Length);

        using var msIn = new MemoryStream(compressed);
        using var brotli = new BrotliStream(msIn, CompressionMode.Decompress);
        using var msOut = new MemoryStream();
        brotli.CopyTo(msOut);
        return Encoding.UTF8.GetString(msOut.ToArray());
    }

    private static string DecompressBrotli(byte[] compressed)
    {
        using var input = new MemoryStream(compressed);
        using var brotli = new BrotliStream(input, CompressionMode.Decompress);
        using var output = new MemoryStream();
        brotli.CopyTo(output);
        return Encoding.UTF8.GetString(output.ToArray());
    }
}
