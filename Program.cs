using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test_ConsoleProgress
{
	class Program
	{
		static ConsoleProgressBar.Options options=new ConsoleProgressBar.Options{
			BarColor=ConsoleColor.Green,
			TextColor=ConsoleColor.Gray,
			fClearOnDone=true
		};
		static ConsoleProgressBar.Options options2=new ConsoleProgressBar.Options{
			BarColor=ConsoleColor.DarkCyan,
			TextColor=ConsoleColor.White,
			maximumWidth=40,
			fClearOnDone=true
		};

		static void Main(string[] args)
		{
			ConsoleProgressBar progress2 = new ConsoleProgressBar(options2);

			Console.WriteLine("---------------------------------------------------------");
			Console.WriteLine(" Previous Text");
			for(int i=0; i<5; i++)
				Console.WriteLine("Line "+i.ToString());
				Console.WriteLine(" Previous Text");

			while(true){
				Console.WriteLine("---------------------------------------------------------");

				using(ConsoleProgressBar progress = new ConsoleProgressBar(options) ) 
				{
					int i;
					for (i=0; i<=100; i++)
					{
						progress.Update(i);
						progress2.Update(i/2);
						if(i==30) options.BarColor=ConsoleColor.Red;
						if(i%20==0) Console.WriteLine($" text below {Console.CursorTop} ");

						Thread.Sleep(50);
					}
					progress.Done(TextDone: " DONE!", ColorTextDone: ConsoleColor.Yellow );

					for (i=50; i<=100; i++)
					{
						progress2.Update(i);
						Thread.Sleep(50);
					}
					progress2.Done(TextDone: " DONE 2!", ColorTextDone: ConsoleColor.DarkCyan );

				}

				Thread.Sleep(500);
			}
			//Thread.Sleep(500);
			//Console.WriteLine("demo end");
			//Console.ReadKey();

		}


	}
}
