using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Pipes;
using System.Diagnostics;

namespace RenewEDSenderM
{
    public partial class SenderUI : Form
    {
        public SenderUI()
        {
            InitializeComponent();
        }

        private void btmStart_Click(object sender, EventArgs e)
        {
            LogManager.Logger.FuncEntryLog(sender, e);
            //LogManager.Logger.getInstance().logger.Debug(LogManager.Logger.FUN_ENTRY);
            
            //LogManager.Logger.WriteInfoLog(LogManager.Logger.FUN_ENTRY);
            //LogManager.Logger.WriteInfoLog(LogManager.Logger.FUN_EXIT);
            //LogManager.Logger.getInstance().logger.Debug(LogManager.Logger.FUN_EXIT);
            LogManager.Logger.FuncExitLog();
        }
    }
}
