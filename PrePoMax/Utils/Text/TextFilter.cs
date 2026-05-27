using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Text
{
    public static class TextFilter
    {
        /// <summary>
        /// Devolver si el texto pasa el filtro (todos los caracteres deben estar en el filtro).
        /// </summary>
        public static bool PassTextFilter(string text, string filter)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(filter))
                return false;

            foreach (char c in text)
            {
                if (!filter.Contains(c))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Ignorar los caracteres que no estén en el filtro.
        /// </summary>
        public static string IgnoreTextFilter(string text, string filter)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(filter))
                return null;

            var result = new System.Text.StringBuilder();
            foreach (char c in text)
            {
                if (filter.Contains(c))
                    result.Append(c);
            }

            return result.Length > 0 ? result.ToString() : null;
        }

        /// <summary>
        /// Ignorar texto con un caracter específico en el inicio del texto (comentarios).
        /// </summary>
        public static string IgnoreComment(string text, string comment = "#")
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(comment))
                return text;

            if (text.Contains('\n') && text.Contains(comment))
            {
                var lines = text.Split('\n');
                var result = new System.Text.StringBuilder();
                foreach (var line in lines)
                {
                    result.AppendLine(IgnoreComment(line, comment));
                }
                return result.ToString().TrimEnd();
            }
            else if (text.Contains(comment))
            {
                return text.Split(new[] { comment }, 2, StringSplitOptions.None)[0];
            }
            else
            {
                return text;
            }
        }

        /// <summary>
        /// Obtener solo los comentarios de un texto.
        /// </summary>
        public static string OnlyTheComment(string text, string comment = "#")
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(comment))
                return null;

            if (text.Contains('\n') && text.Contains(comment))
            {
                var lines = text.Split('\n');
                var result = new System.Text.StringBuilder();
                foreach (var line in lines)
                {
                    var commentLine = OnlyTheComment(line, comment);
                    if (commentLine != null)
                        result.AppendLine(commentLine);
                }
                return result.ToString().TrimEnd();
            }
            else if (text.Contains(comment))
            {
                var parts = text.Split(new[] { comment }, 2, StringSplitOptions.None);
                return parts.Length > 1 ? parts[1] : null;
            }
            else
            {
                return null;
            }
        }
    }
}
