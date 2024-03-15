using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDS.Code
{
    public class WordCloud
    {
        private ReadFile _readFile;
        private String _fileName = "StopWordsLyricsRaw.txt";
        private IList<String> _data;
        private int _inYear = 1950;
        private int _topRank = 10;

        public WordCloud()
        {
            _readFile = new ReadFile();
        }
        public List<WordCount> getAllCloudList( )
        {
            List<WordCount> words = new List<WordCount>();
            _data = _readFile.readFile(_fileName);

            words = countAll(_data);
            return words;
        }
        public List<WordCount> countAll(IList<String> words)
        {
            Hashtable hashwords = new Hashtable();
            List<WordCount> results = new List<WordCount>();

            if(words.Count == 0)
            {
                return results;
            }
            foreach (String line in words)
            {
                String clearTxt = line.Trim();
                if (clearTxt.Length == 0) { continue; }

                String[] tmp = clearTxt.Split(' ');
                foreach(String word in tmp)
                {
                    String wordValue = word.Trim();
                    wordValue = wordValue.ToLower();

                    if (hashwords.ContainsKey(wordValue))
                    {
                        int value = (int)hashwords[wordValue];
                        value++;
                        hashwords[wordValue] = value;
                    }
                    else
                    {
                        hashwords.Add(wordValue, 1);
                    }
                }
            }
            foreach(var k in hashwords.Keys)
            {
                String key = k.ToString();
                int value = (int)hashwords[key];

                WordCount wc = new WordCount();
                wc.word = key;
                wc.count = value;
                wc.real = value;

                results.Add(wc);
            }

            List<WordCount> sorted = results.OrderByDescending(w => w.count).ToList();
            return sorted;
        }

        public List<WordCount> getCloudList(int year, int rank)
        {
            List<WordCount> words = new List<WordCount>();
            _data = _readFile.readFile(_fileName);
            List<String> selectedlist = new List<String>();

            int start = year - _inYear;
            if (rank == -1) // One Year
            {
                for (int i = 0; i < _topRank; i++)
                {
                    selectedlist.Add(_data[start * 10 + i]);
                }
            }
            else
            {
                selectedlist.Add(_data[start * 10 + rank]);
            }

            words = countAll(selectedlist);
            return words;
        }
        public List<WordCount> getAllCloudbyRank(int[] rank)
        {
            List<WordCount> words = new List<WordCount>();
            _data = _readFile.readFile(_fileName);
            List<String> selectedlist = new List<String>();

            for(int i = 0; i < _data.Count; i++)
            {
                int pos = i % _topRank;
                if (isValue(pos, rank))
                {
                    selectedlist.Add(_data[i]);
                }
            }

            words = countAll(selectedlist);
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
    }
}