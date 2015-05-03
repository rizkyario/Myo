using System;
using System.Diagnostics;
using Xamarin;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.IO;

namespace Aura
{

	public static class Debug
	{
		public static void WriteLine (string message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
		{
			String trackType;
			var filename = Path.GetFileNameWithoutExtension(sourceFilePath);

			if(message.Contains("Triggered"))
				trackType = "Command";
			else
				trackType = "Other";

			Track(trackType, filename, memberName, message);
		}

		public static void Track (string trackType, string sourceFilePath, string memberName, string message )
		{
			var filename = Path.GetFileNameWithoutExtension(sourceFilePath);

			System.Diagnostics.Debug.WriteLine (String.Format ("({0}){1}>{2}: {3}", trackType, filename, memberName, message));
			Insights.Track (String.Format ("({0}){1}>{2}: {3}", trackType, filename, memberName, message));
		}

		public static void WriteLine (Exception value, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "")
		{
			var filename = Path.GetFileNameWithoutExtension(sourceFilePath);

			Track("Report", filename, memberName, value.Message);
			Insights.Report(value);
		}
	}
}

