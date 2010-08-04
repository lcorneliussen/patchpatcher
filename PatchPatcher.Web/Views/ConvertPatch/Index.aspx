<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Patch Patcher</h2>
    <h3>Create a SVN patch for a single commit</h3>
    Commit url: <input type="text" value="http://github.com/..." onchange=""/>
    <h2>..or..</h2>
    <h3>Compare two commits</h3>
    Original: <input type="text" value="http://github.com/..." /><br />
    New: <input type="text" value="http://github.com/..." />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">

    <script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
    <script type="text/javascript">
    $(
    </script>
</asp:Content>
