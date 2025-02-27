﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using OneScript.StandardLibrary.Collections;
using OneScript.StandardLibrary.Text;
using System.Text;
using OneScript.Contexts;
using OneScript.Exceptions;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;
namespace OneScript.StandardLibrary.Binary
{
    /// <summary>
    /// Глобальный контекст. Операции с двоичными данными.
    /// </summary>
    [GlobalContext(Category = "Процедуры и функции работы с двоичными данными")]
    public sealed class GlobalBinaryData : GlobalContextBase<GlobalBinaryData>
    {
        private static byte[] HexStringToByteArray(string hex)
        {
            var newHex = System.Text.RegularExpressions.Regex.Replace(hex, @"[^0-9A-Fa-f]", "");
            int numberChars = newHex.Length;
            if (numberChars % 2 == 1)
                throw new FormatException("Неверный формат шестнадцатеричной строки");

            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(newHex.Substring(i, 2), 16);

            return bytes;
        }

        private static int[] hexDigitsValues = new[]
        {
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                 0, 1, 2, 3, 4, 5, 6, 7, 8, 9,-1,-1,-1,-1,-1,-1,
                -1,10,11,12,13,14,15,16,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,10,11,12,13,14,15,16,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,
                -1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1
        };
        private static int CharCodeToHex(byte code)
        {
            return hexDigitsValues[code];
        }

        private static byte[] HexArrayToByteArray(byte[] hex)
        {
            var bytes = new byte[hex.Length / 2];
            int pos = 0;

            int hexDig1;
            int hexDig2 =-1;
            for (int i = 0; i < hex.Length; ++i)
            {
                hexDig1 = CharCodeToHex(hex[i]);
                if (hexDig1 < 0)
                    continue;

                if (hexDig2 < 0)
                {
                    hexDig2 = hexDig1;
                    continue;
                }

                bytes[pos] = (byte)(hexDig2 * 16 + hexDig1);
                ++pos;
                hexDig2 = -1;
            }

            if (pos < bytes.Length)
                Array.Resize(ref bytes, pos);
            return bytes;
        }

        private static string GetStringFromByteBuffer(byte[] buf, Encoding enc)
        {
            var bom = enc.GetPreamble();

            if (bom.Length == 0)
            {
                return enc.GetString(buf);
            }

            int startPos = 0;
            if (buf.Length >= 3 && buf[0] == 0xEF && buf[1] == 0xBB && buf[2] == 0xBF)
            {
                enc = Encoding.UTF8;
                startPos = 3;
            }
            else if (buf.Length >= 2 && buf[0] == 0xFF && buf[1] == 0xFE)
            {
                if (buf.Length >= 4 && buf[2] == 0x00 && buf[3] == 0x00)
                {
                    enc = Encoding.UTF32; // UTF32LE
                    startPos = 4;
                }
                else
                {
                    enc = Encoding.Unicode; // UTF16LE
                    startPos = 2;
                }
            }
            else if (buf.Length >= 2 && buf[0] == 0xFE && buf[1] == 0xFF)
            {
                enc = Encoding.BigEndianUnicode; // UTF16BE
                startPos = 2;
            }
            else if (buf.Length >= 4 && buf[0] == 0x00 && buf[1] == 0x00 && buf[2] == 0xFE && buf[3] == 0xFF)
            {
                enc = new UTF32Encoding(true, true); // UTF32BE with BOM
                startPos = 4;
            }

            return enc.GetString(buf, startPos, buf.Length - startPos);
        }

        private static void CheckAndThrowIfNull<T>(AutoContext<T> obj) where T : AutoContext<T>
        {
            if (obj == null)
                throw RuntimeException.InvalidArgumentType();
        }

        private static void CheckAndThrowIfNull<T>(AutoContext<T> obj, int argNumber, string argName) where T : AutoContext<T>
        {
            if (obj == null)
                throw RuntimeException.InvalidArgumentType(argNumber, argName);
        }

        public static IAttachableContext CreateInstance()
        {
            return new GlobalBinaryData();
        }

        /// <summary>
        /// Менеджер файловых потоков.
        /// </summary>
        [ContextProperty("ФайловыеПотоки", "FileStreams")]
        public FileStreamsManager FileStreams { get; } = new FileStreamsManager();
        
