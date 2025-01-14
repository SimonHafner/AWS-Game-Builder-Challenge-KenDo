using System.Collections.Generic;
using System.IO;

public static class TextFileParser
{
    public static List<List<int>> ParseFile(string filePath)
    {
        List<List<int>> dataList = new List<List<int>>();

        // Read the text file
        string fileText = File.ReadAllText(filePath);

        // Split the text file by line breaks
        string[] lines = fileText.Split('\n');

        // Parse each line and add it to the list
        foreach (string line in lines)
        {
            List<int> rowList = new List<int>();

            // Split the line by commas and parse each value as an integer
            string[] values = line.Trim().Split(',');
            foreach (string value in values)
            {
                int parsedValue;
                if (int.TryParse(value.Trim(), out parsedValue))
                {
                    rowList.Add(parsedValue);
                }
            }

            // Add the row list to the data list
            if (rowList.Count > 0)
            {
                dataList.Add(rowList);
            }
        }

        return dataList;
    }

    public static List<string> GetTxtFilesInFolder(string folderPath)
{
    // Create a list to store the names of the .txt files
    List<string> txtFileNames = new List<string>();

    // Get an array of all the file paths in the folder
    string[] filePaths = Directory.GetFiles(folderPath);

    // Loop through all the file paths
    foreach (string filePath in filePaths)
    {
        // Check if the file is a .txt file
        if (Path.GetExtension(filePath).Equals(".txt"))
        {
            // If it is, add the file name to the list of .txt file names
            txtFileNames.Add(Path.GetFileName(filePath));
        }
    }

    // Return the list of .txt file names
    return txtFileNames;
}
}
