using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ConsoleProgressBar : IDisposable
{
	private static List<ConsoleProgressBar> Instances=new List<ConsoleProgressBar>(3);
	private Options options;

	public ConsoleProgressBar(Options options=null)
	{
		this.options=(options!=null) ? options : new Options();
		Instances.Add(this);
	}

	public void StartNew()
	{
		_Line=-1;
		_fDone=false;
		_lastOutputLength=0;
	}
	public void Update(int percent)
	{
		// Remove the last state           
		string clear = string.Empty.PadRight(_lastOutputLength, '\b');
		Show(clear, ConsoleColor.Black);		//Color unused

		// Generate new state:
		ShowBar(percent);
		_fDone=false;
	}
	private int _lastOutputLength;
	const int _SpaceOnBottom=50;


	public void Done(bool fClear=false, string TextDone="", ConsoleColor? ColorTextDone=null)
	{
		if(_fDone) return;

		ConsoleColor textColor= (ColorTextDone!=null) ? ColorTextDone.Value : options.TextColor;
		if(fClear || options.fClearOnDone)
			WriteLine(_Line,TextDone, ColorTextDone);
		else Show("\r\n"+TextDone+"\r\n", textColor);
		_fDone=true;
		_Line=-1;
	}
	bool _fDone=false;

	private void Show(string value, ConsoleColor color)
	{
		CheckY();	//Max. Length prüfen
		int currentLeft=Console.CursorLeft;
		int currentLine=Console.CursorTop;
		if(_Line<0)	{
			_Line=Console.CursorTop;
			currentLine++;
		}

		Console.SetCursorPosition(0, _Line);
		var currentColor=Console.ForegroundColor;
		Console.ForegroundColor=color;
		Console.WriteLine(value);
		Console.ForegroundColor=currentColor;
		Console.SetCursorPosition(currentLeft, Math.Min(currentLine, Console.BufferHeight-1));		//Zeile wiederherstellen
	}
	protected int _Line {get; set; } = -1;
	private void ShowBar(int percent)
	{
		ShowBar(percent, string.Format(" {0}%", percent).PadRight(5) );
	}
	private void ShowBar(int percent, string Text)
	{
		CheckY();	//Max. Length prüfen
		int currentLeft=Console.CursorLeft;
		int currentLine=Console.CursorTop;
		if(_Line<0)	{
			_Line=Console.CursorTop;
			currentLine++;
		}

		Console.SetCursorPosition(0, _Line);
		var currentColor=Console.ForegroundColor;
		Console.ForegroundColor=options.BarColor;

		int maxWidth= (options.maximumWidth==int.MaxValue) ? Console.WindowWidth-5 : options.maximumWidth;
		int width = (int)(percent*maxWidth/100);
		string bar = new string(options.ProgressCharacter, width);
		Console.Write(bar);

		//Bar background:
		Console.ForegroundColor=options.BarBackgroundColor;
		Console.Write(string.Empty.PadLeft(maxWidth-width, options.ProgressBackgroundCharacter));

		Console.ForegroundColor=options.TextColor;
		Console.WriteLine(Text);
		Console.ForegroundColor=currentColor;
		Console.SetCursorPosition(currentLeft, Math.Min(currentLine, Console.BufferHeight-1));		//Zeile wiederherstellen
		_lastOutputLength = maxWidth+Text.Length;
	}

	public static void ClearCurrentConsoleLine()
	{
		int currentLineCursor = Console.CursorTop;
		Console.SetCursorPosition(0, Console.CursorTop);
		Console.Write(new string(' ', Console.WindowWidth)); 
		Console.SetCursorPosition(0, currentLineCursor);
	}
	public static void ClearLine(int line)
	{
		if(line<0) return;
		int currentLineCursor = Console.CursorTop;
		Console.SetCursorPosition(0, line);
		Console.Write(new string(' ', Console.WindowWidth)); 
		Console.SetCursorPosition(0, currentLineCursor);
	}
	public static void WriteLine(int line, string Text, ConsoleColor? ColorTextDone=null)
	{
		int currentLineCursor = Console.CursorTop;
		Console.SetCursorPosition(0, Math.Min(line, Console.BufferHeight-1));
		if(Text.Length<Console.WindowWidth) Text+= new string(' ', Console.WindowWidth-Text.Length);
		else Text=NormalizeLength(Text,Console.WindowWidth);
		var currentColor=Console.ForegroundColor;
		if(ColorTextDone!=null) Console.ForegroundColor= ColorTextDone.GetValueOrDefault();
		Console.Write(Text);
		if(ColorTextDone!=null) Console.ForegroundColor=currentColor;
		Console.SetCursorPosition(0, Math.Min(currentLineCursor, Console.BufferHeight-1));
	}
	static string NormalizeLength(string value, int maxLength)
	{
		return (String.IsNullOrEmpty(value) || value.Length<=maxLength) ?  value : value.Substring(0, maxLength);
	} 

	static int CheckY()
	{
		int spaceOnBotom=Math.Min(_SpaceOnBottom, Console.BufferHeight/5);
		int i=Console.CursorTop+(spaceOnBotom/2);
		if(i>=Console.BufferHeight)
		{
			(int X, int Y) Pos=(Console.CursorLeft, Console.CursorTop);
			for(i=0; i<spaceOnBotom; i++)
				Console.WriteLine();

			int offset=Pos.Y-Console.CursorTop+spaceOnBotom;
			MoveAllBars(offset);
			Console.SetCursorPosition(Pos.X, Pos.Y-offset);
			return spaceOnBotom;
		}
		else return 0;
	}

	private static void MoveAllBars(int Y)
	{
		foreach(ConsoleProgressBar Bar in Instances)
			Bar._Line -= Y;
	}

	public void Dispose()
	{
		Done();
		Instances.Remove(this);
	}

	public class Options
	{
		public bool fClearOnDone {get; set;} = false;
		/// <summary>
		/// If maximumWidth==int.MaxValue the whole window-width will be used
		/// </summary>
		public int maximumWidth {get; set;} = int.MaxValue;
		public ConsoleColor BarColor {get; set;} =ConsoleColor.White;
		public char ProgressCharacter {get; set;} = '\u2588';
		public ConsoleColor BarBackgroundColor {get; set;} =ConsoleColor.DarkGray;
		public char ProgressBackgroundCharacter {get; set;} = '\u2591';
		public ConsoleColor TextColor {get; set;} =ConsoleColor.White;
	}
}