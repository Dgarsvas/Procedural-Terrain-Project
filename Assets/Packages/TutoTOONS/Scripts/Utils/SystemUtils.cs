using System;
using System.Globalization;
using UnityEngine;

namespace TutoTOONS
{
    public class SystemUtils
    {
        private static CultureInfo current_culture;

        //This is required for games with "air." bundle ID prefix. Games must always use bundle ID without "air.".
        public static string GetBundleID(string _bundle_id = null)
        {
            if (_bundle_id == null)
            {
                _bundle_id = Application.identifier;
            }
            if (_bundle_id.IndexOf("air.") == 0)
            {
                _bundle_id = _bundle_id.Substring(4);
            }
            return _bundle_id;
        }

        public static int ConvertVersionStringToCode(string version)
        {
            int versionCode = 0;
            string[] versionSplit = version.Split('.');
            int versionInt = 0;

            try
            {
                for (int i = 0; i < versionSplit.Length; i++)
                {
                    if (int.TryParse(versionSplit[i], out versionInt))
                    {
                        versionCode = checked(versionCode * 1000 + versionInt);
                    }
                    else
                    {
                        Debug.LogError("SystemUtils.ConvertVersionStringToCode(string version) - string parameter couldn't be converted into an int");
                        return -1;
                    }
                }
            }
            catch (System.OverflowException e)
            {
                Debug.LogError("SystemUtils.ConvertVersionStringToCode(string version) - CHECKED and CAUGHT: " + e.ToString());
                return -1;
            }

            return versionCode;
        }

        public static string ConvertVersionCodeToString(int code)
        {
            //return string.Format((code.))
            return string.Format((code / 1000000) + "_" + (code / 1000 % 1000) + "_" + (code % 1000));
        }

        public static DateTime DateTimeParse(string _date_string, DateTime _default_date_time)
        {
            DateTime _parsed_date = DateTime.MinValue;

            // Initial date time parsing
            try
            {
                _parsed_date = TutoTOONS.SystemUtils.DateTimeParse(_date_string);
                return _parsed_date;
            }
            catch (Exception e)
            {
                Debug.Log("DateTime parse failed with error: " + e.Message);
            }

            // Try parse with latest working culture
            try
            {
                if (current_culture != null)
                {
                    _parsed_date = TutoTOONS.SystemUtils.DateTimeParse(_date_string, current_culture);
                    return _parsed_date;
                }
            }
            catch (Exception e)
            {
                Debug.Log("DateTime parse failed with error: " + e.Message);
            }

            // Try parse with all possible cultures
            foreach (CultureInfo _culture_info in CultureInfo.GetCultures(CultureTypes.AllCultures))
            {
                try
                {
                    _parsed_date = TutoTOONS.SystemUtils.DateTimeParse(_date_string, _culture_info);
                    current_culture = _culture_info;
                    return _parsed_date;
                }
                catch (Exception e)
                {
                    Debug.Log("DateTime parse failed with error: " + e.Message);
                }
            }

            return _default_date_time;
        }

        public static DateTime DateTimeParse(string _date_string)
        {
            return DateTimeParse(_date_string, DateTime.Now);
        }
    }
}