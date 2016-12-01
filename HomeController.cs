using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Contentful()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        public ActionResult Index()
        {
            string profileID = "134923353";
            string serviceAccountEmail = "jaymehta-ce@analytic-151206.iam.gserviceaccount.com";
            getAnalyticsData(profileID, serviceAccountEmail);
            return View();
        }

        public void getAnalyticsData(string ProfileID, string serviceAccountEmail)
        {

            var certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(Server.MapPath("~/Content/Analytic-79a3457fd485.p12"), "notasecret", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
            var credential = new ServiceAccountCredential(
            new ServiceAccountCredential.Initializer(serviceAccountEmail)
            {
                Scopes = new[] { AnalyticsService.Scope.Analytics }
            }.FromCertificate(certificate));

            // Create the service.
            var GoogleAnalyticsService = new AnalyticsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "MyApp",
            });


            var request = GoogleAnalyticsService.Data.Ga.Get("ga:" + ProfileID, "2016-01-01", "2016-11-11", "ga:users");
            request.Dimensions = "ga:day";

            Google.Apis.Analytics.v3.Data.GaData d = request.Execute();
            if (d != null)
            {
                var str = ("<table>");
                foreach (IList<string> row in d.Rows)
                {
                    foreach (string col in row) { str += "<td>" + col + "</td>"; }
                    str += ("</tr><tr>");
                }
                str += ("</tr><table>");
            }
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Data() {
            return View();
        }
        public PartialViewResult BulkUpload()
        {
            var date = Request["Date"];
            var FindIN = Request["FindIN"];
            var IpAddress = Request["IpAddress"];
            Response.Expires = 360000000;

            List<DataTable> listDatatbl = new List<DataTable>();

            List<string> objIpAddress = new List<string>();

            string Imagename = "";

            // Verify that the user selected a file
            string[] array = new string[7];

            array[0] = "172.018.012.014";
            array[1] = "172.018.017.133";
            array[2] = "172.018.017.134";
            array[3] = "172.018.028.045";
            array[4] = "172.018.028.046";
            array[5] = "172.018.028.072";
            array[6] = "172.018.028.073";

            for (int k = 0; k < array.Length; k++)
            {
                try
                {
                    DataTable d = GetData(array[k], date, FindIN);
                    listDatatbl.Add(d);
                    objIpAddress.Add(array[k]);
                }
                catch(Exception e)
                {
                    //throw e;
                }


            }


            ViewBag.IpAddressList = objIpAddress;
            return PartialView(listDatatbl);

        }


        public DataTable GetData(string IpAddress, string date, string FindIN)
        {

            #region Get Data

            string FolderPath = "";
            DataTable dt = new DataTable();

            FolderPath = "~/Content/172.018.028.073/" + IpAddress + "/";
            bool ErrorFound = false, IsFirstRow = true, IsDMR = false;


            if (date != "" && date != null)
            {
                string oldPath = Server.MapPath(FolderPath + "day-" + FindIN + date + ".pmon");
                string newpath = Server.MapPath(FolderPath);
                string newFileName = "NewFile22";
                FileInfo f1 = new FileInfo(oldPath);

                if (f1.Exists)
                {
                    FileInfo f2 = new FileInfo(newpath + newFileName + ".csv");
                    if (f2.Exists)
                    {
                        f2.Delete();
                    }

                    f1.CopyTo(string.Format("{0}{1}{2}", newpath, newFileName, ".csv"));


                }




                using (CsvReader reader = new CsvReader(newpath + newFileName + ".csv"))
                {
                    int cnt = 1;
                    Dictionary<string, int> dictionaryColumn = new Dictionary<string, int>();
                    foreach (string[] values in reader.RowEnumerator)
                    {
                        #region Else Part


                        for (int i = 0; i < values.Length; i++)
                        {
                            if (dictionaryColumn.Count < i) break;

                            string vColumnName = "";
                            if (values[i].ToString().Trim() != "")
                            {

                                string GetAll = values[i].ToString().Trim();
                                //string[] split = GetAll.Split(',');
                                if (!IsFirstRow && cnt > 2)
                                {
                                    string Time = values[0];
                                    string tValue = values[0];
                                    string OFS = values[2];
                                    string oValue = values[3];
                                    string SEP = values[4];
                                    string sepValue = values[5];
                                    string BBE = values[6];
                                    string bValue = values[7];
                                    string ES = values[8];
                                    string eValue = values[9];
                                    string SES = values[10];
                                    string sesValue = values[11];

                                    string UAS = values[12];
                                    string uValue = values[13];

                                    DataRow dr = dt.NewRow();
                                    dr["Time"] = tValue;
                                    dr["OFS"] = oValue;
                                    dr["SEP"] = sepValue;
                                    dr["BBE"] = bValue;
                                    dr["ES"] = eValue;
                                    dr["SES"] = sesValue;
                                    dr["UAS"] = uValue;

                                    if (IsDMR)
                                    {
                                        //string UAS = split[14];
                                        string Rx_lvl1_MIN = values[15];

                                        //string UAS = split[16];
                                        string Rx_lvl1_MAX = values[17];

                                        dr["RX LEV1(MIN)"] = Rx_lvl1_MIN;
                                        dr["RX LEV1(MAX)"] = Rx_lvl1_MAX;

                                        //string UAS = split[18];
                                        if (values.Length > 18)
                                        {
                                            string Rx_lvl2_MIN = values[19];

                                            //string UAS = split[20];
                                            string Rx_lvl2_MAX = values[21];

                                            dr["RX LEV2(MIN)"] = Rx_lvl2_MIN;
                                            dr["RX LEV2(MAX)"] = Rx_lvl2_MAX;
                                        }



                                    }

                                    dt.Rows.Add(dr);
                                }
                                if (IsFirstRow)
                                {
                                    IsFirstRow = false;
                                    dt.Clear();
                                    dt.Columns.Add("Time");
                                    dt.Columns.Add("OFS");
                                    dt.Columns.Add("SEP");
                                    dt.Columns.Add("BBE");
                                    dt.Columns.Add("ES");
                                    dt.Columns.Add("SES");
                                    dt.Columns.Add("UAS");

                                    if (values[2].Contains("DMR"))
                                    {
                                        IsDMR = true;
                                        dt.Columns.Add("RX LEV1(MIN)");
                                        dt.Columns.Add("RX LEV1(MAX)");
                                        dt.Columns.Add("RX LEV2(MIN)");
                                        dt.Columns.Add("RX LEV2(MAX)");
                                    }
                                }

                            }

                        }

                        #endregion

                        cnt++;
                    }


                }
            }
            return dt;
            #endregion
        }
        public ActionResult BulkUpload_old(HttpPostedFileBase file)
        {

            Response.Expires = 360000000;
            bool ErrorFound = false, IsFirstRow = true;

            string Imagename = "";
            DataTable dt = new DataTable();

            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // Get file info
                var fileName = Path.GetFileName(file.FileName);

                string gs = Server.MapPath("/Content/");

                Imagename = fileName.Replace(".pmon", "") + ".csv";
                file.SaveAs(gs + Imagename);

                //string filepath = Server.MapPath("/Content/" + Imagename);


                //day-DMR 2016-01-15

                string filepath = Server.MapPath("/Content/" + Imagename);
                using (CsvReader reader = new CsvReader(filepath))
                {
                    int cnt = 1;
                    Dictionary<string, int> dictionaryColumn = new Dictionary<string, int>();
                    foreach (string[] values in reader.RowEnumerator)
                    {
                        if (cnt == 1)
                        {
                            try
                            {
                                if (values.Length >= 1)
                                {
                                    for (int i = 0; i < values.Length; i++)
                                    {
                                        if (values[i].ToString().Trim() != "")
                                        {
                                            dictionaryColumn.Add(values[i], i);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                                throw ex;
                            }
                        }
                        else
                        {
                            #region Else Part


                            for (int i = 0; i < values.Length; i++)
                            {
                                if (dictionaryColumn.Count < i) break;
                                if (dictionaryColumn.ContainsValue(i))
                                {
                                    string vColumnName = "";
                                    if (values[i].ToString().Trim() != "")
                                    {
                                        var thisTag = dictionaryColumn.FirstOrDefault(t => t.Value == i);
                                        if (dictionaryColumn.ContainsValue(i))
                                        {
                                            vColumnName = thisTag.Key.ToString();
                                        }


                                        string GetAll = values[i].ToString().Trim();
                                        string[] split = GetAll.Split(',');
                                        if (!IsFirstRow)
                                        {
                                            //00:15,0,NORMAL,0,NORMAL,0,NORMAL,0,NORMAL,0,NORMAL,0,NORMAL,VALID
                                            //string Time = split[0];
                                            //string tValue = split[0];
                                            //string OFS = split[2];
                                            //string oValue = split[3];
                                            //string SEP = split[4];
                                            //string sepValue = split[5];
                                            //string BBE = split[6];
                                            //string bValue = split[7];
                                            //string ES = split[8];
                                            //string eValue = split[9];
                                            //string SES = split[10];
                                            //string sesValue = split[11];

                                            //string UAS = split[12];
                                            //string uValue = split[13];



                                            //DataRow dr = dt.NewRow();
                                            //dr["Time"] = tValue;
                                            //dr["OFS"] = oValue;
                                            //dr["SEP"] = sepValue;
                                            //dr["BBE"] = bValue;
                                            //dr["ES"] = eValue;
                                            //dr["SES"] = sesValue;
                                            //dr["UAS"] = uValue;

                                            //if (vColumnName.Contains("DMR"))
                                            //{
                                            //    //string UAS = split[14];
                                            //    string Rx_lvl1_MIN = split[15];

                                            //    //string UAS = split[16];
                                            //    string Rx_lvl1_MAX = split[17];

                                            //    //string UAS = split[18];
                                            //    string Rx_lvl2_MIN = split[19];

                                            //    //string UAS = split[20];
                                            //    string Rx_lvl2_MAX = split[21];

                                            //    dr["Rx_lvl1_MIN"] = Rx_lvl1_MIN;
                                            //    dr["Rx_lvl1_MAX"] = Rx_lvl1_MAX;
                                            //    dr["Rx_lvl2_MIN"] = Rx_lvl2_MIN;
                                            //    dr["Rx_lvl2_MAX"] = Rx_lvl2_MAX;

                                            //}

                                            //dt.Rows.Add(dr);
                                        }
                                        if (IsFirstRow)
                                        {
                                            IsFirstRow = false;
                                            dt.Clear();
                                            dt.Columns.Add("Time");
                                            dt.Columns.Add("OFS");
                                            dt.Columns.Add("SEP");
                                            dt.Columns.Add("BBE");
                                            dt.Columns.Add("ES");
                                            dt.Columns.Add("SES");
                                            dt.Columns.Add("UAS");

                                            if (vColumnName.Contains("DMR"))
                                            {
                                                dt.Columns.Add("Rx_lvl1_MIN");
                                                dt.Columns.Add("Rx_lvl1_MAX");
                                                dt.Columns.Add("Rx_lvl2_MIN");
                                                dt.Columns.Add("Rx_lvl2_MAX");
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                        }
                        cnt++;

                    Finish:
                        Response.Write("");
                    }

                    var final = dt;
                }
            }

            return View(dt);

        }

    }
}