using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using RestSharp;
using SmorcIRL.TempMail;
using TravianHelper.SeleniumHost;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils
{
    public class Driver : NotificationObject
    {
        public  Account             Account     { get; set; }
        public  ChromeDriverService Service     { get; set; }
        private ChromeOptions       Options     { get; set; }
        public  ChromeDriver        Chrome      { get; set; }
        public  IJavaScriptExecutor JsExec      { get; set; }
        public  Actions             Act         { get; set; }
        public  SeleniumHostWPF     Host        { get; set; }
        public  RestClientOptions   RestOptions { get; set; }
        public  RestClient          RestClient  { get; set; }
        public  MailClient          MClient { get; set; }

        public void Init(Account acc)
        {

        }
    }

    
}
