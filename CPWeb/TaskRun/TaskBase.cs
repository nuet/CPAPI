using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using FluentScheduler; 
using LLibrary;
using CPiao.TaskRun;
using ProBusiness;
using ProTools;

namespace CPiao.TaskRun
{
    public class TaskBase : Registry
    {
        #region
        string start = "09:03";
        string end = "23:59";
        #endregion
        private Object thisLock = new Object(); 
        private Object gdlock = new Object();
        private Object sdlock = new Object();
        private Object hljlock = new Object();
        private Object jxlock = new Object();
        private Object xjlock = new Object();
        private Object tjlock = new Object();
        private Object fc3dlock = new Object(); 
        private Object shlock = new Object();
        private Object qcscc = new Object();
        public TaskBase()
        {
            NonReentrantAsDefault();

           // InsertLottery();
            UpdateLotteryStatus();
            UpdateSD11X5Result();
            UpdateGD11X5Result();
            UpdateJX11X5Result();
            UpdateSHSSLResult();
            UpdateTJSSCResult();
            UpdateXJSSCResult();
            UpdateHLJSSCResult();
            UpdateFC3DResult();
            UpdateCQSSCResult();
            CheckLotteryResult();
            BeetAuto();
            //NonReentrant();
            //Reentrant();
            //Disable();

            //Faulty();
            //Removed();
            //Parameter();
            //Disposable();

            //FiveMinutes();
            //TenMinutes();
            //Hour();
            //Day();
            //Weekday();
            //Week();
        }