        /// <summary>
        /// Объединяет несколько объектов типа ДвоичныеДанные в один.
        /// </summary>
        /// <param name="array">Массив объектов типа ДвоичныеДанные.</param>
        /// <returns>Тип: ДвоичныеДанные.</returns>
        [ContextMethod("ConcatenateBinaryData", IsDeprecated = true)]
        [ContextMethod("СоединитьДвоичныеДанные", "ConcatBinaryData")]
        public BinaryDataContext ConcatBinaryData(ArrayImpl array)
        {
            CheckAndThrowIfNull(array);

            // Сделано на int т.к. BinaryContext.Size имеет тип int;
            using var stream = new System.IO.MemoryStream();

            foreach (var cbd in array)
            {
                byte[] buffer = ((BinaryDataContext)cbd.AsObject()).Buffer;
                stream.Write(buffer, 0, buffer.Length);
            }
            stream.Position = 0;

            return new BinaryDataContext(stream);
        }

        /// <summary>
        /// Разделяет двоичные данные на части заданного размера. Размер задается в байтах.
        /// </summary>
        /// <param name="data">Объект типа ДвоичныеДанные.</param>
        /// <param name="size">Размер одной части данных.</param>
        /// <returns>Массив объектов типа ДвоичныеДанные.</returns>
        [ContextMethod("РазделитьДвоичныеДанные")]
        public ArrayImpl SplitBinaryData(BinaryDataContext data, long size)
        {
            CheckAndThrowIfNull(data, 1, nameof(data));

            if (size <= 0 || size > Int32.MaxValue)
                throw RuntimeException.InvalidNthArgumentValue(2);

            ArrayImpl array = new ArrayImpl();
            long dataSize = data.Size();

            if (dataSize < size)
            {
                array.Add(data);
                return array;
            }

            int readedBytes = 0;
            var dataStream = data.GetStream();

            while (readedBytes < dataSize)
            {
                int bytesToRead = (int)size;
                if (bytesToRead > dataSize - readedBytes)
                    bytesToRead = (int)(dataSize - readedBytes);

                byte[] buffer = new byte[bytesToRead];
                dataStream.Read(buffer, 0, bytesToRead);
                readedBytes += bytesToRead;
                array.Add(new BinaryDataContext(buffer));
            }

            return array;
        }

        /// <summary>
        /// Преобразует строку в значение типа ДвоичныеДанные с учетом кодировки текста.
        /// </summary>
        /// <param name="str">Строка, которую требуется преобразовать в ДвоичныеДанные.</param>
        /// <param name="encoding">Кодировка текста</param>
        /// <param name="addBOM">Определяет, будет ли добавлена метка порядка байт (BOM) кодировки текста в начало данных.</param>
        /// <returns>Тип: ДвоичныеДанные.</returns>
        [ContextMethod("ПолучитьДвоичныеДанныеИзСтроки")]
        public BinaryDataContext GetBinaryDataFromString(string str, IValue encoding = null, bool addBOM = false)
        {
            // Получаем кодировку
            // Из синтаксис помощника. Если кодировка не задана, используем UTF8 без BOM.

            System.Text.Encoding enc = new UTF8Encoding(false);
            if (encoding != null)
                enc = TextEncodingEnum.GetEncoding(encoding, addBOM);

            var stream = new System.IO.MemoryStream();

            var bom = enc.GetPreamble();
            var inputString = enc.GetBytes(str);

            stream.Write(bom, 0, bom.Length);
            stream.Write(inputString, 0, inputString.Length);
            stream.Position = 0;
            
            return new BinaryDataContext(stream);
        }

        /// <summary>
        /// Преобразует строку в буфер двоичных данных с учетом кодировки текста.
        /// </summary>
        /// <param name="str">Строка, которую требуется преобразовать в БуферДвоичныхДанных.</param>
        /// <param name="encoding">Кодировка текста</param>
        /// <param name="addBOM">Определяет, будет ли добавлена метка порядка байт (BOM) кодировки текста в начало данных.</param>
        /// <returns>Тип: БуферДвоичныхДанных.</returns>
        [ContextMethod("ПолучитьБуферДвоичныхДанныхИзСтроки")]
        public BinaryDataBuffer GetBinaryDataBufferFromString(string str, IValue encoding = null, bool addBOM = false)
        {
            var enc = (encoding != null)? TextEncodingEnum.GetEncoding(encoding, addBOM) : Encoding.UTF8;

            return new BinaryDataBuffer(enc.GetBytes(str));
        }

