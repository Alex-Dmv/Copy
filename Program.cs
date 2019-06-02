/*
 * Создано в SharpDevelop.
 * Пользователь: Администратор
 * Дата: 01.06.2019
 * Время: 22:42
 * 
 * Для изменения этого шаблона используйте меню "Инструменты | Параметры | Кодирование | Стандартные заголовки".
 */
using System;
using System.IO;
using System.Timers;
using NDesk.Options;


namespace CopyFiles
{
	class Program
	{
		static void CopyFiles(string sourceDir, string destDir, bool print, bool delete)
		{
			if (Directory.Exists(sourceDir))
			{
				string[] fileNames = Directory.GetFiles(sourceDir);
				foreach (string fileName in fileNames)
				{
					string fileNameDest = Path.Combine(destDir, Path.GetFileName(fileName));
					File.Copy(fileName, fileNameDest);
					if (print) 
						Console.WriteLine(fileName);
					if (delete)
						File.Delete(fileName);
				}
			}
			else
			{
				Console.WriteLine("Directory isn't exists.");
				return;
			}
		}
				
		public static void Main(string[] args)
		{
			string sourceDir = null,
			       destDir = null;
			bool delete = false,
			     recursive = false,
			     print = false;
			byte count_threads = 1;
			int interval = -1;
			var p = new OptionSet() {
				{"IN=", "Path to source directory", v => sourceDir = v},
				{"OUT=", "Path to destination directory", v => destDir = v},
				{"I=", "Interval of copy files", v => interval = Int32.Parse(v)},
				{"T", "Count of threads", v => count_threads = Byte.Parse(v)},
				{"D", "Delete files that copied", v => delete = v != null},
				{"R", "Recursive copy files", v => recursive = v !=null},
				{"P", "Print copy files", v => print = v != null}
			};
			try 
			{
				p.Parse(args);
				if (sourceDir == null) 
				{
					Console.WriteLine("You didn't specify the source directory.");
					return;
				}
				if (destDir == null)
				{
					Console.WriteLine("You didn't specify the destination directory.");
					return;
				}
				if (interval == -1)
				{
					Console.WriteLine("You didn't specify the interval of copy.");
					return;
				}
				CopyFiles(sourceDir, destDir, print, delete);
			}
			catch (OptionException e) 
			{
	            Console.Write ("bundling: ");
	            Console.WriteLine (e.Message);
	            Console.WriteLine ("Try `greet --help' for more information.");
	            return;
            }
			Console.Write(@"Input 'stop' and press 'Enter'");
			Console.ReadKey(true);
		}
	}
}