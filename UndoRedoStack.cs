using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Kesco.Lib.Win.Document
{
    public struct UndoRedoElement
    {
        private string nameString;
        private string undoString;
        private string redoString;
        private Func<object[], bool> undoCmd;
        private Func<object[], bool> redoCmd;
        private object[] args;
        private string addString;
        public int EmpID;

        public UndoRedoElement(string nameString, string undoString, string redoString, Func<object[], bool> undoCmd, Func<object[], bool> redoCmd, object[] args, string addString, int empID)
        {
            this.nameString = nameString;
            this.undoString = undoString;
            this.redoString = redoString;
            this.undoCmd = undoCmd;
            this.redoCmd = redoCmd;
            this.args = args;
            this.addString = addString;
            EmpID = empID;
        }

        public object[] Args
        {
            get { return args; }
        }

        /// <summary>
        /// Операция отмены
        /// </summary>
        /// <param name="executed">исполнено - результат операции</param>
        /// <returns>Успех(отстутсвие ошибки)</returns>
        public bool Undo(out bool executed)
        {
            executed = false;

            if (undoCmd == null)
                throw new Exception("Отсутствует операция отмены");
            
            try
            {
                executed = undoCmd(args);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool Redo()
        {
            if (redoCmd == null)
                throw new Exception("Отсутствует операция возврата");
            
            try
            {
                redoCmd(args);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public string UndoString
        {
            get
            {
                return nameString;
            }
        }

        public string UndoLongString
        {
            get {
                return undoString.Length > 0
                           ? undoString
                           : string.Concat(Environment.StringResources.GetString("Cancel"), " ", nameString);
            }
        }

        public string RedoString
        {
            get
            {
                return nameString;
            }
        }

        public string RedoLongString
        {
            get
            {
                return redoString.Length > 0 
                    ? redoString
                    : string.Concat(Environment.StringResources.GetString("Cancel"), " ", nameString);
            }
        }

        public string TypeString
        {
            get
            {
                return addString;
            }
        }
    }
    /// <summary>
    /// Summary description for UndoRedoStaс.
    /// </summary>
    public class UndoRedoStaсk
    {
        const int stackCount = 50;

        #region Events
        /// <summary>
        //// Окончание выполнения всего выбраного на отмену стека Undo
        /// </summary>
        public event Action<string, int, int> UndoEnd;

        private void OnUndoEnd(string command, int count, int lastID)
        {
            if (UndoEnd != null)
                UndoEnd(command, count, lastID);
        }

        /// <summary>
        //// Окончание выполнения всего выбраного на отмену стека Redo
        /// </summary>
        public event Action<string, int, int> RedoEnd;

        private void OnRedoEnd(string command, int count, int lastID)
        {
            if (RedoEnd != null)
                RedoEnd(command, count, lastID);
        }

        /// <summary>
        /// Окончание отдельной операции Undo
        /// </summary>
        public event Func<string, object[], bool> UndoComplete;

        private void OnUndoComplete(string command, object[] objs)
        {
            try
            {
                if (UndoComplete != null)
                    UndoComplete(command, objs);
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        /// <summary>
        /// Окончание отдельной операции Redo
        /// </summary>
        public event Func<string, object[], bool> RedoComplete;

        private void OnRedoComplete(string command, object[] objs)
        {
            try
            {
                if (RedoComplete != null)
                    RedoComplete(command, objs);
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        #endregion

        private SynchronizedCollection<UndoRedoElement> undoElements;
        private SynchronizedCollection<UndoRedoElement> redoElements;

        public UndoRedoStaсk()
        {
            undoElements = new SynchronizedCollection<UndoRedoElement>();
            redoElements = new SynchronizedCollection<UndoRedoElement>();
        }

        public bool Add(string type, string name, string undoString, string redoString, Func<object[], bool> command, object[] args, int empID)
        {
            UndoRedoElement undoelem;
            switch (type)
            {
                case "AddDoc":
                    break;
                case "AddDocToWork":
                    undoelem = new UndoRedoElement(name, undoString, redoString, new Func<object[], bool>(UndoRedoCommands.UndoAddToWork), new Func<object[], bool>(UndoRedoCommands.RedoAddToWork), args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
                case "RemoveDocFromWork":
                    undoelem = new UndoRedoElement(name, undoString, redoString, new Func<object[], bool>(UndoRedoCommands.RedoAddToWork), new Func<object[], bool>(UndoRedoCommands.UndoAddToWork), args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
                case "EditDocProp":
                    undoelem = new UndoRedoElement(name, undoString, redoString, new Func<object[], bool>(UndoRedoCommands.EditDocProp), new Func<object[], bool>(UndoRedoCommands.RedoEditDocProp), args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
                case "AddPerson":
                    undoelem = new UndoRedoElement(name, undoString, redoString, new Func<object[], bool>(UndoRedoCommands.AddPerson), new Func<object[], bool>(UndoRedoCommands.RemovePerson), args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
                case "RemovePerson":
                    undoelem = new UndoRedoElement(name, undoString, redoString, new Func<object[], bool>(UndoRedoCommands.RemovePerson), new Func<object[], bool>(UndoRedoCommands.AddPerson), args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
                case "AddLink":
                    undoelem = new UndoRedoElement(name, undoString, redoString, new Func<object[], bool>(UndoRedoCommands.AddLink), new Func<object[], bool>(UndoRedoCommands.RemoveLink), args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
                case "RemoveLink":
                    undoelem = new UndoRedoElement(name, undoString, redoString, new Func<object[], bool>(UndoRedoCommands.RemoveLink), new Func<object[], bool>(UndoRedoCommands.AddLink), args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
                case "RotateCW":
                    undoelem = new UndoRedoElement(name, undoString, redoString, (Func<object[], bool>)args[1], (Func<object[], bool>)args[2], args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
                case "RotateCCW":
                    undoelem = new UndoRedoElement(name, undoString, redoString, (Func<object[], bool>)args[1], (Func<object[], bool>)args[2], args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
                case "EditImageProperty":
                    break;
                case "AddImageToDoc":
                    undoelem = new UndoRedoElement(name, undoString, redoString, (Func<object[], bool>)args[1], (Func<object[], bool>)args[2], args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
                case "RemoveImageFromDoc":
                    //undoelem = new UndoRedoElement(name, undoString, redoString, (Func<object[], bool>)args[1], (Func<object[], bool>)args[2], args, type, empID);
                    //if (undoElements.Count == stackCount)
                    //    undoElements.RemoveAt(0);
                    //undoElements.Add(undoelem);
                    break;
                case "AddImageCurrentDoc":
                    undoelem = new UndoRedoElement(name, undoString, redoString, (Func<object[], bool>)args[0], (Func<object[], bool>)args[1], args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;

                    //  Свойство изображения документа. Установка места хранения.
                case "SetDocImageArchive":
                    undoelem = new UndoRedoElement(name, undoString, redoString, (Func<object[], bool>)args[1], (Func<object[], bool>)args[2], args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;

                    //  Свойство изображения. Сделать основным.
                case "SetMainImage":
                    undoelem = new UndoRedoElement(name, undoString, redoString, (Func<object[], bool>)args[1], (Func<object[], bool>)args[2], args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;

                case "Sign":
                case "RemoveSign":
                    undoelem = new UndoRedoElement(name, undoString, redoString, (Func<object[], bool>)args[1], (Func<object[], bool>)args[2], args, type, empID);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;

                case "MarkDocAsRead":
                    undoelem = new UndoRedoElement(name, undoString, redoString, new Func<object[], bool>(UndoRedoCommands.MarkDocMessagesAsNotRead), new Func<object[], bool>(UndoRedoCommands.MarkDocMessagesAsRead), args, type, empID);
                    if (undoElements.Contains(undoelem) || redoElements.Contains(undoelem))
                        return true;
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
                case "MarkDocAsNotRead":
                    undoelem = new UndoRedoElement(name, undoString, redoString, new Func<object[], bool>(UndoRedoCommands.MarkDocMessagesAsRead), new Func<object[], bool>(UndoRedoCommands.MarkDocMessagesAsNotRead), args, type, empID);
                    if (undoElements.Contains(undoelem) || redoElements.Contains(undoelem))
                        return true;
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(undoelem);
                    break;
            }
            redoElements.Clear();
            return true;
        }

        public void Undo(int commandNumber)
        {
            string command = "";
            int i = 0;
            for (i = 0; i < commandNumber && undoElements.Count > 0; i++)
            {
                UndoRedoElement element = undoElements[undoElements.Count - 1];
                undoElements.RemoveAt(undoElements.Count - 1);
                command = element.TypeString;

                bool executed;
                if (element.Undo(out executed))
                {
                    OnUndoComplete(element.TypeString, element.Args);

                    if (element.TypeString == "AddImageToDoc" || element.TypeString == "AddImageCurrentDoc")
                    {
                        if (executed)
                            redoElements.Add(element);
                    }
                    else
                    {
                        redoElements.Add(element);
                    }

                    if (element.TypeString == "RotateCW" || element.TypeString == "RotateCCW")
                        return;
                }
                else
                    return;
            }
            OnUndoEnd(command, i, 0);
        }

        public void Redo(int commandNumder)
        {
            string command = "";
            int i = 0;
            for (i = 0; i < commandNumder && redoElements.Count > 0; i++)
            {
                UndoRedoElement element = redoElements[redoElements.Count - 1];
                redoElements.RemoveAt(redoElements.Count - 1);
                if (element.Redo())
                {
                    OnRedoComplete(element.TypeString, element.Args);
                    if (undoElements.Count == stackCount)
                        undoElements.RemoveAt(0);
                    undoElements.Add(element);

                    if (element.TypeString == "RotateCW" || element.TypeString == "RotateCCW")
                        return;
                }
                else
                    return;
            }
            OnRedoEnd(command, i, 0);
        }

        public void Clear()
        {
            undoElements.Clear();
            redoElements.Clear();
        }

        public SynchronizedCollection<UndoRedoElement> UndoItems
        {
            get { return undoElements; }
        }

        public SynchronizedCollection<UndoRedoElement> RedoItems
        {
            get { return redoElements; }
        }

        public string UndoText
        {
            get
            {
                if (undoElements.Count > 0)
                {
                    UndoRedoElement element = undoElements[undoElements.Count - 1];
                    return element.UndoLongString;
                }
                return "";
            }
        }

        public string RedoText
        {
            get
            {
                if (redoElements.Count > 0)
                {
                    UndoRedoElement element = redoElements[redoElements.Count - 1];
                    return element.RedoLongString;
                }
                return "";
            }
        }

        public void RemoveFromStackForEmployee(int empID)
        {
            try
            {
                for (int i = undoElements.Count - 1; i > -1; i--)
                {
                    if (undoElements[i].EmpID == empID)
                        undoElements.RemoveAt(i);
                }

                for (int i = redoElements.Count - 1; i > -1; i--)
                {
                    if (redoElements[i].EmpID == empID)
                        redoElements.RemoveAt(i);
                }
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }

        /// <summary>
        /// Удалить команды поворота изображения
        /// </summary>
        public void RemoveFromStackRotateImage()
        {
            try
            {
                undoElements.ToList().RemoveAll(x => x.TypeString == "RotateCW" || x.TypeString == "RotateCCW");
                undoElements.ToList().RemoveAll(x => x.TypeString == "RotateCW" || x.TypeString == "RotateCCW");

                foreach (var item in undoElements.ToList())
                {
                    if (item.TypeString == "RotateCW" || item.TypeString == "RotateCCW")
                        undoElements.Remove(item);
                }

                foreach (var item in redoElements.ToList())
                {
                    if (item.TypeString == "RotateCW" || item.TypeString == "RotateCCW")
                        redoElements.Remove(item);
                }
            }
            catch (Exception ex)
            {
                Data.Env.WriteToLog(ex);
            }
        }
    }
}
