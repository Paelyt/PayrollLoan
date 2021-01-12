using DataAccessLayerT.DataManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UvlotApplication.Classes
{
    public class LogginHelper
    {
        static UvlotEntities uvDb = new UvlotEntities();

        public string LoggedInInstitution()
        {
            try
            {
                string sessionInstFk = HttpContext.Current.Session["InstFkEmail"].ToString().Trim();


                return sessionInstFk;
            
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public string LoggedInUser()
        {
            try
            {
                string sessionUserId = HttpContext.Current.Session["id"].ToString().Trim();


                return sessionUserId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public int LoggedInUserID(string email)
        {
            try
            {

                var userid = (from a in uvDb.Users where a.EmailAddress == email select a.ID).FirstOrDefault();

                return userid;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public bool getCurrentUser()
        {
            try
            {
                string sessionUserId = HttpContext.Current.Session["id"].ToString().Trim();

                if (sessionUserId != null)
                {

                    return true;
                }
                else
                {


                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}