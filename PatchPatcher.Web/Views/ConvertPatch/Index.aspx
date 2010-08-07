<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Git-SVN Patch Utility</h1>
    <p>
        When working with git-svn in the open source world, you sometimes want to submit
        patches for your work that can be applied on the svn repository using TortoiseSVN.
        But sadly the patch formats are incompatible - Tortoise fails reading git patches.</p>
    <p>
        The Git-SVN Patch Utility creates patches that are applyable via TortoiseSVN. Just provide the
        github url of your branch containing the changes and see the magic happen.
    </p>
    <p>
        If you do not have your changes online, you can use the commandline version to create
        patches from your local git repository.</p>
    <div id="caption">
        A github commit or branch url:</div>
    <input id="path" type="text" value="<%=ViewData["path"] %>" onkeyup="scheduleAnalyzePath()" onchange="scheduleAnalyzePath()" />
    <div id="pathresult">
        <script id="AnalysisResult" type="text/html">
            <#if (result.IsBranch && result.SvnUrl) {#>
                <div class="status">Detected a branch "<#= result.CommitOrBranch#>" with <#= result.Changes.length#> changes to "<#= result.SvnUrl#> (rev <#= result.SvnRevision#>)"</div>
                
                <h3>Options</h3>
                <ul>
                  <li><a href="<#= result.PermaLink#>">Link to this overview</a></li>
                  <li><a href="<#= result.DownloadUrl #>">Download as patch</a></li>
                  <li><a href="<#= result.ViewUrl #>">View as patch</a></li>
                  <li><a href="<#= result.CompareUrl #>">Show changes with Github Compare View</a></li>
                </ul>

                <h3 class="commits">Changes in <#= result.CommitOrBranch#></h3>
                <ul class="commits">
                    <# for (var pos = 0; pos < result.Changes.length; pos++) {#>
                        <#= parseTemplate(commitTemplate, result.Changes[pos]) #>
                    <# } #>
                </ul>
                
                <h3 class="commits">Based on r<#= result.SvnRevision#> in <#= result.SvnUrl#></h3>
                <ul class="commits">
                    <#= parseTemplate(commitTemplate, result.LastSvnCommit) #>
                </ul>

                <!--iframe width="100%" height="1000" src="<#= result.CompareUrl#>"></iframe-->
            <#} else if (result.IsBranch) {#>
                <div class="status error">Detected a branch "<#= result.CommitOrBranch#>", but none of the commits are from git-svn.</div>
            <#}#>
        </script>
        <script id="CommitTemplate" type="text/html">
         <li>
            <b>by <#= author.name #>:</b> 
            <span>
                <#if (message.length > 100) {#>
                   <#= htmlEncode(message.substring(0,100))#>
                   <a title="<#= message#>" href="#" onclick="$(this).parent().html(htmlEncode(this.title)); return false;">[more...]</a>
                <# } else { #>
                  <#= htmlEncode(message)#>
                <#}#>
            </span>
         </li>
        </script>
        <script id="ErrorTemplate" type="text/html">
         <div class="status error">
            <#= message.length > 100 ? message.substring(0,100) + "..." : message #>
         </div>
        </script>
        <script id="StatusTemplate" type="text/html">
         <div class="status">
            <#= message.length > 100 ? message.substring(0,100) + "..." : message #>
         </div>
        </script>
    </div>
    <pre id="result" width="100%">
      
    </pre>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        var template;
        var commitTemplate;
        var errorTemplate;
        var lastPath;

        var pathTimeout;
        function scheduleAnalyzePath() {
            if (pathTimeout != null) {
                window.clearTimeout(pathTimeout);
            }
            pathTimeout = setTimeout(analyzePath, 500)
        }

        var validate = '<%= Url.Action("Analyze", "ConvertPatch") %>';
        function analyzePath() {
            var path = $('#path').val();

            if (path == lastPath)
                return;

            lastPath = path;

            $('input#path').removeClass("success").removeClass("error");
            if (path == "http://github.com/" || path == "http://github.com" || path == "") {
                $('#pathresult').html('');
                return;
            }
            $('#pathresult').html(parseTemplate(statusTemplate, { message: "validating..." }));
            $.post(validate, { path: path }, callback);
            
        }

        function callback(data) {
            if (data.result.Path != lastPath)
                return;

            // $('#result').html(JSON.stringify(data));

            if (data.success && data.result.IsValid) {
                $('input#path').addClass("success");
                $('#pathresult').html(parseTemplate(template, data));
            } else if (!data.success) {
                $('input#path').addClass("error");
                $('#pathresult').html(parseTemplate(errorTemplate, { message: data.message }));
            } else if (!data.result.IsValid) {
                $('input#path').addClass("error");
                $('#pathresult').html(parseTemplate(errorTemplate, { message: data.result.ValidationMessage }));
            }
        }

        $(function () {
            template = $("#AnalysisResult").html();
            commitTemplate = $("#CommitTemplate").html();
            errorTemplate = $("#ErrorTemplate").html();
            statusTemplate = $("#StatusTemplate").html();

            $(document).ajaxError(function (event, request, settings, thrownError) {
                callback({ success: false, message: request.status + ": " + request.statusText });
            });
            analyzePath();
        });
    </script>
</asp:Content>
