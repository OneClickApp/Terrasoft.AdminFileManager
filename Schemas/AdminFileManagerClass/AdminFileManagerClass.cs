namespace Terrasoft.Configuration
{
	using System;
	using System.Text;
	using Terrasoft.Core;
	using System.Collections.Generic;
	using System.Linq;
	using System.IO;
	using System.ServiceModel.Web;
	using System.IO.Compression;

	public class AdminFileManagerClass
	{
		public class AdminFileInfo
		{
			public Guid Id { get; set; }
			public string Name { get; set; }
			public string FullPath { get; set; }
			public string Type { get; set; }
			public string ModifiedOn { get; set; }
			public string TextContent { get; set; }
			public string Size { get; set; }
		}

		public class Response
		{
			public List<AdminFileInfo> Files { get; set; }
			public string CurrentDir { get; set; }
		}

		private UserConnection userConnection;

		public AdminFileManagerClass(UserConnection userConnection)
		{
			this.userConnection = userConnection;
		}

		private bool isAdmin(UserConnection userConnection, bool throwException = true)
		{
			if (!userConnection.DBSecurityEngine.GetCanExecuteOperation("CanManageAdministration"))
			{
				if (throwException)
				{
					throw new Exception("user is not admin");
				}
				return false;
			}

			return true;
		}
		
		public string GetFileList(string path)
		{
			isAdmin(userConnection);

			var fileList = new List<AdminFileInfo>();
			var response = new Response();

			var _path = path;

			if (string.IsNullOrEmpty(path))
			{
				_path = System.AppDomain.CurrentDomain.BaseDirectory;
			}
			
			fileList.Add(new AdminFileInfo() { Id = Guid.NewGuid(), Name = "⮤ ..", FullPath = Path.GetDirectoryName(_path), Size = "", Type = "UpButton", ModifiedOn = "" });

			var directories = Directory.GetDirectories(_path);

			foreach (var dir in directories)
			{
				fileList.Add(new AdminFileInfo() { Id = Guid.NewGuid(), Name = "📁 " + Path.GetFileName(dir), FullPath = dir, Size = "", Type = "Directory", ModifiedOn = new DirectoryInfo(dir).LastWriteTime.ToString() });
			}

			DirectoryInfo directory = new DirectoryInfo(_path);

			foreach (var file in directory.GetFiles("*.*"))
			{
				var fileSize = BytesToString(new FileInfo(file.FullName).Length);

				fileList.Add(new AdminFileInfo() { Id = Guid.NewGuid(), Name = "📄 " + file.Name, FullPath = file.FullName, Size = fileSize, Type = "File", ModifiedOn = new FileInfo(file.FullName).LastWriteTime.ToString() });
			}

			response.Files = fileList;
			response.CurrentDir = _path;

			return Newtonsoft.Json.JsonConvert.SerializeObject(response);
		}

		public string UploadFile(Stream fileContent, string path)
		{
			isAdmin(userConnection);

			try
			{
				MultipartParserClass parser = new MultipartParserClass(fileContent);
				if (parser.Success)
				{
					System.IO.File.WriteAllBytes(Uri.UnescapeDataString(path), parser.FileContents);
					return parser.FileContents.Count().ToString() + Uri.UnescapeDataString(path);
				}
				else
				{
					return "Error";
				}
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
		
		public Stream DownloadFile(string path)
		{
			isAdmin(userConnection);

			if (!System.IO.File.Exists(path))
			{
				return null;
			}

			WebOperationContext.Current.OutgoingResponse.ContentType = "application/force-download";
			FileStream f = new FileStream(path, FileMode.Open);
			int length = (int)f.Length;
			WebOperationContext.Current.OutgoingResponse.ContentLength = length;
			byte[] buffer = new byte[length];
			int sum = 0;
			int count;
			while ((count = f.Read(buffer, sum, length - sum)) > 0)
			{
				sum += count;
			}
			f.Close();
			WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = string.Format("attachment; filename=\"{0}\"", Uri.EscapeUriString(Path.GetFileName(path)));
			return new MemoryStream(buffer);
		}
		
		public string DeleteFile(string path)
		{
			isAdmin(userConnection);

			try
			{
				if (!System.IO.File.Exists(path))
				{
					return "null";
				}
				System.IO.File.Delete(path);
				return "Ok";
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
		
		public string DeleteDirectory(string path)
		{
			isAdmin(userConnection);

			try
			{
				if (!System.IO.Directory.Exists(path))
				{
					return "null";
				}
				System.IO.Directory.Delete(path, true);
				return "Ok";
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}

		public string Rename(string oldPath, string newPath)
		{
			if (System.IO.File.Exists(oldPath))
			{
				try
				{
					System.IO.File.Move(oldPath, newPath);
					return "Ok";
				}
				catch (Exception e)
				{
					return e.ToString();
				}
			}
			else if(Directory.Exists(oldPath))
			{
				try
				{
					Directory.Move(oldPath, newPath);
					return "Ok";
				}
				catch (Exception e)
				{
					return e.ToString();
				}
			}

			return "File not found";
		}
		
		public string CreateDirectory(string path)
		{
			isAdmin(userConnection);

			try
			{
				if (!System.IO.Directory.Exists(Path.GetDirectoryName(path)))
				{
					return "null";
				}
				System.IO.Directory.CreateDirectory(path);
				return "Ok";
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}
		
		public Stream DownloadTextFile(string path)
		{
			isAdmin(userConnection);

			if (!System.IO.File.Exists(path))
			{
				return null;
			}

			WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
			FileStream f = new FileStream(path, FileMode.Open);
			int length = (int)f.Length;
			WebOperationContext.Current.OutgoingResponse.ContentLength = length;
			byte[] buffer = new byte[length];
			int sum = 0;
			int count;
			while ((count = f.Read(buffer, sum, length - sum)) > 0)
			{
				sum += count;
			}
			f.Close();

			return new MemoryStream(buffer);
		}

		public string Unzip(string path)
		{
			try
			{
				ZipFile.ExtractToDirectory(path, Path.GetDirectoryName(path));
				return "Ok";
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}

		private String BytesToString(long byteCount)
		{
			string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
			if (byteCount == 0)
				return "0" + suf[0];
			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return (Math.Sign(byteCount) * num).ToString() + suf[place];
		}
	}
}
