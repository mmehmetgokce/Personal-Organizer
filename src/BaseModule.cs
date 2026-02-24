using System;
using System.IO;
using System.Windows.Forms;

namespace PersonalOrganizer
{
    public abstract class BaseModule
    {
        protected string DataFilePath { get; set; }

        protected BaseModule(string dataFileName)
        {
            string appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PersonalOrganizer");
            Directory.CreateDirectory(appDataPath);
            DataFilePath = Path.Combine(appDataPath, dataFileName);
        }

        protected virtual void SaveData(string data)
        {
            File.WriteAllText(DataFilePath, data);
        }

        protected virtual string LoadData()
        {
            return File.Exists(DataFilePath) ? File.ReadAllText(DataFilePath) : string.Empty;
        }

        public abstract void Show();
    }
}