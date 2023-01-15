using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Stratus.Extensions;

namespace Stratus.IO
{
	/// <summary>
	/// Utlity functions for IO operations
	/// </summary>
	public static class StratusIO
	{
		//------------------------------------------------------------------------/
		// Properties
		//------------------------------------------------------------------------/
		public static bool IsInMacOS => SystemInfo.operatingSystem.IndexOf("Mac OS") != -1;
		public static bool IsInWinOS => SystemInfo.operatingSystem.IndexOf("Windows") != -1;

		//------------------------------------------------------------------------/
		// Methods
		//------------------------------------------------------------------------/
		/// <summary>
		/// Returns a relative path of an asset
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string MakeRelative(string path)
		{
			var relativePath = "Assets" + path.Substring(Application.dataPath.Length);
			relativePath = relativePath.Replace("\\", "/");
			return relativePath;
		}

		/// <summary>
		/// Reveals the file/directory at the given path
		/// </summary>
		/// <param name="path"></param>
		public static void Open(string path)
		{
			if (IsInWinOS)
			{
				OpenInWin(path);
			}
			else if (IsInMacOS)
			{
				OpenInMac(path);
			}
			else // couldn't determine OS
			{
				OpenInWin(path);
				OpenInMac(path);
			}
		}

		/// <summary>
		/// Returns the relative path of a given folder name (if found within the application's assets folder)
		/// </summary>
		/// <param name="folderName"></param>
		/// <returns></returns>
		public static string GetFolderPath(string folderName)
		{
			//var dirInfo = new DirectoryInfo(Application.dataPath);
			var dirs = GetDirectories(Application.dataPath);
			var folderPath = dirs.Find(x => x.Contains(folderName));
			return folderPath;
		}

		public static Texture2D LoadImage2D(string filePath, StratusImageEncoding encoding = StratusImageEncoding.JPG)
		{
			byte[] data = FileUtility.FileReadAllBytes(filePath);
			if (data == null)
			{
				return null;
			}
			Texture2D image = new Texture2D(0, 0);
			image.LoadImage(data);
			return image;
		}

		public static bool SaveImage2D(Texture2D texture, string filePath, StratusImageEncoding encoding = StratusImageEncoding.JPG)
		{
			byte[] data = null;
			switch (encoding)
			{
				case StratusImageEncoding.PNG:
					data = texture.EncodeToPNG();
					break;
				case StratusImageEncoding.JPG:
					data = texture.EncodeToJPG();
					break;
				default:
					break;
			}
			if (data == null)
			{
				return false;
			}
			File.WriteAllBytes(filePath, data);
			return true;
		}

		private static List<string> GetDirectories(string path)
		{
			List<string> dirs = new List<string>();
			ProcessDirectory(path, dirs);
			return dirs;
		}

		private static void ProcessDirectory(string dirPath, List<string> dirs)
		{
			// Add this directory
			dirs.Add(MakeRelative(dirPath));
			// Now look for any subdirectories
			var subDirectoryEntries = Directory.GetDirectories(dirPath);
			foreach (var dir in subDirectoryEntries)
			{
				ProcessDirectory(dir, dirs);
			}
		}

		public static void OpenInMac(string path)
		{
			bool openInsidesOfFolder = false;

			// try mac
			string macPath = path.Replace("\\", "/"); // mac finder doesn't like backward slashes

			if (Directory.Exists(macPath)) // if path requested is a folder, automatically open insides of that folder
			{
				openInsidesOfFolder = true;
			}

			if (!macPath.StartsWith("\""))
			{
				macPath = "\"" + macPath;
			}

			if (!macPath.EndsWith("\""))
			{
				macPath = macPath + "\"";
			}

			string arguments = (openInsidesOfFolder ? "" : "-R ") + macPath;

			try
			{
				System.Diagnostics.Process.Start("open", arguments);
			}
			catch (System.ComponentModel.Win32Exception e)
			{
				// tried to open mac finder in windows
				// just silently skip error
				// we currently have no platform define for the current OS we are in, so we resort to this
				e.HelpLink = ""; // do anything with this variable to silence warning about not using it
			}
		}

		public static void OpenInWin(string path)
		{
			bool openInsidesOfFolder = false;

			// try windows
			string winPath = path.Replace("/", "\\"); // windows explorer doesn't like forward slashes

			if (Directory.Exists(winPath)) // if path requested is a folder, automatically open insides of that folder
			{
				openInsidesOfFolder = true;
			}

			try
			{
				System.Diagnostics.Process.Start("explorer.exe", (openInsidesOfFolder ? "/root," : "/select,") + winPath);
			}
			catch (System.ComponentModel.Win32Exception e)
			{
				// tried to open win explorer in mac
				// just silently skip error
				// we currently have no platform define for the current OS we are in, so we resort to this
				e.HelpLink = ""; // do anything with this variable to silence warning about not using it
			}
		}
	}
}
