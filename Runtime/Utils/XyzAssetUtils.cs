using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace XyzAssets.Runtime
{
    public static class StringUtility
    {
        [ThreadStatic]
        private static readonly StringBuilder _sharedStringBuilder = new StringBuilder(1024);
        public static string Format(string format, params object[] args)
        {
            if (string.IsNullOrEmpty(format))
                throw new ArgumentNullException();

            if (args == null)
                throw new ArgumentNullException();

            _sharedStringBuilder.Length = 0;
            _sharedStringBuilder.AppendFormat(format, args);
            return _sharedStringBuilder.ToString();
        }
    }

    public static class XyzAssetUtils
    {
        [ThreadStatic]
        private static readonly StringBuilder _sharedStringBuilder = new StringBuilder(1024);
        private static readonly MD5 m_SharedHashProvider = new MD5CryptoServiceProvider();

        public static string CalculateMD5(string file)
        {
            try
            {
                FileStream fs = File.OpenRead(file);
                byte[] retVal = m_SharedHashProvider.ComputeHash(fs);
                fs.Close();

                _sharedStringBuilder.Length = 0;
                for (int i = 0; i < retVal.Length; i++)
                {
                    _sharedStringBuilder.Append(retVal[i].ToString("x2"));
                }
                return _sharedStringBuilder.ToString();
            }
            catch (Exception ex)
            {
                Debug.LogError("md5file() fail :" + file);
                throw ex;
            }
        }

        public static string CalculateMD5(byte[] binary)
        {
            try
            {
                byte[] retVal = m_SharedHashProvider.ComputeHash(binary);
                _sharedStringBuilder.Length = 0;
                for (int i = 0; i < retVal.Length; i++)
                {
                    _sharedStringBuilder.Append(retVal[i].ToString("x2"));
                }
                return _sharedStringBuilder.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}
