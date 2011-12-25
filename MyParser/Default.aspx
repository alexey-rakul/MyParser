<%@ Page Title="Домашняя страница" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="MyParser._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Button ID="ButtonRefresh" runat="server" Text="Refresh" onclick="ButtonRefresh_Click" Visible="False" />
    <asp:Button ID="ButtonFind" runat="server" Text="Find" onclick="ButtonFind_Click" />
    <asp:Label ID="Label1" runat="server" />
    <br />

    <asp:SqlDataSource ID="sourceRegions" runat="server"
        ProviderName="System.Data.SqlClient" 
        ConnectionString="<%$ ConnectionStrings:AdsDb %>"
        SelectCommand="GetRegions" SelectCommandType="StoredProcedure">
    </asp:SqlDataSource>

    <asp:Label ID="lbRegion" runat="server" Text="Region ">
        <asp:DropDownList ID="ddlRegions" runat="server"
        DataSourceID="sourceRegions" DataTextField="name" DataValueField="id">
        </asp:DropDownList>
    </asp:Label>

    <asp:Label ID="lbBedroomCount" runat="server" Text="Bedroom = ">
        <asp:TextBox ID="tbBedroomCount" runat="server" Text="1" Width="25"></asp:TextBox>
    </asp:Label>

    <asp:Label ID="lbArea" runat="server" Text="Area min = ">
        <asp:TextBox ID="tbMinArea" runat="server" Text="0" Width="25"></asp:TextBox>
        max = 
        <asp:TextBox ID="tbMaxArea" runat="server" Text="1000" Width="25"></asp:TextBox>
    </asp:Label>

    <asp:Label ID="lbPrice" runat="server" Text="Price min = ">
        <asp:TextBox ID="tbMinPrice" runat="server" Text="0" Width="25"></asp:TextBox>
        max = 
        <asp:TextBox ID="tbMaxPrice" runat="server" Text="100000" Width="25"></asp:TextBox>
    </asp:Label>

    <asp:ObjectDataSource ID="objSourceAds" runat="server"
        TypeName="AdProvider.Ad" SelectMethod="GetAds" >
        <SelectParameters>
        <asp:ControlParameter ControlID="ddlRegions" Name="region"
        PropertyName="SelectedValue" />
        <asp:ControlParameter ControlID="tbBedroomCount" Name="bedroomCount"
        PropertyName="Text" />
        <asp:ControlParameter ControlID="tbMinArea" Name="minArea"
        PropertyName="Text" />
        <asp:ControlParameter ControlID="tbMaxArea" Name="maxArea"
        PropertyName="Text" />
        <asp:ControlParameter ControlID="tbMinPrice" Name="minPrice"
        PropertyName="Text" />
        <asp:ControlParameter ControlID="tbMaxPrice" Name="maxPrice"
        PropertyName="Text" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:GridView ID="GridAds" runat="server" AutoGenerateColumns="true" 
        DataSourceID="objSourceAds">
    </asp:GridView>

</asp:Content>
