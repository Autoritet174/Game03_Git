using System.Text;
using TMPro;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    /// <summary>Собирает текст с разметкой TextMeshPro и применяет его к подписи.</summary>
    public class RichTextBuilder
    {
        public RichTextBuilder(int size = 64)
        {
            sb = new(size);
        }

        private readonly StringBuilder sb;

        #region Цвет текста
        public RichTextBuilder Red(string text)
        {
            return AppendColor("#FF0000", text);
        }

        public RichTextBuilder Green(string text)
        {
            return AppendColor("#00FF00", text);
        }

        public RichTextBuilder Blue(string text)
        {
            return AppendColor("#0000FF", text);
        }

        public RichTextBuilder Yellow(string text)
        {
            return AppendColor("#FFFF00", text);
        }

        public RichTextBuilder Orange(string text)
        {
            return AppendColor("#FF8800", text);
        }

        public RichTextBuilder Purple(string text)
        {
            return AppendColor("#AA00FF", text);
        }

        public RichTextBuilder White(string text)
        {
            return AppendColor("#FFFFFF", text);
        }

        public RichTextBuilder Black(string text)
        {
            return AppendColor("#000000", text);
        }

        public RichTextBuilder Color(string hex, string text)
        {
            if (!hex.StartsWith("#"))
            {
                hex = "#" + hex;
            }

            return AppendColor(hex, text);
        }

        /// <summary>Принимает Unity Color и автоматически конвертирует в #RRGGBBAA</summary>
        public RichTextBuilder Color(Color unityColor, string text)
        {
            string hex = ColorUtility.ToHtmlStringRGBA(unityColor);

            return AppendColor($"#{hex}", text);
        }

        private RichTextBuilder AppendColor(string hex, string text)
        {
            _ = sb.Append("<color=").Append(hex).Append(">").Append(text).Append("</color>");
            return this;
        }

        #endregion Цвет текста

        #region Разметка текста
        public RichTextBuilder Bold(string text)
        {
            return Wrap(text, "<b>", "</b>");
        }

        public RichTextBuilder Italic(string text)
        {
            return Wrap(text, "<i>", "</i>");
        }

        public RichTextBuilder Underline(string text)
        {
            return Wrap(text, "<u>", "</u>");
        }

        private RichTextBuilder Wrap(string text, string open, string close)
        {
            _ = sb.Append(open).Append(text).Append(close);
            return this;
        }
        public RichTextBuilder Link(string id, string text)
        {
            _ = sb.Append("<link=").Append(id).Append(">").Append(text).Append("</link>");
            return this;
        }

        #endregion Разметка текста

        #region Сборка и применение текста
        public RichTextBuilder Append(string text)
        {
            _ = sb.Append(text);
            return this;
        }

        public RichTextBuilder AppendLine(string text = "")
        {
            _ = sb.Append(text).Append("\n");
            return this;
        }
        public override string ToString()
        {
            return sb.ToString();
        }

        public void Clear()
        {
            _ = sb.Clear();
        }

        public void ApplyTo(TextMeshProUGUI tmp)
        {
            tmp.text = ToString();
            Clear();
        }

        #endregion Сборка и применение текста
    }
}
