using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace TestTask
{
    public class Program
    {

        /// <summary>
        /// Программа принимает на вход 2 пути до файлов.
        /// Анализирует в первом файле кол-во вхождений каждой буквы (регистрозависимо). Например А, б, Б, Г и т.д.
        /// Анализирует во втором файле кол-во вхождений парных букв (не регистрозависимо). Например АА, Оо, еЕ, тт и т.д.
        /// По окончанию работы - выводит данную статистику на экран.
        /// </summary>
        /// <param name="args">Первый параметр - путь до первого файла.
        /// Второй параметр - путь до второго файла.</param>
        static void Main(string[] args)
        {
            IReadOnlyStream inputStream1 = GetInputStream(args[0]);
            IReadOnlyStream inputStream2 = GetInputStream(args[1]);

            IList<LetterStats> singleLetterStats = FillSingleLetterStats(inputStream1);
            IList<LetterStats> doubleLetterStats = FillDoubleLetterStats(inputStream2);

            RemoveCharStatsByType(singleLetterStats, CharType.Vowel);
            RemoveCharStatsByType(doubleLetterStats, CharType.Consonants);

            PrintStatistic(singleLetterStats);
            PrintStatistic(doubleLetterStats);

            Console.WriteLine("Нажмите любую клавишу для завершения программы.");
            Console.ReadKey();
        }

        /// <summary>
        /// Ф-ция возвращает экземпляр потока с уже загруженным файлом для последующего посимвольного чтения.
        /// </summary>
        /// <param name="fileFullPath">Полный путь до файла для чтения</param>
        /// <returns>Поток для последующего чтения.</returns>
        private static IReadOnlyStream GetInputStream(string fileFullPath)
        {
            return new ReadOnlyStream(fileFullPath);
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения каждой буквы.
        /// Статистика РЕГИСТРОЗАВИСИМАЯ!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillSingleLetterStats(IReadOnlyStream stream)
        { 
            List<LetterStats> letters = new List<LetterStats>();

            using (stream)
            {
                stream.ResetPositionToStart();
                while (!stream.IsEof)
                {
                    char c = stream.ReadNextChar();

                    var existingStat = letters.FirstOrDefault(ls => ls.Letter == c.ToString());
                    if (existingStat != null)
                    {
                        IncStatistic(existingStat);
                    }
                    else
                    {
                        letters.Add(new LetterStats(c.ToString()));
                    }
                }
            }
            
            return letters;
        }

        /// <summary>
        /// Ф-ция считывающая из входящего потока все буквы, и возвращающая коллекцию статистик вхождения парных букв.
        /// В статистику должны попадать только пары из одинаковых букв, например АА, СС, УУ, ЕЕ и т.д.
        /// Статистика - НЕ регистрозависимая!
        /// </summary>
        /// <param name="stream">Стрим для считывания символов для последующего анализа</param>
        /// <returns>Коллекция статистик по каждой букве, что была прочитана из стрима.</returns>
        private static IList<LetterStats> FillDoubleLetterStats(IReadOnlyStream stream)
        {
            List<LetterStats> letters = new List<LetterStats>();

            stream.ResetPositionToStart();
            char? prev_c;
            char? curr_c = char.ToLower(stream.ReadNextChar());
            while (!stream.IsEof)
            {
                prev_c = curr_c;
                curr_c = char.ToLower(stream.ReadNextChar());
                if (prev_c == curr_c)
                {
                    string s = String.Concat(prev_c, curr_c);
                    var existingStat = letters.FirstOrDefault(ls => ls.Letter == s.ToString());
                    if (existingStat != null)
                    {
                        IncStatistic(existingStat);
                    }
                    else
                    {
                        letters.Add(new LetterStats(s.ToString()));
                    }
                    curr_c = null;
                }
            }

            return letters;
        }

        /// <summary>
        /// Ф-ция перебирает все найденные буквы/парные буквы, содержащие в себе только гласные или согласные буквы.
        /// (Тип букв для перебора определяется параметром charType)
        /// Все найденные буквы/пары соответствующие параметру поиска - удаляются из переданной коллекции статистик.
        /// </summary>
        /// <param name="letters">Коллекция со статистиками вхождения букв/пар</param>
        /// <param name="charType">Тип букв для анализа</param>
        private static void RemoveCharStatsByType(IList<LetterStats> letters, CharType charType)
        {
            List<LetterStats> lettersToRemove;
            switch (charType)
            {
                case CharType.Consonants:
                    lettersToRemove = letters.Where(l => Alphapet.Consonants.Contains(char.ToLower(l.Letter[0]))).ToList();
                    break;
                case CharType.Vowel:
                    lettersToRemove = letters.Where(l => Alphapet.Vowel.Contains(char.ToLower(l.Letter[0]))).ToList();
                    break;
                default:
                    lettersToRemove = new List<LetterStats>();
                    break;

            }
            foreach (var letter in lettersToRemove)
            {
                letters.Remove(letter);
            }
        }

        /// <summary>
        /// Ф-ция выводит на экран полученную статистику в формате "{Буква} : {Кол-во}"
        /// Каждая буква - с новой строки.
        /// Выводить на экран необходимо предварительно отсортировав набор по алфавиту.
        /// В конце отдельная строчка с ИТОГО, содержащая в себе общее кол-во найденных букв/пар
        /// </summary>
        /// <param name="letters">Коллекция со статистикой</param>
        private static void PrintStatistic(IEnumerable<LetterStats> letters)
        {
            var sortLetters = letters.OrderBy(l => l.Letter, StringComparer.OrdinalIgnoreCase);
            foreach (var letter in sortLetters)
            {
                Console.WriteLine($"{letter.Letter} : {letter.Count}");
            }
            int sum = sortLetters.Sum(l => l.Count);
            Console.WriteLine($"ИТОГО : {sum}");
        }

        /// <summary>
        /// Метод увеличивает счётчик вхождений по переданной структуре.
        /// </summary>
        /// <param name="letterStats"></param>
        private static void IncStatistic(LetterStats letterStats)
        {
            letterStats.Count++;
        }
    }
}
