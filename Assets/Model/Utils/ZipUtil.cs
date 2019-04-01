using ICSharpCode.SharpZipLib.GZip;
using System;
using System.IO;

namespace Myth
{
    /// <summary>
    /// 压缩类
    /// </summary>
    public static class ZipUtil
    {
        private static ZipHelper s_ZipHelper = new ZipHelper();
        
        /// <summary>
        /// 设置压缩解压缩辅助器
        /// </summary>
        /// <param name="zipHelper">要设置的压缩解压缩辅助器</param>
        public static void SetZipHelper(ZipHelper zipHelper)
        {
            s_ZipHelper = zipHelper;
        }

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="bytes">要压缩的数据的二进制流</param>
        /// <returns>压缩后的数据的二进制流</returns>
        public static byte[] Compress(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new Exception("Bytes is invalid.");
            }

            return Compress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="bytes">要压缩的数据的二进制流</param>
        /// <param name="compressedStream">压缩后的数据的二进制流</param>
        /// <returns>是否压缩数据成功</returns>
        public static bool Compress(byte[] bytes, Stream compressedStream)
        {
            if (bytes == null)
            {
                throw new Exception("Bytes is invalid.");
            }

            return Compress(bytes, 0, bytes.Length, compressedStream);
        }

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="bytes">要压缩的数据的二进制流</param>
        /// <param name="offset">要压缩的数据的二进制流的偏移</param>
        /// <param name="length">要压缩的数据的二进制流的长度</param>
        /// <returns>压缩后的数据的二进制流</returns>
        public static byte[] Compress(byte[] bytes, int offset, int length)
        {
            using (MemoryStream result = new MemoryStream())
            {
                if (Compress(bytes, offset, length, result))
                {
                    return result.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="bytes">要压缩的数据的二进制流</param>
        /// <param name="offset">要压缩的数据的二进制流的偏移</param>
        /// <param name="length">要压缩的数据的二进制流的长度</param>
        /// <param name="compressedStream">压缩后的数据的二进制流</param>
        /// <returns>是否压缩数据成功</returns>
        public static bool Compress(byte[] bytes, int offset, int length, Stream compressedStream)
        {
            if (s_ZipHelper == null)
            {
                throw new Exception("Zip helper is invalid.");
            }

            if (bytes == null)
            {
                throw new Exception("Bytes is invalid.");
            }

            if (offset < 0)
            {
                throw new Exception("Offset is invalid.");
            }

            if (length > bytes.Length)
            {
                throw new Exception("Length is invalid.");
            }

            if (compressedStream == null)
            {
                throw new Exception("Result is invalid.");
            }

            try
            {
                return s_ZipHelper.Compress(bytes, offset, length, compressedStream);
            }
            catch (Exception exception)
            {
                if (exception is Exception)
                {
                    throw;
                }

                throw new Exception(TextUtil.Format("Can not compress bytes with exception '{0}'.", exception.ToString()), exception);
            }
        }

        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="bytes">要解压缩的数据的二进制流</param>
        /// <returns>解压缩后的数据的二进制流</returns>
        public static byte[] Decompress(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new Exception("Bytes is invalid.");
            }

            return Decompress(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="bytes">要解压缩的数据的二进制流</param>
        /// <param name="decompressedStream">解压缩后的数据的二进制流</param>
        /// <returns>是否解压缩数据成功</returns>
        public static bool Decompress(byte[] bytes, Stream decompressedStream)
        {
            if (bytes == null)
            {
                throw new Exception("Bytes is invalid.");
            }

            return Decompress(bytes, 0, bytes.Length, decompressedStream);
        }

        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="bytes">要解压缩的数据的二进制流</param>
        /// <param name="offset">要解压缩的数据的二进制流的偏移</param>
        /// <param name="length">要解压缩的数据的二进制流的长度</param>
        /// <returns>解压缩后的数据的二进制流</returns>
        public static byte[] Decompress(byte[] bytes, int offset, int length)
        {
            using (MemoryStream result = new MemoryStream())
            {
                if (Decompress(bytes, offset, length, result))
                {
                    return result.ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="bytes">要解压缩的数据的二进制流</param>
        /// <param name="offset">要解压缩的数据的二进制流的偏移</param>
        /// <param name="length">要解压缩的数据的二进制流的长度</param>
        /// <param name="decompressedStream">解压缩后的数据的二进制流</param>
        /// <returns>是否解压缩数据成功</returns>
        public static bool Decompress(byte[] bytes, int offset, int length, Stream decompressedStream)
        {
            if (s_ZipHelper == null)
            {
                throw new Exception("Zip helper is invalid.");
            }

            if (bytes == null)
            {
                throw new Exception("Bytes is invalid.");
            }

            if (offset < 0)
            {
                throw new Exception("Offset is invalid.");
            }

            if (length > bytes.Length)
            {
                throw new Exception("Length is invalid.");
            }

            if (decompressedStream == null)
            {
                throw new Exception("Result is invalid.");
            }

            try
            {
                return s_ZipHelper.Decompress(bytes, offset, length, decompressedStream);
            }
            catch (Exception exception)
            {
                if (exception is Exception)
                {
                    throw;
                }

                throw new Exception(TextUtil.Format("Can not decompress bytes with exception '{0}'.", exception.ToString()), exception);
            }
        }
    }

    public class ZipHelper
    {
        private readonly byte[] m_BytesCache = new byte[0x10000];

        /// <summary>
        /// 压缩数据
        /// </summary>
        /// <param name="bytes">要压缩的数据的二进制流</param>
        /// <param name="offset">要压缩的数据的二进制流的偏移</param>
        /// <param name="length">要压缩的数据的二进制流的长度</param>
        /// <param name="compressedStream">压缩后的数据的二进制流</param>
        /// <returns>是否压缩数据成功</returns>
        public bool Compress(byte[] bytes, int offset, int length, Stream compressedStream)
        {
            if (bytes == null)
            {
                return false;
            }

            if (offset < 0)
            {
                return false;
            }

            if (length > bytes.Length)
            {
                return false;
            }

            if (compressedStream == null)
            {
                return false;
            }

            try
            {
                using (GZipOutputStream gZipOutputStream = new GZipOutputStream(compressedStream))
                {
                    gZipOutputStream.Write(bytes, offset, length);
                    if (compressedStream.Length >= 8L)
                    {
                        long current = compressedStream.Position;
                        compressedStream.Position = 4L;
                        compressedStream.WriteByte(25);
                        compressedStream.WriteByte(134);
                        compressedStream.WriteByte(2);
                        compressedStream.WriteByte(32);
                        compressedStream.Position = current;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 解压缩数据
        /// </summary>
        /// <param name="bytes">要解压缩的数据的二进制流</param>
        /// <param name="offset">要解压缩的数据的二进制流的偏移</param>
        /// <param name="length">要解压缩的数据的二进制流的长度</param>
        /// <param name="decompressedStream">解压缩后的数据的二进制流</param>
        /// <returns>是否解压缩数据成功</returns>
        public bool Decompress(byte[] bytes, int offset, int length, Stream decompressedStream)
        {
            if (bytes == null)
            {
                return false;
            }

            if (offset < 0)
            {
                return false;
            }

            if (length > bytes.Length)
            {
                return false;
            }

            if (decompressedStream == null)
            {
                return false;
            }

            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream(bytes, offset, length, false);
                using (GZipInputStream gZipInputStream = new GZipInputStream(memoryStream))
                {
                    int bytesRead = 0;
                    while ((bytesRead = gZipInputStream.Read(m_BytesCache, 0, m_BytesCache.Length)) > 0)
                    {
                        decompressedStream.Write(m_BytesCache, 0, bytesRead);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                    memoryStream = null;
                }
            }
        }
    }
}
