using LMS.Models;
using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace LMS.Core
{
    public enum EmailDownloaderErrors
    {
        HOST_NAME_EMPTY = 1
        , INVALID_POP_PORT_NUMBER = 2
        , USER_NAME_EMPTY = 3
        , USER_PASSWORD_EMPTY = 4
    }
    public class EmailDownloader
    {        
        private List<string> _Errors = null;
        public List<string> Errors
        {
            get
            {
                if(_Errors == null)
                {
                    _Errors = new List<string>();
                }
                return _Errors;
            }
        }

        public string PopServer { get; private set; }
        public int PopPort { get; private set; }
        public bool PopUseSsl { get; private set; }
        public string PopUserName { get; private set; }
        public string PopPassword { get; private set; }
        
        private int faqEmailId { get; set; }

        private FAQEmail _EmailAccount = null;
        private FAQEmail EmailAccount
        {
            get
            {
                if(_EmailAccount == null)
                {
                    Log(string.Format("Finding FAQ email against id {0}.", faqEmailId));
                    _EmailAccount = (from e in db.FAQEmails
                                       where e.faq_email_id == faqEmailId
                                       && (e.active == true)
                                       select e)
                                   .FirstOrDefault();
                    if(_EmailAccount != null)
                    {
                        Log(string.Format("Account details found against id {0}.", _EmailAccount.faq_email_id));
                    }
                    else
                    {
                        Log(string.Format("No account detail found against id {0}.", faqEmailId));
                    }
                }
                return _EmailAccount;
            }
        }

        private SmtpPopSetting _Settings = null;
        private SmtpPopSetting Settings
        {
            get
            {
                if (_Settings == null)
                {
                    if (EmailAccount != null)
                    {
                        Log(string.Format("Finding POP settings against account id {0}.", EmailAccount.faq_email_id));
                        _Settings = (from sps in db.SmtpPopSettings
                                     where sps.smtp_pop_id == EmailAccount.smtp_pop_id
                                     select sps)
                                               .FirstOrDefault();
                        if(_Settings != null)
                        {
                            Log(string.Format("POP settings {0} found against account id {1}.", _Settings.setting_name, faqEmailId));
                        }
                        else
                        {
                            Log(string.Format("No POP settings found against account id {0}.", faqEmailId));
                        }
                    }
                }
                return _Settings;
            }
        }

        private List<FAQEmailsDownload> _KnownEmails = null;
        private List<FAQEmailsDownload> KnownEmails
        {
            get
            {
                if(_KnownEmails == null)
                {
                    Log(string.Format("Getting number of emails already downloaded against account id {0}.", faqEmailId));
                    _KnownEmails = (from ed in db.FAQEmailsDownloads
                                         where (ed.faq_email_id == this.faqEmailId)
                                         orderby ed.download_date descending
                                         select ed
                                         )
                                         .ToList();
                    if(_KnownEmails != null)
                    {
                        Log(string.Format("{0} email(s) already downloaded against account id {1}.", _KnownEmails.Count, faqEmailId));
                    }
                    

                }
                return _KnownEmails;
            }
        }

        private List<string> _KnownEmailUIDs = null;
        private List<string> KnownEmailUIDs
        {
            get
            {
                if(_KnownEmailUIDs == null)
                {
                    _KnownEmailUIDs = new List<string>();
                    if(KnownEmails != null && KnownEmails.Count > 0)
                    {
                        Log(string.Format("{0} email(s) already downloaded against account id {1}.", KnownEmails.Count, faqEmailId));
                        _KnownEmailUIDs = (from ke in KnownEmails
                                           select ke.email_uid
                                         ).ToList();
                    }
                }
                return _KnownEmailUIDs;
            }
        }

        private List<string> _EmailUIDs = null;
        private List<string> EmailUIDs
        {
            get
            {
                if(_EmailUIDs == null)
                {
                    _EmailUIDs = new List<string>();
                }
                return _EmailUIDs;
            }
            set
            {
                _EmailUIDs = value;
            }
        }

        private FAQRespondLogger _FaqLogger = null;
        public FAQRespondLogger FaqLogger
        {
            get
            {
                if(_FaqLogger == null)
                {
                    _FaqLogger = new FAQRespondLogger();
                }
                return _FaqLogger;
            }
            set
            {
                _FaqLogger = value;
            }
        }

        private LMSEntities _db = null;
        private LMSEntities db
        {
            get
            {
                if(_db == null)
                {
                    _db = new LMSEntities();
                }
                return _db;
            }
        }

        private bool IsOK
        {
            get
            {
                Errors.Clear();
                if (string.IsNullOrEmpty(PopServer))
                {
                    Errors.Add(GetErrorMessage(EmailDownloaderErrors.HOST_NAME_EMPTY));
                }
                if (PopPort <= 0)
                {
                    Errors.Add(GetErrorMessage(EmailDownloaderErrors.INVALID_POP_PORT_NUMBER));
                }
                if (string.IsNullOrEmpty(PopUserName))
                {
                    Errors.Add(GetErrorMessage(EmailDownloaderErrors.USER_NAME_EMPTY));
                }
                if (string.IsNullOrEmpty(PopPassword))
                {
                    Errors.Add(GetErrorMessage(EmailDownloaderErrors.USER_PASSWORD_EMPTY));
                }
                return Errors.Count <= 0;
            }
        }

        private List<Message> _AllEmails = null;
        private List<Message> AllEmails
        {
            get
            {
                if(_AllEmails == null)
                {
                    using (Pop3Client client = new Pop3Client())
                    {
                        try
                        {
                            // Connect to the server
                            client.Connect(this.PopServer, this.PopPort, this.PopUseSsl);

                            // Authenticate ourselves towards the server
                            client.Authenticate(this.PopUserName, this.PopPassword);

                            // Fetch all the current uids seen
                            //_EmailUIDs = client.GetMessageUids();
                            _EmailUIDs = null;

                            
                            // Get the number of messages in the inbox
                            int messageCount = client.GetMessageCount();

                            // We want to download all messages
                            _AllEmails = new List<Message>(messageCount);

                            // Messages are numbered in the interval: [1, messageCount]
                            // Ergo: message numbers are 1-based.
                            // Most servers give the latest message the highest number
                            for (int i = messageCount; i > 0; i--)
                            {
                                Message aMessage = client.GetMessage(i);
                                EmailUIDs.Add(aMessage.Headers.MessageId);
                                _AllEmails.Add(aMessage);
                            }
                        }
                        catch(Exception ex)
                        {
                            Log(ex.Message);
                        }
                    }
                }
                return _AllEmails;
            }
        }

        private List<Message> _UnreadEmails = null;
        public async Task<List<Message>> UnreadEmails()
        {
            if (_UnreadEmails == null)
            {
                if (AllEmails != null)
                {
                    _UnreadEmails = new List<Message>();
                    if (EmailUIDs.Count > 0)
                    {
                        foreach (Message aEmail in AllEmails)
                        {
                            string sEmailUid = aEmail.Headers.MessageId;
                            if (!KnownEmailUIDs.Contains(sEmailUid))
                            {
                                _UnreadEmails.Add(aEmail);
                            }
                        }
                    }
                }
            }
            return _UnreadEmails;
        }

        public EmailDownloader(int faqEmailId)
        {
            this.faqEmailId = faqEmailId;
            if(this.Settings != null && this.EmailAccount != null)
            {
                SetupValues(Settings.pop_server, Settings.pop_port, Settings.pop_use_ssl, EmailAccount.user_name, EmailAccount.use_password);
            }
        }

        private string GetErrorMessage(EmailDownloaderErrors err)
        {
            string sMessage = string.Empty;
            switch(err)
            {
                case EmailDownloaderErrors.HOST_NAME_EMPTY:
                    sMessage = "POP server cannot be null or empty.";
                    break;

                case EmailDownloaderErrors.INVALID_POP_PORT_NUMBER:
                    sMessage = "POP port has not been initialized.";
                    break;

                case EmailDownloaderErrors.USER_NAME_EMPTY:
                    sMessage = "POP user cannot be null or empty.";
                    break;

                case EmailDownloaderErrors.USER_PASSWORD_EMPTY:
                    sMessage = "POP user password cannot be null or empty.";
                    break;
            }
           
            return sMessage;
        }
        private void SetupValues(string PopServer, int PopPort, bool PopUseSsl, string PopUserName, string PopPassword)
        {
            this.PopServer = PopServer;
            this.PopPort = PopPort;
            this.PopUseSsl = PopUseSsl;
            this.PopUserName = PopUserName;
            this.PopPassword = PopPassword;
        }
        private void Log(string sMessage)
        {
            FaqLogger.Log(sMessage);
        }
    }
}