        private void InsertLottery()
        { 
            //Schedule(() =>
            //{ 
            //   TaskService.BasService.InsertAllLottery(); 
            //}).NonReentrant().WithName("[insertlottery]").ToRunEvery(1).Days().At(04, 30);
        }
        private void UpdateLotteryStatus()
        { 
            LogHelper.Info("UpdateLotteryStatus", "TaskBase", "Begin");

            Schedule(() =>
            {
                TimeSpan startTime = DateTime.Parse(start).TimeOfDay;
                TimeSpan endTime = DateTime.Parse(end).TimeOfDay;
                TimeSpan tmNow = DateTime.Now.TimeOfDay;
                int min = DateTime.Now.Minute;
                int sec = DateTime.Now.Second;
                var s = min.ToString().Length > 1 ? min.ToString().Substring(1, 1) : min.ToString();
                if (tmNow > DateTime.Parse("00:00").TimeOfDay && tmNow < DateTime.Parse("02:20").TimeOfDay)
                {
                    if (s == "0" || s == "5")
                    {
                        lock (thisLock)
                        {
                            LotteryResultBusiness.UpdateStatus("XJSSC,CQSSC,HLJSSC,", 1);
                            LogHelper.Info("UpdateStatus", "TaskBase", "XJSSC,CQSSC,HLJSSC End");
                        }
                    }
                }
               
                if (tmNow >= startTime && tmNow <= endTime)
                {
                   
                    if (s == "3")
                    {
                        lock (thisLock)
                        {
                            LotteryResultBusiness.UpdateStatus("SD11X5,", 1);
                            LogHelper.Info("UpdateStatus", "TaskBase", "SD11X5 End");
                        }
                    }
                    else if (s == "9")
                    {
                        lock (thisLock)
                        {
                            if ((min > 20 && DateTime.Now.Hour > 8) || DateTime.Now.Hour > 9)
                            {
                                LotteryResultBusiness.UpdateStatus("HLJSSC,", 1);
                            }
                            if (DateTime.Now.Hour > 9)
                            {
                                LotteryResultBusiness.UpdateStatus("XJSSC,CQSSC,", 1);
                            }
                            LotteryResultBusiness.UpdateStatus("GD11X5,JX11X5,TJSSC,", 1);
                            LogHelper.Info("UpdateStatus", "TaskBase", "GD11X5,JX11X5,HLJSSC,XJSSC,TJSSC,CQSSC End");
                        }
                    }
                    else if (s == "4" && tmNow > DateTime.Parse("20:00").TimeOfDay )
                    {
                        lock (thisLock)
                        {
                            LotteryResultBusiness.UpdateStatus("CQSSC,", 1);
                        }
                    }
                    if (min > 50 && (DateTime.Now.Hour == 20 || DateTime.Now.Hour == 21))
                    {
                        lock (thisLock)
                        {
                            LotteryResultBusiness.UpdateStatus("FCSD,", 1);
                            LogHelper.Info("UpdateStatus", "TaskBase", "FCSD End");
                        }
                    }
                    if ((min == 0 && DateTime.Now.Hour > 10) || (min == 30 && DateTime.Now.Hour > 9))
                    {
                        lock (thisLock)
                        {
                            LotteryResultBusiness.UpdateStatus("SHSSL,", 1);
                            LogHelper.Info("UpdateStatus", "TaskBase", "SHSSL End");
                        }
                        
                    }

                }
            }).NonReentrant().WithName("[updatelotterystatus]").ToRunNow().AndEvery(1).Minutes();
           
        }
        private void UpdateSD11X5Result()
        { 
            LogHelper.Info("UpdateResult", "TaskBase", "SD11X5 Begin");
            Schedule(() =>
            {
                TimeSpan startTime = DateTime.Parse("09:05").TimeOfDay;
                TimeSpan endTime = DateTime.Parse(end).TimeOfDay;
                TimeSpan tmNow = DateTime.Now.TimeOfDay;
                int min = DateTime.Now.Minute;
                int sec = DateTime.Now.Second;
                if (tmNow >= startTime && tmNow <= endTime)
                { 
                    lock (sdlock)
                    {
                        //方法处理
                        KCWBase<DataResult> kcwresult = ProTools.HttpRequest.RequestServer<KCWBase<DataResult>>(ProTools.KCWAppUrl.NewLy,
                                Getparas("SD11X5"));
                        if (kcwresult != null && kcwresult.Data.Count > 0)
                        {
                            var suc =
                                TaskService.BasService.OpenLotteryResult(kcwresult.Data[0].OpenCode.Replace(',', ' '),
                                    kcwresult.Data[0].Expect, "SD11X5"); 
                            LogHelper.Info("UpdateResult", "TaskBase",
                                "SD11X5:" + kcwresult.Data[0].Expect + (suc ? "开奖成功!" : "开奖失败") + " End");
                        }
                        else
                        {
                            LogHelper.Info("UpdateResult", "TaskBase", "SD11X5  End");
                        }
                    }  
                }
            }).NonReentrant().WithName("[updatesd11x5result]").ToRunNow().AndEvery(1).Minutes(); 
        }
        private void UpdateJX11X5Result()
        { 
            LogHelper.Info("UpdateResult", "TaskBase", "JX11X5 Begin");
            Schedule(() =>
            {
                TimeSpan startTime = DateTime.Parse("09:05").TimeOfDay;
                TimeSpan endTime = DateTime.Parse(end).TimeOfDay;
                TimeSpan tmNow = DateTime.Now.TimeOfDay;
                int min = DateTime.Now.Minute;
                int sec = DateTime.Now.Second;
                if (tmNow >= startTime && tmNow <= endTime)
                { 
                    lock (jxlock)
                    {
                        //方法处理
                        KCWBase<DataResult> kcwresult = ProTools.HttpRequest.RequestServer<KCWBase<DataResult>>(ProTools.KCWAppUrl.NewLy,
                                Getparas("JX11X5"));
                        if (kcwresult!=null && kcwresult.Data.Count > 0)
                        {
                            var suc = TaskService.BasService.OpenLotteryResult(kcwresult.Data[0].OpenCode.Replace(',',' '), kcwresult.Data[0].Expect, "JX11X5"); 
                            LogHelper.Info("UpdateResult", "TaskBase", "JX11X5:" + kcwresult.Data[0].Expect + (suc ? "开奖成功!" : "开奖失败")+"  End");
                        }
                        else
                        {
                            LogHelper.Info("UpdateResult", "TaskBase", "JX11X5  End");
                        }
                    }  
                }
            }).NonReentrant().WithName("[UpdateJX11X5Result]").ToRunNow().AndEvery(1).Minutes();
        }
        private void UpdateGD11X5Result()
        { 
            LogHelper.Info("UpdateResult", "TaskBase", "JX11X5 Begin");
            Schedule(() =>
            {
                TimeSpan startTime = DateTime.Parse("09:05").TimeOfDay;
                TimeSpan endTime = DateTime.Parse(end).TimeOfDay;
                TimeSpan tmNow = DateTime.Now.TimeOfDay;
                int min = DateTime.Now.Minute;
                int sec = DateTime.Now.Second;
                if (tmNow >= startTime && tmNow <= endTime)
                { 
                    lock (gdlock)
                    {
                        KCWBase<DataResult> kcwresult = ProTools.HttpRequest.RequestServer<KCWBase<DataResult>>(ProTools.KCWAppUrl.NewLy,
                                Getparas("GD11X5"));
                        if (kcwresult != null && kcwresult.Data.Count > 0)
                        {
                            var suc = TaskService.BasService.OpenLotteryResult(kcwresult.Data[0].OpenCode.Replace(',',' '), kcwresult.Data[0].Expect, "GD11X5"); 
                            LogHelper.Info("UpdateResult", "TaskBase", "GD11X5:" + kcwresult.Data[0].Expect + (suc ? "开奖成功!" : "开奖失败") + "  End");
                        }
                        else
                        {
                            LogHelper.Info("UpdateResult", "TaskBase", "GD11X5  End");
                        }
                    } 
                }
            }).NonReentrant().WithName("[UpdateGD11X5Result]").ToRunNow().AndEvery(1).Minutes();
        }
        private void UpdateSHSSLResult()
        { 
            LogHelper.Info("UpdateResult", "TaskBase", "SHSSL End");
            Schedule(() =>
            {
                TimeSpan startTime = DateTime.Parse("10:26").TimeOfDay;
                TimeSpan endTime = DateTime.Parse("22:10").TimeOfDay;
                TimeSpan tmNow = DateTime.Now.TimeOfDay; 
                if (tmNow >= startTime && tmNow <= endTime)
                { 
                    lock (shlock)
                    {
                        KCWBase<DataResult> kcwresult = ProTools.HttpRequest.RequestServer<KCWBase<DataResult>>(ProTools.KCWAppUrl.NewLy,
                                Getparas("SHSSL"));
                        if (kcwresult != null && kcwresult.Data.Count > 0)
                        {
                                var suc=TaskService.BasService.OpenLotteryResult(kcwresult.Data[0].OpenCode.Replace(',',' '), kcwresult.Data[0].Expect, "SHSSL"); 
                                LogHelper.Info("UpdateResult", "TaskBase", "SHSSL:" + kcwresult.Data[0].Expect + (suc ? "开奖成功!" : "开奖失败") + "  End");
                        }
                        else
                        {
                            LogHelper.Info("UpdateResult", "TaskBase", "SHSSL End");
                        }
                    } 
                }
            }).NonReentrant().WithName("[UpdateSHSSLResult]").ToRunNow().AndEvery(1).Minutes();
        }
        private void UpdateTJSSCResult()
        { 
            LogHelper.Info("UpdateResult", "TaskBase", "TJSSC Begin");
            Schedule(() =>
            {
                TimeSpan startTime = DateTime.Parse("09:09").TimeOfDay;
                TimeSpan endTime = DateTime.Parse("23:05").TimeOfDay;
                TimeSpan tmNow = DateTime.Now.TimeOfDay; 
                if (tmNow >= startTime && tmNow <= endTime)
                { 
                    lock (tjlock)
                    {
                        KCWBase<DataResult> kcwresult = ProTools.HttpRequest.RequestServer<KCWBase<DataResult>>(ProTools.KCWAppUrl.NewLy,
                                Getparas("TJSSC"));
                        if (kcwresult != null && kcwresult.Data.Count > 0)
                        {
                                var suc=TaskService.BasService.OpenLotteryResult(kcwresult.Data[0].OpenCode.Replace(',',' '), kcwresult.Data[0].Expect, "TJSSC"); 
                                LogHelper.Info("UpdateResult", "TaskBase", "TJSSC:" + kcwresult.Data[0].Expect + (suc ? "开奖成功!" : "开奖失败") + "  End");
                        }
                        else
                        {
                            LogHelper.Info("UpdateResult", "TaskBase", "TJSSC End");
                        }
                    }
                }
            }).NonReentrant().WithName("[UpdateTJSSCResult]").ToRunNow().AndEvery(1).Minutes();
        }
        private void UpdateHLJSSCResult()
        { 
            LogHelper.Info("UpdateResult", "TaskBase", "HLJSSC Begin");
            Schedule(() =>
            {
                TimeSpan startTime = DateTime.Parse("09:09").TimeOfDay;
                TimeSpan endTime = DateTime.Parse("23:02").TimeOfDay;
                TimeSpan tmNow = DateTime.Now.TimeOfDay; 
                if (tmNow >= startTime && tmNow <= endTime)
                { 
                    lock (hljlock)
                    {
                        KCWBase<DataResult> kcwresult = ProTools.HttpRequest.RequestServer<KCWBase<DataResult>>(ProTools.KCWAppUrl.NewLy,
                                Getparas("HLJSSC"));
                        if (kcwresult != null && kcwresult.Data.Count > 0)
                        {
                                var suc=TaskService.BasService.OpenLotteryResult(kcwresult.Data[0].OpenCode.Replace(',',' '), kcwresult.Data[0].Expect, "HLJSSC");
                                LogHelper.Info("UpdateResult", "TaskBase", "HLJSSC:" + kcwresult.Data[0].Expect + (suc ? "开奖成功!" : "开奖失败") + "  End");
                        }
                        else
                        {
                            LogHelper.Info("UpdateResult", "TaskBase", "HLJSSC End");
                        }
                    }
                }
            }).NonReentrant().WithName("[UpdateHLJSSCResult]").ToRunNow().AndEvery(1).Minutes();
        }

