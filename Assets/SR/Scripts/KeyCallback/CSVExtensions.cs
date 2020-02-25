using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Reflection;
using System.IO;
using System.Linq;

namespace ListExtensions
{
	public static class CSVEXtensions 
	{
		public static void SaveToCSV<T>(this IEnumerable<T> items, string fileName)
		{
			SaveToCSV<T>(items, fileName, false);
		}
		public static void SaveToCSV<T>(this IEnumerable<T> items, string fileName, bool append)
		{
			// Debug.Log (string.Format("{0}.SaveToCSV(): {1} log file '{2}'", typeof(T).ToString(), (append?"APPEND TO":"OVERWRITE"), fileName));

			Type itemType = typeof(T);

			var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).OrderBy(f => f.Name);

			// Debug.Log (string.Format ("Item of type {0} has {1} public properties.", props, props.Count()));

			using (var writer = new StreamWriter(fileName, append))
			{
				// Debug.Log (string.Format("Writing header to file with columns: {0}", string.Join (", ", props.Select(p => p.Name).ToArray<string>())));

				if (!append) writer.WriteLine(string.Join(", ", props.Select(p => p.Name).ToArray<string>()));

				foreach (var item in items)
				{
					// Debug.Log (string.Format("Writing properties for '{0}'.", item.ToString()));

					writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null).ToString()).ToArray<string>()));
				}
			}
		}
	}
}