using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace Kesco.Lib.Win.Document.Classes
{
	public class CommandBackgroundWorker : System.ComponentModel.BackgroundWorker
	{
		object locker = 0;
		public SqlCommand Cmd;

		public CommandBackgroundWorker() : base()
		{
			
		}

		public new void CancelAsync()
		{
			base.CancelAsync();
			lock(locker)
			{
				if(Cmd != null)
				{
					Cmd.Cancel();
					Cmd = null;
				}
			}
		}

		public void SetCmd(SqlCommand cmd)
		{
			lock(locker)
			{
				if(this.Cmd != null)
					this.Cmd.Cancel();
				this.Cmd = cmd;
			}
		}

		protected override void OnRunWorkerCompleted(System.ComponentModel.RunWorkerCompletedEventArgs e)
		{
			Cmd = null;
			base.OnRunWorkerCompleted(e);
		}
	}
}
