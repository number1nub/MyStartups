using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;


namespace MyStartups
{
	public partial class MainWindow : Form
	{
		StartupFiles fileList = new StartupFiles();


		public MainWindow()
		{
			InitializeComponent();			
		}

		private void MainWindow_Load(object sender, EventArgs e)
		{
			var ver = Assembly.GetExecutingAssembly().GetName().Version;
			this.Text = Application.ProductName + " version " + ver.Major + "." + ver.Minor + "." + ver.Build;

			LoadForm();
		}


		private void LoadForm()
		{
			fileList.Load();
            cProgList.Items.Clear();
			
			if (fileList.Count > 0)
			{
				int i = 0;				
			
				foreach (var entry in fileList)
				{
					cProgList.Items.Add(entry.FileName).SubItems.Add(entry.FilePath);
					cProgList.Items[i].Checked = !CheckRunning(entry.FileName);
					i++;
				}
			}
		}


		private bool CheckRunning(string procName)
		{
			foreach (var proc in Process.GetProcesses())
			{
				if (proc.ProcessName.Contains(procName))
					return true;
			}
			return false;
		}


		private void btnRunAll_Click(object sender, EventArgs e)
		{
			int i = 0;
			foreach (var file in fileList)
			{
				if (cProgList.Items[i].Checked)
					Process.Start(file.FilePath);
				i++;
			}
			Close();
		}


		private void btnAdd_Click(object sender, EventArgs e)
		{
			var dlg = new OpenFileDialog();
			dlg.Multiselect = false;
			var selFile = dlg.ShowDialog(this);

			if (selFile == DialogResult.OK)
			{
				fileList.Add(new StartupFile(dlg.FileName));
				fileList.Export();
				LoadForm();
			}
		}


		private void btnEdit_Click(object sender, EventArgs e)
		{
			var newPath = Interaction.InputBox(Properties.Resources.promptEditItem, "Edit Item", cProgList.SelectedItems[0].SubItems[1].Text);
			if (newPath == "" || newPath == null)
				return;
			fileList.Delete(cProgList.SelectedItems[0].SubItems[1].Text);
			fileList.Add(new StartupFile(newPath));
			fileList.Export();
			LoadForm();
		}


		private void btnDelete_Click(object sender, EventArgs e)
		{
			var res = MessageBox.Show(this, Properties.Resources.promptDeleteItem, "Detele Item", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
			if (res == DialogResult.Cancel)
				return;
			fileList.Delete(cProgList.SelectedItems[0].SubItems[1].Text);
			fileList.Export();
			LoadForm();
		}


		private void cProgList_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cProgList.SelectedItems.Count > 0)
			{
				btnDelete.Enabled = true;
				btnDelete.BackColor = Color.LightCoral;
				btnEdit.Enabled = true;
				btnEdit.BackColor = Color.LightSkyBlue;
			}
			else
			{
				btnDelete.Enabled = false;
				btnEdit.Enabled = false;
				btnDelete.BackColor = btnEdit.BackColor = Color.Transparent;
			}
		}

		private void All_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyData == Keys.Escape)
				this.Close();
		}

		private void cProgList_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				var itm = cProgList.GetItemAt(e.X, e.Y);
				if (itm != null)
				{
					itm.Selected = true;
					contextMenu.Show(cProgList, e.Location);
				}
			}
		}

		private void runNowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			string errList = null;
			if (cProgList.SelectedItems.Count > 0)
			{
				for (int i = 0; i < cProgList.SelectedItems.Count; i++)
				{
					string thisPath = cProgList.SelectedItems[i].SubItems[1].Text;
					if (!RunFile(thisPath))
						errList += (string.IsNullOrEmpty(errList) ? "" : "\n") + thisPath;
					else
						cProgList.SelectedItems[i].Checked = false;
				}
			}
			if (errList != null)
				MessageBox.Show(this, "Unable to run the following file(s):\n\n" + errList, "My Startups - Errors", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}



		private static bool RunFile(string fPath)
		{
			var proc = new Process();
			var pStart = new ProcessStartInfo(fPath);

			pStart.WorkingDirectory = new FileInfo(fPath).DirectoryName;
			pStart.ErrorDialog = false;
			
			proc.StartInfo = pStart;
			return proc.Start();
		}




		//private static string RunFile(string fPath)
		//{
		//	string errList = null;
		//	try
		//	{
		//		if (File.Exists(fPath)) {
		//			var startInfo = new ProcessStartInfo(fPath);
					
		//			startInfo.RedirectStandardOutput = true;
		//			startInfo.UseShellExecute  = false;
		//			startInfo.CreateNoWindow   = true;
		//			startInfo.ErrorDialog      = false;
		//			startInfo.WorkingDirectory = new FileInfo(fPath).DirectoryName;

		//			Process proc = new Process();
		//			proc.StartInfo = startInfo;
		//			proc.Start();

		//			var logWriter = File.CreateText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "MyStartups.log"));
		//			logWriter.WriteLine();
		//			logWriter.WriteLine();
		//			logWriter.WriteLine(DateAndTime.Today.ToShortDateString() + "\t" + fPath);
		//			logWriter.WriteLine("------------------------------------------------------------------------------------");
		//			logWriter.WriteLine(proc.StandardOutput.ReadToEnd());
		//		}
		//		else
		//			errList = fPath;
		//	}
		//	catch (Exception ex) {
		//		errList += (errList != null ? "\n" : "") + string.Concat(fPath, " - [", ex.Message, "]");
		//		return errList;
		//	}
		//	return errList;
		//}
	}
}
