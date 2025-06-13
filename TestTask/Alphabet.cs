namespace TestTask
{
    /// <summary>
    /// Алфавит русского и английского языка
    /// </summary>
    public static class Alphapet
    {

        /// <summary>
        /// Строка со всеми буквами.
        /// </summary>
        public static char[] AlphabetChars = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяabcdefghijklmnopqrstuvwxyz".ToCharArray();

        /// <summary>
        /// Строка со всеми согласными буквами.
        /// </summary>
        public static char[] Consonants = "бвгджзйклмнпрстфхцчшщbcdfghjklmnpqrstvwxz".ToCharArray();

        /// <summary>
        /// Строка со всеми гласными буквами.
        /// </summary>
        public static char[] Vowel = "аеёиоуыэюяaeiouy".ToCharArray();
    }
}