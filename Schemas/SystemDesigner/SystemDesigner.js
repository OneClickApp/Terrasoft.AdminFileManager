define("SystemDesigner", ["SystemDesignerResources"],
	function(resources) {
	return {
		attributes: {},
		methods: {
			onAdminFileManagerLinkClick: function() {
              	this.sandbox.publish("PushHistoryState", {hash: "SectionModuleV2/AdminFileManager372b1fefSection/"});
				return false;
			}
		},
		diff: [
			{
				"operation": "insert",
				"propertyName": "items",
				"parentName": "ConfigurationTile",
				"name": "EsqFiltersHelperProcessLink",
				"values": {
					"itemType": Terrasoft.ViewItemType.LINK,
					"caption": {"bindTo": "Resources.Strings.AdminFileManagerLinkCaption"},
					"click": {"bindTo": "onAdminFileManagerLinkClick"}
				}
			}
		]
	};
});