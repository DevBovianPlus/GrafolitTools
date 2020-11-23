using DatabaseWebService.Common;
using DatabaseWebService.Common.Enums;
using DatabaseWebService.Domain;
using DatabaseWebService.Domain.Abstract;
using DatabaseWebService.Domain.Concrete;
using Ninject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using DatabaseWebService.DomainOTP.Abstract;
using DatabaseWebService.DomainOTP.Concrete;
using DatabaseWebService.DomainOTP;
using System.Reflection;
using System.Net.Mime;
using System.IO;
using DatabaseWebService.DomainPDO.Abstract;
using DatabaseWebService.DomainPDO.Concrete;
using DatabaseWebService.DomainPDO;
using DatabaseWebService.ModelsPDO.Settings;
using DatabaseWebService.DomainNOZ;
using DatabaseWebService.DomainNOZ.Abstract;
using DatabaseWebService.DomainNOZ.Concrete;

namespace Service1
{
    public partial class SendMessageEmailService : ServiceBase
    {
        private IKernel kernel;
        private ISystemEmailMessageRepository systemEmailMessageRepo;
        private ISystemEmailMessageRepository_OTP otpSystemEmailMessageRepo;
        private ISystemEmailMessageRepository_PDO pdoSystemEmailMessageRepo;
        private ISettingsRepository pdoSettingsRepo;
        private ISystemEmailMessageRepository_NOZ nozSystemEmailMessageRepo;

        private bool isSending = false;

        public SendMessageEmailService()
        {
            InitializeComponent();

            kernel = new StandardKernel();
            kernel.Bind<ISystemEmailMessageRepository>().To<SystemEmailMessageRepository>();
            kernel.Bind<ISystemEmailMessageRepository_OTP>().To<SystemEmailMessageRepository_OTP>();
            kernel.Bind<ISystemEmailMessageRepository_PDO>().To<SystemEmailMessageRepository_PDO>();
            kernel.Bind<ISettingsRepository>().To<SettingsRepository>();
            kernel.Bind<ISystemEmailMessageRepository_NOZ>().To<SystemEmailMessageRepository_NOZ>();

            systemEmailMessageRepo = kernel.Get<ISystemEmailMessageRepository>();
            otpSystemEmailMessageRepo = kernel.Get<ISystemEmailMessageRepository_OTP>();
            pdoSystemEmailMessageRepo = kernel.Get<ISystemEmailMessageRepository_PDO>();
            pdoSettingsRepo = kernel.Get<ISettingsRepository>();
            nozSystemEmailMessageRepo = kernel.Get<ISystemEmailMessageRepository_NOZ>();
        }

        protected override void OnStart(string[] args)
        {
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["TimerInterval"].ToString());// 6 seconds  
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            if (isSending)
                return;

            isSending = true;
            SmtpClient client = new SmtpClient();

