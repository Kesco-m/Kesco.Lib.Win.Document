using System;
using System.Collections.Generic;
using System.IO;
using Kesco.Lib.Win.Data.Temp.Objects;
using Kesco.Lib.Win.ImageControl;

namespace Kesco.Lib.Win.Document.Objects
{
    public class ImageObject : IDObject
    {
        #region Events

        public event FileSystemEventHandler FileChanged;

        protected internal void OnFileChanged(FileSystemEventArgs args)
        {
            if (FileChanged != null)
                FileChanged(this, args);
        }

        public event EventHandler NeedRePaint;

        private void OnNeedRePaint()
        {
            if (NeedRePaint != null)
                NeedRePaint(this, EventArgs.Empty);
        }

        public event SaveEventHandler NeedSave;

        protected internal void OnNeedSave(SaveEventArgs e)
        {
            if (NeedSave != null)
                NeedSave(e);
        }

        #endregion

        private string fileName;
        private ServerInfo curServer;
        private List<SignItem> signs;
        private List<StampItem> stampItems;

        public ImageObject(int imageID) : base(imageID)
        {
            Modified = false;
            if (signs == null)
                signs = new List<SignItem>();
            else if (HasSings())
                signs.Clear();
        }

        public ImageObject(string fileName)
            : base(-1)
        {
            Modified = false;
            this.fileName = fileName;
            if (HasSings())
                signs.Clear();
            if (signs != null)
                signs = null;
        }

        ~ImageObject()
        {
            if (FileChanged != null)
                FileChanged = null;
        }

        #region Properties

        public string FileName
        {
            get
            {
                if (id > 0 && string.IsNullOrEmpty(fileName))
                    GetFileName();
                return fileName;
            }
            set
            {
                if (id > 0)
                    throw new Exception("ASDFGHJKL");
                
                fileName = value;
            }
        }

        private void GetFileName()
        {
            if (id <= 0)
                return;
            try
            {
                if (Environment.GetServers().Count <= 0)
                    throw new Exception(Environment.StringResources.GetString("Enviroment_Servers_Error1"));
                
                List<Int32> serverIDs = Environment.DocImageData.GetLocalDocImageServers(id, Environment.GetLocalServersString());
                if (serverIDs == null || serverIDs.Count <= 0)
                    return;

                var rand = new Random();

                curServer = Environment.GetLocalServer(serverIDs[rand.Next(serverIDs.Count)]);
                string fileName = curServer.Path + "\\" + Environment.GetFileNameFromID(id) + ".tif";
                if (File.Exists(fileName))
                {
                    this.fileName = fileName;
                }
                else
                {
                    if (serverIDs.Count > 1)
                        foreach (int t in serverIDs)
                        {
                            curServer = Environment.GetLocalServer(t);
                            fileName = curServer.Path + "\\" + Environment.GetFileNameFromID(id) + ".tif";
                            if (File.Exists(fileName))
                            {
                                this.fileName = fileName;
                                break;
                            }
                        }
                }

                if (stampItems != null)
                {
                    stampItems.Clear();
                    stampItems = null;
                }

                stampItems = Environment.DocSignatureData.GetStamps(id);
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        #endregion

        #region Checks

        public bool Modified { get; private set; }

        public bool HasStamps()
        {
            return true;
        }

        public bool HasStampsOnPage(int page)
        {
            return true;
        }

        public bool HasMarks()
        {
            return true;
        }

        public bool HasMarksOnPage(int page)
        {
            return true;
        }

        public bool HasSings()
        {
            return signs != null && signs.Count > 0;
        }

        public bool CanModify()
        {
            return !HasSings();
        }

        public bool CanModifyStamps()
        {
            return true;
        }

        #endregion
    }

    public enum ImageState
    {
        None = 0,
        Unchanged,
        Added,
        Changed,
        AddedStamps,
        AddedMarks,
        ChangedStamps,
        DeletedMarks,
        DeletedStamps,
        Deleted
    }
}