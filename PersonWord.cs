using System.Collections;

namespace Kesco.Lib.Win.Document
{
    public enum PersonModes
    {
        Person,
        Contragent,
        PersonContragent
    }

    public enum Cases
    {
        I,
        R,
        D,
        V,
        T,
        P
    }

    public class PersonWord
    {
        private bool russ;

        private string[] personEnds = {"�", "�", "�", "�", "��", "�"};
        private string[] contragentEnds = {"", "�", "�", "", "��", "�"};

        private string[] personContragentEnds = {
                                                    "�-����������", "�-�����������", "�-�����������", "�-����������",
                                                    "��-������������", "�-�����������"
                                                };

        private string[] personPluralEnds = {"�", "", "��", "�", "���", "��"};
        private string[] contragentPluralEnds = {"�", "��", "��", "�", "���", "��"};

        private string[] personContragentPluralEnds = {
                                                          "�-�����������", "-������������", "��-������������",
                                                          "�-�����������", "���-�������������", "��-������������"
                                                      };

        private const string personBase = "���";
        private const string contragentBase = "����������";

        private const string personBaseEng = "person";
        private const string personPluralEndsEng = "s";

        private ArrayList bases;
        private ArrayList ends;
        private ArrayList pluralEnds;

        public PersonWord(PersonModes mode)
        {
            russ = Environment.CurCultureInfo.TwoLetterISOLanguageName.Equals("ru");
            bases = new ArrayList {personBase, contragentBase, personBase};
            ends = new ArrayList {personEnds, contragentEnds, personContragentEnds};
            pluralEnds = new ArrayList {personPluralEnds, contragentPluralEnds, personContragentPluralEnds};
            Mode = mode;
        }

        public PersonModes Mode { get; set; }

        public string GetForm(Cases c, bool plural, bool cap)
        {
            string word = "";

            if (russ)
            {
                var wIndex = (int) Mode;
                var cIndex = (int) c;

                word = (string) bases[wIndex];
                string[] curEnds = null;

                if (plural)
                    curEnds = (string[]) pluralEnds[wIndex];
                else
                    curEnds = (string[]) ends[wIndex];

                word += curEnds[cIndex];
            }
            else
            {
                word = personBaseEng;
                if (plural)
                {
                    word += personPluralEndsEng;
                }
            }

            if (cap)
            {
                string first = word.Substring(0, 1).ToUpper();
                word = word.Remove(0, 1);
                word = first + word;
            }

            return word;
        }
    }
}