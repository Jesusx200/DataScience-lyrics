using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDS.Code
{
    public class HSentiment
    {
        private String _model;
        private int _dimension;
        private ReadFile _readFile;
        private IList<float[]> _data;
        private int _topRank = 10;
        private int _negNeuPos = 3; // to Javascript Bar
        private int _minYear = 1950;

        public HSentiment(String model, int dimension)
        {
            _model = model;
            _dimension = dimension;
            _readFile = new ReadFile();
        }
        public List<Words> readFullData()
        {
            List<Words> words = new List<Words>();
            _data = _readFile.readCompleteFloats(_model, _dimension);

            int[] tmp = new int[_negNeuPos];
            int k = 0;
            for (int i = 0; i < _data.Count; i++)
            {
                int indx = maxSentiment(_data[i]);
                if(indx > -1)
                {
                    if (_dimension == 2)
                    {
                        indx = (indx == 1) ? 2 : indx;
                    }
                    tmp[indx] += 1;
                }

                if ((i + 1) % _topRank == 0)
                {
                    Words w = new Words((1950 + k).ToString(), tmp);
                    words.Add(w);
                    k++;

                    tmp = new int[_negNeuPos];
                }
            }
            return words;
        }
        private int maxSentiment(float[] values)
        {
            if(values.Length == 1)
            {
                return -1; // No data
            }
            int indx = 0;
            float maxSent = values[0];

            for(int i=1; i<values.Length; i++)
            {
                if (values[i] > maxSent)
                {
                    maxSent = values[i];
                    indx = i;
                }
            }
            return indx;
        }
        public List<Words> readFullData(int[] rank)
        {
            List<Words> words = new List<Words>();
            _data = _readFile.readCompleteFloats(_model, _dimension);

            int add = 0;
            for (int i = 0; i < _data.Count; i++)
            {
                int pos = i % _topRank;
                if (isValue(pos, rank))
                {
                    int[] tmp = new int[_negNeuPos];
                    int indx = maxSentiment(_data[i]);
                    if (indx > -1)
                    {
                        if (_dimension == 2)
                        {
                            indx = (indx == 1) ? 2 : indx;
                        }
                        tmp[indx] += 1;
                    }

                    Words w = new Words((1950 + add).ToString(), tmp);
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
            _data = _readFile.readCompleteFloats(_model, _dimension);

            int start = year - _minYear;
            for (int i = 0; i < _topRank; i++)
            {
                int[] tmp = new int[_negNeuPos];
                int indx = maxSentiment(_data[start * 10 + i]);
                if (indx > -1)
                {
                    if (_dimension == 2)
                    {
                        indx = (indx == 1) ? 2 : indx;
                    }
                    tmp[indx] += 1;
                }

                Words w = new Words((i + 1).ToString(), tmp);
                words.Add(w);
            }
            return words;
        }
        public List<Words> readDataSingleSong(int year, int rank)
        {
            List<Words> words = new List<Words>();
            int start = year - _minYear;
            _data = _readFile.readCompleteFloats(_model, _dimension);

            int[] tmp = new int[_negNeuPos];
            int indx = maxSentiment(_data[start * 10 + rank]);
            if (indx > -1)
            {
                if (_dimension == 2)
                {
                    indx = (indx == 1) ? 2 : indx;
                }
                tmp[indx] += 1;

                Words w = new Words((year).ToString(), tmp);
                words.Add(w);
            }
            return words;
        }
    }
}
