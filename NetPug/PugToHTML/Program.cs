using System;
using System.Diagnostics;

string scriptPath = "scripts/convertPug.js"; // Make sure to provide the correct path

// Validate the script path
if (!File.Exists(scriptPath))
{
    Console.WriteLine("The specified script path does not exist: " + scriptPath);
    return;
}

ProcessStartInfo startInfo = new ProcessStartInfo
{
    FileName = "node",
    Arguments = scriptPath,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
    UseShellExecute = false,
    CreateNoWindow = true,
};

using (Process process = new Process { StartInfo = startInfo })
{
    try
    {
        process.Start();

        // Read output (stdout)
        string output = process.StandardOutput.ReadToEnd();
        Console.WriteLine("Standard Output: ");
        Console.WriteLine(output);

        // Read errors (stderr)
        string errors = process.StandardError.ReadToEnd();
        if (!string.IsNullOrEmpty(errors))
        {
            Console.WriteLine("Standard Error: ");
            Console.WriteLine(errors);
        }

        process.WaitForExit();
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred: " + ex.Message);
    }
}