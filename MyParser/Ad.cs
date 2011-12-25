using System;
using System.Net;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;

namespace AdProvider
{
    public class AdDetails
    {
        public AdDetails(string msg)
        {
            this.Msg = msg;
        }

        public string Msg { get; set; }
    }

    public class Ad
    {
        private string conStr;
        private string[] regions = { "869", "870", "871" };
        private SqlConnection sqlCon;

        public Ad()
        {
            conStr = WebConfigurationManager.ConnectionStrings["AdsDb"].ConnectionString;
            sqlCon = new SqlConnection(conStr);
        }

        public Ad(string conStrName)
        {
            conStr = WebConfigurationManager.ConnectionStrings[conStrName].ConnectionString;
        }

        private string GetPrice(string ad)
        {
            const string pattern = @"(\d+\s*\d*)\s*у.\s*е.";
            Match match = Regex.Match(ad, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].ToString().Replace(" ", String.Empty);
            }
            else
            {
                return "NULL";
            }
        }

        private string GetArea(string ad)
        {
            const string pattern = @"(\d+(.|,)?\d*)\s*м2";
            Match match = Regex.Match(ad, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].ToString();
            }
            else
            {
                return "NULL";
            }
        }

        private string GetBedroomCount(string ad)
        {
            const string pattern = @"(\d)\s*ком.\s*кв.";
            Match match = Regex.Match(ad, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].ToString();
            }
            else
            {
                return "NULL";
            }
        }

        private void InsertAds(string values)
        {
            string sql = "INSERT INTO ads VALUES " + values;
            SqlCommand cmd = new SqlCommand(sql, sqlCon);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException)
            {
            }
        }

        protected string AddSlashes(string InputTxt)
        {
            // List of characters handled:
            // \000 null 
            // \010 backspace  
            // \011 horizontal tab 
            // \012 new line  
            // \015 carriage return 
            // \032 substitute  
            // \042 double quote  
            // \047 single quote  
            // \134 backslash  
            // \140 grave accent 

            string Result = InputTxt;
            try
            {
                Result = System.Text.RegularExpressions.Regex.Replace(InputTxt, @"[\000\010\011\012\015\032\042\047\134\140]", "\\$0");
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }
            return Result;
        }

        protected string StripSlashes(string InputTxt)
        {
            // List of characters handled:
            // \000 null 
            // \010 backspace 
            // \011 horizontal tab 
            // \012 new line  
            // \015 carriage return
            // \032 substitute  
            // \042 double quote 
            // \047 single quote 
            // \134 backslash  
            // \140 grave accent

            string Result = InputTxt;
            try
            {
                Result = System.Text.RegularExpressions.Regex.Replace(InputTxt, @"(\\)([\000\010\011\012\015\032\042\047\134\140])", "$2");
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.Message);
            }
            return Result;
        }

        private void DeleteAds()
        {
            SqlCommand cmd = new SqlCommand("DeleteAds", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
        }

        public void RefreshDb()
        {
            WebRequest request;
            WebResponse response;
            Stream dataStream;
            StreamReader reader;
            string content;
            string price, area, bedroom_count, ad, ads;
            int indDiv;
            int indA, indBr;
            int ads_count;
            sqlCon.Open();
            DeleteAds();
            for (int i = 0; i < regions.Length; i++)
            {
                request = WebRequest.Create("http://md5.com.ua/adv.php?id_sgroop=" +
                    regions[i]);
                response = request.GetResponse();
                dataStream = response.GetResponseStream();
                reader = new StreamReader(dataStream, Encoding.Default);
                content = reader.ReadToEnd();
                reader.Close();
                response.Close();
                ads = "";
                ads_count = 1;
                indDiv = content.IndexOf("<div class='sep'>", 0);
                do
                {
                    indA = content.IndexOf("</a>", indDiv);
                    indBr = content.IndexOf("<br>", indA);
                    ad = content.Substring(indA + 5, indBr - indA - 5);
                    price = GetPrice(ad);
                    area = GetArea(ad);
                    bedroom_count = GetBedroomCount(ad);

                    ads += "('" + AddSlashes(ad) + "'," + price + "," + area + "," +
                        bedroom_count + "," + regions[i] + ")";
                    indDiv = content.IndexOf("<div class='sep'>", indBr);

                    if (indDiv == -1)
                    {
                        InsertAds(ads);
                        break;
                    }
                    else
                        if (ads_count == 1)
                        {
                            InsertAds(ads);
                            ads = "";
                            ads_count = 1;
                        }
                        else
                        {
                            ads += ",";
                            ads_count++;
                        }
                } while (true);
            }
            sqlCon.Close();
        }

        public List<AdDetails> GetAds(int region, int bedroomCount, int minArea,
            int maxArea, int minPrice, int maxPrice)
        {
            SqlCommand cmd = new SqlCommand("GetAds", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@region", SqlDbType.Int, 4));
            cmd.Parameters["@region"].Value = region;
            cmd.Parameters.Add(new SqlParameter("@bedroom_count", SqlDbType.TinyInt, 1));
            cmd.Parameters["@bedroom_count"].Value = bedroomCount;
            cmd.Parameters.Add(new SqlParameter("@minArea", SqlDbType.Int, 4));
            cmd.Parameters["@minArea"].Value = minArea;
            cmd.Parameters.Add(new SqlParameter("@maxArea", SqlDbType.Int, 4));
            cmd.Parameters["@maxArea"].Value = maxArea;
            cmd.Parameters.Add(new SqlParameter("@minPrice", SqlDbType.Money, 8));
            cmd.Parameters["@minPrice"].Value = minArea;
            cmd.Parameters.Add(new SqlParameter("@maxPrice", SqlDbType.Money, 8));
            cmd.Parameters["@maxPrice"].Value = maxArea;
            List<AdDetails> ads = new List<AdDetails>();
            sqlCon.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                AdDetails ad = new AdDetails((string)reader["msg"]);
                ads.Add(ad);
            }
            reader.Close();
            sqlCon.Close();
            return ads;
        }
    }

}