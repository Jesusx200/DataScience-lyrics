using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebDS.Code;

namespace WebDS
{
    public partial class Sentiment : System.Web.UI.Page
    {
        private static int _topRank = 10;
        private static string _all = "Todo";
        private static int _maxJSONLoad = 200;
        private static int _minCloud = 10;
        private static int _maxCloud = 150;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                fillCmbYear();
                fillCmbRank();
                fillSentiment();
            }
        }

        #region DropDownLists
        private void fillCmbYear()
        {
            DropDownYear.Items.Add("Seleccione una opcion");
            DropDownYear.Items.Add(_all);
            for (int i = 0; i < 70; i++)
            {
                DropDownYear.Items.Add((i + 1950).ToString());
            }
        }

        private void fillCmbRank()
        {
            DropDownRank.Items.Add(_all);
            for (int i = 1; i <= _topRank; i++)
            {
                DropDownRank.Items.Add(i.ToString());
            }
        }

        private void fillSentiment()
        {
            DropDownSentiment.Items.Add(new ListItem("SentiWordNet", "1"));
            DropDownSentiment.Items.Add(new ListItem("DistilBERT base model - (BERT)", "2"));
            DropDownSentiment.Items.Add(new ListItem("Twitter-Roberta", "3"));
            DropDownSentiment.Items.Add(new ListItem("hBERT Sentiment Analysis", "4"));
            DropDownSentiment.Items.Add(new ListItem("DistilBert-Amazon", "5"));
        }
        #endregion

        #region Ajax
        [WebMethod]
        public static List<Words> GetBar(String year, String rank, String senti)
        {
            Analyzer analyzer = new Analyzer();
            List<Words> list = new List<Words>();


            if (year.Equals(_all) && rank.Equals(_all))
            {
                list = analyzer.readAnalyzerAll(senti);
            }
            else if (year.Equals(_all) && !rank.Equals(_all))
            {
                try
                {
                    int rk = int.Parse(rank);
                    int[] ranks = new int[1];
                    ranks[0] = rk - 1;
                    list = analyzer.readAnalyzerAllRank(senti, ranks);
                }
                catch (Exception e)
                {
                    return list;
                }
            }
            else if (rank.Equals(_all))
            {
                try
                {
                    int yr = int.Parse(year);
                    int[] ranks = new int[1];
                    ranks[0] = -1;
                    list = analyzer.readAnalyzerByYear(senti, yr, ranks);
                }
                catch (Exception e)
                {
                    return list;
                }
            }
            else //especific song
            {
                try
                {
                    int yr = int.Parse(year);
                    int rk = int.Parse(rank);
                    int[] ranks = new int[1];
                    ranks[0] = rk - 1;
                    list = analyzer.readAnalyzerByYear(senti, yr, ranks);
                }
                catch (Exception e)
                {
                    return list;
                }
            }

            return list;
        }

        [WebMethod]
        public static List<Lyric> GetLyrics(String year, String rank)
        {
            Analyzer analyzer = new Analyzer();
            List<Lyric> list = new List<Lyric>();

            try
            {
                int yr = int.Parse(year);
                int rk = int.Parse(rank);
                list = analyzer.getLyric(yr, rk - 1);
            }
            catch (Exception e)
            {
                return list;
            }

            return list;
        }

        [WebMethod]
        public static String GetAuthor(String year, String rank)
        {
            Analyzer analyzer = new Analyzer();
            String result = "";
            try
            {
                int yr = int.Parse(year);
                int rk = int.Parse(rank);
                result = analyzer.getAuthorTitle(yr, rk - 1);
            }
            catch (Exception e)
            {
            }
            return result;
        }

        [WebMethod]
        public static List<WordCount> GetCloud(String year, String rank)
        {
            WordCloud wc = new WordCloud();
            List<WordCount> words = new List<WordCount>();

            if (year.Equals(_all) && rank.Equals(_all))
            {
                words = wc.getAllCloudList(); // Posible Max 30150
            }
            else if (year.Equals(_all))
            {
                int rk = int.Parse(rank);
                int[] ranks = new int[1];
                ranks[0] = rk - 1;

                words = wc.getAllCloudbyRank(ranks);
            }
            else if (rank.Equals(_all))
            {
                try
                {
                    int yr = int.Parse(year);
                    words = wc.getCloudList(yr, -1);
                }
                catch (Exception e)
                {
                }
            }
            else
            {
                try
                {
                    int yr = int.Parse(year);
                    int rk = int.Parse(rank);
                    words = wc.getCloudList(yr, rk - 1);
                }
                catch (Exception e)
                {
                }
            }

            if (words.Count == 0) { return words; }

            if(words.Count <= _maxJSONLoad) // Max Load by JSON
            {
                int maxAnt = words[0].count;
                words = AdjustValues(words, maxAnt, _minCloud, _maxCloud);
                return words;
            }

            List<WordCount> words2 = new List<WordCount>();
            int maxVal = words[0].count;

            for (int i = 0; i < _maxJSONLoad; i++)
            {
                WordCount wordc = words[i];
                words2.Add(wordc);
            }

            words2 = AdjustValues(words2, maxVal, _minCloud, _maxCloud);
            return words2;
        }
       private static List<WordCount> AdjustValues(List<WordCount> values, int maxAnt, int bottom, int top)
        {
            List<WordCount> result = new List<WordCount>();

            foreach (var value in values)
            {
                double norm = (value.count * 1.0) / (maxAnt * 1.0);
                int newValue = (int)(bottom + norm * ((top * 1.0) - (bottom * 1.0)));
                value.count = newValue;
                result.Add(value);
            }

            return result;
        }
        #endregion
    }
}