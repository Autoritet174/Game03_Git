using IniParser;
using IniParser.Model;
namespace Assets.GameData.Scripts
{

    public class Ini
    {
        private readonly FileIniDataParser parser = new();
        private readonly string fileName;

        public Ini(string fileName)
        {
            this.fileName = fileName;
        }

        public void Write(string section, string key, string value)
        {
            IniData data = parser.ReadFile(fileName);
            data[section][key] = value;
            parser.WriteFile(fileName, data);
        }

        public string Read(string section, string key)
        {
            IniData data = parser.ReadFile(fileName);
            return data[section][key];
        }

        public static Ini Create(string fileName)
        {
            return new Ini(fileName);
        }
    }
}