define("AdminFileManager372b1fefSection", ["AdminFileManager372b1fefSectionResources"], function(resources) {
	return {
		entitySchemaName: "AdminFileManager",
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "merge",
				"name": "SeparateModeAddRecordButton",
				"values": {
					"visible": false
				}
			},
			{
				"operation": "merge",
				"name": "DataGridActiveRowOpenAction",
				"values": {
					"visible": false
				}
			},
			{
				"operation": "merge",
				"name": "DataGridActiveRowCopyAction",
				"values": {
					"visible": false
				}
			},
			{
				"operation": "merge",
				"name": "DataGridActiveRowDeleteAction",
				"values": {
					"visible": false
				}
			},
			{
				"operation": "merge",
				"name": "DataGridActiveRowPrintAction",
				"values": {
					"visible": false
				}
			},
			{
				"operation": "merge",
				"name": "DataGridRunProcessAction",
				"values": {
					"visible": false
				}
			},
			{
				"operation": "merge",
				"name": "ProcessEntryPointGridRowButton",
				"values": {
					"visible": false
				}
			},
			{
				"operation": "merge",
				"name": "QuickFilterModuleContainer",
				"values": {
					"visible": false
				}
			}
		]/**SCHEMA_DIFF*/,
		methods: {
			
			getSectionActions: function() {
				var actionMenuItems = this.Ext.create("Terrasoft.BaseViewModelCollection");
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Click": {"bindTo": "openFileContent"},
					"Caption": "Открыть содержимое файла как текст",
					"Enabled": {"bindTo": "IsFile"}
				}));
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Click": {"bindTo": "openFileEditor"},
					"Caption": "Открыть файл в редакторе",
					"Enabled": {"bindTo": "IsFile"}
				}));
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Click": {"bindTo": "deleteItem"},
					"Caption": "Удалить",
					"Enabled": {"bindTo": "IsRealItem"}
				}));
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Click": {"bindTo": "renameItem"},
					"Caption": "Переименовать",
					"Enabled": {"bindTo": "IsRealItem"}
				}));
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Click": {"bindTo": "unzipFile"},
					"Caption": "Разархивировать в эту папку",
					"Enabled": {"bindTo": "IsZip"}
				}));
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Click": {"bindTo": "createDir"},
					"Caption": "Создать папку"
				}));
				actionMenuItems.addItem(this.getButtonMenuItem({
					"Click": {"bindTo": "uploadFile"},
					"Caption": "Загрузить файл"
				}));
				return actionMenuItems;
			},
			
			onActiveRowChange: function() {
				this.callParent(arguments);
				if(this.get("ActiveRow")){
					var data = this.getGridData();
					var activeRow = data.get(this.get("ActiveRow"));
					var type = activeRow.get("Type");
					var name = activeRow.get("FullPath");
					
					this.set("IsRealItem", false);
					this.set("IsZip", false);
					
					if(type === "File"){ 
						this.set("IsFile", true);
						this.set("IsDirectory", false);
						
						if(name.length > 4 && name.slice(-3) === "zip"){
							this.set("IsZip", true);
						}
					
					}
					
					if(type === "Directory"){ 
						this.set("IsFile", false);
						this.set("IsDirectory", true);
					}
					
					if(type !== "UpButton"){ 
						this.set("IsRealItem", true);
					}
				}
			},
			
			openFileContent: function(){
				var data = this.getGridData();
				var activeRow = data.get(this.get("ActiveRow"));
				var path = activeRow.get("FullPath");
				
				window.open("/0/rest/AdminFileManagerService/DownloadTextFile?path=" + path, "_blank");
			},
			
			deleteItem: function(){
				
				var data = this.getGridData();
				var activeRow = data.get(this.get("ActiveRow"));
				var path = activeRow.get("FullPath");
				
				if(this.get("IsFile") === true){
					this.showConfirmationDialog("Удалить файл? " + path,
						function(returnCode) {
							
							
							this.callService({
								serviceName: "AdminFileManagerService",
								methodName: "DeleteFile",
								data: { path: path }
							}, function(res){
								this.set("OpenPath", this.get("CurrentDir"));
								this.reloadGridData();
								this.Terrasoft.showInformation(res.DeleteFileResult);
							}, this);
						}, ["ok", "cancel"]);
				}
				
				if(this.get("IsDirectory") === true){
					this.showConfirmationDialog("Удалить папку вместе с содержимым? " + path,
						function(returnCode) {
							
							
							this.callService({
								serviceName: "AdminFileManagerService",
								methodName: "DeleteDirectory",
								data: { path: path }
							}, function(res){
								this.set("OpenPath", this.get("CurrentDir"));
								this.reloadGridData();
								this.Terrasoft.showInformation(res.DeleteDirectoryResult);
							}, this);
						}, ["ok", "cancel"]);
				}
			},
			
			unzipFile: function(){
				
				var data = this.getGridData();
				var activeRow = data.get(this.get("ActiveRow"));
				var path = activeRow.get("FullPath");
				
				this.callService({
					serviceName: "AdminFileManagerService",
					methodName: "Unzip",
					data: { path: path }
				}, function(res){
					
					this.set("OpenPath", this.get("CurrentDir"));
					this.reloadGridData();
					
				}, this);
			},
			
			renameItem: function(){
				
				var data = this.getGridData();
				var activeRow = data.get(this.get("ActiveRow"));
				var path = activeRow.get("FullPath");
				
				var oldName = path.split('\\').pop().split('/').pop();
				var newName = "";
				var scope = this;
				
				Terrasoft.utils.inputBox(
					"Переименовать",
					function(button, values){
						if(button === "ok"){
							var newName = values.name.value;
							
							var newPath = this.get("CurrentDir") + newName;
							var oldPath = this.get("CurrentDir") + oldName;
							
							this.callService({
								serviceName: "AdminFileManagerService",
								methodName: "Rename",
								data: { oldPath: oldPath, newPath: newPath }
							}, function(res){
								
								this.reloadGridData();
								
							}, this);
						}
					},
					["ok", "cancel"],
					this,
					{
						name: {
							dataValueType: Terrasoft.DataValueType.TEXT,
							caption: "Название",
							value: oldName,
							customConfig: {
								focused: true
							}
						}
					},
					{
						defaultButton: 0
					}
				);
			},
			
			createDir: function(){
				var scope = this;
				Terrasoft.utils.inputBox(
					"Введите название",
					function(button, values){
						if(button === "ok"){
							var caption = values.name.value;
							var path = this.get("CurrentDir") + "\\" + caption;
							this.callService({
								serviceName: "AdminFileManagerService",
								methodName: "CreateDirectory",
								data: { path: path }
							}, function(res){
								
								this.set("OpenPath", this.get("CurrentDir"));
								this.reloadGridData();
								
							}, this);
						}
					},
					["ok", "cancel"],
					this,
					{
						name: {
							dataValueType: Terrasoft.DataValueType.TEXT,
							caption: "Название",
							value: "",
							customConfig: {
								focused: true
							}
						}
					},
					{
						defaultButton: 0
					}
				);
			},
			
			initCanLoadMoreData: function(){
				this.set("CanLoadMoreData", false);
			},
		
			addPrimaryColumnLink: function(item, column) {
				var scope = this;
				var columnPath = column.columnPath;
				this.addColumnLinkClickHandler(item, column, function() {
					var displayValue = item.get(columnPath);
					return {caption: displayValue, target: "_self", title: displayValue, url: "#"};
				});
			},
			
			createLink: function(entitySchemaName, columnPath, displayValue, recordId) {
				return {
					caption: displayValue,
					target: "_self",
					title: displayValue,
					url: "#"
				};
			},
			
			openCard: function(){
				this.linkClicked(this.get("ActiveRow"));
			},
			
			linkClicked: function(rowId, columnName) {
				var name = this.getGridDataRow(rowId).get("FullPath");
				var type = this.getGridDataRow(rowId).get("Type");
				if(type === "Directory" || type === "UpButton"){
					this.set("OpenPath", name);
					this.reloadGridData();
				}
				
				if(type === "File"){
					if(name.slice(-2) === "cs" || 
						name.slice(-2) === "js" || 
						(name.length > 7 && name.slice(-6) === "config")){
							
						window.open("/0/rest/AdminFileEditorService/Editor?path=" + name, "_blank");
						return;
					}
					this.downloadFile(name);
				}
			},
			
			openFileEditor: function(){
				var data = this.getGridData();
				var activeRow = data.get(this.get("ActiveRow"));
				var path = activeRow.get("FullPath");
				window.open("/0/rest/AdminFileEditorService/Editor?path=" + path, "_blank");
			},
			
			downloadFile: function(path){
				var a = document.createElement("a");
				document.body.appendChild(a);
				a.style = "display: none";
				a.href = "/0/rest/AdminFileManagerService/DownloadFile?path=" + path;
				a.click();
				document.body.removeChild(a);
			},
			
			onFileSelect: function(file) {
				
				var grid = this.getGridData();
				var currentDir = this.get("CurrentDir");
				var path = encodeURI(currentDir + "\\" + file[0].name);
				
				var data = new FormData();
				data.append('fileContent', file[0]);
				var csrfToken = Ext.util.Cookies.get("BPMCSRF");
				$.ajax({
					url: "/0/rest/AdminFileManagerService/UploadFile?path=" + path,
					type: 'post',
					dataType: 'json',
					data: data,
					processData: false,
					contentType: false,
					headers: {"BPMCSRF": csrfToken || ""},
					success: function(arg)
					{
						this.set("OpenPath", currentDir);
						this.reloadGridData();
					}.bind(this)
				});
			},
			
			uploadFile: function() {
				var element = Ext.create("Terrasoft.Button", {
					"click": {"bindTo": "uploadFile"},
					"caption": "___",
					"fileUpload": true,
					"filesSelected": {"bindTo": "onFileSelect"}
				});
			   
				element.bindings.filesSelected.config = this;
				element.init();
				element.mixins.fileUpload.addInputFile.call(element);
				element.onClick(Ext.EventObject);
				element.on("filesSelected", this.onFileSelect, this);
			},

			loadGridData: function(){
				this.beforeLoadGridData();
				var esq = this.getGridDataESQ();
				this.initQueryColumns(esq);
				this.initQuerySorting(esq);
				this.initQueryFilters(esq);
				this.initQueryOptions(esq);
				this.initQueryEvents(esq);
				
				if(!this.get("OpenPath")){
					this.set("OpenPath", "");
				}

				this.set("IsFile", false);
				this.set("IsDirectory", false);		
				this.set("IsRealItem", false);	
				this.set("IsZip", false);				
				
				this.callService({
					serviceName: "AdminFileManagerService",
					methodName: "GetFileList",
					data: { path: this.get("OpenPath") }
				}, function(res){
				
					this.set("OpenPath", "");
					
					var rowConfig = { };
					
					var serviceResponse = JSON.parse(res.GetFileListResult);
					var fileList = serviceResponse.Files;
					var currentDir = serviceResponse.CurrentDir;
					this.set("CurrentDir", currentDir);
					
					if(fileList.length > 0){
						for(var colName in fileList[0]){
							var dataValueType = 1;
							if(colName === "Id"){
								dataValueType = 0;
							}
							rowConfig[colName] = {
								dataValueType:dataValueType, 
								columnPath: colName
							};
						}
					}
					
					var g = Ext.create("Terrasoft.BaseViewModelCollection", {
						entitySchema: "AdminFileManager",
						rowConfig: rowConfig
					});
				
					Terrasoft.each(fileList, function(column, columnName) {
						var rowConfig = { };
						for(var colName in column) {
								var dataValueType = 1;
								if(colName === "Id"){
								dataValueType = 0;
							}
							rowConfig[colName] = {
								dataValueType:dataValueType, 
								columnPath: colName
							};
						}
							
						var item = Ext.create(esq.rowViewModelClassName, {
							entitySchema: this.entitySchema,
							rowConfig: rowConfig,
							values: column,
							isNew: !1,
							isDeleted: !1
						});
						
						g.add(item.get("Id"), item);
					}, this);
				
					var response = {
						success: true,
						collection: g,
						errorInfo: ""
					};
					
					this.destroyQueryEvents(esq);
					this.updateLoadedGridData(response, this.onGridDataLoaded, this);
					this.checkNotFoundColumns(response);
				
				}, this);
			}
		}
	};
});
