using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Configuration;
using AdProvider;

namespace MyParser
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected void ButtonFind_Click(object sender, EventArgs e)
        {
            //
        }

        protected void ButtonRefresh_Click(object sender, EventArgs e)
        {
            Ad ad = new Ad();
            ad.RefreshDb();
        }
    }
}