        private void UpdateCQSSCResult()
        {
            LogHelper.Info("UpdateResult", "TaskBase", "CQSSC Begin");
            Schedule(() =>
            {
                TimeSpan startTime = DateTime.Parse("09:09").TimeOfDay;
                TimeSpan endTime = DateTime.Parse("23:59").TimeOfDay;
                TimeSpan tmNow = DateTime.Now.TimeOfDay;
                if ((tmNow >= startTime && tmNow <= endTime) || (tmNow >= DateTime.Parse("00:00").TimeOfDay && tmNow <= DateTime.Parse("02:07").TimeOfDay))
                {
                    lock (qcscc)
                    {
                        KCWBase<DataResult> kcwresult =
                            ProTools.HttpRequest.RequestServer<KCWBase<DataResult>>(ProTools.KCWAppUrl.NewLy,
                                Getparas("CQSSC"));
                        if (kcwresult != null && kcwresult.Data.Count > 0)
                        {
                            var suc =
                                TaskService.BasService.OpenLotteryResult(kcwresult.Data[0].OpenCode.Replace(',', ' '),
                                    kcwresult.Data[0].Expect, "CQSSC");
                            LogHelper.Info("UpdateResult", "TaskBase", "CQSSC:" + kcwresult.Data[0].Expect + (suc ? "开奖成功!" : "开奖失败") + "  End");
                        }
                        else
                        {
                            LogHelper.Info("UpdateResult", "TaskBase", "CQSSC End");
                        }
                    }
                }
            }).NonReentrant().WithName("[UpdateCQSSCResult]").ToRunNow().AndEvery(1).Minutes();
        }

