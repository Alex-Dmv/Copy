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
using System.Threading;
using NDesk.Options;


namespace CopyFiles
{
	class Program
	{	
		static ulong totalSize;
		
		class CommandLineArguments {
			
			public string SourceDir {get; set;}
			public string DestDir {get; set;}
			public bool Delete {get; set;}
			public bool Recursive {get; set;}
			public bool Print {get; set;}
			public byte CountThreads {get; set;}
			public int Interval {get; set;}
			
			public CommandLineArguments()
			{
				this.SourceDir = null;
				this.DestDir = null;
				this.Interval = -1;
				this.Delete = false;
				this.Print = false;
				this.Recursive = false;
				this.CountThreads = 1;
			}
			
		}
		
		
		static void RecursiveCopyFiles(string sourceDir, string destDir, bool print, bool delete)
		{
			string[] includeDirs = Directory.GetDirectories(sourceDir);
			foreach (string dirName in includeDirs)
			{
				string s = Path.GetFileName(dirName); //Костыль, получает имя вложенного каталога как имя файла
				string dirNameDest = Path.Combine(destDir, s);
				if (!Directory.Exists(dirNameDest))
				{
					Directory.CreateDirectory(dirNameDest);
				}
				RecursiveCopyFiles(dirName, dirNameDest, print, delete);
			}	
			NotRecursiveCopyFiles(sourceDir, destDir, print, delete);
		}
		
		static void NotRecursiveCopyFiles(string sourceDir, string destDir, bool print, bool delete)
		{
			string[] fileNames = Directory.GetFiles(sourceDir);
			foreach (string fileName in fileNames)
			{
				string fileNameDest = Path.Combine(destDir, Path.GetFileName(fileName));
				if (!File.Exists(fileNameDest))
				{
					File.Copy(fileName, fileNameDest);
					totalSize += (ulong)(new FileInfo(fileName).Length);
					if (print) 
					Console.WriteLine(fileName);
					if (delete)
					File.Delete(fileName);
				}
			}
		}
		
		static void CopyFiles(CommandLineArguments args)
		{
			if (args.Recursive)
			{
				RecursiveCopyFiles(args.SourceDir, args.DestDir, args.Print, args.Delete);
			}
			else
			{
				NotRecursiveCopyFiles(args.SourceDir, args.DestDir, args.Print, args.Delete);
			}
		}
		
		//Обертка для функции копирования для передачи в Timer
		static void OnTimer(object obj)
		{
			CommandLineArguments args = obj as CommandLineArguments;
			if (args == null)
				return;
			CopyFiles(args);
		}
				
		public static void Main(string[] args)
		{
			CommandLineArguments arguments = new CommandLineArguments();
			var options = new OptionSet() {
				{"IN=", "Path to source directory", v => arguments.SourceDir = v},
				{"OUT=", "Path to destination directory", v => arguments.DestDir = v},
				{"I=", "Time interval (seconds)", v => arguments.Interval = Int32.Parse(v)},
				{"T=", "Count of threads", v => arguments.CountThreads = Byte.Parse(v)},
				{"D", "Delete files that copied", v => arguments.Delete = v != null},
				{"R", "Recursive copy files", v => arguments.Recursive = v !=null},
				{"P", "Print copy files", v => arguments.Print = v != null},
			};
			try 
			{
				//Проверяем переданные параметры
				var unrecognized = options.Parse(args);
				if (unrecognized.Count != 0)
				{
					foreach (var item in unrecognized)
					{
						Console.WriteLine("Unrecognized option: {0}", item);
					}
					return;
				}
				if (arguments.SourceDir == null || !Directory.Exists(arguments.SourceDir))
				{
					Console.WriteLine("Source directory don't specify or don't exist's.");
					return;
				}
				if (arguments.DestDir == null || !Directory.Exists(arguments.DestDir))
				{
					Console.WriteLine("Destination directory don't specify or don't exist's.");
					return;
				}
				if (arguments.Interval <= 0)
				{
					Console.WriteLine("You didn't specify time interval.");
					return;
				}
				
				//Запускаем копирование
                Timer timer = new Timer(OnTimer, arguments, 0, arguments.Interval * 1000);
                string s = "";
        		do
        		{
        			Console.WriteLine(@"Input 'stop' to terminate process");
            		s = Console.ReadLine(); 
        		} while (s != "stop");
        		timer.Dispose();
        		Console.WriteLine("Total size: {0} bytes", totalSize);
			}
			catch (FormatException e) 
			{
	            Console.Write ("FormatException: ");
	            Console.WriteLine (e.Message);
	            return;
            }
			catch (Exception e)
			{
				Console.WriteLine (e.Message);
	            return;
			}
            Console.ReadKey(true);			
		}
	}
}