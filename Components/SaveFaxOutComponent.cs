using System;
using System.ComponentModel;
using System.Data;

namespace Kesco.Lib.Win.Document.Components
{
    public class SaveFaxOutComponent : SaveFaxComponent
    {
        public SaveFaxOutComponent()
        {
        }

        public SaveFaxOutComponent(IContainer container, Controls.DocControl control)
            : base(container, control)
        {
        }

        public SaveFaxOutComponent(IContainer container) : base(container)
        {
        }

        public override string GetStringToSave()
        {
            DataRow dr = Environment.FaxOutData.GetFaxOut(ID);
            string val;

            // doc type
            string s = Environment.StringResources.GetString("SentFax");

            // date
            object obj = dr[Environment.FaxOutData.DateField];
            if (obj is DateTime)
            {
                DateTime date = (DateTime) obj;
                s = TextProcessor.StuffSpace(s) + date;
            }

            // phone number
            obj = dr[Environment.FaxOutData.RecvAddressField];
            if (obj is String)
            {
                val = (string) obj;
                if (val.Length > 0)
                    s = TextProcessor.StuffSpace(s) + Environment.StringResources.GetString("On") + " " + val;
            }

            // recipient
            obj = dr[Environment.FaxOutData.RecipField];
            if (obj is string)
            {
                val = (string) obj;
                if (val.Length > 0)
                    s = TextProcessor.StuffNewLine(s) + Environment.StringResources.GetString("Receiver") + ": " + val;
            }

            // description
            obj = dr[Environment.FaxOutData.DescriptionField];
            if (obj is string)
            {
                val = (string) obj;
                if (val.Length > 0)
                    s = TextProcessor.StuffNewLine(s) + Environment.StringResources.GetString("Description") + ": " +
                        val;
            }

            return s;
        }

        public override void Save()
        {
            DataRow dr = Environment.FaxOutData.GetFaxOut(ID);
            if (dr != null && dr[Environment.FaxOutData.DocImageIDField] == DBNull.Value)
            {
                DataRow rowSender = Environment.FaxData.GetSenderPersonInfo(dr["АдресОтправителя"].ToString());
                DataRow rowRecip = Environment.FaxData.GetRecipPersonInfo((int) dr["КодПапкиФаксов"]);
                int recipId = 0, senderId = 0;
                string recipName = "", senderName = "";
                if (rowSender != null)
                {
                    senderId = (int) rowSender["КодЛицаОтправителя"];
                    senderName = rowSender["КличкаОтправителя"].ToString();
                }
                if (rowRecip != null)
                {
                    recipId = (int) rowRecip["КодЛицаПолучателя"];
                    recipName = rowRecip["КличкаПолучателя"].ToString();
                }
                saveFaxInfo = new SaveFaxInfo(senderId, recipId, senderName, recipName);
                base.Save();
            }
        }


        public override DateTime RetrieveScanDate()
        {
            DataRow dr = Environment.FaxOutData.GetFaxOut(ID);
            if (dr != null && dr[Environment.FaxOutData.DateField] is DateTime)
                return (DateTime) dr[Environment.FaxOutData.DateField];
            
            throw new Exception("RetrieveScanDate() Error");
        }
    }
}