            try
            {


                client.Host = ConfigurationManager.AppSettings["SmtpHost"];
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);//Port 465 (SSL required)
                client.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpEnableSsl"]);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpTimeout"]);

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["HasCredentials"]))
                    client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["Username"], ConfigurationManager.AppSettings["Password"]);
                else
                    client.UseDefaultCredentials = true;

                systemEmailMessageRepo.UpdateFailedMessges();

                List<SystemEmailMessage> emailList = systemEmailMessageRepo.GetUnprocessedEmails();
                DataTypesHelper.LogThis("List of Unproccessed mail's. Count: " + emailList.Count.ToString());
                foreach (var item in emailList)
                {
                    SendMessages(client, item, null);
                }

            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("CRM:" + ex.Message);
            }
            finally
            {
                isSending = false;
            }

            try
            {
                otpSystemEmailMessageRepo = kernel.Get<ISystemEmailMessageRepository_OTP>();

                //OTP MAIL SETTINGS
                client = new SmtpClient();
                client.Host = ConfigurationManager.AppSettings["SmtpHostOTP"];
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPortOTP"]);//Port 465 (SSL required)
                client.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpEnableSslOTP"]);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpTimeout"]);

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["HasCredentialsOTP"]))
                    client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["UsernameOTP"], ConfigurationManager.AppSettings["PasswordOTP"]);
                else
                    client.UseDefaultCredentials = true;



                otpSystemEmailMessageRepo.UpdateFailedMessges();

                List<SystemEmailMessage_OTP> otpEmailList = otpSystemEmailMessageRepo.GetUnprocessedEmails();
                DataTypesHelper.LogThis("List of OTP Unproccessed mail's. Count: " + otpEmailList.Count.ToString());
                foreach (var item in otpEmailList)
                {
                    SendMessages(client, null, item);
                }

            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("OTP : " + ex.Message);
            }
            finally
            {
                isSending = false;
            }
            try
            {
                pdoSystemEmailMessageRepo = kernel.Get<ISystemEmailMessageRepository_PDO>();
                pdoSettingsRepo = kernel.Get<ISettingsRepository>();

                DataTypesHelper.LogThis("PDO Mailing setup");
                //PDO MAIL SETTINGS
                client = new SmtpClient();
                client.Host = ConfigurationManager.AppSettings["SmtpHostPDO"];
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPortPDO"]);//Port 465 (SSL required)
                client.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpEnableSslPDO"]);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpTimeout"]);

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["HasCredentialsPDO"]))
                    client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["UsernamePDO"], ConfigurationManager.AppSettings["PasswordPDO"]);
                else
                    client.UseDefaultCredentials = true;



                pdoSystemEmailMessageRepo.UpdateFailedMessges();

                List<SystemEmailMessage_PDO> pdoEmailList = pdoSystemEmailMessageRepo.GetUnprocessedEmails();
                DataTypesHelper.LogThis("List of PDO Unproccessed mail's. Count: " + pdoEmailList.Count.ToString());
                foreach (var item in pdoEmailList)
                {
                    SendMessages(client, null, null, item);
                }

            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("PDO" + ex.Message);
            }
            finally
            {
                isSending = false;
            }

            try
            {
                nozSystemEmailMessageRepo = kernel.Get<ISystemEmailMessageRepository_NOZ>();

                DataTypesHelper.LogThis("NOZ Mailing setup");
                // NOZ MAIL SETTINGS
                client = new SmtpClient();
                client.Host = ConfigurationManager.AppSettings["SmtpHostNOZ"];
                DataTypesHelper.LogThis("NOZ - 1");
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPortNOZ"]);//Port 465 (SSL required)
                DataTypesHelper.LogThis("NOZ - 2");
                client.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["SmtpEnableSslNOZ"]);
                DataTypesHelper.LogThis("NOZ - 3");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                DataTypesHelper.LogThis("NOZ - 4");
                client.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["SmtpTimeout"]);
                DataTypesHelper.LogThis("NOZ - 5");
                if (Convert.ToBoolean(ConfigurationManager.AppSettings["HasCredentialsNOZ"]))
                    client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["UsernameNOZ"], ConfigurationManager.AppSettings["PasswordNOZ"]);
                else
                    client.UseDefaultCredentials = true;
                DataTypesHelper.LogThis("NOZ - 6");
                nozSystemEmailMessageRepo.UpdateFailedMessges();
                DataTypesHelper.LogThis("NOZ - 7");
                List<SystemEmailMessage_NOZ> nozEmailList = nozSystemEmailMessageRepo.GetUnprocessedEmails();
                DataTypesHelper.LogThis("List of NOZ Unproccessed mail's. Count: " + nozEmailList.Count.ToString());
                foreach (var item in nozEmailList)
                {
                    SendMessages(client, null, null, null, item);
                }

            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis("NOZ: " + ex.Message);
            }
            finally
            {
                isSending = false;
            }
        }

        private void SendMessages(SmtpClient client, SystemEmailMessage item = null, SystemEmailMessage_OTP otpItem = null, SystemEmailMessage_PDO pdoItem = null, SystemEmailMessage_NOZ nozItem = null)
        {
            bool isOTPitem = false;
            bool isPDOitem = false;
            bool isNOZItem = false;

            try
            {
                if (otpItem != null)
                    isOTPitem = true;
                else if (pdoItem != null)
                    isPDOitem = true;
                else if (nozItem != null)
                    isNOZItem = true;

                //if (String.IsNullOrEmpty(isOTPitem ? otpItem.EmailTo : (isPDOitem ? ((pdoItem.TOEmails != null && pdoItem.TOEmails.Length > 0) ? pdoItem.TOEmails : pdoItem.EmailTo) : (isNOZItem ? nozItem.EmailTo : item.EmailTo))))
                if (String.IsNullOrEmpty(isOTPitem ? otpItem.EmailTo : (isPDOitem ? pdoItem.EmailTo : (isNOZItem ? nozItem.EmailTo : item.EmailTo))))
                {
                    if (isOTPitem)
                        otpItem.Status = (int)Enums.SystemEmailMessageStatus.Processed;
                    else if (isPDOitem)
                        pdoItem.Status = (int)Enums.SystemEmailMessageStatus.Processed;
                    else if (isNOZItem)
                        pdoItem.Status = (int)Enums.SystemEmailMessageStatus.Processed;
                    else
                        item.Status = (int)Enums.SystemEmailMessageStatus.Processed;

                    DataTypesHelper.LogThis("Couldn't send email! Email to is empty");
                }
                else
                {
                    string sender = "", emailTitle = "", emailTo = "", emailSubject = "", emailBody = "";

                    if (isOTPitem)
                    {
                        DataTypesHelper.LogThis("OTP ITEM!");
                        sender = ConfigurationManager.AppSettings["SenderOTP"].ToString();
                        //sender = otpItem.EmailFrom;

                        emailTitle = "(" + otpItem.EmailFrom + ") - " + ConfigurationManager.AppSettings["EmailTitleOTP"].ToString();
                        emailTo = otpItem.EmailTo;
                        emailSubject = otpItem.EmailSubject;
                        emailBody = otpItem.EmailBody;
                    }
                    else if (isPDOitem)
                    {
                        DataTypesHelper.LogThis("PDO ITEM");
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["UseUserEmailSettings"]))
                        {
                            if (pdoItem.OsebaEmailFromID.HasValue)
                            {
                                SettingsModel set = pdoSettingsRepo.GetLatestSettings();

                                string host = String.IsNullOrEmpty(set.EmailStreznik) ? pdoItem.Osebe_PDO.EmailStreznik : set.EmailStreznik;
                                //int port = 587;//set.EmailVrata > 0 ? set.EmailVrata : pdoItem.Osebe_PDO.EmailVrata.Value;
                                int port = set.EmailVrata > 0 ? set.EmailVrata : pdoItem.Osebe_PDO.EmailVrata.Value;
                                //bool sslEnable = false;//!set.EmailSifriranjeSSL ? pdoItem.Osebe_PDO.EmailSifriranjeSSL.Value : set.EmailSifriranjeSSL;
                                bool sslEnable = set.EmailSifriranjeSSL ? pdoItem.Osebe_PDO.EmailSifriranjeSSL.Value : set.EmailSifriranjeSSL;
                                //var credentials = new NetworkCredential("bovianplus@gmail.com"/*pdoItem.Osebe_PDO.Email, "Geslo123."/*pdoItem.Osebe_PDO.EmailGeslo);
                                var credentials = new NetworkCredential(pdoItem.Osebe_PDO.Email, pdoItem.Osebe_PDO.EmailGeslo);

                                client = new SmtpClient
                                {
                                    Host = host,
                                    Port = port,
                                    EnableSsl = sslEnable,
                                    DeliveryMethod = SmtpDeliveryMethod.Network,
                                    UseDefaultCredentials = false,
                                    Credentials = credentials
                                };
                            }
                        }

                        string emailTitleOseba = pdoItem.Osebe_PDO != null ? pdoItem.Osebe_PDO.Ime + " " + pdoItem.Osebe_PDO.Priimek : ConfigurationManager.AppSettings["EmailTitlePDO"].ToString(); ;
                        sender = pdoItem.OsebaEmailFromID != null ? pdoItem.Osebe_PDO.Email : ConfigurationManager.AppSettings["SenderPDO"].ToString();
                        emailTitle = emailTitleOseba;


                        emailTo = pdoItem.EmailTo;
                        emailSubject = pdoItem.EmailSubject;
                        emailBody = pdoItem.EmailBody;
                    }
                    else if (isNOZItem)
                    {
                        sender = ConfigurationManager.AppSettings["SenderNOZ"].ToString();
                        //sender = otpItem.EmailFrom;

                        //string emailTitleOseba = nozItem.Osebe_PDO != null ? pdoItem.Osebe_PDO.Ime + " " + pdoItem.Osebe_PDO.Priimek : ConfigurationManager.AppSettings["EmailTitlePDO"].ToString(); ;
                        //sender = nozItem.OsebaEmailFromID != null ? nozItem.Osebe_PDO.Email : ConfigurationManager.AppSettings["SenderNOZ"].ToString();


                        emailTitle = "(" + nozItem.EmailFrom + ") - " + ConfigurationManager.AppSettings["EmailTitleNOZ"].ToString();
                        emailTo = nozItem.EmailTo;
                        emailSubject = nozItem.EmailSubject;
                        emailBody = nozItem.EmailBody;
                    }
                    else
                    {
                        sender = ConfigurationManager.AppSettings["Sender"].ToString();
                        emailTitle = ConfigurationManager.AppSettings["EmailTitle"].ToString();
                        emailTo = item.EmailTo;
                        emailSubject = item.EmailSubject;
                        emailBody = item.EmailBody;
                    }




                    MailMessage message = new MailMessage();

                    // preverimo če je emailTO prazen pomeni, da vazame list pošiljateljev
                    if (emailTo.Length > 0)
                    {
                        if (emailTo != null && emailTo.Length > 0)
                        {
                            string[] splitTOEmails = emailTo.Split(';');
                            if (splitTOEmails.Length > 1)
                            {
                                foreach (var email in splitTOEmails)
                                {
                                    MailAddress ToEmail = new MailAddress(email);
                                    message.To.Add(ToEmail);
                                }
                            }
                            else
                            {
                                message.To.Add(emailTo);
                            }
                        }
                    }
                   
                    message.Sender = new MailAddress(sender);
                    message.From = new MailAddress(sender, emailTitle);
                    message.Subject = emailSubject;
                    message.IsBodyHtml = true;
                    message.Body = emailBody;
                    message.BodyEncoding = Encoding.UTF8;


                    if (isOTPitem)
                        otpItem.Status = (int)Enums.SystemEmailMessageStatus.Processed;
                    else if (isPDOitem)
                        pdoItem.Status = (int)Enums.SystemEmailMessageStatus.Processed;
                    else if (isNOZItem)
                        nozItem.Status = (int)Enums.SystemEmailMessageStatus.Processed;
                    else
                        item.Status = (int)Enums.SystemEmailMessageStatus.Processed;

                    // Boris 20.11.2019:  attachments
                    string attachmentFilename = "";
                    if (isOTPitem || isPDOitem || isNOZItem)
                    {
                        attachmentFilename = isOTPitem ? otpItem.Attachments : (isPDOitem ? pdoItem.Attachments : nozItem.Attachments);
                    }
                    else
                    { }

                    // Boris 27.04.2020: CC seznam emailov
                    if (isPDOitem)
                    {
                        if (pdoItem.CCEmails != null && pdoItem.CCEmails.Length > 0)
                        {
                            string[] splitCCEmails = pdoItem.CCEmails.Split(';');
                            foreach (var email in splitCCEmails)
                            {
                                MailAddress copy = new MailAddress(email);
                                message.CC.Add(copy);
                            }
                        }
                    }


                    if (!string.IsNullOrEmpty(attachmentFilename))
                    {
                        string[] splitAttachments = attachmentFilename.Split(';');
                        foreach (var fileAttachment in splitAttachments)
                        {
                            if (File.Exists(attachmentFilename))
                            {
                                Attachment attachment = new Attachment(fileAttachment, MediaTypeNames.Application.Octet);
                                ContentDisposition disposition = attachment.ContentDisposition;
                                disposition.CreationDate = File.GetCreationTime(fileAttachment);
                                disposition.ModificationDate = File.GetLastWriteTime(fileAttachment);
                                disposition.ReadDate = File.GetLastAccessTime(fileAttachment);
                                disposition.FileName = Path.GetFileName(fileAttachment);
                                disposition.Size = new FileInfo(fileAttachment).Length;
                                disposition.DispositionType = DispositionTypeNames.Attachment;
                                message.Attachments.Add(attachment);
                            }
                        }
                    }
                    // ------------------------------------------------------------------------------------

                    client.Send(message);
                }

                if (isOTPitem)
                    otpSystemEmailMessageRepo.SaveEmail(otpItem);
                else if (isPDOitem)
                    pdoSystemEmailMessageRepo.SaveEmail(pdoItem);
                else if (isNOZItem)
                    nozSystemEmailMessageRepo.SaveEmail(nozItem);
                else
                    systemEmailMessageRepo.SaveEmail(item);
            }
            catch (SmtpFailedRecipientsException ex)
            {
                if (isOTPitem)
                    otpItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                else if (isPDOitem)
                    pdoItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                else if (isNOZItem)
                    nozItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                else
                    item.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;

                DataTypesHelper.LogThis("Couldn't send the email to receipient: " + item.EmailTo + "\n" + ex.Message);

                if (isOTPitem)
                    otpSystemEmailMessageRepo.SaveEmail(otpItem);
                else if (isPDOitem)
                    pdoSystemEmailMessageRepo.SaveEmail(pdoItem);
                else if (isNOZItem)
                    nozSystemEmailMessageRepo.SaveEmail(nozItem);
                else
                    systemEmailMessageRepo.SaveEmail(item);
            }
            catch (SmtpException ex)
            {
                if (ex.Message.Contains("Mailbox unavailable"))
                {
                    if (isOTPitem)
                        otpItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                    else if (isPDOitem)
                        pdoItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                    else if (isNOZItem)
                        nozItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                    else
                        item.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;

                    DataTypesHelper.LogThis("Could not send the email to receipient: " + item.EmailTo);

                    if (isOTPitem)
                        otpSystemEmailMessageRepo.SaveEmail(otpItem);
                    else if (isPDOitem)
                        pdoSystemEmailMessageRepo.SaveEmail(pdoItem);
                    else if (isNOZItem)
                        nozSystemEmailMessageRepo.SaveEmail(nozItem);
                    else
                        systemEmailMessageRepo.SaveEmail(item);
                }
                else
                {
                    if (isOTPitem)
                        otpItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                    else if (isPDOitem)
                        pdoItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                    else if (isNOZItem)
                        nozItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                    else
                        item.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;

                    DataTypesHelper.LogThis("SmtpException: " + ex.Message);

                    if (isOTPitem)
                        otpSystemEmailMessageRepo.SaveEmail(otpItem);
                    else if (isPDOitem)
                        pdoSystemEmailMessageRepo.SaveEmail(pdoItem);
                    else if (isNOZItem)
                        nozSystemEmailMessageRepo.SaveEmail(nozItem);
                    else
                        systemEmailMessageRepo.SaveEmail(item);
                }
            }
            catch (Exception ex)
            {
                if (isOTPitem)
                    otpItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                else if (isPDOitem)
                    pdoItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                else if (isNOZItem)
                    nozItem.Status = (int)Enums.SystemEmailMessageStatus.RecipientError;
                else
                    item.Status = (int)Enums.SystemEmailMessageStatus.Error;

                if (isOTPitem)
                    otpSystemEmailMessageRepo.SaveEmail(otpItem);
                else if (isPDOitem)
                    pdoSystemEmailMessageRepo.SaveEmail(pdoItem);
                else if (isNOZItem)
                    nozSystemEmailMessageRepo.SaveEmail(nozItem);
                else
                    systemEmailMessageRepo.SaveEmail(item);

                DataTypesHelper.LogThis("LOG1: " + ex.Message);
            }
        }
    }
}
