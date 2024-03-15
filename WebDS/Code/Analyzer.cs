using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDS.Code
{
    public class Analyzer
    {
        /// <summary>
        /// Data dimension Neg-Neu-Pos
        /// </summary>
        private SentiWordNet _sentiWordNet;
        /// <summary>
        /// Data dimension Neg-Pos
        /// </summary>
        private HSentiment _hModels;
        private string[] listAnalyzers;
        private string[] listAnalyzersFile;

        public Analyzer()
        {
            listAnalyzers = new string[] {
                "1", // SentiWordNet
                "2", // DistilBERT base model - (BERT)
                "3", // Twitter-Roberta
                "4", // hBERT Sentiment Analysis
                "5", // DistilBert-Amazon
            };
            listAnalyzersFile = new string[]
            {
                "",
                "sentiment_analysis.txt",
                "twitter-roberta.txt",
                "heBERT_sentiment.txt",
                "distilbert_sentiment_amazon.txt"
            };

            _sentiWordNet = new SentiWordNet();
        }

        public List<Words> readAnalyzerAll(string analyzer)
        {
            List<Words> result = new List<Words>();

            if (analyzer.Equals(listAnalyzers[0]))
            {
                result = _sentiWordNet.readFullData();
            }
            else if (analyzer.Equals(listAnalyzers[1]))
            {
                _hModels = new HSentiment(listAnalyzersFile[1], 2);
                result = _hModels.readFullData();
            }
            else if (analyzer.Equals(listAnalyzers[2]))
            {
                _hModels = new HSentiment(listAnalyzersFile[2], 3);
                result = _hModels.readFullData();
            }
            else if (analyzer.Equals(listAnalyzers[3]))
            {
                _hModels = new HSentiment(listAnalyzersFile[3], 3);
                result = _hModels.readFullData();
            }
            else if (analyzer.Equals(listAnalyzers[4]))
            {
                _hModels = new HSentiment(listAnalyzersFile[4], 2);
                result = _hModels.readFullData();
            }

            return result;
        }
        public List<Words> readAnalyzerAllRank(string analyzer, int[] rank)
        {
            List<Words> result = new List<Words>();

            if (analyzer.Equals(listAnalyzers[0]))
            {
                result = _sentiWordNet.readFullData(rank);
            }
            else if (analyzer.Equals(listAnalyzers[1]))
            {
                _hModels = new HSentiment(listAnalyzersFile[1], 2);
                result = _hModels.readFullData(rank);
            }
            else if (analyzer.Equals(listAnalyzers[2]))
            {
                _hModels = new HSentiment(listAnalyzersFile[2], 3);
                result = _hModels.readFullData(rank);
            }
            else if (analyzer.Equals(listAnalyzers[3]))
            {
                _hModels = new HSentiment(listAnalyzersFile[3], 3);
                result = _hModels.readFullData(rank);
            }
            else if (analyzer.Equals(listAnalyzers[4]))
            {
                _hModels = new HSentiment(listAnalyzersFile[4], 2);
                result = _hModels.readFullData(rank);
            }

            return result;
        }
        public List<Words> readAnalyzerByYear(string analyzer, int year, int[] rank)
        {
            List<Words> result = new List<Words>();

            if (rank[0] == -1)
            {
                if (analyzer.Equals(listAnalyzers[0]))
                {
                    result = _sentiWordNet.readDatabyYear(year);
                }
                else if (analyzer.Equals(listAnalyzers[1]))
                {
                    _hModels = new HSentiment(listAnalyzersFile[1], 2);
                    result = _hModels.readDatabyYear(year);
                }
                else if (analyzer.Equals(listAnalyzers[2]))
                {
                    _hModels = new HSentiment(listAnalyzersFile[2], 3);
                    result = _hModels.readDatabyYear(year);
                }
                else if (analyzer.Equals(listAnalyzers[3]))
                {
                    _hModels = new HSentiment(listAnalyzersFile[3], 3);
                    result = _hModels.readDatabyYear(year);
                }
                else if (analyzer.Equals(listAnalyzers[4]))
                {
                    _hModels = new HSentiment(listAnalyzersFile[4], 2);
                    result = _hModels.readDatabyYear(year);
                }
            }
            else
            {
                if (rank.Length == 1)
                {
                    if (analyzer.Equals(listAnalyzers[0]))
                    {
                        result = _sentiWordNet.readDataSingleSong(year, rank[0]);
                    }
                    else if (analyzer.Equals(listAnalyzers[1]))
                    {
                        _hModels = new HSentiment(listAnalyzersFile[1], 2);
                        result = _hModels.readDataSingleSong(year, rank[0]);
                    }
                    else if (analyzer.Equals(listAnalyzers[2]))
                    {
                        _hModels = new HSentiment(listAnalyzersFile[2], 3);
                        result = _hModels.readDataSingleSong(year, rank[0]);
                    }
                    else if (analyzer.Equals(listAnalyzers[3]))
                    {
                        _hModels = new HSentiment(listAnalyzersFile[3], 3);
                        result = _hModels.readDataSingleSong(year, rank[0]);
                    }
                    else if (analyzer.Equals(listAnalyzers[4]))
                    {
                        _hModels = new HSentiment(listAnalyzersFile[4], 2);
                        result = _hModels.readDataSingleSong(year, rank[0]);
                    }
                }
            }

            return result;
        }

        public List<Lyric> getLyric(int year, int rank)
        {
            List<Lyric> result = new List<Lyric>();
            result = _sentiWordNet.getLyric(year, rank);
            return result;
        }

        public String getAuthorTitle(int year, int rank)
        {
            String result = "";
            result = _sentiWordNet.getAuthorTitle(year, rank);
            return result;
        }
    }
}