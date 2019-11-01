using System.Data;

namespace Kesco.Lib.Win.Document.Classes
{
    public class LinksClass
    {
        private int changeLinkID;
        private int parentOrderID;

        public LinksClass(DataRow dr, bool isMain)
        {
            LinkID = (int) dr[Environment.DocLinksData.IDField];
            ParentDocID = (int) dr[Environment.DocLinksData.ParentDocIDField];
            ChildDocID = (int) dr[Environment.DocLinksData.ChildDocIDField];
            parentOrderID = (int) dr[Environment.DocLinksData.ParentOrderField];
            ChildOrderID = (int) dr[Environment.DocLinksData.ChildOrderField];
            FieldID = (int) dr[Environment.DocLinksData.SubFieldIDField];
            IsMain = isMain;
            IsVirtual = false;
        }

        public LinksClass(int docID, bool isMain, int order)
        {
            LinkID = 0;
            if (isMain)
            {
                ChildDocID = docID;
                ChildOrderID = order;
                ParentDocID = 0;
                parentOrderID = 0;
            }
            else
            {
                ParentDocID = docID;
                parentOrderID = order;
                ChildDocID = 0;
                ChildOrderID = 0;
            }
            FieldID = 0;
            IsMain = isMain;
            IsVirtual = true;
        }

        public int LinkID { get; private set; }
        public int ParentDocID { get; private set; }
        public int ChildDocID { get; private set; }
        public int ChildOrderID { get; set; }
        public int FieldID { get; private set; }
        public bool IsMain { get; set; }
        public bool IsVirtual { get; private set; }
        public int ParentOrderID
        {
            get { return parentOrderID; }
            set { ParentDocID = value; }
        }

        public void ChangeOrder(int linkID, int orderID)
        {
            changeLinkID = linkID;
            if (IsMain)
                ChildOrderID = orderID;
            else
                parentOrderID = orderID;
        }
    }
}