using System;
using System.Linq;
using System.Diagnostics;

public class MainClass {
	const int min_seconds = 60;
	
	static void Main() {
		for (;;)
		{
			var start = DateTime.Now;
			var info = new ProcessStartInfo("xbuild") {
				RedirectStandardError = true,
				UseShellExecute = false,
				RedirectStandardOutput = true,
			};
			var proc = Process.Start(info);
			proc.WaitForExit();
			if (proc.ExitCode != 0) {
				var allerrs = proc.StandardError
				.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
				.Concat(
					proc.StandardOutput.ReadToEnd().Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
				)
				.Where(line => line.Contains("error"))
				.ToList();
				
				
				Process.Start("osascript", string.Format("-e 'display notification \"{0}\" with title \"Home CI\"'",
					string.Join(" / ", allerrs.Select(line => line.Replace("\"", "\\\"").Replace("'", "`")))
					.Substring(0, 500)));
			}
			var wait = min_seconds - (int)(DateTime.Now - start).TotalSeconds;
			if (wait > 0)
				System.Threading.Thread.Sleep(wait * 1000);
		}
	}
}
