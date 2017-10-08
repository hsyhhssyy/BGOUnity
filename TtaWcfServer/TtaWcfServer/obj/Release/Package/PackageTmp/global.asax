<%@ Application Language="C#" Debug="true" %>
<%@ Import Namespace="TtaWcfServer.ServerApi.ServerInitialize" %>

<script runat="server">

    void Application_Start(object sender, EventArgs e)
    {
        ServerHostMain.Start();
    }

</script>