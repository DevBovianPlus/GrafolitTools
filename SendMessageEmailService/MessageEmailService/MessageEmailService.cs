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
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Threading;

namespace Service1
{
    public partial class MessageEmailService : ServiceBase
    {
        private IKernel kernel;

        private ISystemMessageEventsRepository systemMessageEventsRepo;
        //private IPostRepository postRepo;
        private DateTime timeToStartAvtomatika;

        private Timer timerSchedular;
        public MessageEmailService()
        {
            try
            {
                InitializeComponent();

                kernel = new StandardKernel();

                kernel.Bind<ISystemMessageEventsRepository>().To<SystemMessageEventsRepository>();
                kernel.Bind<IEventRepository>().To<EventRepository>();
                kernel.Bind<IEmployeeRepository>().To<EmployeeRepository>();
                kernel.Bind<IPostRepository>().To<PostRepository>();

                systemMessageEventsRepo = kernel.Get<ISystemMessageEventsRepository>();

                timeToStartAvtomatika = DateTime.Now;
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message);
            }

        }

        protected override void OnStart(string[] args)
        {

            try
            {
                /*System.Timers.Timer timer = new System.Timers.Timer();
                timer.Interval = Convert.ToDouble(ConfigurationManager.AppSettings["TimeInterval"].ToString()); // every 6min
                timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
                timer.Start();*/

                this.ScheduleService();
            }
            catch (Exception e)
            {
                DataTypesHelper.LogThis(e.Message);
                throw;
            }
        }

        protected override void OnStop()
        {
        }

        private void ScheduleService()
        {
            try
            {
                timerSchedular = new Timer(new TimerCallback(TimerScheduleCallback));
                //Set the Default Time.
                DateTime scheduledTime = DateTime.MinValue;

                string scheduleMode = System.Configuration.ConfigurationManager.AppSettings["ScheduleMode"].ToString();

                if (scheduleMode == "Dnevno")
                {
                    scheduledTime = DateTime.Parse(System.Configuration.ConfigurationManager.AppSettings["ScheduledTime"]);
                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next day.
                        scheduledTime = scheduledTime.AddDays(1);
                    }
                }
                else if (scheduleMode == "Interval")
                {
                    int intervalMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalMin"]);
                    scheduledTime = DateTime.Now.AddMinutes(intervalMinutes);

                    if (DateTime.Now > scheduledTime)
                    {
                        //If Scheduled Time is passed set Schedule for the next Interval.
                        scheduledTime = scheduledTime.AddMinutes(intervalMinutes);
                    }
                }

                TimeSpan timeSpan = scheduledTime.Subtract(DateTime.Now);
                //Get the difference in Minutes between the Scheduled and Current Time.
                int dueTime = Convert.ToInt32(timeSpan.TotalMilliseconds);

                //Change the Timer's Due Time.
                timerSchedular.Change(dueTime, Timeout.Infinite);
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message + ex.StackTrace);
            }
        }

        private void TimerScheduleCallback(object e)
        {
            try
            {
                DataTypesHelper.LogThis("*****TimerScheduleCallback start*****");
                systemMessageEventsRepo.GetEventsWithNoReportForMeeting();
                
                systemMessageEventsRepo.GetEventsWithNoPreparationForMeeting();

                DataTypesHelper.LogThis("*****Before Firing Method GetUnProcessedRecordsAvtomatika*****");
                systemMessageEventsRepo.GetUnProcessedRecordsAvtomatika();
                DataTypesHelper.LogThis("*****After Firing Method GetUnProcessedRecordsAvtomatika*****");

                foreach (SystemMessageEvents item in systemMessageEventsRepo.GetUnProcessedMesseges())
                {
                    DataTypesHelper.LogThis("*****in foreach GetUnProcessedMesseges - instance values: *****" + item.Code);
                    if (item.Code == Enums.SystemMessageEventCodes.NewMessage.ToString())
                        systemMessageEventsRepo.ProcessNewMessage(item);
                    else if (item.Code == Enums.SystemMessageEventCodes.AUTO.ToString())
                    {
                        //DataTypesHelper.LogThis("*****Before ProcesAutoMesages*****");
                        systemMessageEventsRepo.ProcessAutoMessage(item);
                        //DataTypesHelper.LogThis("*****After ProcesAutoMesages*****");
                    }
                    else if (item.Code == Enums.SystemMessageEventCodes.EVENT_DOGODEK.ToString())
                        systemMessageEventsRepo.ProcessEventMessage(item);
                    else if (systemMessageEventsRepo.EventPreparationOrReport(item.Code))
                        systemMessageEventsRepo.ProcessEventMessage(item, (Enums.SystemMessageEventCodes)Enum.Parse(typeof(Enums.SystemMessageEventCodes), item.Code, true));
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message + "\n" + ex.InnerException != null ? ex.InnerException.Message : "" + "\n" + ex.StackTrace);
            }
            
            this.ScheduleService();
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {

            // TODO: Insert monitoring activities here.
            DataTypesHelper.LogThis(args.SignalTime.ToLongTimeString() + "\n");
            try
            {
                //TODO: Preveri tabelo Avtomatika in prenesi zapise v SystemMessageEvents.
                if (timeToStartAvtomatika.CompareTo(args.SignalTime) <= 0)
                {
                    systemMessageEventsRepo.GetUnProcessedRecordsAvtomatika();
                    timeToStartAvtomatika = args.SignalTime;
                    timeToStartAvtomatika = timeToStartAvtomatika.AddMinutes(Convert.ToDouble(ConfigurationManager.AppSettings["TimeIntervalAvtomatika"].ToString()));//every 3 min
                    DataTypesHelper.LogThis("Current time: " + args.SignalTime.ToLongTimeString() + "\n");
                    DataTypesHelper.LogThis("Next start at: " + timeToStartAvtomatika.ToLongTimeString() + "\n");
                }

                systemMessageEventsRepo.GetEventsWithNoReportForMeeting();
                systemMessageEventsRepo.GetEventsWithNoPreparationForMeeting();

                foreach (SystemMessageEvents item in systemMessageEventsRepo.GetUnProcessedMesseges())
                {
                    if (item.Code == Enums.SystemMessageEventCodes.NewMessage.ToString())
                        systemMessageEventsRepo.ProcessNewMessage(item);
                    else if (item.Code == Enums.SystemMessageEventCodes.AUTO.ToString())
                        systemMessageEventsRepo.ProcessAutoMessage(item);
                    else if (item.Code == Enums.SystemMessageEventCodes.EVENT_DOGODEK.ToString())
                        systemMessageEventsRepo.ProcessEventMessage(item);
                    else if (item.Code == Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO.ToString())
                        systemMessageEventsRepo.ProcessEventMessage(item, Enums.SystemMessageEventCodes.EVENT_PRIPRAVA_OPOZORILO);
                    else if (item.Code == Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO.ToString())
                        systemMessageEventsRepo.ProcessEventMessage(item, Enums.SystemMessageEventCodes.EVENT_POROCILO_OPOZORILO);
                }
            }
            catch (Exception ex)
            {
                DataTypesHelper.LogThis(ex.Message + "\n" + ex.InnerException != null ? ex.InnerException.Message : "" + "\n" + ex.StackTrace);
            }
        }
    }
}
