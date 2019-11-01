using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Kesco.Lib.Win.Web;

namespace Kesco.Lib.Win.Document.Components
{
    public class SaveFaxInComponent : SaveFaxComponent
    {
        public SaveFaxInComponent()
        {
        }

        public SaveFaxInComponent(IContainer container, Controls.DocControl control)
            : base(container, control)
        {
        }

        public SaveFaxInComponent(IContainer container)
            : base(container)
        {
        }

        public override string GetStringToSave()
        {
            DataRow dr = Environment.FaxInData.GetFaxIn(ID);
            string val;

            // doc type
            string s = Environment.StringResources.GetString("IncomingFax");

            // date
            object obj = dr[Environment.FaxInData.DateField];
            if (obj is DateTime)
            {
                DateTime date = (DateTime) obj;
                s = TextProcessor.Stuff(s, ", ") + Environment.StringResources.GetString("Received") + " " +
                    date.ToLocalTime().ToString();
            }

            // sender
            obj = dr[Environment.FaxInData.SenderField];
            if (obj is string)
            {
                val = (string) obj;
                if (val.Length > 0)
                    s = TextProcessor.Stuff(s, ", ") + Environment.StringResources.GetString("Sender").ToLower() + ": " +
                        val;
            }

            // description
            obj = dr[Environment.FaxInData.DescriptionField];
            if (obj is string)
            {
                val = (string) obj;
                if (val.Length > 0)
                    s = TextProcessor.StuffSpace(s) + "(" + val + ")";
            }

            return s;
        }

        public override void Save()
        {
            DataRow dr = Environment.FaxInData.GetFaxIn(ID);
            if (dr != null && dr[Environment.FaxInData.DocImageIDField] == DBNull.Value)
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
            DataRow dr = Environment.FaxInData.GetFaxIn(ID);
            if (dr != null && dr[Environment.FaxInData.DateField] is DateTime)
                return ((DateTime) dr[Environment.FaxInData.DateField]);
            else
                throw new Exception("RetrieveScanDate() Error");
        }

        internal override void OnDocumentSaved(DocumentSavedEventArgs doc)
        {
            base.OnDocumentSaved(doc);
            if (saveFaxInfo.SenderId == 0)
                CreateContactAfterFaxInSave(faxID, saveFaxInfo.RecipId, doc.DocID);
        }

        internal override void CreateContactAfterFaxInSave(int faxId, int recipId, int docId)
        {
            DataRow dataRow = Environment.FaxInData.GetFaxIn(faxId);
            if (dataRow == null)
                return;

            using (DataTable dt = Environment.DocData.GetDocPersonsLite(docId, false))
            {
                string[] prsn =
                    dt.Rows.Cast<DataRow>().Where(r => (int) r[Environment.PersonData.IDField] != recipId).
                        Select(r => r[Environment.PersonData.IDField].ToString()).ToArray();

                var ccDialog = new ContactDialog(Environment.CreateContactString,
                                                 "personContactCategor=3&personContactType=30&personContactText=" +
                                                 dataRow[
                                                     Environment.FaxData.
                                                         SenderAddressField] +
                                                 ((prsn != null && prsn.Length > 0)
                                                      ? "&PersonsListContact=" +
                                                        string.Join(",", prsn)
                                                      : "") + "&docview=yes");
                ccDialog.DialogEvent += CreateContactDialog_DialogEvent;
                ccDialog.PersonID = faxId;
                ccDialog.TopLevel = true;
                ccDialog.Show();

                dt.Dispose();
            }
        }

        private void CreateContactDialog_DialogEvent(object source, DialogEventArgs e)
        {
            if (e.Dialog.DialogResult != DialogResult.OK)
                return;
            try
            {
                var dialog = e.Dialog as ContactDialog;
                if (dialog == null)
                    return;
                dialog.DialogEvent -= CreateContactDialog_DialogEvent;
                if (dialog.ContactID <= 0)
                    return;
                DataRow dr = Environment.FaxRecipientData.GetPersonContact(dialog.ContactID);
                if (dr != null)
                {
                    DataRow fdr = Environment.FaxInData.GetFaxIn(dialog.PersonID);
                    OnFaxInContactCreated(new NogeIdEventArgs((int) fdr[Environment.FaxData.FolderFaxIDField]));
                }
            }
            catch
            {
            }
        }
    }

    public struct SaveFaxInfo
    {
        private Int32 senderId; //Код лица
        private Int32 recipId; //Код лица
        private String senderName;
        private String recipName;

        public Int32 SenderId
        {
            get { return senderId; }
        }

        public Int32 RecipId
        {
            get { return recipId; }
        }

        public String SenderName
        {
            get { return senderName; }
        }

        public String RecipName
        {
            get { return recipName; }
        }

        public static SaveFaxInfo Empty
        {
            get
            {
                var empty = new SaveFaxInfo(0, 0, "", "");
                return empty;
            }
        }

        public SaveFaxInfo(Int32 senderId, Int32 recipId, String senderName, String recipName)
        {
            this.senderId = senderId;
            this.recipId = recipId;
            this.senderName = senderName;
            this.recipName = recipName;
        }
    }
}