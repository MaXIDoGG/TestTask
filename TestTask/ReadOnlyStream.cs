using System;
using System.IO;
using System.Text;

namespace TestTask
{
    public class ReadOnlyStream : IReadOnlyStream
    {
        private StreamReader _localReader;

        /// <summary>
        /// Конструктор класса. 
        /// Т.к. происходит прямая работа с файлом, необходимо 
        /// обеспечить ГАРАНТИРОВАННОЕ закрытие файла после окончания работы с таковым!
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        public ReadOnlyStream(string fileFullPath)
        {
            IsEof = true;

            _localReader = new StreamReader(File.OpenRead(fileFullPath), Encoding.UTF8);
        }
                
        /// <summary>
        /// Флаг окончания файла.
        /// </summary>
        public bool IsEof
        {
            get;
            private set;
        }

        /// <summary>
        /// Ф-ция чтения следующего символа из потока.
        /// Если произведена попытка прочитать символ после достижения конца файла, метод 
        /// должен бросать соответствующее исключение
        /// </summary>
        /// <returns>Считанный символ.</returns>
        public char ReadNextChar()
        {

            if (IsEof) {
                throw new InvalidOperationException("Попытка чтения после достижения конца файла");
            }
            char nextChar = (char) _localReader.Read();

            if (_localReader.EndOfStream) {
                IsEof = true;
            }

            return nextChar;
        }

        /// <summary>
        /// Сбрасывает текущую позицию потока на начало.
        /// </summary>
        public void ResetPositionToStart()
        {
            if (_localReader == null)
            {
                IsEof = true;
                return;
            }

            _localReader.BaseStream.Position = 0;
            IsEof = false;
        }

        /// <summary>
        /// Реализ
        /// </summary>
        public void Dispose()
        {
            _localReader.Dispose();
        }
    }
}
