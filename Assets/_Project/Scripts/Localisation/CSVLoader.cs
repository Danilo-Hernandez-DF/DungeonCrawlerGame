using System.Text.RegularExpressions;

namespace Localisation
{
    public class CSVLoader
    {
        private TextAsset csvFile;
        private char lineSeparator = '\n';
        private char surround = '"';
        private string[] fieldSeparator = { "\",\"" };

        public void LoadCSV(TextAsset language)
        {
            csvFile = language;
        }

        public Dictionary<string, string> GetDictionaryValues()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string[] lines = csvFile.text.Split(lineSeparator);

            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string[] fields = CSVParser.Split(line);

                for (int f = 0; f < fields.Length; f++)
                {
                    fields[f] = fields[f].TrimStart(' ', surround);
                    fields[f] = fields[f].TrimEnd(surround);
                }

                var key = fields[0];
                if (dictionary.ContainsKey(key)) { continue; }

                var value = fields[1];
                dictionary.Add(key, value);
            }

            return dictionary;
        }
    }
}   