        /// <summary>
        /// Преобразует двоичные данные в строку с заданной кодировкой текста.
        /// </summary>
        /// <param name="data">Двоичные данные, которые требуется преобразовать в строку.</param>
        /// <param name="encoding">Кодировка текста</param>
        /// <returns>Тип: Строка.</returns>
        [ContextMethod("ПолучитьСтрокуИзДвоичныхДанных")]
        public string GetStringFromBinaryData(BinaryDataContext data, IValue encoding = null)
        {
            CheckAndThrowIfNull(data, 1, nameof(data));

            // Получаем кодировку
            // Из синтаксис помощника если кодировка не задана используем UTF8
            var enc = (encoding != null) ? TextEncodingEnum.GetEncoding(encoding) : Encoding.UTF8;

            return GetStringFromByteBuffer(data.Buffer, enc);
        }

        /// <summary>
        /// Преобразует буфер двоичных данных в строку с заданной кодировкой текста.
        /// </summary>
        /// <param name="buffer">Буфер двоичных данных, который требуется преобразовать в строку.</param>
        /// <param name="encoding">Кодировка текста</param>
        /// <returns>Тип: Строка.</returns>
        [ContextMethod("ПолучитьСтрокуИзБуфераДвоичныхДанных")]
        public string GetStringFromBinaryDataBuffer(BinaryDataBuffer buffer, IValue encoding = null)
        {
            CheckAndThrowIfNull(buffer, 1, nameof(buffer));

            var enc = (encoding != null) ? TextEncodingEnum.GetEncoding(encoding) : Encoding.UTF8;

            return GetStringFromByteBuffer(buffer.Bytes, enc);
        }

        /// <summary>
        /// Преобразует строку формата Base64 в двоичные данные.
        /// </summary>
        /// <param name="str">Строка в формате Base64.</param>
        /// <returns>Тип: ДвоичныеДанные.</returns>
        [ContextMethod("ПолучитьДвоичныеДанныеИзBase64Строки")]
        public BinaryDataContext GetBinaryDataFromBase64String(string str)
        {
            try
            {
                return new BinaryDataContext(Convert.FromBase64String(str));
            }
            catch
            {
                return new BinaryDataContext(new byte[0]);
            }
        }

        /// <summary>
        /// Преобразует строку формата Base64 в буфер двоичных данных.
        /// </summary>
        /// <param name="str">Строка в формате Base64.</param>
        /// <returns>Тип: ДвоичныеДанные.</returns>
        [ContextMethod("ПолучитьБуферДвоичныхДанныхИзBase64Строки")]
        public BinaryDataBuffer GetBinaryDataBufferFromBase64String(string str)
        {
            try
            {
                return new BinaryDataBuffer(Convert.FromBase64String(str));
            }
            catch
            {
                return new BinaryDataBuffer(new byte[0]);
            }
        }

        /// <summary>
        /// Преобразует двоичные данные в строку формата Base64.
        /// Полученный текст разбивается на строки длиной 76 символов.
        /// В качестве разделителя строк используется сочетание символов CR+LF.
        /// </summary>
        /// <param name="data">Двоичные данные.</param>
        /// <returns>Тип: Строка.</returns>
        [ContextMethod("ПолучитьBase64СтрокуИзДвоичныхДанных")]
        public string GetBase64StringFromBinaryData(BinaryDataContext data)
        {
            CheckAndThrowIfNull(data, 1, nameof(data));

            return Convert.ToBase64String(data.Buffer, Base64FormattingOptions.InsertLineBreaks);
        }

        /// <summary>
        /// Преобразует буфер двоичных данных в строку формата Base64.
        /// Полученный текст разбивается на строки длиной 76 символов.
        /// В качестве разделителя строк используется сочетание символов CR+LF.
        /// </summary>
        /// <param name="buffer">Буфер двоичных данных.</param>
        /// <returns>Тип: Строка.</returns>
        [ContextMethod("ПолучитьBase64СтрокуИзБуфераДвоичныхДанных")]
        public string GetBase64StringFromBinaryDataBuffer(BinaryDataBuffer buffer)
        {
            CheckAndThrowIfNull(buffer);

            return Convert.ToBase64String(buffer.Bytes, Base64FormattingOptions.InsertLineBreaks);
        }

