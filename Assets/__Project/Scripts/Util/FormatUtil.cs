using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engarde_Bryan {

	/// <summary>
	/// Provides quality of life string conversion utilities.
	/// </summary>
	public static class FormatUtil {

		private const int IndentationSpaces = 4;
		private static readonly string Indentation = new string(' ', IndentationSpaces);

		private static void AppendIndentedLine(this StringBuilder sb, object obj) {
			sb.Append(Indentation).AppendLine(obj.ToString());
		}

		/// <summary>
		/// Creates a string with a header followed by an indented list of items in the sequence.
		/// </summary>
		public static string ToHeaderedList<T>(string header, IEnumerable<T> objects, Func<T, object> formatter = null) {

			StringBuilder sb = new StringBuilder();
			sb.AppendLine(header);
			foreach (var item in objects) {
				sb.AppendIndentedLine(formatter == null ? item : formatter(item));
			}
			if (!objects.Any()) {
				sb.AppendIndentedLine("(None)");
			}
			return sb.ToString();

		}

		/// <summary>
		/// Indents all lines of a string by a given amount.
		/// </summary>
		public static string Indent(string value, int indentCount = 1) {
			StringBuilder sb = new StringBuilder(value);
			string indent = new string(' ', IndentationSpaces * indentCount);
			sb.Insert(0, indent);
			sb.Replace("\n", "\n" + indent);
			return sb.ToString();
		}

		/// <summary>
		/// Creates a comma separated list of items in the sequence.
		/// </summary>
		public static string ToCommaString<T>(this IEnumerable<T> enumerable) {
			return "{ " + string.Join(", ", enumerable) + " }";
		}

		/// <summary>
		/// Creates a comma separated list of items in the sequence, applying the conversion function to each.
		/// </summary>
		public static string ToCommaString<T>(this IEnumerable<T> enumerable, Func<T, object> converter) {
			if (converter == null) throw new ArgumentNullException("Converter cannot be null");
			return string.Join(", ", enumerable.Select(x => converter(x)));
		}

		/// <summary>
		/// Prints a dictionary as a header followed by each item in the format "key : value", applying conversion functions to each.
		/// </summary>
		public static string ToHeaderedDict<TKey, TValue>(this Dictionary<TKey, TValue> dict, string name, Func<TKey, object> keyConverter = null, Func<TValue, object> valueConverter = null) {
			return ToHeaderedList(name, dict, x => $"{(keyConverter == null ? x.Key : keyConverter(x.Key))} : {(valueConverter == null ? x.Value : valueConverter(x.Value))}");
		}

	}

}