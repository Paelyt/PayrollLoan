using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Web;
using UvlotApplication.Controllers;

namespace UvlotApplication.Service
{
    public class WinServe
    {
        private readonly Timer _timer;

        public WinServe()
        {
            _timer = new Timer(1000) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            string[] lines = new string[] { DateTime.Now.ToString() };
            File.AppendAllLines(@"C:\Temp\Demos\Heartbeat.txt", lines);
            DoRemitaCharge();
            CheckRemitaStatus();
        }

        public string DoRemitaCharge()
        {
            try
            {
                AdminController _AC = new AdminController();
                DateTime today = DateTime.Today;
                DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

                if (DateTime.Today.Date == endOfMonth.Date)
                {
                    // RunRemitaChargeMonthly
                    _AC.MonthlyRepayments(today);
                    
                }
                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;

            }
        }

        public string CheckRemitaStatus()
        {
            try
            {
                AdminController _AC = new AdminController();
                DateTime today = DateTime.Today;
                DateTime endOfMonth = new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month));

                if (DateTime.Today.Date == endOfMonth.Date.AddDays(1))
                {
                    // Check Remita Status 
                    _AC.MonthlyRepaymentStatus(today);

                }
                return null;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
