using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MyStartups
{

	class StartupFile
	{
		public string FileName { get; set; }
		public string FilePath { get; set; }

		public StartupFile(string filePath, string fileName = "") {
			FilePath = filePath;
			FileName = fileName != "" ? fileName : GetNameFromPath();
		}

		public string GetNameFromPath() {
			return new FileInfo(FilePath).Name.Replace(new FileInfo(FilePath).Extension, "");
		}

		public string GetNameFromPath(string fPath) {
			return new FileInfo(fPath).Name.Replace(new FileInfo(fPath).Extension, "");
		}
	}



	class StartupFiles : List<StartupFile>
	{
		string configPath;
		string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		//
		// Initialize
		//
		public void Load() {
			this.Clear();
			configPath = Path.Combine(appDataPath, Application.CompanyName, Application.ProductName, "config.json");

			List<StartupFile> fList = Import();

			if (fList != null) {
				foreach (var file in fList) {
					if (file.FileName == null) {
						file.FileName = file.GetNameFromPath();
					}
					this.Add(file);
				}
			}
		}
		//
		// Export
		//
		public void Export() {
			if (!(new DirectoryInfo(Path.GetDirectoryName(configPath)).Exists))
				new DirectoryInfo(Path.GetDirectoryName(configPath)).Create();

			File.WriteAllText(configPath, JsonConvert.SerializeObject(this));
		}
		//
		// Delete
		//
		public void Delete(int fileIndex) {
			this.RemoveAt(fileIndex);
		}
		public void Delete(string filePath) {
			this.RemoveAt(GetFileIndex(filePath));
		}
		//
		// Get Index
		//
		private int GetFileIndex(string fPath) {
			int count = 0;
			foreach (var file in this) {
				if (file.FilePath == fPath)
					break;
				count++;
			}
			return count;
		}
		//
		// Import
		//
		private List<StartupFile> Import() {
			if (new FileInfo(configPath).Exists)
				return JsonConvert.DeserializeObject<List<StartupFile>>(File.ReadAllText(configPath));
			else
				return null;
		}
	}
}