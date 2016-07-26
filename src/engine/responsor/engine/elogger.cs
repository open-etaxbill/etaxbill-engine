﻿using System;
using System.Diagnostics;
using OpenETaxBill.SDK.Configuration;

namespace OpenETaxBill.Engine.Responsor
{
    /// <summary>
    /// 
    /// </summary>
    public class ELogger : IDisposable
    {
        //-------------------------------------------------------------------------------------------------------------------------
        // logger
        //-------------------------------------------------------------------------------------------------------------------------
        private readonly static Lazy<ELogger> m_logger = new Lazy<ELogger>(() => new ELogger());

        /// <summary>
        /// 
        /// </summary>
        public static ELogger SNG
        {
            get
            {
                return m_logger.Value;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------
        //
        //-------------------------------------------------------------------------------------------------------------------------
        private OpenETaxBill.Channel.Interface.IResponsor m_iresponsor = null;
        private OpenETaxBill.Channel.Interface.IResponsor IResponsor
        {
            get
            {
                if (m_iresponsor == null)
                    m_iresponsor = new OpenETaxBill.Channel.Interface.IResponsor();

                return m_iresponsor;
            }
        }

        private OpenETaxBill.SDK.Logging.QFileLog m_qfilelog = null;
        private OpenETaxBill.SDK.Logging.QFileLog QFileLog
        {
            get
            {
                if (m_qfilelog == null)
                    m_qfilelog = new OpenETaxBill.SDK.Logging.QFileLog();

                return m_qfilelog;
            }
        }

        private OpenETaxBill.SDK.Logging.OEventLog m_oeventLog = null;
        private OpenETaxBill.SDK.Logging.OEventLog OEventLogger
        {
            get
            {
                if (m_oeventLog == null)
                    m_oeventLog = new OpenETaxBill.SDK.Logging.OEventLog(IResponsor.Proxy.EventSource);

                return m_oeventLog;
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------
        //
        //-------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_format"></param>
        /// <param name="p_args"></param>
        public void WriteLog(string p_format, params object[] p_args)
        {
            var _message = String.Format(p_format, p_args);
            WriteLog(CfgHelper.SNG.TraceMode ? String.Format("{0} -> {1}", (new StackTrace()).GetFrame(1).GetMethod().Name, _message) : _message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_message">전달하고자 하는 메시지</param>
        public void WriteLog(string p_message)
        {
            WriteLog("I", CfgHelper.SNG.TraceMode ? String.Format("{0} -> {1}", (new StackTrace()).GetFrame(1).GetMethod().Name, p_message) : p_message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_exception">exception 에러 값</param>
        /// <param name="p_warnning"></param>
        public void WriteLog(Exception p_exception, bool p_warnning = false)
        {
            if (p_warnning == false)
                WriteLog("X", CfgHelper.SNG.TraceMode ? String.Format("{0} -> {1}", (new StackTrace()).GetFrame(1).GetMethod().Name, p_exception.ToString()) : p_exception.Message);
            else
                WriteLog("L", CfgHelper.SNG.TraceMode ? String.Format("{0} -> {1}", (new StackTrace()).GetFrame(1).GetMethod().Name, p_exception.ToString()) : p_exception.Message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_exception">exception 에러 값</param>
        /// <param name="p_message">전달하고자 하는 메시지</param>
        public void WriteLog(string p_exception, string p_message)
        {
            if (Environment.UserInteractive == true)
                IResponsor.WriteDebug(p_exception, p_message);
            else
            {
                try
                {
                    QFileLog.WriteLog(IResponsor.Manager.HostName, p_exception, p_message);
                }
                catch (Exception)
                {
                    OEventLogger.WriteEntry(p_message, EventLogEntryType.Information);
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------------------
        //
        //-------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_iresponsor != null)
                {
                    m_iresponsor.Dispose();
                    m_iresponsor = null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ~ELogger()
        {
            Dispose(false);
        }

        //-------------------------------------------------------------------------------------------------------------------------
        //
        //-------------------------------------------------------------------------------------------------------------------------
    }
}