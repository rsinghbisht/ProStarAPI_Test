using System;
using System.Text;
using System.Web.Mvc;
using System.Data;
using Npgsql;
using System.Configuration;
using System.Net.Mail;
using System.Net;

namespace WebApplication.Controllers
{
    public class IssueDetailsController 
    {
        [HttpGet]
        public ActionResult IssueIdentified(string MapID)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(MapID))
                {
                    return ReturnError("Invalid argument.");
                }

                var oLogin = m_oGeoSecurity.GetLogin(m_oGeoAPI, Request, Session);
                if (oLogin == null) { return ReturnInvalidCredentials(); }

                int nRights = oLogin.GetMapACL(MapID);
                if (nRights < GeoSecurity.ACL_READ)
                { return ReturnError("Access denied."); }

                var oJSON = string.Empty;

                using (var db = ProStarContext.get(MapID, oLogin))
                {
                    string szSqlCmd = "SELECT \"Issue_Identify_Id\" as IssueIidentifyID,\"Issue_Identities\" as IssueIdentities from \"Form_Issue_Identifies\" ";

                    var aRows = db.Database.SqlQuery<IssueIdentify>(szSqlCmd);
                    oJSON = Newtonsoft.Json.JsonConvert.SerializeObject(aRows);
                }

                return Content(oJSON, MimeType);
            }
            catch (Exception e)
            {
                ErrorSignal.FromCurrentContext().Raise(e);
                return ReturnError("Failed to fetch  Issue Identified. " + e.Message);
            }
        }
    }