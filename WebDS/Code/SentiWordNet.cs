using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;

namespace WebDS.Code
{
    public class SentiWordNet
    {
        private static int _topRank = 10;
        private static int _minYear = 1950;
        private IList<int[]> _data;
        private ReadFile _readFile;

        /// <summary>
        /// year, rank, artist, song, lyrics
        /// </summary>
        private IList<String[]> _cvs;
        /// <summary>
        /// Negative - positive
        /// </summary>
        private IList<String> _detailWord;

        public SentiWordNet()
        {
            _readFile = new ReadFile();
        }
        public List<Words> readFullData()
        {
            List<Words> words = new List<Words>();
            _data = _readFile.readCompleteInt("sentiWordFull.txt", 3); // Negative-neutral-positive

            int[] tmp = new int[3];
            int k = 0;
            for (int i = 0; i < _data.Count; i++)
            {
                for (int j = 0; j < tmp.Length; j++)
                {
                    tmp[j] += _data[i][j];
                }

                if ((i + 1) % _topRank == 0)
                {
                    Words w = new Words((1950 + k).ToString(), tmp);
                    words.Add(w);
                    k++;

                    tmp = new int[3];
                }
            }
            return words;
        }
        public List<Words> readFullData(int[] rank)
        {
            List<Words> words = new List<Words>();
            _data = _readFile.readCompleteInt("sentiWordFull.txt", 3); // Negative-neutral-positive

            int add = 0;
            for (int i = 0; i < _data.Count; i++)
            {
                int pos = i % _topRank;
                if (isValue(pos, rank))
                {
                    Words w = new Words((1950 + add).ToString(), _data[i]);
                    words.Add(w);
                    add++;
                }
            }
            return words;
        }
        private bool isValue(int value, int[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (value == data[i])
                {
                    return true;
                }
            }
            return false;
        }
        public List<Words> readDatabyYear(int year)
        {
            List<Words> words = new List<Words>();
            _data = _readFile.readCompleteInt("sentiWordFull.txt", 3); // Negative-neutral-positive

            int start = year - _minYear;
            for (int i = 0; i < _topRank; i++)
            {
                Words w = new Words((i + 1).ToString(), _data[start * 10 + i]);
                words.Add(w);
            }
            return words;
        }
        public List<Words> readDataSingleSong(int year, int rank)
        {
            List<Words> words = new List<Words>();
            int start = year - _minYear;
            _data = _readFile.readCompleteInt("sentiWordFull.txt", 3); // Negative-neutral-positive

            Words w = new Words((year).ToString(), _data[start * 10 + rank]);
            words.Add(w);
            
            return words;
        }
        #region getLyric
        public List<Lyric> getLyric(int year, int rank)
        {
            _detailWord = _readFile.readFile("sentiWordDetailByWord.txt"); // raw data
            _cvs = _readFile.readCvsFile("topSongsLyrics1950_2019.csv"); // raw data

            int start = year - _minYear;
            string[] data = _cvs[start * 10 + rank];

            String tmp = data[4];
            char[] delimiters = { ' ', ',', '.', '!', '?', '\"', '?'};
            tmp = tmp.Replace("|", " | "); // allow newline
            String[] words = tmp.Split(delimiters);

            String pos_neg_words = _detailWord[start * 10 + rank];
            List<Lyric> cont = new List<Lyric>();

            if (pos_neg_words[0] != '0')
            {
                Hashtable hashWords = getHashWords(pos_neg_words);
                foreach(String s in words)
                {
                    if (s.Equals(string.Empty)) continue;

                    string query = s.Trim();
                    query = query.ToLower();
                    bool ok = hashWords.ContainsKey(query);
                    if (ok)
                    {
                        cont.Add((Lyric)hashWords[query]);
                    }
                    else
                    {
                        Lyric ly = new Lyric();
                        ly.word = s;
                        ly.value = 0;
                        cont.Add(ly);
                    }
                }
            }
            else
            {
                Lyric ly = new Lyric();
                ly.word = "Registro erroneo";
                ly.value = 0;
                cont.Add(ly);
            }

            return cont;
        }
        private Hashtable getHashWords(String words)
        {
            Hashtable hashWords = new Hashtable();
            String[] reg = words.Split(' ');

            string pattern = @"\[(.*?)\]";
            MatchCollection matches = Regex.Matches(words, pattern);

            foreach (Match match in matches)
            {
                Lyric ly = new Lyric();
                String tmp = match.Groups[1].Value;
                String[] fields = tmp.Split(',');

                String word = fields[0].Trim();
                word = word.Substring(1, word.Length - 2);
                ly.word = word.ToLower();
                ly.value = 0; // neutral

                bool ok = hashWords.ContainsKey(word);
                if (ok) continue;

                try
                {
                    String neg = fields[1].Trim();
                    float negative = float.Parse(neg);

                    String pos = fields[2].Trim();
                    float positive = float.Parse(pos);

                    if(positive-negative > 0)
                    {
                        ly.value = 2; // positive
                    }
                    else if(positive-negative < 0)
                    {
                        ly.value = 1; // negative
                    }
                }
                catch(Exception e) 
                { 
                }
                hashWords.Add(word, ly);
            }

            return hashWords;
        }
        #endregion

        public String getAuthorTitle(int year, int rank)
        {
            _cvs = _readFile.readCvsFile("topSongsLyrics1950_2019.csv"); // raw data

            int start = year - _minYear;
            string[] data = _cvs[start * 10 + rank];
            String tmp = data[2] + " - " + data[3], result = "";

            foreach(Char c in tmp)
            {
                if(char.IsLetterOrDigit(c) || c == ' ' || c == '-' || c == '&' || c == '/'
                    || c == '.' || c == '(' || c == ')')
                {
                    result += c;
                }
            }

            return result;
        }
    }
}