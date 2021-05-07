namespace Terrasoft.Configuration
{
	using System;
	using System.Text;
	using System.Web;
	using System.ServiceModel;
	using System.ServiceModel.Web;
	using System.ServiceModel.Activation;
	using Terrasoft.Core;
	using System.Collections.Generic;

	using System.IO;
	using System.Linq;
	using Terrasoft.Core.Entities;
	using System.Text.RegularExpressions;

	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class AdminFileEditorService
	{

		public UserConnection userConnection = null;

		public UserConnection UserConnection
		{
			get
			{
				if (this.userConnection == null)
				{
					this.userConnection = (UserConnection)HttpContext.Current.Session[@"UserConnection"];
				}
				return this.userConnection;
			}
		}

		[OperationContract]
		[WebInvoke(Method = @"GET",
			RequestFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		public Stream Editor(string path)
		{
			try
			{
				var lang = "csharp";

				Path.GetExtension(path); //.cs

				switch (Path.GetExtension(path))
				{
					case ".cs":
						lang = "csharp";
						break;
					case ".js":
						lang = "javascript";
						break;
					case ".config":
						lang = "xml";
						break;
				}

				var fileContent = System.IO.File.ReadAllText(path);
				var editor = GetContent("AdminEditor");
				editor = editor.Replace("<%code%>", Base64Encode(fileContent));
				editor = editor.Replace("<%title%>", Path.GetFileName(path));
				editor = editor.Replace("<%currentFilePath%>", path.Replace("\\", "\\\\"));
				editor = editor.Replace("<%lang%>", lang);
				var resp = WebOperationContext.Current.OutgoingResponse;
				resp.ContentType = "text/html";
				var bytes = Encoding.UTF8.GetBytes(editor);

				WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = string.Format("inline; filename=\"{0}\"", "editor");

				return new MemoryStream(bytes);
			}
			catch (Exception e)
			{
				return new MemoryStream(Encoding.UTF8.GetBytes(e.ToString()));
			}
		}

		[OperationContract]
		[WebInvoke(Method = @"GET",
			RequestFormat = WebMessageFormat.Json,
			BodyStyle = WebMessageBodyStyle.Wrapped,
			ResponseFormat = WebMessageFormat.Json)]
		public Stream AceJs(string lang)
		{
			try
			{
				var ace = GetContent("acejs");

				switch (lang)
				{
					case "csharp":
						ace += GetContent("acejs_mode_csharp");
						break;
					case "javascript":
						ace += GetContent("acejs_mode_javascript");
						break;
					case "xml":
						ace += GetContent("acejs_mode_xml");
						break;
				}

				var resp = WebOperationContext.Current.OutgoingResponse;
				resp.ContentType = "text/javascript";
				var bytes = Encoding.UTF8.GetBytes(ace);

				WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = string.Format("inline; filename=\"{0}\"", "ace.js");

				return new MemoryStream(bytes);
			}
			catch (Exception e)
			{
				return new MemoryStream(Encoding.UTF8.GetBytes(e.ToString()));
			}
		}

		[OperationContract]
		[WebInvoke(Method = @"POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public string SaveText(string path, string content)
		{
			try
			{
				System.IO.File.WriteAllText(path, content);
				UpdateDescriptor(path);

				return "File saved";
			}
			catch (Exception e)
			{
				return e.ToString();
			}
		}

		private void UpdateDescriptor(string filePath)
		{
			var dirPath = Path.GetDirectoryName(filePath);
			var descriptionPath = Path.Combine(dirPath, "descriptor.json");

			if (System.IO.File.Exists(descriptionPath))
			{
				var descriptionContent = System.IO.File.ReadAllText(descriptionPath);
				DateTime date = DateTime.Now;
				long unixTime = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0))).TotalSeconds;
				var descriptionContentNew = Regex.Replace(descriptionContent, "Date\\(([0-9])+\\)", $"Date({unixTime}000)");
				System.IO.File.WriteAllText(descriptionPath, descriptionContentNew);
			}
		}

		private string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}

		private string GetContent(string name)
		{
			var esq = new EntitySchemaQuery(UserConnection.EntitySchemaManager, "SysSchemaContent");
			esq.AddColumn("Content");
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "SysSchema.Name", name));
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "ContentType", 0));
			esq.Filters.Add(esq.CreateFilterWithParameters(FilterComparisonType.Equal, "SysSchema.ManagerName", "ClientUnitSchemaManager"));
			var esqCollection = esq.GetEntityCollection(UserConnection);

			if (esqCollection.Count > 0)
			{
				return Encoding.UTF8.GetString(esqCollection[0].GetBytesValue("Content"));
			}
			else
			{
				return null;
			}
		}
	}
}
