using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebDS.Code
{
    public class ReadFile
    {
        private string localpath;

        public ReadFile()
        {
            localpath = HttpRuntime.AppDomainAppPath + "\\App_Data\\";
        }
        public string[] readFile(string fname)
        {
            string[] lines = File.ReadAllLines(localpath + fname);
            return lines;
        }
        public IList<int[]> readCompleteInt(string filename, int cols)
        {
            string[] lines = readFile(filename);
            IList<int[]> cont = new List<int[]>();

            for (int i = 0; i < lines.Length; i++)
            {
                string[] tmp = lines[i].Split(' ');
                int[] nums = new int[cols];
                if (tmp.Length > 0)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        try
                        {
                            float num = float.Parse(tmp[j]);
                            nums[j] = (int)(num);
                        }
                        catch (Exception e)
                        {
                            nums[j] = 0;
                        }
                    }
                }
                cont.Add(nums);
            }

            return cont;
        }
        public IList<float[]> readCompleteFloats(string filename, int cols)
        {
            string[] lines = readFile(filename);
            IList<float[]> cont = new List<float[]>();

            for (int i = 0; i < lines.Length; i++)
            {
                string[] tmp = lines[i].Split(' ');
                float[] nums = new float[cols];
                if (tmp.Length > 1)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        try
                        {
                            float fnum = float.Parse(tmp[j]);
                            nums[j] = fnum;
                        }
                        catch (Exception e)
                        {
                            nums[j] = 0;
                        }
                    }
                }
                else
                {
                    nums = new float[1];
                }
                cont.Add(nums);
            }

            return cont;
        }
        public IList<String[]> readCvsFile(string filename)
        {
            List<string[]> cont = new List<string[]>();
            bool title = true;
            try
            {
                using (var reader = new StreamReader(localpath + filename))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');
                        string remainingText = string.Join(",", values.Skip(4)); // remaining
                        if (title)
                        {
                            title = false;
                        }
                        else
                        {
                            String[] tmp = new string[5];
                            tmp[0] = values[0];
                            tmp[1] = values[1];
                            tmp[2] = values[2];
                            tmp[3] = values[3];
                            tmp[4] = remainingText;

                            cont.Add(tmp);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return cont;
            }
            return cont;
        }        
    }
}