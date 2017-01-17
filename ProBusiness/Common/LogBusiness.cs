﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ProDAL;
using ProEntity;
using ProEnum;

namespace ProBusiness.Common
{
    public class LogBusiness
    {
        public static LogBusiness BaseBusiness = new LogBusiness();
        public static int a = 0;

        #region 查询
        /// <summary>
        /// 获取日志
        /// </summary>
        /// <returns></returns>
        public static List<LogEntity> GetLogs( string type, int pageSize, int pageIndex, ref int totalCount, ref int pageCount, string btime="",string etime="")
        {
            string tablename = "UsersLog  a left join M_Users b  on a.UserID =b.UserID ";

            string sqlwhere = " 1=1 ";
            if (!string.IsNullOrEmpty(btime))
            {
               sqlwhere += " and a.CreateTime>='" + (string.IsNullOrEmpty(btime)
                    ? DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss")
                    : btime) + "'";
            }
            if (!string.IsNullOrEmpty(type))
            {
                sqlwhere += " and a.Type in(" + type + ")";
            }
            if (!string.IsNullOrEmpty(etime))
            {
                sqlwhere += " and a.CreateTime<'" + etime + "'";
            }
            DataTable dt = CommonBusiness.GetPagerData(tablename, "a.*,b.Avatar as Avatar ", sqlwhere, "a.AutoID ", pageSize, pageIndex, out totalCount, out pageCount);
            List<LogEntity> list = new List<LogEntity>();
            foreach (DataRow dr in dt.Rows)
            {
                LogEntity model = new LogEntity();
                model.FillData(dr); 
                list.Add(model);
            }
            return list;
        }
        public static LogEntity GetLogsByUserID(string userID,int tops=1)
        {
            string tablename = "select top "+tops+" a.*  from UsersLog  a left join M_Users b  on a.UserID =b.UserID ";

            string sqlwhere = " where 1=1 "; 
            if (!string.IsNullOrEmpty(userID))
            {
                sqlwhere += " and a.userID='" + userID + "'";
            }
            DataTable dt = LogDAL.GetDataTable(tablename + sqlwhere + " order by  a.AutoID  desc ");
            LogEntity model = new LogEntity();
            foreach (DataRow dr in dt.Rows)
            {
                model.FillData(dr); 
            }
            return model;
        }
        #endregion

        #region 添加

        /// <summary>
        /// 记录登录日志
        /// </summary>
        /// <param name="loginname">用户名</param>
        /// <param name="status">登录结果</param>
        /// <param name="systemtype">系统类型</param>
        /// <param name="operateip">登录IP</param>
        public static async void AddLoginLog(string loginname, string operateip, string userid, EnumUserOperateType systemtype = EnumUserOperateType.Login)
        {
            await LogDAL.AddLoginLog(loginname, (int)systemtype, operateip, userid);
        }

        /// <summary>
        /// 记录操作日志
        /// </summary>
        public static async void AddOperateLog(string userid, string username, string leveid, string seeid, string seename, EnumUserOperateType type,string message, string operateip)
        {
            await LogDAL.AddOperateLog(userid, username,leveid,seeid,seename, (int)type, message, operateip);
        }
        /// <summary>
        /// UpdateLastIP
        /// </summary>
        public static async void UpdateLastIP(string userid, string operateip)
        {
            await LogDAL.UpdateLastIP(userid, operateip);
        }
        #endregion
    }
}