        private void UpdateFC3DResult()
        { 
            LogHelper.Info("UpdateResult", "TaskBase", "FC3D Begin");
            Schedule(() =>
            {
                TimeSpan startTime = DateTime.Parse("20:30").TimeOfDay;
                TimeSpan endTime = DateTime.Parse("21:40").TimeOfDay;
                TimeSpan tmNow = DateTime.Now.TimeOfDay;
                if (tmNow >= startTime && tmNow <= endTime)
                {
                    if (DateTime.Now.Minute%2 == 0)
                    {
                        lock (fc3dlock)
                        {

                            KCWBase<DataResult> kcwresult =
                                ProTools.HttpRequest.RequestServer<KCWBase<DataResult>>(ProTools.KCWAppUrl.NewLy,
                                    Getparas("FC3D"));
                            if (kcwresult != null && kcwresult.Data.Count > 0)
                            {
                                var suc =
                                    TaskService.BasService.OpenLotteryResult(
                                        kcwresult.Data[0].OpenCode.Replace(',', ' '),
                                        kcwresult.Data[0].Expect, "FC3D");
                                LogHelper.Info("UpdateResult", "TaskBase", "FC3D:" + kcwresult.Data[0].Expect + (suc ? "开奖成功!" : "开奖失败") + "  End");
                            }
                            else
                            {
                                LogHelper.Info("UpdateResult", "TaskBase", "FC3D End");
                            }
                        }
                    }
                }
            }).NonReentrant().WithName("[UpdateFC3DResult]").ToRunNow().AndEvery(5).Minutes();
        }
        private void UpdateXJSSCResult()
        { 
            LogHelper.Info("UpdateResult", "TaskBase", "XJSSC Begin");
            Schedule(() =>
            {
                TimeSpan startTime = DateTime.Parse("09:09").TimeOfDay;
                TimeSpan endTime = DateTime.Parse("23:59").TimeOfDay;
                TimeSpan tmNow = DateTime.Now.TimeOfDay;
                if ((tmNow >= startTime && tmNow <= endTime) || (tmNow >= DateTime.Parse("00:00").TimeOfDay && tmNow <= DateTime.Parse("02:07").TimeOfDay))
                {
                    lock (xjlock)
                    {
                        KCWBase<DataResult> kcwresult =
                            ProTools.HttpRequest.RequestServer<KCWBase<DataResult>>(ProTools.KCWAppUrl.NewLy,
                                Getparas("XJSSC"));
                        if (kcwresult != null && kcwresult.Data.Count > 0)
                        {
                            var suc =
                                TaskService.BasService.OpenLotteryResult(kcwresult.Data[0].OpenCode.Replace(',', ' '),
                                    kcwresult.Data[0].Expect, "XJSSC");
                            LogHelper.Info("UpdateResult", "TaskBase", "XJSSC:" + kcwresult.Data[0].Expect + (suc ? "开奖成功!" : "开奖失败") + "  End");
                        }
                        else
                        {
                            LogHelper.Info("UpdateResult", "TaskBase", "XJSSC End");
                        }
                    }
                }
            }).NonReentrant().WithName("[UpdateXJSSCResult]").ToRunNow().AndEvery(1).Minutes();
        }

