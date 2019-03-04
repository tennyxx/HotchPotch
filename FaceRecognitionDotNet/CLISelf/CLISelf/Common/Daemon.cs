using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Face.CLISelf.Common
{
    /// <summary>
    /// 守护进程
    /// </summary>
    public class Daemon
    {
        //字段
        private Process _process;
        private string _address;
        private string _arguments;
        private Thread _daemon;
        private static NLog.Logger m_logger = LogManager.GetCurrentClassLogger();
        const string DaemonTag = "--daemon.";
        private bool isRun = false;

        public bool IsRun { get => isRun; set => isRun = value; }

        public int state = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address">进程地址</param>
        /// <param name="arguments">参数</param>
        public Daemon(string address, string arguments)
        {
            this._process = new Process();
            this._address = address;
            this._arguments = arguments;
        }

        public void Start() {
            // 判断是否已经进入Daemon状态
            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(DaemonTag)))
            {
                Environment.SetEnvironmentVariable(DaemonTag, "yes");
                _daemon = new Thread(new ThreadStart(RestartProcess));
                _daemon.IsBackground = false;
                _daemon.Start();
                m_logger.Info("Daemon Start");
            }
        }

        public void Stop() {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(DaemonTag))) {
                Environment.SetEnvironmentVariable(DaemonTag, "");
                try {
                    IsRun = false;
                    this._process.Kill();
                } catch(Exception ex){
                    m_logger.Info("Daemon Stop process close:" + ex.Message);
                }
            }                
        }

        /// <summary>
        /// 重启进程
        /// </summary>
        public void RestartProcess()
        {
            isRun = true;
            state = 1;
            this._process.StartInfo.FileName = this._address;
            this._process.StartInfo.Arguments = this._arguments;
            while (IsRun)
            {
                this._process.Start();
                this._process.WaitForExit();
                this._process.Close();    //释放已退出进程的句柄
                if (IsRun) {
                    Thread.Sleep(3000);
                }
            }
            state = 2;
        }
    }
}
