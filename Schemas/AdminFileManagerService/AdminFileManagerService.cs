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

	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class AdminFileManagerService
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
		[WebInvoke(Method = @"POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public string GetFileList(string path)
		{
			var fmClass = new AdminFileManagerClass(UserConnection);
			return fmClass.GetFileList(path);
		}
		
		[OperationContract]
		[WebInvoke(Method = @"POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public string UploadFile(Stream fileContent)
		{
			string path = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["path"];
			var fmClass = new AdminFileManagerClass(UserConnection);
			return fmClass.UploadFile(fileContent, path);
		}

		[OperationContract]
		[WebInvoke(Method = @"GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public Stream DownloadFile(string path)
		{
			var fmClass = new AdminFileManagerClass(UserConnection);
			return fmClass.DownloadFile(path);
		}

		[OperationContract]
		[WebInvoke(Method = @"POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public string DeleteFile(string path)
		{
			var fmClass = new AdminFileManagerClass(UserConnection);
			return fmClass.DeleteFile(path);
		}
		
		[OperationContract]
		[WebInvoke(Method = @"POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public string DeleteDirectory(string path)
		{
			var fmClass = new AdminFileManagerClass(UserConnection);
			return fmClass.DeleteDirectory(path);
		}

		[OperationContract]
		[WebInvoke(Method = @"POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public string CreateDirectory(string path)
		{
			var fmClass = new AdminFileManagerClass(UserConnection);
			return fmClass.CreateDirectory(path);
		}

		[OperationContract]
		[WebInvoke(Method = @"GET", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public Stream DownloadTextFile(string path)
		{
			var fmClass = new AdminFileManagerClass(UserConnection);
			return fmClass.DownloadTextFile(path);
		}

		[OperationContract]
		[WebInvoke(Method = @"POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public string Rename(string oldPath, string newPath)
		{
			var fmClass = new AdminFileManagerClass(UserConnection);
			return fmClass.Rename(oldPath, newPath);
		}

		[OperationContract]
		[WebInvoke(Method = @"POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public string Unzip(string path)
		{
			var fmClass = new AdminFileManagerClass(UserConnection);
			return fmClass.Unzip(path);
		}
	}
}
