using System.Text;
using TMPro;
using UnityEngine;

namespace Assets.GameData.Scripts
{
    public class RichTextBuilder
    {
        public RichTextBuilder(int size = 64)
        {
            _sb = new(size);
        }
        private readonly StringBuilder _sb;

        // ====================== ЦВЕТА ======================
        public RichTextBuilder Red(string text) => AppendColor("#FF0000", text);
        public RichTextBuilder Green(string text) => AppendColor("#00FF00", text);
        public RichTextBuilder Blue(string text) => AppendColor("#0000FF", text);
        public RichTextBuilder Yellow(string text) => AppendColor("#FFFF00", text);
        public RichTextBuilder Orange(string text) => AppendColor("#FF8800", text);
        public RichTextBuilder Purple(string text) => AppendColor("#AA00FF", text);
        public RichTextBuilder White(string text) => AppendColor("#FFFFFF", text);
        public RichTextBuilder Black(string text) => AppendColor("#000000", text);

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
            _ = _sb.Append("<color=").Append(hex).Append(">").Append(text).Append("</color>");
            return this;
        }

        // ====================== ФОРМАТИРОВАНИЕ ======================
        public RichTextBuilder Bold(string text) => Wrap(text, "<b>", "</b>");
        public RichTextBuilder Italic(string text) => Wrap(text, "<i>", "</i>");
        public RichTextBuilder Underline(string text) => Wrap(text, "<u>", "</u>");

        private RichTextBuilder Wrap(string text, string open, string close)
        {
            _ = _sb.Append(open).Append(text).Append(close);
            return this;
        }

        // ====================== ГИПЕРССЫЛКИ ======================
        public RichTextBuilder Link(string id, string text)
        {
            _ = _sb.Append("<link=").Append(id).Append(">").Append(text).Append("</link>");
            return this;
        }

        // ====================== ПРОСТОЕ ДОБАВЛЕНИЕ ======================
        public RichTextBuilder Append(string text)
        {
            _ = _sb.Append(text);
            return this;
        }

        public RichTextBuilder AppendLine(string text = "")
        {
            _ = _sb.Append(text).Append("\n");
            return this;
        }

        // ====================== ФИНАЛ ======================
        public override string ToString() => _sb.ToString();

        public void Clear() => _sb.Clear();

        public void ApplyTo(TextMeshProUGUI tmp)
        {
            tmp.text = ToString();
            Clear();
        }
    }
}
