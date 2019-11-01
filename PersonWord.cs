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

        private string[] personEnds = {"о", "а", "у", "о", "ом", "е"};
        private string[] contragentEnds = {"", "а", "у", "", "ом", "е"};

        private string[] personContragentEnds = {
                                                    "о-контрагент", "а-контрагента", "у-контрагенту", "о-контрагент",
                                                    "ом-контрагентом", "е-контрагенте"
                                                };

        private string[] personPluralEnds = {"а", "", "ам", "а", "ами", "ах"};
        private string[] contragentPluralEnds = {"ы", "ов", "ам", "ы", "ами", "ах"};

        private string[] personContragentPluralEnds = {
                                                          "а-контрагенты", "-контрагентов", "ам-контрагентам",
                                                          "а-контрагенты", "ами-контрагентами", "ах-контрагентах"
                                                      };

        private const string personBase = "лиц";
        private const string contragentBase = "контрагент";

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