        private void BeetAuto()
        {
            LogHelper.Info("BeetAuto", "TaskBase", "自动投注 Begin");
            Schedule(() =>
            { 
                TaskService.BasService.BettAutoInsert();
                LogHelper.Info("BeetAuto", "TaskBase", "自动投注 End");
            }).WithName("[beetAuto]").ToRunNow().AndEvery(1).Minutes();
        }

        public Dictionary<string, object> Getparas(string code,int row=1,string format="json",string adate="")
        {
            //{row:"1-20",format:"json/xml",date:"2016-12-06",code:"彩票代码"}
            Dictionary<string, object> paraDir = new Dictionary<string, object>();
            paraDir.Add("code",code); 
            if (!string.IsNullOrEmpty(format))
            {
                paraDir.Add("format", format);
            }
            if (!string.IsNullOrEmpty(adate))
            {
                paraDir.Add("date", adate);
            }
            paraDir.Add("rows", row > 0 ? row : 5);
            return paraDir;
        }



        private void CheckLotteryResult()
        {
            LogHelper.Info("CheckResult", "TaskBase","漏开奖检测 Begin");
            Schedule(() =>
            {
                TimeSpan startTime = DateTime.Parse("08:00").TimeOfDay;
                TimeSpan endTime = DateTime.Parse("23:59").TimeOfDay;
                TimeSpan tmNow = DateTime.Now.TimeOfDay;
                if ((tmNow >= startTime && tmNow <= endTime) || (tmNow > DateTime.Parse("00:00").TimeOfDay && tmNow < DateTime.Parse("04:00").TimeOfDay))
                {
                    string codes = "";
                    CommonBusiness.LottertList.ForEach(x =>
                    {
                        codes += x.CPCode + ",";
                    });
                    TaskService.BasService.UpdByStatusAndOpenTime(codes,
                        DateTime.Now.AddSeconds(-30).ToString("yyyy-MM-dd HH:mm:ss"));

                    foreach (var item in CommonBusiness.LottertList)
                    {
                        KCWBase<DataResult> kcwresult =
                        ProTools.HttpRequest.RequestServer<KCWBase<DataResult>>(ProTools.KCWAppUrl.NewLy,
                            Getparas(item.CPCode.ToUpper(), 20));
                        if (kcwresult != null && kcwresult.Data.Count > 0)
                        {
                            foreach (var child in kcwresult.Data)
                            {
                                var suc =
                               TaskService.BasService.OpenLotteryResult(child.OpenCode.Replace(',', ' '),
                                   child.Expect, item.CPCode.ToUpper());
                                LogHelper.Info("CheckResult", "TaskBase", item.CPCode.ToUpper() + "漏开奖检测 " + child.Expect + (suc ? "开奖成功!" : "开奖失败") + " End");
                            }
                           
                        } 
                        Thread.Sleep(10000);
                    }
                    
                }
            }).NonReentrant().WithName("[CheckLotteryResult]").ToRunNow().AndEvery(1).Hours(); 
        }