        /// <summary>
        /// Преобразует двоичные данные из формата Base64 в ДвоичныеДанные.
        /// </summary>
        /// <param name="data">Двоичные данные, закодированные по методу Base64.</param>
        /// <returns>Тип: ДвоичныеДанные.</returns>
        [ContextMethod("ПолучитьДвоичныеДанныеИзBase64ДвоичныхДанных")]
        public BinaryDataContext GetBinaryDataFromBase64BinaryData(BinaryDataContext data)
        {
            CheckAndThrowIfNull(data);

            try
            {
                var enc = new UTF8Encoding(false,true);
                var str = enc.GetString(data.Buffer, 0, data.Buffer.Length);
                return new BinaryDataContext(Convert.FromBase64String(str));
            }
            catch
            {
                return new BinaryDataContext(new byte[0]);
            }
        }

        /// <summary>
        /// Преобразует буфер двоичных данных из формата Base64 в БуферДвоичныхДанных.
        /// </summary>
        /// <param name="buffer">Буфер двоичных данных.</param>
        /// <returns>Тип: ДвоичныеДанные.</returns>
        [ContextMethod("ПолучитьБуферДвоичныхДанныхИзBase64БуфераДвоичныхДанных")]
        public BinaryDataBuffer GetBinaryDataBufferFromBase64BinaryDataBuffer(BinaryDataBuffer buffer)
        {
            CheckAndThrowIfNull(buffer);

            try
            {
                var enc = new UTF8Encoding(false, true);
                var str = enc.GetString(buffer.Bytes, 0, buffer.Bytes.Length);
                return new BinaryDataBuffer(Convert.FromBase64String(str));
            }
            catch
            {
                return new BinaryDataBuffer(new byte[0]);
            }
        }

        /// <summary>
        /// Преобразует двоичные данные в формат Base64.
        /// Полученный текст разбивается на строки длиной 76 символов.
        /// В качестве разделителя строк используется сочетание символов CR+LF.
        /// </summary>
        /// <param name="data">Двоичные данные.</param>
        /// <returns>Тип: ДвоичныеДанные.</returns>
        [ContextMethod("ПолучитьBase64ДвоичныеДанныеИзДвоичныхДанных")]
        public BinaryDataContext GetBase64BinaryDataFromBinaryData(BinaryDataContext data)
        {
            CheckAndThrowIfNull(data);

            var base64str = Convert.ToBase64String(data.Buffer, Base64FormattingOptions.InsertLineBreaks);
            return new BinaryDataContext(Encoding.ASCII.GetBytes(base64str));
        }

        /// <summary>
        /// Преобразует буфер двоичных данных в формат Base64.
        /// Полученный текст разбивается на строки длиной 76 символов.
        /// В качестве разделителя строк используется сочетание символов CR+LF.
        /// </summary>
        /// <param name="buffer">Буфер двоичных данных.</param>
        /// <returns>Тип: БуферДвоичныхДанных.</returns>
        [ContextMethod("ПолучитьBase64БуферДвоичныхДанныхИзБуфераДвоичныхДанных")]
        public BinaryDataBuffer GetBase64BinaryDataBufferFromBinaryDataBuffer(BinaryDataBuffer buffer)
        {
            CheckAndThrowIfNull(buffer);

            var base64str = Convert.ToBase64String(buffer.Bytes, Base64FormattingOptions.InsertLineBreaks);
            return new BinaryDataBuffer(Encoding.ASCII.GetBytes(base64str));
        }

        /// <summary>
        /// Преобразует строку формата Base 16 (Hex) в двоичные данные.
        /// </summary>
        /// <param name="hex">Строка в формате Base 16 (Hex).</param>
        /// <returns>Тип: ДвоичныеДанные.</returns>
        [ContextMethod("ПолучитьДвоичныеДанныеИзHexСтроки")]
        public BinaryDataContext GetBinaryDataFromHexString(string hex)
        {
            return new BinaryDataContext(HexStringToByteArray(hex));
        }
        
        /// <summary>
        /// Преобразует строку в формате Base 16 (Hex) в буфер двоичных данных.
        /// </summary>
        /// <param name="hex">Строка в формате Base 16 (Hex).</param>
        /// <returns>Тип: БуферДвоичныхДанных.</returns>
        [ContextMethod("ПолучитьБуферДвоичныхДанныхИзHexСтроки")]
        public BinaryDataBuffer GetBinaryDataBufferFromHexString(string hex)
        {
            return new BinaryDataBuffer(HexStringToByteArray(hex));
        }
        
        /// <summary>
        /// Преобразует двоичные данные в строку формата Base 16 (Hex).
        /// </summary>
        /// <param name="data">Двоичные данные.</param>
        /// <returns>Тип: Строка.</returns>
        [ContextMethod("ПолучитьHexСтрокуИзДвоичныхДанных")]
        public string GetHexStringFromBinaryData(BinaryDataContext data)
        {
            CheckAndThrowIfNull(data);

            return BitConverter.ToString(data.Buffer).Replace("-","");
        }

