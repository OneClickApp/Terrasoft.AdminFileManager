<html lang="en">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1">
    <title><%title%></title></head>
<body>
<pre style="width: 100%; height: 100%" class="editor" id="editor"></pre>
<script src="/0/rest/AdminFileEditorService/AceJs?lang=<%lang%>"></script>

<script>
var currentFilePath = "<%currentFilePath%>";
ace.require("ace/ext/language_tools");
var editor = ace.edit("editor");
editor.session.setMode("ace/mode/<%lang%>");
editor.setTheme("ace/theme/visualstudio");
// enable autocompletion and snippets
editor.setOptions({
	enableBasicAutocompletion: true,
	enableSnippets: true,
	enableLiveAutocompletion: true

});

var code = window.atob('<%code%>');

editor.setValue(code, -1);

document.addEventListener("keydown", function(e) {
  if (e.keyCode == 83 && (navigator.platform.match("Mac") ? e.metaKey : e.ctrlKey)) {
    e.preventDefault();
	var config = {
		serviceName: "AdminFileEditorService",
		methodName: "SaveText",
		data: { path: currentFilePath, content: editor.getValue()},
		callback: function(responseText){
			var responseObj = JSON.parse(responseText);
			alert(responseObj.SaveTextResult);
		}
	}
	callService(config);
  }
}, false);

function callService(config){
	
	var serviceName = config.serviceName;
	var methodName = config.methodName;
	var data = config.data;
	var callback = config.callback;
	var csrfToken = getCookie("BPMCSRF");
	
	var http = new XMLHttpRequest();
	var url = "/0/rest/" + serviceName + "/" + methodName;
	var params = JSON.stringify(data);
	http.open("POST", url, true);
	http.setRequestHeader("Content-type", "application/json");
	http.setRequestHeader("BPMCSRF", csrfToken || "");
	http.onreadystatechange = function() {//Call a function when the state changes.
		if(http.readyState == 4 && http.status == 200) {
			callback.call(this, http.responseText);
		}
	}
	http.send(params);
}

function getCookie(name) {
	const value = `; ${document.cookie}`;
	const parts = value.split(`; ${name}=`);
	if (parts.length === 2) return parts.pop().split(';').shift();
}

</script>
</body>
</html>