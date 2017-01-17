using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 
using Newtonsoft.Json; 
using ProEntity;

namespace ProBusiness
{
    public class TaskService
    {
        public static TaskService BasService = new TaskService();

        public void InsertAllLottery()
        {
            LotteryResultBusiness.InsertAllLottery();
        }

        public string BettAutoInsert()
        { 
            var list = LotteryOrderBusiness.GetBettAutoByStatus();
            string msg = "";
             list.ForEach(x =>
             {
                 string errmsg = "";
                 var issuenum = GetIssueNum(x.ComNum, x.JsonContent);
                 var lottery = LotteryResultBusiness.GetLotteryResult(x.CPCode, "1,2");
                 if (IsEquelNum(x.CPCode, lottery.IssueNum, issuenum))
                 {
                     var totalmuch = GetIssueNum(x.ComNum, x.JsonContent, 1);
                     var pMuch = string.IsNullOrEmpty(totalmuch) ? x.BMuch : Convert.ToInt32(totalmuch);
                     int comnum = x.ComNum + 1;
                     try
                     {
                         if (!string.IsNullOrEmpty(issuenum))
                         {
                             LotteryOrderBusiness.CreateLotteryOrder(x.BCode + comnum, issuenum, x.Type, x.TypeName, x.CPCode,
                                 x.CPName,
                                 x.Content, x.Num, x.PayFee*pMuch/x.PMuch, x.UserID, pMuch, x.RPoint, x.IP, 0,4,x.BCode,
                                 ref errmsg);
                             if (!string.IsNullOrEmpty(errmsg))
                             {
                                 errmsg = issuenum + ":" + errmsg + ";";
                             }
                         }
                     }
                     catch (Exception ex)
                     {
                         errmsg = x.BCode + "第" + comnum + "期插入失败";

                        // L.Log("[BettAutoInsert] ", x.BCode + "第" + comnum + "期插入失败");
                     }
                     msg += errmsg;
                     LotteryOrderBusiness.UpdateBettAutoByCode(x.BCode, comnum, pMuch * x.PayFee, errmsg);
                 }
             });
             return msg;
        }


        public bool OpenLotteryResult(string result,string issnum,string cpcode)
        {
            var s= LotteryResultBusiness.UpdateSD11X5Result(result, issnum, cpcode); 
            return s;
        }

        public bool UpdByStatusAndOpenTime(string cpcode,string opentime)
        {
            return LotteryResultBusiness.UpdateByStatusAndOpentTime(cpcode, opentime);
        }

        public bool IsEquelNum(string cpcode, string issuenum, string nowNum)
        {
            bool result =false;
            int num = Convert.ToInt32(issuenum.Substring(issuenum.Length - 2, 2));
            var comissuenum = DateTime.Now.ToString("yyyyMMdd").Substring(2, 6);
            if (cpcode == "SD11X5")
            {
                if (num == 78)
                {
                    comissuenum = DateTime.Now.AddDays(1).ToString("yyyyMMdd").Substring(2, 6);
                    num = num - 78;
                }
                comissuenum = comissuenum + num.ToString().PadLeft(2, '0');
                return (comissuenum == nowNum);
            }
            else if (cpcode == "GD115" || cpcode == "TJSSC")
            {
                if (num == 84)
                {
                    comissuenum = DateTime.Now.AddDays(1).ToString("yyyyMMdd").Substring(2, 6);
                    num = num - 84;
                }
                comissuenum = comissuenum + num.ToString().PadLeft(2, '0');
                return (comissuenum == nowNum);
            }
            else if (cpcode == "JX115")
            {
                comissuenum = DateTime.Now.ToString("yyyyMMdd");
                if (num == 65)
                {
                    comissuenum = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                    num = num - 65;
                }
                comissuenum = comissuenum + num.ToString().PadLeft(2, '0');
                return (comissuenum == nowNum);
            }
            else if (cpcode == "XJ115")
            {
                if (num == 65)
                {
                    comissuenum = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                    num = num - 65;
                }
                comissuenum = comissuenum + num.ToString().PadLeft(2, '0');
                return (comissuenum == nowNum);
            }
            else if (cpcode == "HLJSSC")
            {
                num = Convert.ToInt32(issuenum) + 1;
                return num == Convert.ToInt32(nowNum);
            }
            return result;
        }

        public string GetIssueNum(string cpcode, string issuenum, int comnum, string jsoncontent)
        {
            string comissuenum = DateTime.Now.ToString("yyyyMMdd");
            int num = Convert.ToInt32(issuenum.Substring(issuenum.Length - 2, 2));
            num = num + comnum;
            if (cpcode == "SD11X5")
            {
                comissuenum = comissuenum.Substring(2, 6);
                if (num > 78)
                {
                    comissuenum = DateTime.Now.AddDays(1).ToString("yyyyMMdd").Substring(2, 6);
                    num = num - 78;
                }
            }
            else if (cpcode == "GD115" || cpcode == "TJSSC")
            {
                comissuenum = comissuenum.Substring(2, 6);
                if (num > 84)
                {
                    comissuenum = DateTime.Now.AddDays(1).ToString("yyyyMMdd").Substring(2, 6);
                    num = num - 84;
                }
            }
            else if (cpcode == "JX115")
            {
                comissuenum = DateTime.Now.ToString("yyyyMMdd");
                if (num > 65)
                {
                    comissuenum = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                    num = num - 65;
                }
            }
            else if (cpcode == "XJSSC")
            {
                if (num > 96)
                {
                    comissuenum = DateTime.Now.AddDays(1).ToString("yyyyMMdd");
                    num = num - 96;
                }
            }
            else if (cpcode=="HLJSSC")
            {
                num = Convert.ToInt32(issuenum) + 1;
                return num.ToString().PadLeft(7, '0');
            }
            comissuenum = comissuenum + num.ToString().PadLeft(2,'0');
            return comissuenum;
        }
        public string GetIssueNum(int comnum, string jsoncontent,int type=0)
        {
            string comissuenum = "";
            var jsonarr = jsoncontent.Split('|');
            if (jsonarr.Length>0 && !string.IsNullOrEmpty(jsonarr[comnum]))
            {
                comissuenum= jsonarr[comnum].Split(',')[type];
            } 
            return comissuenum;
        } 

    }
}