        /// <summary>
        /// Преобразует буфер двоичных данных в строку формата Base 16 (Hex).
        /// </summary>
        /// <param name="buffer">Буфер двоичных данных.</param>
        /// <returns>Тип: Строка.</returns>
        [ContextMethod("ПолучитьHexСтрокуИзБуфераДвоичныхДанных")]
        public string GetHexStringFromBinaryDataBuffer(BinaryDataBuffer buffer)
        {
            CheckAndThrowIfNull(buffer); 

            return BitConverter.ToString(buffer.Bytes).Replace("-","");
        }

        /// <summary>
        /// Преобразует двоичные данные из формата Base 16 (Hex) в ДвоичныеДанные.
        /// </summary>
        /// <param name="data">Двоичные данные в формате Base 16 (Hex).</param>
        /// <returns>Тип: ДвоичныеДанные. </returns>
        [ContextMethod("ПолучитьДвоичныеДанныеИзHexДвоичныхДанных")]
        public BinaryDataContext GetBinaryDataFromHexBinaryData(BinaryDataContext data)
        {
            CheckAndThrowIfNull(data);

            return new BinaryDataContext(HexArrayToByteArray(data.Buffer));
        }

        /// <summary>
        /// Преобразует буфер двоичных данных из формата Base 16 (Hex) в БуферДвоичныхДанных.
        /// </summary>
        /// <param name="buffer">Буфер двоичных данных в формате Base 16 (Hex).</param>
        /// <returns>Тип: БуферДвоичныхДанных.</returns>
        [ContextMethod("ПолучитьБуферДвоичныхДанныхИзHexБуфераДвоичныхДанных")]
        public BinaryDataBuffer GetBinaryDataBufferFromHexBinaryDataBuffer(BinaryDataBuffer buffer)
        {
            CheckAndThrowIfNull(buffer); 

            return new BinaryDataBuffer(HexArrayToByteArray(buffer.Bytes));
        }

        /// <summary>
        /// Преобразует двоичные данные в формат Base 16 (Hex).
        /// </summary>
        /// <param name="data">Двоичные данные.</param>
        /// <returns>Тип: ДвоичныеДанные. </returns>
        [ContextMethod("ПолучитьHexДвоичныеДанныеИзДвоичныхДанных")]
        public BinaryDataContext GetHexBinaryDataFromBinaryData(BinaryDataContext data)
        {
            var str = GetHexStringFromBinaryData(data);
            return GetBinaryDataFromString(str);
        }

        /// <summary>
        /// Преобразует буфер двоичных данных в формат Base 16 (Hex).
        /// </summary>
        /// <param name="buffer">Буфер двоичных данных.</param>
        /// <returns>Тип: БуферДвоичныхДанных.</returns>
        [ContextMethod("ПолучитьHexБуферДвоичныхДанныхИзБуфераДвоичныхДанных")]
        public BinaryDataBuffer GetHexBinaryDataBufferFromBinaryDataBuffer(BinaryDataBuffer buffer)
        {
            var str = GetHexStringFromBinaryDataBuffer(buffer);
            return GetBinaryDataBufferFromString(str);
        }

        /// <summary>
        /// Преобразует двоичные данные в буфер двоичных данных.
        /// </summary>
        /// <param name="data">Двоичные данные.</param>
        /// <returns>Тип: БуферДвоичныхДанных.</returns>
        [ContextMethod("ПолучитьБуферДвоичныхДанныхИзДвоичныхДанных")]
        public BinaryDataBuffer GetBinaryDataBufferFromBinaryData(BinaryDataContext data)
        {
            CheckAndThrowIfNull(data);

            return new BinaryDataBuffer(data.Buffer);
        }

        /// <summary>
        /// Преобразует буфер двоичных данных в значение типа ДвоичныеДанные.
        /// </summary>
        /// <param name="buffer">Буфер двоичных данных.</param>
        /// <returns>Тип: ДвоичныеДанные.</returns>
        [ContextMethod("ПолучитьДвоичныеДанныеИзБуфераДвоичныхДанных")]
        public BinaryDataContext GetBinaryDataFromBinaryDataBuffer(BinaryDataBuffer buffer)
        {
            CheckAndThrowIfNull(buffer);

            return new BinaryDataContext(buffer.Bytes);
        }

    }
}