        private void Disable()
        { 
            Schedule(() =>
            {
                JobManager.RemoveJob("[reentrant]");
                JobManager.RemoveJob("[non reentrant]");
                L.Log("[disable]", "Disabled the reentrant and non reentrant jobs.");
            }).WithName("[disable]").ToRunOnceIn(3).Minutes();
        }

        private void Faulty()
        {
            Schedule(() =>
            {
                L.Register("[faulty]", "I'm going to raise an exception!");
                throw new Exception("I warned you.");
            }).WithName("[faulty]").ToRunEvery(20).Minutes();
        }

        private void Removed()
        {
            Schedule(() =>
            {
                L.Register("[removed]", "SOMETHING WENT WRONG.");
            }).WithName("[removed]").ToRunOnceIn(2).Minutes();
        }

        private void Parameter()
        {
            Schedule(new ParameterJob { Parameter = "Foo" }).WithName("[parameter]").ToRunOnceIn(10).Seconds();
        }

        private void Disposable()
        {
            Schedule<DisposableJob>().WithName("[disposable").ToRunOnceIn(10).Seconds();
        }

        private void FiveMinutes()
        {
            Schedule(() => L.Log("[five minutes]", "Five minutes has passed."))
                .WithName("[five minutes]").ToRunOnceAt(DateTime.Now.AddMinutes(5)).AndEvery(5).Minutes();
        }

        private void TenMinutes()
        {
            Schedule(() => L.Log("[ten minutes]", "Ten minutes has passed."))
                .WithName("[ten minutes]").ToRunEvery(10).Minutes();
        }

        private void Hour()
        {
            Schedule(() => L.Log("[hour]", "A hour has passed."))
                .WithName("[hour]").ToRunEvery(1).Hours();
        }

        private void Day()
        {
            Schedule(() => L.Log("[day]", "A day has passed."))
                .WithName("[day]").ToRunEvery(1).Days();
        }

        private void Weekday()
        {
            Schedule(() => L.Log("[weekday]", "A new weekday has started."))
                .WithName("[weekday]").ToRunEvery(1).Weekdays();
        }

        private void Week()
        {
            Schedule(() => L.Log("[week]", "A new week has started."))
                .WithName("[week]").ToRunEvery(1).Weeks();
        }

    }

}
 